using GeekCoding.Common.EmailGenerator;
using GeekCoding.Common.Helpers;
using GeekCoding.Compilation.Api.Model;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.Hubs;
using GeekCoding.MainApplication.Jobs;
using GeekCoding.MainApplication.Pagination;
using GeekCoding.MainApplication.Utilities;
using GeekCoding.MainApplication.Utilities.DTO;
using GeekCoding.MainApplication.Utilities.Enum;
using GeekCoding.MainApplication.Utilities.Services;
using GeekCoding.MainApplication.ViewModels;
using GeekCoding.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GeekCoding.MainApplication.Controllers
{
    [Authorize]
    public class ContestController : Controller
    {
        private IAnnouncementRepository _announcementRepository;
        private IProblemRepository _problemRepository;
        private ISubmisionRepository _submisionRepository;
        private IContestRepository _contestRepository;
        private IUserContestRepository _userContestRepository;
        private IProblemContestRepository _problemContestRepository;
        private ISubmisionContestRepository _submisionContestRepository;
        private List<SelectListItem> _compilers = new List<SelectListItem>();
        private IMessageBuilder _emailSender;
        private UserManager<User> _userManager;
        private IConfiguration _configuration;
        private ITestsRepository _testRepository;
        private SubmissionHub _submissionHub;
        private IEvaluationRepository _evaluationRepository;
        private IHubContext<SubmissionHub> _hubContext;
        private ISerializeTests _serializeTests;
        private IUserInformationRepository _userInformation;
        static SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        private string _compilationApi;
        private string _executionApi;

        public ContestController(IContestRepository contestRepository, IUserContestRepository userContestRepository, IProblemContestRepository problemContestRepository,
                                 ISubmisionContestRepository submisionContestRepository, IAnnouncementRepository announcementRepository, IProblemRepository problemRepository,
                                 ISubmisionRepository submisionRepository, IMessageBuilder emailSender, UserManager<User> userManager,
                                 IConfiguration configuration, ITestsRepository testsRepository, SubmissionHub submissionHub,
                                 IHubContext<SubmissionHub> hubContext, ISerializeTests serializeTests,
                                 IEvaluationRepository evaluationRepository, IUserInformationRepository userInformation)
        {
            _contestRepository = contestRepository;
            _userContestRepository = userContestRepository;
            _problemContestRepository = problemContestRepository;
            _submisionContestRepository = submisionContestRepository;
            _announcementRepository = announcementRepository;
            _problemRepository = problemRepository;
            _submisionRepository = submisionRepository;
            _compilers = Compilator.Compilers;
            _emailSender = emailSender;
            _userManager = userManager;


            _configuration = configuration;
            _testRepository = testsRepository;
            _submissionHub = submissionHub;
            _evaluationRepository = evaluationRepository;
            _hubContext = hubContext;
            _serializeTests = serializeTests;
            _userInformation = userInformation;

            //intialize compilation and running api
            _compilationApi = _configuration.GetSection("Api")["CompilationApi"];
            _executionApi = _configuration.GetSection("Api")["ExecutionApi"];
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index(int? page)
        {
            var lst = _contestRepository.GetAll().ToList();
            var goodList = lst.Select(prop =>
            {
                prop.Duration = (prop.EndDate - prop.StartDate).TotalHours;
                return prop;
            }).ToList();

            int pageSize = 4;
            return View(PaginatedList<Contest>.CreateAsync(goodList, page ?? 1, pageSize));
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var contestToEdit = _contestRepository.GetItem(id);
            if (contestToEdit == null)
            {
                return BadRequest("Nothing to edit");
            }
            return View(contestToEdit);
        }

        [HttpPost]
        public IActionResult Edit([FromForm] ContestViewModel contest)
        {
            var contestToEdit = _contestRepository.GetItem(contest.ContestId);
            if(contestToEdit == null)
            {
                return BadRequest("Nothing to edit");
            }

            contestToEdit.Title = contest.Title;
            contestToEdit.Content = contest.Content;
            contestToEdit.StartDate = contest.StartDate;
            contestToEdit.EndDate = contest.EndDate;

            _contestRepository.Update(contestToEdit);
            _contestRepository.Save();

            return RedirectToAction(nameof(Index));

        }

        [Authorize]
        [HttpPost]
        public IActionResult RegisterUser([FromForm] UserContestViewModel model)
        {
            //register user in database
            if (ModelState.IsValid)
            {
                var contestUser = new UserContest
                {
                    UserContestId = Guid.NewGuid(),
                    ContestId = model.Contest.ContestId,
                    UserName = User.Identity.Name
                };
                _userContestRepository.Create(contestUser);
                _userContestRepository.Save();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpPost]
        public IActionResult UnRegisterUser([FromForm] UserContestViewModel model)
        {
            if (ModelState.IsValid)
            {
                var userContest = _userContestRepository.GetAll().Where(x => x.ContestId == model.Contest.ContestId && x.UserName == User.Identity.Name).FirstOrDefault();
                _userContestRepository.Delete(userContest.UserContestId);
                _userContestRepository.Save();
                return RedirectToAction("Details", new { id = model.Contest.ContestId });
            }
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Details(Guid id)
        {
            var contest = _contestRepository.GetItem(id);
            var listOfParticipants = _userContestRepository.GetAll().Where(x => x.ContestId == id).ToList();
            bool isRegistered = true;
            var participant = listOfParticipants.Where(part => part.UserName == User.Identity?.Name).FirstOrDefault();
            if (participant == null)
            {
                isRegistered = false;
            }
            var userContest = new UserContestViewModel
            {
                Contest = contest,
                UserRegistered = isRegistered
            };
            if (contest == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(userContest);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Add([FromForm] ContestViewModel contest)
        {
            if (ModelState.IsValid)
            {
                var contestModel = new Contest
                {
                    ContestId = Guid.NewGuid(),
                    Content = contest.Content,
                    StartDate = contest.StartDate,
                    EndDate = contest.EndDate,
                    StatusContest = ContestStatus.UnderConstruction.ToString(),
                    Title = contest.Title,
                };
                _contestRepository.Create(contestModel);
                _contestRepository.Save();

                //send mail to all the registered user with the contest
                //InformUsersNewContest(contest.Title);

                return RedirectToAction(nameof(Index));
            }
            return View();
        }

        private void InformUsersNewContest(string contestName)
        {
            var lstUsers = _userManager.Users.ToList();
            foreach (var item in lstUsers)
            {
                _emailSender.AddReceiver(item.Email)
                                    .AddSubject($"Contest {contestName} ")
                                    .AddBody("A new contest will start soon!. Please check our page")
                                    .BuildAndSend();
            }
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult Delete(Guid id)
        {
            var contest = _contestRepository.GetItem(id);
            if (contest == null)
            {
                return RedirectToAction(nameof(Index));
            }
            return View(contest);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Delete([FromForm] ContestViewModel contest)
        {
            //delete the user assigned to this contest from user contest
            var listOfParticipant = _userContestRepository.GetAll().Where(part => part.ContestId == contest.ContestId).ToList();
            foreach (var item in listOfParticipant)
            {
                _userContestRepository.Delete(item);
            }

            //delete submission
            var listOfSubmission = _submisionContestRepository.GetAll().Where(subm => subm.ContestId == contest.ContestId).ToList();
            foreach (var item in listOfSubmission)
            {
                _submisionContestRepository.Delete(item);
            }

            //delete problem assigned to a contest
            var listOfProblems = _problemContestRepository.GetAll().Where(prob => prob.ContestId == contest.ContestId).ToList();
            foreach (var item in listOfProblems)
            {
                _problemContestRepository.Delete(item);
            }


            _contestRepository.Delete(contest.ContestId);
            _contestRepository.Save();
            return RedirectToAction(nameof(Index));
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Announcement(Guid id)
        {

            var announcements = _announcementRepository.GetAll().Where(ann => ann.ContestId == id).OrderByDescending(ann => ann.DateAdded).ToList();
            var announcementWithContestId = new Tuple<List<Announcement>, Guid>(announcements, id);
            return View(announcementWithContestId);
        }

        [HttpGet]
        public IActionResult AddAnnouncement(Guid id)
        {
            var announcement = new ContestAnnouncementViewModel
            {
                Contestid = id
            };
            return View(announcement);
        }

        [HttpPost]
        public IActionResult AddAnnouncement([FromForm] ContestAnnouncementViewModel model)
        {
            if (ModelState.IsValid)
            {
                var announcement = new Announcement
                {
                    AnnouncementContent = model.AnnouncementContent,
                    ContestId = model.Contestid,
                    DateAdded = DateTime.Now,
                    AnnouncementId = Guid.NewGuid()
                };

                _announcementRepository.Create(announcement);
                _announcementRepository.Save();

                return RedirectToAction(nameof(Announcement), new { id = model.Contestid });
            }
            return BadRequest("Something bad happened");
        }

        [HttpGet]
        public IActionResult DeleteAnnouncement(Guid id, Guid contest)
        {
            var announcementToDelete = _announcementRepository.GetItem(id);
            if (announcementToDelete == null)
            {
                RedirectToAction(nameof(Announcement), new { id = contest });
            }
            return View(announcementToDelete);
        }

        [HttpPost]
        public IActionResult DeleteAnnouncement([FromForm]ContestAnnouncementViewModel announcement)
        {
            _announcementRepository.Delete(announcement.AnnouncementId);
            _announcementRepository.Save();

            return RedirectToAction(nameof(Announcement), new { id = announcement.Contestid });
        }

        [HttpGet]
        public IActionResult EditAnnouncement(Guid id)
        {
            var announcementToEdit = _announcementRepository.GetItem(id);
            if (announcementToEdit == null)
            {
                return BadRequest("Nothing to edit");
            }
            return View(announcementToEdit);
        }

        [HttpPost]
        public IActionResult EditAnnouncement([FromForm]ContestAnnouncementViewModel announcement)
        {
            var announcementToUpdate = _announcementRepository.GetItem(announcement.AnnouncementId);
            if (announcementToUpdate == null)
            {
                return BadRequest("Nothing to update");
            }
            announcementToUpdate.AnnouncementContent = announcement.AnnouncementContent;
            announcementToUpdate.DateAdded = DateTime.Now;
            _announcementRepository.Update(announcementToUpdate);
            _announcementRepository.Save();

            return RedirectToAction(nameof(Announcement), new { id = announcement.Contestid });
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Overview(Guid id)
        {
            return RedirectToAction(nameof(Details), new { id = id });
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult ProblemsOverview(Guid id)
        {
            //check if the contest started
            var currentContest = _contestRepository.GetItem(id);
            if (User.Identity.Name == null || !User.IsInRole("Admin"))
            {
                //not registered or not an admin user
                if (DateTime.Now < currentContest.StartDate)
                {
                    //showing nothing if the contest didn't started
                    return View(new ProblemsOverviewViewModel { ContestId = id, ContestProblemList = new List<Problem>() });
                }
            }

            //list with the problems
            var contestProblemList = _problemContestRepository.GetListOfProblemForSpecificContest(id).ToList();
            List<Problem> problems = new List<Problem>();
            foreach (var item in contestProblemList)
            {
                problems.Add(item.Problem);
            }

            var problemContestViewModel = new ProblemsOverviewViewModel
            {
                ContestId = id,
                ContestProblemList = problems
            };

            return View(problemContestViewModel);
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult ProblemContestOverview(Guid id, Guid contest)
        {

            var problem = _problemRepository.GetItem(id);
            var currentContest = _contestRepository.GetItem(contest);
            var user = User?.Identity?.Name;
            int score = 0;


            //see the score for this problem in contest for the user that is logged in
            if (user != null)
            {
                //list with submission
                var contestSubmissionList = _submisionContestRepository.GetListOfSubmisionForSpecificContest(contest).ToList();
                var lstOfSubmissionForAProblem = contestSubmissionList.Where(x => x.Submision.ProblemId == id &&
                                                                                x.Submision.UserName == user).
                                                                                OrderByDescending(x => x.Submision.Score).ToList();
                score = lstOfSubmissionForAProblem.FirstOrDefault() == null ? 0 : lstOfSubmissionForAProblem.First().Submision.Score;

            }


            var problemContestViewModel = new ProblemContestDetailsViewModel
            {
                ContestId = contest,
                Problem = problem,
                SelectListItems = _compilers,
                Score = score,
                StartDate = currentContest.StartDate,
                EndDate = currentContest.EndDate
            };

            return View(problemContestViewModel);
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> ProblemContestExecute([FromForm] FileExecutionContestViewModel model)
        {
            //read the content of the file
            Tuple<string, long> fileContent = await FileHelpers.ProcessFormFile(model.File, ModelState);
            double sizeOfFile = (fileContent.Item2) % 1000;
            var compilationModel = new CompilationModel { Content = fileContent.Item1, Language = model.Compilator, ProblemName = model.ProblemName, Username = User.Identity.Name };

            //save the submission
            var submission = new Submision
            {
                SubmisionId = Guid.NewGuid(),
                DataOfSubmision = DateTime.Now,
                Compilator = model.Compilator,
                ProblemId = Guid.Parse(model.ProblemId),
                SourceSize = sizeOfFile.ToString(),
                StateOfSubmision = SubmissionStatus.NotCompiled.ToString(),
                UserName = User.Identity.Name,
                MessageOfSubmision = string.Empty,
                Score = 0,
                JobQueued = false,
                SourceCode = fileContent.Item1
            };

            await _submisionRepository.AddAsync(submission);

            var submissionContest = new SubmisionContest
            {
                SubmisionContestId = Guid.NewGuid(),
                ContestId = model.ContestId,
                SubmisionId = submission.SubmisionId
            };

            await _submisionContestRepository.AddAsync(submissionContest);


            //build the submission dto
            var problem = _problemRepository.GetItem(submission.ProblemId);
            string problemName = problem.ProblemName;
            int nrOfTests = _testRepository.GetNumberOfTestForProblem(problem.ProblemId);

            var submissionDtoModel = new SubmisionDto
            {
                Compilator = submission.Compilator,
                ProblemName = problemName,
                Content = submission.SourceCode,
                SubmissionId = submission.SubmisionId,
                UserName = User.Identity.Name,
                MemoryLimit = problem.MemoryLimit,
                TimeLimit = problem.TimeLimit,
                NumberOfTests = nrOfTests,
                FileName = problemName.ToLower()
            };

            await Task.Run(() => VerificaThread(submissionDtoModel));

            return RedirectToAction(nameof(ProblemsOverview), new { id = model.ContestId });
        }

        private async Task VerificaThread(SubmisionDto submissionDtoModel)
        {
            semaphoreSlim.Wait();
            try
            {
                var submRequest = new SubmissionRequest(_submissionHub, _submisionRepository, _hubContext,
                                                        _serializeTests, _evaluationRepository);
                await submRequest.MakeSubmissionRequestAsync(submissionDtoModel, _compilationApi, _executionApi);
            }
            finally
            {
                semaphoreSlim.Release();
            }
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult UsersOverview(Guid id)
        {
            //list with the users
            var listOfParticipants = _userContestRepository.GetAll().Where(x => x.ContestId == id).ToList();
            List<string> usersRegistered = new List<string>();
            foreach (var item in listOfParticipants)
            {
                usersRegistered.Add(item.UserName);
            }

            var userRegisteredAndContestId = new Tuple<List<string>, Guid>(usersRegistered, id);
            return View(userRegisteredAndContestId);
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult SubmisionOverview(Guid id, int? page)
        {
            //list with submission
            var contestSubmissionList = _submisionContestRepository.GetListOfSubmisionForSpecificContest(id).ToList();
            List<Submision> submisions = new List<Submision>();
            foreach (var item in contestSubmissionList)
            {
                submisions.Add(item.Submision);
            }

            int pageSize = 10;
            var submissionWithContestId = new Tuple<PaginatedList<Submision>, Guid>(PaginatedList<Submision>.CreateAsync(submisions, page ?? 1, pageSize), id);
            return View(submissionWithContestId);
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult Ranking(Guid id)
        {
            //list with the problems
            var contestProblemList = _problemContestRepository.GetListOfProblemForSpecificContest(id).ToList();
            List<Problem> problems = new List<Problem>();
            foreach (var item in contestProblemList)
            {
                problems.Add(item.Problem);
            }


            //list of participant
            var listOfParticipants = _userContestRepository.GetAll().Where(x => x.ContestId == id).ToList();
            List<string> usersRegistered = new List<string>();
            foreach (var item in listOfParticipants)
            {
                usersRegistered.Add(item.UserName);
            }


            //list with submission
            var contestSubmissionList = _submisionContestRepository.GetListOfSubmisionForSpecificContest(id).ToList();
            List<Submision> submisions = new List<Submision>();
            foreach (var item in contestSubmissionList)
            {
                submisions.Add(item.Submision);
            }

            List<RankingViewModel> rankingList = new List<RankingViewModel>();
            foreach (var participants in listOfParticipants)
            {
                List<int> scores = new List<int>();
                List<string> lstProblems = new List<string>();
                RankingViewModel rankingModel = new RankingViewModel();
                rankingModel.Participant = participants.UserName;
                int total = 0;

                foreach (var problemInContest in contestProblemList)
                {
                    lstProblems.Add(problemInContest.Problem.ProblemName);
                    var lstOfSubmissionForAProblem = contestSubmissionList.Where(x => x.Submision.ProblemId == problemInContest.ProblemId &&
                                                                                x.Submision.UserName == participants.UserName).
                                                                                OrderByDescending(x => x.Submision.Score).ToList();
                    var score = lstOfSubmissionForAProblem.FirstOrDefault() == null ? 0 : lstOfSubmissionForAProblem.First().Submision.Score;
                    scores.Add(score);
                    total = total + score;
                }

                var participantInformation = _userInformation.GetUserInformationByUsername(participants.UserName);

                rankingModel.ProblemList = lstProblems;
                rankingModel.Scores = scores;
                rankingModel.Total = total;
                rankingModel.ParticipantInformation = new ParticipantInformation
                {
                    Nume = participantInformation?.Nume,
                    Prenume = participantInformation?.Prenume,
                    Clasa = participantInformation?.Clasa,
                    Profesor = participantInformation?.Profesor,
                    Scoala = participantInformation?.Scoala
                };
                rankingList.Add(rankingModel);
            }

            var rankingForContext = new Tuple<List<RankingViewModel>, Guid>(rankingList.OrderByDescending(x => x.Total).ToList(), id);
            return View(rankingForContext);
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult AddProblems(Guid id)
        {
            var problemList = _problemRepository.GetAll();
            var contest = _contestRepository.GetItem(id);
            var contestBig = _problemContestRepository.GetListOfProblemForSpecificContest(id).ToList();

            var problemDtoList = new List<ProblemDto>();
            List<Guid> ids = new List<Guid>();
            foreach (var item in problemList)
            {
                ids.Add(item.ProblemId);
                problemDtoList.Add(new ProblemDto
                {
                    ProblemId = item.ProblemId,
                    ProblemName = item.ProblemName
                });
            }

            var existingProblemInContest = new List<ProblemDto>();
            foreach (var item in contestBig)
            {
                existingProblemInContest.Add(new ProblemDto
                {
                    ProblemId = item.Problem.ProblemId,
                    ProblemName = item.Problem.ProblemName
                });
            }

            var contestproblemViewModel = new ContestProblemViewModel
            {
                ProblemId = ids,
                Problems = problemDtoList,
                ContestId = id,
                ContestName = contest.Title,
                ProblemsForCurrentContest = existingProblemInContest
            };

            return View(contestproblemViewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult Select([FromForm] ContestProblemViewModel model)
        {
            if (ModelState.IsValid)
            {
                foreach (var item in model.ProblemId)
                {
                    var problemContest = new ProblemContest
                    {
                        ProblemContestId = Guid.NewGuid(),
                        ContestId = model.ContestId,
                        ProblemId = item
                    };
                    _problemContestRepository.Create(problemContest);
                }
                _problemContestRepository.Save();
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(AddProblems), new { id = model.ContestId });
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public IActionResult DeleteProblemFromContest(Guid id, Guid contest)
        {
            var contestFound = _problemContestRepository.GetAll().Where(x => x.ContestId == contest && x.ProblemId == id).FirstOrDefault();
            if (contestFound != null)
            {
                return View(contestFound);
            }
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public IActionResult DeleteProblemFromContest([FromForm] ProblemContest contest)
        {
            _problemContestRepository.Delete(contest.ProblemContestId);
            _problemContestRepository.Save();
            return RedirectToAction(nameof(Index));
        }
    }
}