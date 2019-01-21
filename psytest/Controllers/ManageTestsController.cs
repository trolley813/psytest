using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using psytest.Models;

namespace psytest.Controllers
{
    public class ManageTestsController : Controller
    {
        private readonly TestContext _context;

        public ManageTestsController(TestContext context)
        {
            _context = context;
        }

        // GET: ManageTests
        public async Task<IActionResult> Index()
        {
            return View(await _context.Tests.ToListAsync());
        }

        // GET: ManageTests/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }

        // POST: ManageTests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: ManageTests/ToggleHide/5
        public async Task<IActionResult> ToggleHide(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var test = await _context.Tests
                .FirstOrDefaultAsync(m => m.Id == id);
            if (test == null)
            {
                return NotFound();
            }

            return View(test);
        }

        // POST: ManageTests/ToggleHide/5
        [HttpPost, ActionName("ToggleHide")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleHideConfirmed(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            test.Hidden = !test.Hidden;
            _context.Update(test);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}