using DCOClearinghouse.Models;
using Microsoft.EntityFrameworkCore.Internal;

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
                    ID = 1,
                    TypeName = "Activities"
                },
                new ResourceType
                {
                    ID = 2,
                    TypeName = "Events"
                },
                new ResourceType
                {
                    ID = 3,
                    TypeName = "Tutorial"
                },
                new ResourceType
                {
                    ID = 4,
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
                    ID = 1,
                    CategoryName = "Assistive Technology"
                },
                new ResourceCategory
                {
                    ID = 2,
                    CategoryName = "Education"
                },
                new ResourceCategory
                {
                    ID = 3,
                    CategoryName = "Employment"
                },
                new ResourceCategory
                {
                    ID = 4,
                    CategoryName = "Healing"
                },
                new ResourceCategory
                {
                    ID = 5,
                    CategoryName = "Additional cat 1"
                },
                new ResourceCategory
                {
                    ID = 6,
                    CategoryName = "Additiona cat 3"
                },
                new ResourceCategory
                {
                    ID = 7,
                    CategoryName = "Additional cat 4"
                }
            };
            foreach (var resourceCategory in categories)
            {
                context.ResourceCategories.Add(resourceCategory);
            }

            context.SaveChanges();

            #endregion

            #region Resources

            var resources = new[]
            {
                new Resource
                {
                    Subject = "Training:Online Workshops",
                    CategoryID = 1,
                    BadlinkVotes = 0,
                    Content = @"A training workshops",
                    Link = "http://www.d.umn.edu/itss/training/online/",
                    Status = ResourceStatus.New,
                    Type = new ResourceType()
                    {
                        ID = 1
                    }
                    
                },
                new Resource
                {
                    Subject = "Assistive Technology Act Programs",
                    CategoryID = 1,
                    BadlinkVotes = 0,
                    Content = @"Website",
                    Link = "http://www.ataporg.org",
                    Status = ResourceStatus.New,
                    Type = new ResourceType()
                    {
                    ID = 2
                }
                },
                new Resource
                {
                    Subject = "TCM Study Tools - Chinese Style",
                    CategoryID = 4,
                    BadlinkVotes = 0,
                    Content = @"Website: http://www.tcmstudent.com/theory/Chinese%20Style.html",
                    Link = "http://www.tcmstudent.com/theory/Chinese%20Style.html",
                    Status = ResourceStatus.New,
                    Type = new ResourceType()
                    {
                        ID = 3
                    }
                },
                new Resource
                {
                    Subject = "ILR Program on Employment and Disability",
                    CategoryID = 3,
                    BadlinkVotes = 0,
                    Content = @"Website: http://www.ilr.cornell.edu/ped/",
                    Link = "http://www.ilr.cornell.edu/ped/",
                    Status = ResourceStatus.Approved,
                    Type = new ResourceType()
                    {
                        ID = 3
                    }

                },
                new Resource
                {
                    Subject = "ClassesUSA.com",
                    CategoryID = 2,
                    BadlinkVotes = 0,
                    Content = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Status = ResourceStatus.Removed,
                    Type = new ResourceType()
                    {
                        ID = 3
                    }
                },
                new Resource
                {
                    Subject = "Google.com",
                    CategoryID = 4,
                    BadlinkVotes = 0,
                    Content = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Status = ResourceStatus.Removed,
                    Type = new ResourceType()
                    {
                        ID = 4
                    }
                },
                new Resource
                {
                    Subject = "Yahoo.com",
                    CategoryID = 5,
                    BadlinkVotes = 0,
                    Content = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "https://yahoo.com",
                    Status = ResourceStatus.Approved,
                    Type = new ResourceType()
                    {
                        ID = 2
                    }
                },
                new Resource
                {
                    Subject = "Bing.com",
                    CategoryID = 7,
                    BadlinkVotes = 0,
                    Content = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "https://bing.com",
                    Status = ResourceStatus.Approved,
                    Type = new ResourceType()
                    {
                        ID = 1
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