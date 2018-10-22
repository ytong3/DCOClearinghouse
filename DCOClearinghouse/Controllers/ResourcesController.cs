using DCOClearinghouse.Data;
using DCOClearinghouse.Models;
using DCOClearinghouse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;

namespace DCOClearinghouse.Controllers
{
    public class ResourcesController : Controller
    {
        private readonly ResourceContext _context;
        private int _uncategorizedId;

        public ResourcesController(ResourceContext context, IConfiguration config)
        {
            _context = context;
            _uncategorizedId = int.Parse(config["UncategorizedID"]);
        }

        public IActionResult Index(int? page)
        {
            //return the latest
            ViewData["pageNumber"] = page ?? 1;
            ViewData["latestTabActive"] = "active";
            return View();
        }

        // GET: Resources
        public async Task<IActionResult> AllCategories()
        {
            ViewData["classifiedTabActive"] = "active";
            var allRootCategories = await _context.ResourceCategories
                .AsNoTracking()
                .Where(c => c.Depth == 0 && c.ID != _uncategorizedId)
                .Include(c => c.ChildrenCategories)
                .ThenInclude(childCategory => childCategory.Resources)
                .ToListAsync();

            return View(allRootCategories);
        }

        // Get: Resources/Category/3
        public async Task<IActionResult> Category(int? id, int? page)
        {
            ViewData["classifiedTabActive"] = "active";
            var resourceCategory = await _context.ResourceCategories
                .AsNoTracking()
                .Include(c => c.ChildrenCategories)
                .Include(c=>c.ParentCategory)
                .Include(c => c.Resources)
                .SingleOrDefaultAsync(c => c.ID == id);

            if (resourceCategory == null)
            {
                throw new InvalidOperationException("resource category.");
            }

            ViewData["pageNumber"] = page??1;


            //TODO: make use of ViewData to pass in multiple models

            return View(resourceCategory);
        }

        public async Task<IActionResult> Tagcloud()
        {
            ViewData["tagCloudTabActive"] = "active";
            var allTags = await _context.Tags
                .AsNoTracking()
                .Include(t => t.ResourceTags)
                .OrderByDescending(t => t.ResourceTags.Count)
                .ToListAsync();

            return View(allTags);
        }

        public async Task<IActionResult> Tag(int? id, int? page)
        {
            ViewData["tagCloudTabActive"] = "active";
            ViewData["pageNumber"] = page??1;

            var resourceTag = await _context.Tags.AsNoTracking()
                .FirstOrDefaultAsync(t=>t.ID == id);

            return View(resourceTag);
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
                .Include(r => r.ResourceTags)
                .ThenInclude(t => t.Tag)
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
            ViewData["latestTabActive"] = "active";
            return View();
        }

        // POST: Resources/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Resource, ContactProvided")] ResourceViewModel resourceVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var resource = resourceVM.Resource;

                    resource.CategoryID = _uncategorizedId;
                    resource.CreateDate = DateTime.UtcNow;
                    resource.Status = ResourceStatus.New;
                    if (resourceVM.ContactProvided)
                    {
                        // perform some validation ...
                    }
                    else
                    {
                        resource.Contact = null;
                    }

                    _context.Add(resource);
                    await _context.SaveChangesAsync();

                    //TODO: show a confirmation page upon successful submission.
                    return RedirectToAction(nameof(CreatedConfirmed));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists " +
                                             "see your system administrator.");
            }

            ViewData["CategoryID"] = new SelectList(_context.ResourceCategories, "ID", "CategoryName");
            ViewData["TypeID"] = new SelectList(_context.ResourceTypes, "ID", "TypeName");
            return View();
        }

        public async Task<IActionResult> CreatedConfirmed()
        {
            return View();
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
        public async Task<IActionResult> Edit(int id,
            [Bind("ID,Subject,Description,CategoryID,BadlinkVotes,CreateDate,Status")]
            Resource resource)
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
            //TODO: this shouldn't be a GET
            var resource = await _context.Resources.FindAsync(id);
            resource.BadlinkVotes++;

            await _context.SaveChangesAsync();
            // TODO: give the user some feedback like greying out the link after reporting.
            return RedirectToAction(nameof(Index));
        }

        public IActionResult GetResourceTable(int? page)
        {
            return ViewComponent("ResourceTable", new {pageNumber = page??1});
        }

        private bool ResourceExists(int id)
        {
            return _context.Resources.Any(e => e.ID == id);
        }
    }
}