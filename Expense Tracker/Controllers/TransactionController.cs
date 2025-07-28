using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Expense_Tracker.Models;

namespace Expense_Tracker.Controllers
{
    public class TransactionController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TransactionController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Transaction
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.transactions.Include(t => t.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Transaction/Details/5
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var transaction = await _context.transactions
        //        .Include(t => t.Category)
        //        .FirstOrDefaultAsync(m => m.TransactionId == id);
        //    if (transaction == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(transaction);
        //}

        // GET: Transaction/Create
        public IActionResult AddOrEdit(int id =0)
        {
            //ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "CategoryId");

            PopulateCategoryList();
            if (id == 0)
                return View(new Transaction());
           else
                return View(_context.transactions.Find(id));


        }

        // POST: Transaction/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrEdit([Bind("TransactionId,CategoryId,Note,Date,Amount")] Transaction transaction)
        {
            Category? obj = _context.categories.FirstOrDefault(x => x.CategoryId == transaction.CategoryId);
            transaction.Category = obj;
            if (ModelState.IsValid)
            {
                if (transaction.TransactionId == 0)
                {
                    _context.Add(transaction);
                }
                else
                {
                    _context.transactions.Update(transaction);
                }

                    await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            //ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "CategoryId", transaction.CategoryId);

            PopulateCategoryList();
            return View(transaction);
        }

        // GET: Transaction/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var transaction = await _context.transactions.FindAsync(id);
        //    if (transaction == null)
        //    {
        //        return NotFound();
        //    }
        //    ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "CategoryId", transaction.CategoryId);
        //    return View(transaction);
        //}

        //// POST: Transaction/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("TransactionId,CategoryId,Note,Date,Amount")] Transaction transaction)
        //{
        //    if (id != transaction.TransactionId)
        //    {
        //        return NotFound();
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(transaction);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!TransactionExists(transaction.TransactionId))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    ViewData["CategoryId"] = new SelectList(_context.categories, "CategoryId", "CategoryId", transaction.CategoryId);
        //    return View(transaction);
        //}

        // GET: Transaction/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var transaction = await _context.transactions
        //        .Include(t => t.Category)
        //        .FirstOrDefaultAsync(m => m.TransactionId == id);
        //    if (transaction == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(transaction);
        //}

        // POST: Transaction/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var transaction = await _context.transactions.FindAsync(id);
            if (transaction != null)
            {
                _context.transactions.Remove(transaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //private bool TransactionExists(int id)
        //{
        //    return _context.transactions.Any(e => e.TransactionId == id);
        //}

        [NonAction]
        public void PopulateCategoryList()
        {
            var CategoryCollection = _context.categories.ToList();
            Category DefaultCategory = new Category()
            {
                CategoryId = 0,
                Title = "Choose a category"
            };

            CategoryCollection.Insert(0, DefaultCategory);

            ViewBag.categories = CategoryCollection;

        }
    }
}
