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
        private readonly CalculationDbContext _context;

        private static int CompareDateTime(History first, History second)
        {
            return second.CreatedDateTime.CompareTo(first.CreatedDateTime);
        }

        public HomeController(CalculationDbContext context)
        {
            _context = context;
        }

        public List<History> SortedByDate(List<History> notes, PageData pageData)
        {
            pageData.TotalPages = notes.Count / pageData.NotesPerPage;
            if (notes.Count % pageData.NotesPerPage > 0)
            {
                if (pageData.TotalPages != 1)
                {
                    pageData.TotalPages++;
                }
            }
            notes.Sort((History h1, History h2) =>
            h2.CreatedDateTime.CompareTo(h1.CreatedDateTime));
            return notes.Skip((pageData.PageNumber - 1) * pageData.NotesPerPage).Take(pageData.NotesPerPage).ToList();
        }

        public IActionResult Index(PageData page, int numberPage)
        {
            page.Histories = SortedByDate(_context.Histories.ToList(), page);
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
                if (page.PageNumber <= 0)
                {
                    page.PageNumber = 0;
                }
                else
                {
                    --page.PageNumber;
                }
            }

            if (action == "Next")
            {
                if (page.PageNumber >= page.TotalPages)
                {
                    page.PageNumber = page.TotalPages;
                }
                else
                {
                    ++page.PageNumber;
                }
            }
            
            if ((String.IsNullOrEmpty(page.PreviousSearchExpression)) && (String.IsNullOrEmpty(page.PreviousSearchHost)))
            {
                page.Histories = SortedByDate(_context.Histories.ToList(), page);
            }
            else
            {
                if (!String.IsNullOrEmpty(page.PreviousSearchExpression))
                    page.Histories = SortedByDate(_context.Histories.Where(s => s.Expression.Contains(page.PreviousSearchExpression)).ToList(), page);
                if (!String.IsNullOrEmpty(page.PreviousSearchHost))
                    page.Histories = SortedByDate(_context.Histories.Where(s => s.Host.Contains(page.PreviousSearchHost)).ToList(), page);
            }
            return View("Index", page);
        }
    }
}
