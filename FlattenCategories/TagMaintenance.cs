using System.Collections.Immutable;
using System.Linq;
using DCOClearinghouse.Data;
using DCOClearinghouse.Models;
using Microsoft.EntityFrameworkCore;

namespace DatabaseMaintenance
{
    public static class TagMaintenance
    {
        public static void MergeDuplicateTags(ResourceContext context)
        {
            //get all unique tag names
            var uniqueTags = context.Tags.AsNoTracking()
                .GroupBy(t => t.Name)
                .Where(g => g.Count() > 1)
                .Select(g=>g.Key)
                .ToList();

            var associations = context.ResourceTags.AsNoTracking()
                .Include(a => a.Resource)
                .Include(a => a.Tag)
                .ToList();

            foreach (var tagName in uniqueTags)
            {
                var newTag = new Tag {Name = tagName};
                context.Tags.Add(newTag);
                context.SaveChanges();

                var oldAssoc = from a in associations
                    where a.Tag.Name == tagName
                    select a;

                var newAssoc = from a in associations
                    where a.Tag.Name == tagName
                    select new ResourceTag
                    {
                        ResourceID = a.ResourceID,
                        TagID = newTag.ID
                    };

                context.ResourceTags.RemoveRange(oldAssoc);
                context.ResourceTags.AddRange(newAssoc);
            }
        }

        public static void RemoveEmptyTags(ResourceContext context)
        {
            var emptyTags = context.Tags.Include(t => t.ResourceTags)
                .Where(t => t.ResourceTags.Count == 0);

            context.Tags.RemoveRange(emptyTags);
            context.SaveChanges();

        }
    }
}