using GeekCoding.Data.Models;
using GeekCoding.MainApplication.Utilities.DTO;
using GeekCoding.MainApplication.Utilities.Enum;
using GeekCoding.MainApplication.ViewModels;
using GeekCoding.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;

namespace GeekCoding.MainApplication.Controllers
{
    [Authorize]
    public class ContestController : Controller
    {
        private IAnnouncementRepository _announcementRepository;
        private IProblemRepository _problemRepository;
        private IContestRepository _contestRepository;
        private IUserContestRepository _userContestRepository;
        private IProblemContestRepository _problemContestRepository;
        private ISubmisionContestRepository _submisionContestRepository;

        public ContestController(IContestRepository contestRepository, IUserContestRepository userContestRepository,
                                 IProblemContestRepository problemContestRepository, ISubmisionContestRepository submisionContestRepository,
                                 IAnnouncementRepository announcementRepository, IProblemRepository problemRepository)
        {
            _contestRepository = contestRepository;
            _userContestRepository = userContestRepository;
            _problemContestRepository = problemContestRepository;
            _submisionContestRepository = submisionContestRepository;
            _announcementRepository = announcementRepository;
            _problemRepository = problemRepository;
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

        [Authorize(Roles ="Admin")]
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

        [Authorize(Roles ="Admin")]
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
            return RedirectToAction(nameof(Index));
        }

        [Authorize(Roles ="Admin")]
        [HttpGet]
        public IActionResult DeleteProblemFromContest(Guid id, Guid contest)
        {
            var contestFound = _problemContestRepository.GetAll().Where(x => x.ContestId == contest && x.ProblemId == id).FirstOrDefault();
            if(contestFound != null)
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