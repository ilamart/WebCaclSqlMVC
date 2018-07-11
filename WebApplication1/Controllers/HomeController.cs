using System;
using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using WebApplication1.Models;
using AppCalculate;
using System.Linq;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Extensions.Internal;

namespace WebApplication1.Controllers
{
    public class HomeController : Controller
    {
        private readonly CalculationDbContext _context;

        private static int CompareDateTime(History first, History second)
        {
            return second.CreatedDateTime.CompareTo(first.CreatedDateTime);
        }

        public HomeController(CalculationDbContext context)
        {
            _context = context;
        }

        public IActionResult Index(PageData page, int numberPage)
        {
            IQueryable<History> result = _context.Histories;
            page.TotalPages = result.Count() / page.NotesPerPage;
            if (result.Count() % page.NotesPerPage > 0)
            {
                if (page.TotalPages != 1)
                {
                    page.TotalPages++;
                }
            }
            page.Histories = result.OrderByDescending(s => s.CreatedDateTime).Skip((page.PageNumber) * page.NotesPerPage).Take(page.NotesPerPage).ToList();
            return View(page);
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        [HttpPost]
        public IActionResult Submit(PageData page, string action)
        {
            

            if (action == "Evaluate")
            {
                var note = new History
                {
                    Expression = page.Expression
                };
                var calculation = new StringCalc();
                note.Result = calculation.DoCalculation(note.Expression).ToString();
                note.Host = Request.Host.ToString();
                note.CreatedDateTime = DateTime.Now;
                _context.Histories.Add(note);
                _context.SaveChanges();
                page.PageNumber = 0;
            }

            if (action == "Search")
            {
                page.PreviousSearchExpression = page.NewSearchExpression;
                page.PreviousSearchHost = page.NewSearchHost;
                page.PageNumber = 0;
            }

            if (action == "Previous")
            {
                --page.PageNumber;
                if (page.PageNumber == 0)
                    page.PageNumber = 0; 
            }

            if (action == "Next")
            {
                ++page.PageNumber;
                if (page.PageNumber == page.TotalPages)
                    page.PageNumber = page.TotalPages;
            }

            IQueryable<History> result = _context.Histories;
            if (page.PreviousSearchExpression != null && page.PreviousSearchExpression != "")
                result = result.Where(s => s.Expression.Contains(page.PreviousSearchExpression));
            if (page.PreviousSearchHost != null && page.PreviousSearchHost != "")
                result = result.Where(s => s.Host.Contains(page.PreviousSearchHost));

            page.TotalPages = result.Count() / page.NotesPerPage;
            if (result.Count() % page.NotesPerPage > 0)
            {
                if (page.TotalPages != 1)
                {
                    page.TotalPages++;
                }
            }
            page.Histories = result.OrderByDescending(s => s.CreatedDateTime).Skip((page.PageNumber) * page.NotesPerPage).Take(page.NotesPerPage).ToList();
            return View("Index", page);
        }
    }
}