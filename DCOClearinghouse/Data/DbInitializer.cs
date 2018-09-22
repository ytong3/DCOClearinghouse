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
                    CategoryName = "Assistive Technology"
                },
                new ResourceCategory
                {
                    CategoryName = "Education"
                },
                new ResourceCategory
                {
                    CategoryName = "Employment"
                },
                new ResourceCategory
                {
                    CategoryName = "Healing"
                },
                new ResourceCategory
                {
                    CategoryName = "Additional cat 1"
                },
                new ResourceCategory
                {
                    CategoryName = "Additiona cat 3"
                },
                new ResourceCategory
                {
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
                    CategoryID = categories[0].ID,
                    BadlinkVotes = 0,
                    Content = @"A training workshops",
                    Link = "http://www.d.umn.edu/itss/training/online/",
                    Status = ResourceStatus.New,
                    Type = new ResourceType()
                    {
                        ID = types[0].ID
                    }
                    
                },
                new Resource
                {
                    Subject = "Assistive Technology Act Programs",
                    CategoryID = categories[0].ID,
                    BadlinkVotes = 0,
                    Content = @"Website",
                    Link = "http://www.ataporg.org",
                    Status = ResourceStatus.New,
                    Type = new ResourceType()
                    {
                    ID = types[2].ID
                }
                },
                new Resource
                {
                    Subject = "TCM Study Tools - Chinese Style",
                    CategoryID = categories[3].ID,
                    BadlinkVotes = 0,
                    Content = @"Website: http://www.tcmstudent.com/theory/Chinese%20Style.html",
                    Link = "http://www.tcmstudent.com/theory/Chinese%20Style.html",
                    Status = ResourceStatus.New,
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
                    Content = @"Website: http://www.ilr.cornell.edu/ped/",
                    Link = "http://www.ilr.cornell.edu/ped/",
                    Status = ResourceStatus.Approved,
                    Type = new ResourceType()
                    {
                        ID = types[2].ID
                    }

                },
                new Resource
                {
                    Subject = "ClassesUSA.com",
                    CategoryID = types[1].ID,
                    BadlinkVotes = 0,
                    Content = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Status = ResourceStatus.Removed,
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
                    Content = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Status = ResourceStatus.Removed,
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
                    Content = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "https://yahoo.com",
                    Status = ResourceStatus.Approved,
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
                    Content = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "https://bing.com",
                    Status = ResourceStatus.Approved,
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