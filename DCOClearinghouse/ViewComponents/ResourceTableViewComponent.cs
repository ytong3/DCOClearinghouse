using System.Linq;
using System.Threading.Tasks;
using DCOClearinghouse.Data;
using DCOClearinghouse.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DCOClearinghouse.ViewComponents
{
    public class ResourceTableViewComponent : ViewComponent
    {
        private static int PageSize = 20;
        
        private readonly ResourceContext _context;

        public ResourceTableViewComponent(ResourceContext context)
        {
            _context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync(int pageNumber, 
                                                            int? categoryID,
                                                            int? tagID)
        {
            var resources = _context.Resources
                                .AsNoTracking();

            if (categoryID!=null)
            {
                resources = resources.Where(r=>r.CategoryID == categoryID);
            }

            if (tagID!=null)
            {
                resources = _context.ResourceTags
                .Include(rt=>rt.Resource)
                .Where(rt=>rt.TagID == tagID)
                .Select(rt=>rt.Resource);
            }
            
            resources = resources.OrderByDescending(r=>r.CreateDate);
            ViewData["startIndex"] = (pageNumber - 1)*PageSize;
            return View(await PaginatedList<Resource>.CreateAsync(resources, pageNumber, PageSize));
        }
    }
}