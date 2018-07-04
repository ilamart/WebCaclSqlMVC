using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using AppCalculate;
using System.Linq;
using System.Collections.Generic;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly CalculationDbContext calculationDbContext;

        private static int CompareDateTime(History first, History second)
        {
            return second.CreatedDateTime.CompareTo(first.CreatedDateTime);
        }

        public static List<History> SortedByDate(List<History> histories)
        {
            return histories.ToList().OrderByDescending(x => x.CreatedDateTime).ToList();
        }

        public HomeController(CalculationDbContext logBase)
        {
            calculationDbContext = logBase;
        }

        public IActionResult Index(string searchExpression, string searchHost)
        {
            var notes = from m in calculationDbContext.Histories
                         select m;

            if (!String.IsNullOrEmpty(searchExpression))
                notes = notes.Where(s => s.Expression.Contains(searchExpression));

            ViewBag.searchExpression = searchExpression;

            if (!String.IsNullOrEmpty(searchHost))
                notes = notes.Where(s => s.Host.Contains(searchHost));

            ViewBag.searchHost = searchHost;

            return View(SortedByDate(notes.ToList()));
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        [HttpPost]
        public IActionResult Submit(History noteConnection)
        {
            var calculation = new StringCalc();
            noteConnection.Result = calculation.DoCalculation(noteConnection.Expression).ToString();
            noteConnection.Host = Request.Host.ToString();
            noteConnection.CreatedDateTime = DateTime.Now;
            calculationDbContext.Add(noteConnection);
            calculationDbContext.SaveChanges();
            return View("Index", SortedByDate(calculationDbContext.Histories.ToList()));
        }
    }
}
