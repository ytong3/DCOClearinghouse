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
    public class ResourcesController : Controller
    {
        private readonly ResourceContext _context;

        public ResourcesController(ResourceContext context)
        {
            _context = context;
        }

        // GET: Resources
        public async Task<IActionResult> Index()
        {
            // showcase sample categories on Index
            var resourceByCategory = _context.Resources.AsNoTracking()
                                .Include(r=>r.Category).Where(r=>r.CategoryID>=1 && r.CategoryID<=12)
                                .GroupBy(r=>r.Category)
                                .ToDictionaryAsync(g=>g.Key, g=>g.Take(10).ToList());

            var resourceDictionary = await resourceByCategory;

            return View(resourceDictionary);
        }

        // Get: Resources/Category/3
        public async Task<IActionResult> Category(int? id)
        {
            var resourceCategory = await _context.ResourceCategories
                .AsNoTracking()
                .Include(c => c.Resources)
                .SingleOrDefaultAsync(c => c.ID == id);

            if (resourceCategory == null)
            {
                throw new InvalidOperationException("resource category.");
            }


            //TODO: make use of ViewData to pass in multiple models
            ViewData["CategoryName"] = resourceCategory.CategoryName;

            return View(resourceCategory.Resources);
        }

        // GET: Resources/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources
                .AsNoTracking()
                .Include(r => r.Category)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        // GET: Resources/Create
        public IActionResult Create()
        {
            ViewData["CategoryID"] = new SelectList(_context.ResourceCategories, "ID", "CategoryName");
            return View();
        }

        // POST: Resources/Create
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

        // GET: Resources/Edit/5
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

        // POST: Resources/Edit/5
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

        // GET: Resources/Delete/5
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

        // POST: Resources/Delete/5
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
