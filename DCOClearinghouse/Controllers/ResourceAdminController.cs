using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using DCOClearinghouse.Data;
using DCOClearinghouse.Models;
using DCOClearinghouse.ViewModels;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

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
        public async Task<IActionResult> Index(int? searchCategoryId, int? searchTypeId, ResourceStatus? searchStatus)
        {
            ViewData["CategoryDropdownList"] = GetCategorySelectList(searchCategoryId, allowAddNew: false);
            ViewData["TypeDropdownList"] = GetTypeSelectList(searchTypeId, allowAddNew: false);
            ViewData["StatusDropdownList"] = GetStatusSelectList(searchStatus);
            ViewData["CurrentSearchType"] = searchTypeId;
            ViewData["CurrentSearchCategory"] = searchCategoryId;
            ViewData["CurrentStatus"] = searchStatus;

            var resources = _context.Resources.AsNoTracking();


            if (searchTypeId != null)
            {
                resources = resources.Where(r => r.TypeID == searchTypeId);
            }

            if (searchCategoryId != null)
            {
                resources = resources.Where(r => r.CategoryID == searchCategoryId);
            }

            if (searchStatus != null)
            {
                searchStatus = (ResourceStatus)searchStatus;
                resources = resources.Where(r=> r.Status == searchStatus);
            }

            return View(await resources.Include(r => r.Category).Include(r => r.Type).ToListAsync());
        }

        // GET: ResourcesController/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources
                .AsNoTracking()
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
        public async Task<IActionResult> Create([Bind("Resource, NewCategoryName, NewTypeName")] ResourceAdminViewModel newResourceVM)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    newResourceVM.Resource.CreateDate = DateTime.UtcNow;
                    var newResource = newResourceVM?.Resource;
                    if (newResource == null)
                        throw new Exception("resource creates");

                    // TODO: prevent overposting

                    // handle new category
                    var newCategoryName = newResourceVM.NewCategoryName;
                    if (!string.IsNullOrEmpty(newCategoryName))
                    {
                        var newCategory = new ResourceCategory
                        {
                            CategoryName = newCategoryName
                        };
                        _context.ResourceCategories.Add(newCategory);
                        await _context.SaveChangesAsync();

                        newResource.CategoryID = newCategory.ID;
                    }

                    // handle new type
                    var newTypeName = newResourceVM.NewTypeName;
                    if (!string.IsNullOrEmpty(newTypeName))
                    {
                        var newType = new ResourceType
                        {
                            TypeName = newTypeName
                        };
                        _context.ResourceTypes.Add(newType);
                        await _context.SaveChangesAsync();

                        newResource.TypeID = newType.ID;
                    }

                    _context.Add(newResource);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. " +
                                             "Try again, and if the problem persists " +
                                             "see your system administrator.");
            }
            ViewData["CategoryDropdownList"] = GetCategorySelectList();
            ViewData["TypeDropdownList"] = GetTypeSelectList();
            return View();
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
            ViewData["CategoryID"] = GetCategorySelectList(resource.CategoryID);
            ViewData["ResourceTypeList"] = GetTypeSelectList(resource.TypeID);

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
                .AsNoTracking()
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

        private List<SelectListItem> GetCategorySelectList(int? currentCategoryId = null, bool allowAddNew = true)
        {
            List<SelectListItem> list = new SelectList(_context.ResourceCategories, "ID", "CategoryName", currentCategoryId).ToList();

            if (allowAddNew)
                list.Add(new SelectListItem
                {
                    Text = "Add new",
                    Value = null
                });
            return list;
        }

        private List<SelectListItem> GetTypeSelectList(int? currentTypeId = null, bool allowAddNew = true)
        {
            List<SelectListItem> list =
                new SelectList(_context.ResourceTypes, "ID", "TypeName", currentTypeId).ToList();

            if (allowAddNew)
                list.Add(new SelectListItem
                {
                    Text = "Add new",
                    Value = null
                });
            return list;
        }

        private SelectList GetStatusSelectList(ResourceStatus? status){
            var allStatuses = from ResourceStatus s in Enum.GetValues(typeof(ResourceStatus))
                select new { ID = s, Name = s.ToString()};
            return new SelectList(allStatuses, "ID", "Name", status);
        }

        #endregion
    }
}
