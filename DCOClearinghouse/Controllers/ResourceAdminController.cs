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
using Microsoft.Extensions.Configuration;
using Tag = DCOClearinghouse.Models.Tag;

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
                        throw new Exception("editedResource creates");

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

            var resource = await _context.Resources
                .Include(r=>r.ResourceTags)
                .ThenInclude(rt=>rt.Tag)
                .FirstOrDefaultAsync(r=>r.ID == id);

            if (resource == null)
            {
                return NotFound();
            }

            var tags = string.Join(",", from t in resource.ResourceTags select t.Tag.Name);

            
            ViewData["CategoryID"] = GetCategorySelectListDFSOrdered(resource.CategoryID);

            return View(new AdminEditViewModel()
            {
                Resource = resource,
                Tags = tags
            });
        }

        // POST: ResourcesController/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Resource, Tags")] AdminEditViewModel editedResource)
        {
            //TODO: implement the anti-overposting mechanism in the Contoso University example.
            if (id != editedResource.Resource.ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var resourceToUpdate = editedResource.Resource;
                    _context.Update(editedResource.Resource);

                    // process tags
                    var currentTagAssociations = await _context.ResourceTags
                        .Include(rt=>rt.Tag)
                        .AsNoTracking()
                        .Where(rt => rt.ResourceID == id)
                        .ToListAsync();

                    //existing tags
                    var tagsBefore = currentTagAssociations.Select(rt => rt.Tag.Name).ToHashSet();
                    var tagsAfter = editedResource.Tags.Split(",").Select(p => p.Trim()).ToList().ToHashSet();


                    var tagNamesToRemoveFromResource = tagsBefore.Except(tagsAfter);
                    if (tagNamesToRemoveFromResource.Count() != 0)
                    {
                        var tagsToRemove = from t in currentTagAssociations where tagNamesToRemoveFromResource.Contains(t.Tag.Name)
                                            select t;
                        _context.ResourceTags.RemoveRange(tagsToRemove);
                    }

                    var tagNamesToAddToResource = new HashSet<string>(tagsAfter.Except(tagsBefore));

                    // add unknown tags
                    if (tagNamesToAddToResource.Count() != 0)
                    {
                        var existingTags = _context.Tags.AsNoTracking().Select(t => t.Name);
                        var unknownTagNames = tagNamesToAddToResource.Except(new HashSet<string>(existingTags));
                        foreach (var unknownTagName in unknownTagNames)
                        {
                            var tagEntity = _context.Tags.Add(new Tag {Name = unknownTagName});
                            await _context.SaveChangesAsync();
                        }
                    }

                    // add tagNamesToAddToResource to the resource
                    foreach (var tagName in tagNamesToAddToResource)
                    {
                        var tag = _context.Tags.AsNoTracking().FirstOrDefault(t => t.Name == tagName);
                        _context.ResourceTags.Add(new ResourceTag()
                        {
                            ResourceID = resourceToUpdate.ID,
                            TagID = tag.ID
                        });
                    }
                    


                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ResourceExists(editedResource.Resource.ID))
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
            return View(editedResource);
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

        private bool TagExists(string tagName)
        {
            return _context.Tags.Any(t => t.Name == tagName);
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

            var dropDownItems = from category in dfsOrdered orderby category.CategoryName!="Uncategorized"
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
