using GeekCoding.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GeekCoding.MainApplication.Controllers
{
    [Authorize]
    public class ContestController : Controller
    {
        private IAnnouncementRepository _announcementRepository;
        private IContestRepository _contestRepository;
        private IUserContestRepository _userContestRepository;
        private IProblemContestRepository _problemContestRepository;
        private ISubmisionContestRepository _submisionContestRepository;

        public ContestController(IContestRepository contestRepository, IUserContestRepository userContestRepository,
                                 IProblemContestRepository problemContestRepository, ISubmisionContestRepository submisionContestRepository,
                                 IAnnouncementRepository announcementRepository)
        {
            _contestRepository = contestRepository;
            _userContestRepository = userContestRepository;
            _problemContestRepository = problemContestRepository;
            _submisionContestRepository = submisionContestRepository;
            _announcementRepository = announcementRepository;
        }
        public IActionResult Index()
        {
            var lst = _contestRepository.GetAll().ToList();
            return View(lst);
        }
    }
}