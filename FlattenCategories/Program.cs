using System;
using System.Linq;
using DCOClearinghouse.Data;
using DCOClearinghouse.Models;
using Microsoft.EntityFrameworkCore;

namespace FlattenCategories
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            FlattenCategories("flattened_dco_resources");
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

                var rootCats = from c in categories where c.Depth == 0 select c;
                foreach (var rootCat in rootCats)
                {
                    foreach (var subCat in rootCat.ChildrenCategories)
                    {
                        FlattenCategoriesHelper(subCat, subCat);
                    }
                }

                Console.WriteLine($"==Start to write to {newDatabase} ==");

                using (var newContext = new ResourceContext(optionsForNewDb))
                {
                    newContext.Database.EnsureCreated();
                    newContext.Resources.UpdateRange(resources);
                    newContext.ResourceCategories.UpdateRange(categories);
                    newContext.SaveChanges();
                }

            }

            Console.WriteLine("Flattening successful.");
            Console.ReadKey();
        }


        private static void FlattenCategoriesHelper(ResourceCategory cat, ResourceCategory destCat)
        {
            if (cat.Depth > 1)
            {
                foreach (var resource in cat.Resources.ToList())
                {
                    cat.Resources.Remove(resource);
                    resource.Category = destCat;
                    destCat.Resources.Add(resource);
                }
            }

            // TODO: convert current resource category to tags
            foreach (var subCat in cat.ChildrenCategories)
            {
                FlattenCategoriesHelper(subCat, destCat);
            }


        }
    }
}
