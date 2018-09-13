using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DCOClearinghouse.Data;
using DCOClearinghouse.Models;

namespace DCOClearinghouse.Controllers
{
    public class Resources : Controller
    {
        private readonly ResourceContext _context;

        public Resources(ResourceContext context)
        {
            _context = context;
        }

        // GET: ResourcesForUsers
        public async Task<IActionResult> Index()
        {
            // showcase sample categories on Index
            var resourceByCategory = _context.Resources.Include(r=>r.Category).Where(r=>r.CategoryID>=1 && r.CategoryID<=4)
                                .GroupBy(r=>r.Category.CategoryName)
                                .ToDictionaryAsync(g=>g.Key, g=>g.Take(10).ToList());

            var resourceDictionary = await resourceByCategory;

            return View(resourceDictionary);
        }

        // GET: ResourcesForUsers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources
                .Include(r => r.Category)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        // GET: ResourcesForUsers/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.ResourceCategories, "ID", "ID");
            return View();
        }

        // POST: ResourcesForUsers/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Subject,Content,CategoryID,BadlinkVotes,CreateDate,Status")] Resource resource)
        {
            if (ModelState.IsValid)
            {
                _context.Add(resource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryID"] = new SelectList(_context.ResourceCategories, "ID", "ID", resource.CategoryID);
            return View(resource);
        }

        // GET: ResourcesForUsers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = new SelectList(_context.ResourceCategories, "ID", "ID", resource.CategoryID);
            return View(resource);
        }

        // POST: ResourcesForUsers/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Subject,Content,CategoryID,BadlinkVotes,CreateDate,Status")] Resource resource)
        {
            if (id != resource.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(resource);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResourceExists(resource.ID))
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
            ViewData["CategoryID"] = new SelectList(_context.ResourceCategories, "ID", "ID", resource.CategoryID);
            return View(resource);
        }

        // GET: ResourcesForUsers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources
                .Include(r => r.Category)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        // POST: ResourcesForUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> ReportBadLink(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            resource.BadlinkVotes++;

            await _context.SaveChangesAsync();
            // TODO: give the user some feedback like greying out the link after reporting.
            return RedirectToAction(nameof(Details), new {id = id});
        }

        private bool ResourceExists(int id)
        {
            return _context.Resources.Any(e => e.ID == id);
        }
    }
}
