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


            context.ResourceCategories.RemoveRange(context.ResourceCategories);
            var categories = new []
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

            var resources = new[]
            {
                new Resource
                {
                    Subject = "Training:Online Workshops",
                    CategoryID = 1,
                    BadlinkVotes = 0,
                    Content = @"A training workshops",
                    Link = "http://www.d.umn.edu/itss/training/online/",
                    Status = ResourceStatus.New
                },
                new Resource
                {
                    Subject = "Assistive Technology Act Programs",
                    CategoryID = 1,
                    BadlinkVotes = 0,
                    Content = @"Website",
                    Link = "http://www.ataporg.org",
                    Status = ResourceStatus.New
                },
                new Resource
                {
                    Subject = "TCM Study Tools - Chinese Style",
                    CategoryID = 4,
                    BadlinkVotes = 0,
                    Content = @"Website: http://www.tcmstudent.com/theory/Chinese%20Style.html",
                    Link = "http://www.tcmstudent.com/theory/Chinese%20Style.html",
                    Status = ResourceStatus.New
                },
                new Resource
                {
                    Subject = "ILR Program on Employment and Disability",
                    CategoryID = 3,
                    BadlinkVotes = 0,
                    Content = @"Website: http://www.ilr.cornell.edu/ped/",
                    Link = "http://www.ilr.cornell.edu/ped/",
                    Status = ResourceStatus.Approved
                },
                new Resource
                {
                    Subject = "ClassesUSA.com",
                    CategoryID = 2,
                    BadlinkVotes = 0,
                    Content = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Status = ResourceStatus.Removed
                }
                ,
                new Resource
                {
                    Subject = "Google.com",
                    CategoryID = 4,
                    BadlinkVotes = 0,
                    Content = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Status = ResourceStatus.Removed
                },
                new Resource
                {
                    Subject = "Yahoo.com",
                    CategoryID = 5,
                    BadlinkVotes = 0,
                    Content = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "https://yahoo.com",
                    Status = ResourceStatus.Approved
                },
                new Resource
                {
                    Subject = "Bing.com",
                    CategoryID = 7,
                    BadlinkVotes = 0,
                    Content = @"Website: http://www.classesusa.com/featuredschools/fos/index.cfm",
                    Link = "https://bing.com",
                    Status = ResourceStatus.Approved
                },

            };
            foreach (var resource in resources)
            {
                context.Resources.Add(resource);
            }
            context.SaveChanges();
        }
    }
}