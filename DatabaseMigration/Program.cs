using DatabaseMigration.Resources;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using DCOClearinghouse.Data;
using DCOClearinghouse.Models;
using Microsoft.EntityFrameworkCore;
using Remotion.Linq.Clauses;

namespace DatabaseMigration
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("==Model translations start==");
            ResourceCategory[] outputCategories = null;
            using (var context = new dco_resourcesContext())
            {
                Console.WriteLine("Translating Categories");
                outputCategories = ModelTranslator.GetAllResourceCategory(context);

                foreach (var item in outputCategories)
                {
                    Console.WriteLine($"{item.ID} | {item.CategoryName} | {item.ParentCategoryID} | {item.Depth}");
                }
            }

            Console.ReadLine();

            Resource[] outputResources = null;
            using (var context = new dco_resourcesContext())
            {
                Console.WriteLine("Translating listings");

                outputResources = ModelTranslator.GetAllResources(context);

                foreach (var outputResource in outputResources.Take(10))
                {
                    Console.WriteLine(JsonConvert.SerializeObject(outputResource, Formatting.Indented, new StringEnumConverter()));
                }
            }

            Console.ReadLine();

            WriteToNewDatabase("new_dco_resources", outputCategories, outputResources);
        }

        static void WriteToNewDatabase(string databaseName, ResourceCategory[] outputCategories, Resource[] outputResources)
        {
            Console.WriteLine("==Start to write to new database==");
            var options = new DbContextOptionsBuilder<ResourceContext>()
                .UseMySql($"Server=localhost;Port=3306;Database={databaseName};Uid=root;Pwd=root;")
                .EnableSensitiveDataLogging()
                .Options;


            using (var context = new ResourceContext(options))
            {
                context.Database.EnsureCreated();
                foreach (var category in outputCategories)
                {
                    context.ResourceCategories.Add(category);
                }
                context.SaveChanges();
            }

            using (var context = new ResourceContext(options))
            {
                foreach (var resource in outputResources)
                {
                    context.Resources.Add(resource);
                }
                context.SaveChanges();
            }


            Console.WriteLine("Succeed.");

            //using (var context = new dco_resourcesContext())
            //{
            //    var binding = context.CategoryListingBindings.AsNoTracking().Take(10).ToList();

            //    foreach (var item in binding)
            //    {
            //        Console.WriteLine(item.CategoryId+"<---->"+item.ListId);
            //    }
            //}
        }
    }
}
