using DCOClearinghouse.Data;
using DCOClearinghouse.Models;
using DCOClearinghouse.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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
            return View();
        }

        // GET: ResourcesController/Create
        public IActionResult Create()
        {
            ViewData["CategoryDropdownListAddAllowed"] = GetCategorySelectListDFSOrdered();
            ViewData["CategoryDropdownList"] = GetCategorySelectListDFSOrdered(currentCategoryId: null, allowAddNew: false);
            ViewData["TypeDropdownList"] = GetTypeSelectList();
            return View();
        }

        // POST: ResourcesController/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Resource, NewCategory, NewTypeName")] ResourceAdminViewModel newResourceVM)
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
                    var newCategory = newResourceVM.NewCategory;
                    if (!string.IsNullOrEmpty(newCategory.CategoryName))
                    {
                        if (newCategory.ParentCategoryID != null)
                        {
                            var parentCategory = await
                                _context.ResourceCategories
                                    .AsNoTracking()
                                    .SingleOrDefaultAsync(c=>c.ID == newCategory.ParentCategoryID);

                            if (parentCategory == null)
                                throw new Exception("Category doesn't exist.");
                            newCategory.Depth = parentCategory.Depth + 1;
                        }
                        else
                        {
                            newCategory.Depth = 0;
                        }
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
            ViewData["CategoryDropdownListAddAllowed"] = GetCategorySelectListDFSOrdered();
            ViewData["CategoryDropdownList"] = GetCategorySelectListDFSOrdered(currentCategoryId:null, allowAddNew:false);
            ViewData["TypeDropdownList"] = GetTypeSelectList();
            return View();
        }

        // GET: ResourcesController/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //requires authentication
            if (id == null)
            {
                return NotFound();
            }

            var resource = await _context.Resources.FindAsync(id);
            if (resource == null)
            {
                return NotFound();
            }
            ViewData["CategoryID"] = GetCategorySelectListDFSOrdered(resource.CategoryID);

            return View(resource);
        }

        // POST: ResourcesController/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ID,Subject,Description,TypeID,CategoryID,BadlinkVotes,CreateDate,Status")] Resource resource)
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

        public async Task<IActionResult> BrowseByCategory()
        {
            var allCategories = await _context.ResourceCategories.AsNoTracking().
                Include(c=>c.ChildrenCategories)
                .Include(c=>c.Resources)
                .ToListAsync();
            return View(allCategories);
        }

        private bool ResourceExists(int id)
        {
            return _context.Resources.Any(e => e.ID == id);
        }

        #region Dropdown list helpers

        private List<SelectListItem> GetCategorySelectListDFSOrdered(int? currentCategoryId = null, bool allowAddNew = false)
        {
            // get all
            List<ResourceCategory> allCategories = _context.ResourceCategories
                .Include(c => c.ChildrenCategories)
                .Include(c=>c.Resources)
                .ToList();
            var rootCategories = from rootCategory in allCategories where rootCategory.Depth == 0 select rootCategory;
            
            List<ResourceCategory> dfsOrdered = new List<ResourceCategory>();
            foreach (var rootCategory in rootCategories)
            {
                var subTree = DepthFirstTranversalCategories(rootCategory);
                if (subTree != null)
                    dfsOrdered.AddRange(subTree);
            }

            var dropDownItems = from category in dfsOrdered
                select new
                {
                    category.ID,
                    CategoryName = string.Concat(Enumerable.Repeat("----", category.Depth))+category.CategoryName+$"({category.Resources.Count})"
                };


            List<SelectListItem> list = new SelectList(dropDownItems, "ID", "CategoryName", currentCategoryId).ToList();

            if (allowAddNew)
                list.Insert(0,new SelectListItem
                {
                    Text = "Add new",
                    Value = null
                });
            return list;
        }

        private List<ResourceCategory> DepthFirstTranversalCategories(ResourceCategory category)
        {
            if (category == null || category.Depth > 3)
            {
                return null;
            }

            var result = new List<ResourceCategory> {category};

            foreach (var childCategory in category.ChildrenCategories)
            {
                var subTreeDFSTraversal = DepthFirstTranversalCategories(childCategory);
                if (subTreeDFSTraversal!=null)
                    result.AddRange(subTreeDFSTraversal);
            }

            return result;
        }

        private List<SelectListItem> GetTypeSelectList(int? currentTypeId = null, bool allowAddNew = true)
        {
            List<SelectListItem> list =
                new SelectList(_context.ResourceTypes, "ID", "TypeName", currentTypeId).ToList();

            if (allowAddNew)
                list.Insert(0, new SelectListItem
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
