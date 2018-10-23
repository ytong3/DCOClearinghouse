using DCOClearinghouse.Models;
using Microsoft.EntityFrameworkCore.Internal;
using System;

namespace DCOClearinghouse.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ResourceContext context)
        {
            context.Database.EnsureCreated();

            // look for any existing resources
            if (context.Resources.Any())
            {
                return; // no need to be seeded again
            }

            #region ResourceTypes

            var types = new[]
            {
                new ResourceType
                {
                    TypeName = "Activities"
                },
                new ResourceType
                {
                    TypeName = "Events"
                },
                new ResourceType
                {
                    TypeName = "Tutorial"
                },
                new ResourceType
                {
                    TypeName = "Product"
                }
            };

            foreach (var resourceType in types)
            {
                context.ResourceTypes.Add(resourceType);
            }

            context.SaveChanges();

            #endregion

            #region Categories

            var categories = new[]
            {
                new ResourceCategory
                {
                    CategoryName = "Assistive Technology",
                    ParentCategoryID = null,
                    Depth = 0
                },
                new ResourceCategory
                {
                    CategoryName = "Education",
                    ParentCategoryID = null
                },
                new ResourceCategory
                {
                    CategoryName = "Employment",
                    ParentCategoryID = null,
                    Depth = 0
                },
                new ResourceCategory
                {
                    CategoryName = "Healing",
                    ParentCategoryID = null,
                    Depth = 0
                },
                new ResourceCategory
                {
                    CategoryName = "Additional cat 1",
                    ParentCategoryID = null,
                    Depth = 0
                },
                new ResourceCategory
                {
                    CategoryName = "Additiona cat 3",
                    ParentCategoryID = null,
                    Depth = 0
                },
                new ResourceCategory
                {
                    CategoryName = "Additional cat 4",
                    ParentCategoryID = null,
                    Depth = 0
                }
            };
            foreach (var resourceCategory in categories)
            {
                context.ResourceCategories.Add(resourceCategory);
            }

            context.SaveChanges();

            #region Subcategories

            var subCategories = new[]
            {
                new ResourceCategory
                {
                    CategoryName = "Subcat 1",
                    ParentCategoryID = categories[0].ID,
                    Depth = categories[0].Depth+1
                },
                new ResourceCategory
                {
                    CategoryName = "Subcat 2",
                    ParentCategoryID = categories[0].ID,
                    Depth = categories[0].Depth+1
                },
                new ResourceCategory
                {
                    CategoryName = "Subcat 3",
                    ParentCategoryID = categories[1].ID,
                    Depth = categories[1].Depth+1
                },
                new ResourceCategory
                {
                    CategoryName = "Subcat 4",
                    ParentCategoryID = categories[2].ID,
                    Depth = categories[2].Depth+1
                }
            };
            foreach (var subcategory in subCategories)
            {
                context.ResourceCategories.Add(subcategory);
            }

            context.SaveChanges();

            #endregion

            #region Subsubcategory

            var subsubCategories = new[]
            {
                new ResourceCategory
                {
                    CategoryName = "Subsubcat 1",
                    ParentCategoryID = subCategories[0].ID,
                    Depth = subCategories[0].Depth+1
                },
                new ResourceCategory
                {
                    CategoryName = "Subsubcat 2",
                    ParentCategoryID = subCategories[0].ID,
                    Depth = subCategories[0].Depth+1
                },
                new ResourceCategory
                {
                    CategoryName = "Subsubcat 3",
                    ParentCategoryID = subCategories[1].ID,
                    Depth = subCategories[1].Depth+1
                },
                new ResourceCategory
                {
                    CategoryName = "Subsubcat 4",
                    ParentCategoryID = subCategories[2].ID,
                    Depth = subCategories[2].Depth+1
                }
            };
            foreach (var subsbucategory in subsubCategories)
            {
                context.ResourceCategories.Add(subsbucategory);
            }

            #endregion

            #endregion

            #region Resources

            var resources = new[]
            {
                new Resource
                {
                    Subject = "Training:Online Workshops",
                    CategoryID = categories[0].ID,
                    BadlinkVotes = 0,
                    Description = @"A training workshops",
                    Link = "http://www.d.umn.edu/itss/training/online/",
                    Status = ResourceStatus.New,
                    CreateDate = DateTime.Today,
                    Type = new ResourceType()
                    {
                        ID = types[0].ID
                    },
                    Contact = new ContactInfo
                    {
                        IsContactInfoPublic = true,
                        FirstName = "Joe",
                        LastName = "Smith",
                        Email = "j.smith@aol.com",
                        Organization = "Acme Inc.",
                        PhoneNumber = "8483932933",
                        Address = new Address
                        {
                            Line1 = "112 Main St",
                            Line2 = "Suite 2",
                            City = "Someville",
                            State = "CA",
                            Zipcode = "32345"
                        }
                    }
                },
                new Resource
                {
                    Subject = "Assistive Technology Act Programs",
                    CategoryID = categories[0].ID,
                    BadlinkVotes = 0,
                    Description = @"Website",
                    Link = "http://www.ataporg.org",
                    Status = ResourceStatus.New,
                    CreateDate = new DateTime(2015, 8, 3),
                    Type = new ResourceType()
                    {
                    ID = types[2].ID
                    },
                    Contact = new ContactInfo
                    {
                        IsContactInfoPublic = true,
                        FirstName = "Joe",
                        LastName = "Smith",
                        Email = "j.smith@aol.com",
                        Organization = "Acme Inc.",
                        PhoneNumber = "8483932933",
                        Address = new Address
                        {
                            Line1 = "112 Main St",
                            Line2 = "Suite 2",
                            City = "Someville",
                            State = "CA",
                            Zipcode = "32345"
                        }
                    }
                },
                new Resource
                {
                    Subject = "TCM Study Tools - Chinese Style",
                    CategoryID = categories[3].ID,
                    BadlinkVotes = 0,
                    Description = @"Website: http://www.tcmstudent.com/theory/Chinese%20Style.html",
                    Link = "http://www.tcmstudent.com/theory/Chinese%20Style.html",
                    Status = ResourceStatus.New,
                    CreateDate = new DateTime(2017, 8, 3),
                    Type = new ResourceType()
                    {
                        ID = types[2].ID
                    }
                },
                new Resource
                {
                    Subject = "ILR Program on Employment and Disability",
                    CategoryID = categories[2].ID,
                    BadlinkVotes = 0,
                    Description = @"Website: http://www.ilr.cornell.edu/ped/",
                    Link = "http://www.ilr.cornell.edu/ped/",
                    Status = ResourceStatus.New,
                    CreateDate = new DateTime(2018, 8, 3),
                    Type = new ResourceType()
                    {
                        ID = types[2].ID
                    },
                    
                    Contact = new ContactInfo
                    {
                        IsContactInfoPublic = false,
                        FirstName = "Helen",
                        LastName = "Lou",
                        Email = "h.lou@yahoo.com",
                        Organization = "Acme Inc.",
                        PhoneNumber = "8483932933",
                        Address = new Address
                        {
                            Line1 = "112 Main St",
                            Line2 = "Suite 2",
                            City = "Someville",
                            State = "CA",
                            Zipcode = "32345"
                        }
                    }
                },
                new Resource
                {
                    Subject = "ClassesUSA.com",
                    CategoryID = types[1].ID,
                    BadlinkVotes = 0,
                    Description = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Status = ResourceStatus.Removed,
                    CreateDate = new DateTime(2017, 9, 3),
                    Type = new ResourceType()
                    {
                        ID = types[2].ID
                    }
                },
                new Resource
                {
                    Subject = "Google.com",
                    CategoryID = 4,
                    BadlinkVotes = 0,
                    Description = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Status = ResourceStatus.Removed,
                    CreateDate = new DateTime(2017, 10, 3),
                    Type = new ResourceType()
                    {
                        ID = types[0].ID
                    }
                },
                new Resource
                {
                    Subject = "Yahoo.com",
                    CategoryID = categories[4].ID,
                    BadlinkVotes = 0,
                    Description = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "https://yahoo.com",
                    Status = ResourceStatus.New,
                    CreateDate = new DateTime(2018, 8, 3),
                    Type = new ResourceType()
                    {
                        ID = types[1].ID
                    }
                },
                new Resource
                {
                    Subject = "Bing.com",
                    CategoryID = categories[5].ID,
                    BadlinkVotes = 0,
                    Description = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "https://bing.com",
                    Status = ResourceStatus.New,
                    CreateDate = new DateTime(2011, 7, 3),
                    Type = new ResourceType()
                    {
                        ID = types[0].ID
                    }
                },
            };
            foreach (var resource in resources)
            {
                context.Resources.Add(resource);
            }

            context.SaveChanges();

            #endregion
        }
    }
}