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
    public class ResourceAdminController : Controller
    {
        private readonly ResourceContext _context;

        public ResourceAdminController(ResourceContext context)
        {
            _context = context;
        }

        // GET: ResourcesController
        public async Task<IActionResult> Index()
        {
            var resourceContext = _context.Resources.Include(r => r.Category).Include(r => r.Type);
            return View(await resourceContext.ToListAsync());
        }

        // GET: ResourcesController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources
                .Include(r => r.Category)
                .Include(r=>r.Type)
                .FirstOrDefaultAsync(m => m.ID == id);
            if (resource == null)
            {
                return NotFound();
            }

            return View(resource);
        }

        // GET: ResourcesController/Create
        public IActionResult Create()
        {
            ViewData["CategoryDropdownList"] = GetCategorySelectList();
            ViewData["TypeDropdownList"] = GetTypeSelectList();
            return View();
        }

        // POST: ResourcesController/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ID,Subject,Content,CategoryID")] Resource resource)
        {
            resource.CreateDate = DateTime.UtcNow;
            if (ModelState.IsValid)
            {
                _context.Add(resource);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CategoryID"] = GetCategorySelectList();
            return View(resource);
        }

        // GET: ResourcesController/Edit/5
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
            ViewData["CategoryID"] = GetCategorySelectList(resource);
            ViewData["ResourceTypeList"] = GetTypeSelectList(resource);

            return View(resource);
        }

        // POST: ResourcesController/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Subject,Content,TypeID,CategoryID,BadlinkVotes,CreateDate,Status")] Resource resource)
        {
            //TODO: implement the anti-overposting mechanism in the Contoso University example.
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
            return View(resource);
        }

        // GET: ResourcesController/Delete/5
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

        // POST: ResourcesController/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var resource = await _context.Resources.FindAsync(id);
            _context.Resources.Remove(resource);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ResourceExists(int id)
        {
            return _context.Resources.Any(e => e.ID == id);
        }

        #region Dropdown list helpers

        private SelectList GetCategorySelectList(Resource resource = null)
        {
            return new SelectList(_context.ResourceCategories, "ID", "CategoryName", resource?.CategoryID);
        }

        private SelectList GetTypeSelectList(Resource resource = null)
        {
            return new SelectList(_context.ResourceTypes, "ID", "TypeName", resource?.TypeID);
        }

        #endregion
    }
}
