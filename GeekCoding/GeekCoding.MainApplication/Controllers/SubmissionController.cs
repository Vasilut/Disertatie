using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GeekCoding.Data.Models;
using GeekCoding.Repository.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace GeekCoding.MainApplication.Controllers
{
    //it will be api
    public class SubmissionController : Controller
    {
        private ISubmisionRepository _submisionRepository;

        public SubmissionController(ISubmisionRepository submisionRepository)
        {
            _submisionRepository = submisionRepository;
        }


        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var submisionList = await _submisionRepository.GetAllAsync();
            return View(submisionList);
        }

        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        [HttpPost] //it will be api
        public async Task CreateSubmision([FromBody] Submision submision)
        {
            await _submisionRepository.AddAsync(submision);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var submision = await _submisionRepository.GetAsync(id);
            return View(submision);
        }
    }
}