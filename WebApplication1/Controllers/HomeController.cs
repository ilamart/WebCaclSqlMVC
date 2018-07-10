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
                if (pageData.TotalPages != 1) { pageData.TotalPages++; }
            notes.Sort(delegate (History h1, History h2)
            { return h2.CreatedDateTime.CompareTo(h1.CreatedDateTime); });
            return notes.Skip((pageData.PageNumber - 1) * pageData.NotesPerPage).Take(pageData.NotesPerPage).ToList();
        }

        public List<History> DoFilter(List<History> notes, PageData page)
        {
            if (!String.IsNullOrEmpty(page.PreviousSearchExpression))
                notes = notes.Where(s => s.Expression.Contains(page.PreviousSearchExpression)).ToList();
            if (!String.IsNullOrEmpty(page.PreviousSearchHost))
                notes = notes.Where(s => s.Host.Contains(page.PreviousSearchHost)).ToList();
            return notes;
        }

        public IActionResult Index(PageData page, int numberPage)
        {
            page.Histories = SortedByDate(DoFilter(_context.Histories.ToList(), page).ToList(), page);
            GetAnswerFilter(page);
            return View(page);
        }

        public void GetAnswerFilter (PageData page)
        {
            page.AnswerFilter = new System.Text.StringBuilder();
            if (page.PreviousSearchExpression != null)
                page.AnswerFilter.AppendLine("Фильтрация по выражению (" + page.PreviousSearchExpression + ") ");
            if (page.PreviousSearchHost != null)
                page.AnswerFilter.AppendLine("Фильтрация по хосту (" + page.PreviousSearchHost + ") ");
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
                var calculation = new StringCalc();
                var note = new History
                {
                    Expression = page.Expression
                };
                note.Result = calculation.DoCalculation(note.Expression).ToString();
                note.Host = Request.Host.ToString();
                note.CreatedDateTime = DateTime.Now;
                _context.Histories.Add(note);
                _context.SaveChanges();
                page.PageNumber = 0;
            }

            var notes = _context.Histories.ToList();
            if (action == "Search")
            {
                page.PreviousSearchExpression = page.NewSearchExpression;
                page.PreviousSearchHost = page.NewSearchHost;
                notes = SortedByDate(DoFilter(notes, page), page);
                page.Histories = notes;
                page.PageNumber = 0;
                GetAnswerFilter(page);
                return View("Index", page);
            }

            if (action == "Previous")
            {
                if (page.PageNumber <= 0)
                {
                    page.PageNumber = 0;
                    page.Histories = SortedByDate(DoFilter(notes, page).ToList(), page); ;
                    GetAnswerFilter(page);
                    return View("Index", page);
                }
                else
                {
                    --page.PageNumber;
                    page.Histories = SortedByDate(DoFilter(notes, page).ToList(), page); ;
                    GetAnswerFilter(page);
                    return View("Index", page);
                }
            }

            if (action == "Next")
            {
                if (page.PageNumber >= page.TotalPages)
                {
                    page.PageNumber = page.TotalPages;
                    page.Histories = SortedByDate(DoFilter(notes, page).ToList(), page);
                    GetAnswerFilter(page);
                    return View("Index", page);
                }
                else
                {
                    ++page.PageNumber;
                    page.Histories = SortedByDate(DoFilter(notes, page).ToList(), page);
                    GetAnswerFilter(page);
                    return View("Index", page);
                }
            }

            page.Histories = SortedByDate(DoFilter(notes, page).ToList(), page);
            return View("Index", page);
        }
    }
}
