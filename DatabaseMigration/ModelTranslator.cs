using System;
using System.Collections.Generic;
using System.Linq;
using DatabaseMigration.Resources;
using DCOClearinghouse.Models;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;

namespace DatabaseMigration
{
    public static class ModelTranslator
    {
        public static ResourceCategory[] GetAllResourceCategory(dco_resourcesContext context)
        {
            var oldCategories = context.Categories.AsNoTracking().ToList();

            var newCategories = from oldCategory in oldCategories
                select new ResourceCategory
                {
                    CategoryName = oldCategory.Name,
                    ID = oldCategory.Id,
                    ParentCategoryID = oldCategory.ParentId
                };

            var resourceCategoryArray = newCategories as ResourceCategory[] ?? newCategories.ToArray();


            var rootCategories = from category in resourceCategoryArray where category.ParentCategoryID == null select category;
            foreach (var rootCategory in rootCategories)
            {
                BFSTranversal(rootCategory, 0, resourceCategoryArray);
            }

            return resourceCategoryArray;
        }

        public static void BFSTranversal(ResourceCategory self, int depth, ResourceCategory[] allCategories)
        {
            self.Depth = depth;
            var children = from category in allCategories where category.ParentCategoryID == self.ID select category;
            foreach (var child in children)
            {
                BFSTranversal(child, depth+1, allCategories);
            }
        }

        public static Resource[] GetAllResources(dco_resourcesContext context)
        {
            var allListings = context.Listings.AsNoTracking().ToList();
            var allCategoryListingBindings = context.CategoryListingBindings.AsNoTracking().ToList();

            var result = (from listing in allListings
                join binding in allCategoryListingBindings
                    on listing.Id equals binding.ListId
                select new Resource
                {
                    ID = listing.Id,
                    CategoryID = binding.CategoryId,
                    Link = listing.Website,
                    Description = listing.Description,
                    Subject = listing.Name,
                    CreateDate = listing.CreatedAt,
                    BadlinkVotes = 0,
                    Contact = new ContactInfo
                    {
                        IsContactInfoPublic = true,
                        FirstName = listing.ContactFirstName,
                        LastName = listing.ContactLastName,
                        Title = listing.ContactTitle,
                        Email = listing.Email,
                        PhoneNumber = listing.Phone,
                        Organization = listing.Department,
                        Address = new Address()
                        {
                            Line1 = listing.Address1,
                            Line2 = listing.Address2,
                            City = listing.City,
                            Country = "USA",
                            State = listing.State,
                            Zipcode = listing.Zipcode
                        }

                    }
                }).GroupBy(r => r.ID).Select(g => g.First());

            return result.ToArray();
        }
    }
}