using System;
using System.Collections.Generic;
using System.Linq;
using DCOClearinghouse.Data;
using DCOClearinghouse.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseMaintenance
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Start to flatten the categories and keep the subcategories as tags.");

            //FlattenCategories("flattened_dco_resources");
            //RemoveEmptyCategories("flattened_dco_resources");
            var optionsForNewDb = new DbContextOptionsBuilder<ResourceContext>()
                .UseMySql($"Server=localhost;Port=3306;Database=flattened_dco_resources;Uid=root;Pwd=root;")
                .EnableSensitiveDataLogging()
                .Options;

            using (var context = new ResourceContext(optionsForNewDb))
            {
                TagMaintenance.MergeDuplicateTags(context);
                TagMaintenance.RemoveEmptyTags(context);
            }
        }

        private static void RemoveEmptyCategories(string database)
        {
            Console.WriteLine("==Start to Remove empty categories the resources ==");

            var optionsForNewDb = new DbContextOptionsBuilder<ResourceContext>()
                .UseMySql($"Server=localhost;Port=3306;Database={database};Uid=root;Pwd=root;")
                .EnableSensitiveDataLogging()
                .Options;

            using (var context = new ResourceContext(optionsForNewDb))
            {
                var secondLevelCategories = context.ResourceCategories.Include(c => c.Resources)
                    .Where(r => r.Depth >= 2)
                    .ToList();

                var emptyCategories = from c in secondLevelCategories where c.Resources.Count == 0 select c;

                context.ResourceCategories.RemoveRange(emptyCategories);
                context.SaveChanges();
            }
        }

        private static void FlattenCategories(string newDatabase)
        {
            Console.WriteLine("==Start to flatten the resources ==");

            var optionsForNewDb = new DbContextOptionsBuilder<ResourceContext>()
                .UseMySql($"Server=localhost;Port=3306;Database={newDatabase};Uid=root;Pwd=root;")
                .EnableSensitiveDataLogging()
                .Options;

            using (var context = new ResourceContext(optionsForNewDb))
            {
                var resources = context.Resources.Include(r => r.Category).ToList();
                var categories = context.ResourceCategories.Include(c => c.ChildrenCategories)
                    .Include(c => c.Resources)
                    .ToList();
                var tagSet = new HashSet<Tag>();

                var rootCats = from c in categories where c.Depth == 0 select c;
                foreach (var rootCat in rootCats)
                {
                    foreach (var subCat in rootCat.ChildrenCategories)
                    {
                        FlattenCategoriesHelper(subCat, subCat, tagSet);
                    }
                }

                Console.WriteLine($"===Start to write to {newDatabase} ===");

                using (var newContext = new ResourceContext(optionsForNewDb))
                {
                    newContext.Database.EnsureCreated();
                    newContext.Resources.UpdateRange(resources);
                    newContext.ResourceCategories.UpdateRange(categories);
                    newContext.Tags.AddRange(tagSet);
                    newContext.SaveChanges();
                }
            }

            Console.WriteLine("Flattening successful.");
            Console.ReadKey();
        }


        private static void FlattenCategoriesHelper(ResourceCategory cat, ResourceCategory destCat, HashSet<Tag> tagSet)
        {
            if (cat.Depth > 1)
            {
                var tag = new Tag
                {
                    Name = cat.CategoryName,
                };
                if (!tagSet.Contains(tag))
                {
                    tag.ResourceTags = new List<ResourceTag>();
                    tagSet.Add(tag);
                }

                //TODO: get the tag from the collection if already exists.

                foreach (var resource in cat.Resources.ToList())
                {
                    cat.Resources.Remove(resource);
                    resource.Category = destCat;
                    destCat.Resources.Add(resource);
                    tag.ResourceTags.Add(new ResourceTag
                    {
                        Resource = resource,
                        Tag = tag
                    });
                }
            }

            // TODO: convert current resource category to tags
            foreach (var subCat in cat.ChildrenCategories)
            {
                FlattenCategoriesHelper(subCat, destCat, tagSet);
            }
        }
    }
}