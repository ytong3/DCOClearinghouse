﻿using DCOClearinghouse.Data;
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
        public async Task<IActionResult> Index(
            int? searchCategoryId, 
            int? searchTypeId, 
            ResourceStatus? searchStatus, 
            string sortOrder,
            int? page)
        {
            ViewData["CategoryDropdownList"] = GetCategorySelectListDFSOrdered(searchCategoryId, allowAddNew: false);
            ViewData["TypeDropdownList"] = GetTypeSelectList(searchTypeId, allowAddNew: false);
            ViewData["StatusDropdownList"] = GetStatusSelectList(searchStatus);
            ViewData["CurrentSearchType"] = searchTypeId;
            ViewData["CurrentSearchCategory"] = searchCategoryId;
            ViewData["CurrentStatus"] = searchStatus;
            ViewData["CurrentSort"] = sortOrder;
            ViewData["TitleSortParam"] = string.IsNullOrEmpty(sortOrder) ? "title_desc" : "";
            ViewData["DateSortParam"] = string.Equals(sortOrder, "Date") ? "date_desc" : "Date";


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

            switch (sortOrder)
            {
                case "title_desc":
                    resources = resources.OrderByDescending(r => r.Subject);
                    break;
                case "Date":
                    resources = resources.OrderBy(r => r.CreateDate);
                    break;
                case "date_desc":
                    resources = resources.OrderByDescending(r => r.CreateDate);
                    break;
                default:
                    resources = resources.OrderBy(r => r.Subject);
                    break;
            }

            // load related data
            resources = resources.Include(r => r.Category).Include(r => r.Type);

            int pageSize = 20;
            //return View(await resources.Include(r => r.Category).Include(r => r.Type).ToListAsync());
            return View(await PaginatedList<Resource>.CreateAsync(resources, page ?? 1, pageSize));
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
            ViewData["ResourceTypeList"] = GetTypeSelectList(resource.TypeID);

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

        private List<SelectListItem> GetCategorySelectListDFSOrdered(int? currentCategoryId = null, bool allowAddNew = true)
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
                    CategoryName = string.Concat(Enumerable.Repeat("--", category.Depth))+category.CategoryName+$"({category.Resources.Count})"
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
