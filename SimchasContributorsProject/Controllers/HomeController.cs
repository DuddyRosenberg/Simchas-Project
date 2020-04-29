using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SimchasContributorsProject.Models;
using SimchasContributorsProject.Data;

namespace SimchasContributorsProject.Controllers
{
    public class HomeController : Controller
    {
        private Database _database = new Database("Data Source=.\\sqlexpress69;Initial Catalog=SimchasContributorsProject;Integrated Security=True");
        public IActionResult Simchas()
        {
            return View(new SimchasViewModel
            {
                Simchas = _database.GetSimchas(),
                TotalContributors = _database.GetTotalContributors(),
                SuccessMessage = (string)TempData["success-message"],
            });
        }
        public IActionResult Contributors()
        {
            return View(new ContributorsViewModel
            {
                Contributors = _database.GetContributors(),
                SuccessMessage = (string)TempData["success-message"],
            });
        }
        [HttpPost]
        public IActionResult AddSimcha(Simcha simcha)
        {
            var simchaId = _database.AddSimcha(simcha);
            foreach (var contributor in _database.GetContributors())
            {
                if (contributor.AlwaysInclude)
                {
                    _database.AddContribution(new Contribution
                    {
                        Amount = 5,
                        ContributorID = contributor.ID,
                    }, simcha.ID);
                }
            }
            TempData["success-message"] = "Simcha Added Successfully";
            return Redirect("/home/Simchas");
        }
        [HttpPost]
        public IActionResult AddContributor(Contributor contributor, decimal amount, DateTime dateCreated)
        {
            _database.AddDeposit(_database.AddContributor(contributor), amount, dateCreated);
            TempData["success-message"] = "Contributor Added Successfully";
            return Redirect("/home/contributors");
        }
        [HttpPost]
        public IActionResult AddDeposit(decimal amount, DateTime date, int contributorID)
        {
            _database.AddDeposit(contributorID, amount, date);
            TempData["success-message"] = "Deposit Added Successfully";
            return Redirect("/home/contributors");
        }
        public IActionResult ShowHistory(int id)
        {
            return View(new ShowHistoryViewModel
            {
                Contributor = _database.GetContributor(id),
                Transections = _database.GetTransections(id)
            });
        }
        public IActionResult Contributions(int id)
        {
            return View(new ContributionsViewModel
            {
                Contributors = _database.GetContributorsWithContributions(id),
                Simcha = _database.GetSimchaByID(id)
            });
        }
        [HttpPost]
        public IActionResult AddContributions(int simchaID, List<Contribution> contributions)
        {
            _database.DeleteAndAddContributions(simchaID, contributions);
            TempData["success-message"] = "Contributions Added Successfully";
            return Redirect("/home/simchas");
        }
    }
}