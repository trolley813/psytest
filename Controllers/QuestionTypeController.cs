using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using psytest.Models;

namespace psytest.Controllers
{
    public class QuestionTypeController : Controller
    {
        private readonly TestContext _context;

        public QuestionTypeController(TestContext context)
        {
            _context = context;
        }

        // GET: QuestionType
        public async Task<IActionResult> Index()
        {
            return View(await _context.QuestionTypes.ToListAsync());
        }

        // GET: QuestionType/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionType = await _context.QuestionTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (questionType == null)
            {
                return NotFound();
            }

            return View(questionType);
        }

        // GET: QuestionType/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: QuestionType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id")] QuestionType questionType)
        {
            if (ModelState.IsValid)
            {
                _context.Add(questionType);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(questionType);
        }

        // GET: QuestionType/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionType = await _context.QuestionTypes.FindAsync(id);
            if (questionType == null)
            {
                return NotFound();
            }
            return View(questionType);
        }

        // POST: QuestionType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id")] QuestionType questionType)
        {
            if (id != questionType.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(questionType);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionTypeExists(questionType.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(questionType);
        }

        // GET: QuestionType/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questionType = await _context.QuestionTypes
                .FirstOrDefaultAsync(m => m.Id == id);
            if (questionType == null)
            {
                return NotFound();
            }

            return View(questionType);
        }

        // POST: QuestionType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var questionType = await _context.QuestionTypes.FindAsync(id);
            _context.QuestionTypes.Remove(questionType);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool QuestionTypeExists(int id)
        {
            return _context.QuestionTypes.Any(e => e.Id == id);
        }
    }
}
