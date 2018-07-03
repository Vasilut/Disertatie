using GeekCoding.Common.Helpers;
using GeekCoding.Compilation.Api.Model;
using GeekCoding.Data.Models;
using GeekCoding.MainApplication.Utilities;
using GeekCoding.MainApplication.Utilities.DTO;
using GeekCoding.MainApplication.Utilities.Enum;
using GeekCoding.MainApplication.ViewModels;
using GeekCoding.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public ContestController(IContestRepository contestRepository, IUserContestRepository userContestRepository,
                                 IProblemContestRepository problemContestRepository, ISubmisionContestRepository submisionContestRepository,
                                 IAnnouncementRepository announcementRepository, IProblemRepository problemRepository, ISubmisionRepository submisionRepository)
        {
            _contestRepository = contestRepository;
            _userContestRepository = userContestRepository;
            _problemContestRepository = problemContestRepository;
            _submisionContestRepository = submisionContestRepository;
            _announcementRepository = announcementRepository;
            _problemRepository = problemRepository;
            _submisionRepository = submisionRepository;
            _compilers = Compilator.Compilers;
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Index()
        {
            var lst = _contestRepository.GetAll().ToList();
            var goodList = lst.Select(prop =>
            {
                prop.Duration = (prop.EndDate - prop.StartDate).TotalHours;
                return prop;
            }).ToList();

            return View(lst);
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
            if(ModelState.IsValid)
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
            if(participant == null)
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
        public IActionResult Add([FromForm] Contest contest)
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

                return RedirectToAction(nameof(Index));
            }
            return View();
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
            //delete first from contestproblem table

            _contestRepository.Delete(contest.ContestId);
            _contestRepository.Save();
            return RedirectToAction(nameof(Index));
        }
        
        [HttpGet]
        public IActionResult Overview(Guid id)
        {
            return RedirectToAction(nameof(Details), new { id = id });
        }

        [HttpGet]
        public IActionResult ProblemsOverview(Guid id)
        {
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

        [HttpGet]
        public IActionResult ProblemContestOverview(Guid id, Guid contest)
        {

            var problem = _problemRepository.GetItem(id);
            
            var problemContestViewModel = new ProblemContestDetailsViewModel
            {
                ContestId = contest,
                Problem = problem,
                SelectListItems = _compilers,
                Score = 0
            };
            //see list
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

            return RedirectToAction(nameof(Details), new { id = model.ContestId });
        }

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
            return View(usersRegistered);
        }

        [HttpGet]
        public IActionResult SubmisionOverview(Guid id)
        {
            //list with submission
            var contestSubmissionList = _submisionContestRepository.GetListOfSubmisionForSpecificContest(id).ToList();
            List<Submision> submisions = new List<Submision>();
            foreach (var item in contestSubmissionList)
            {
                submisions.Add(item.Submision);
            }
            return View(submisions);
        }

        [HttpGet]
        public IActionResult Ranking(Guid id)
        {
            return View();
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