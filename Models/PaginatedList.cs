using Microsoft.EntityFrameworkCore;

namespace FinalProject_AdvancedDatabaseAndORMConcepts_SlackOverload.Models
{
    public class PaginatedList<T> : List<T>
    {
        public int PageIndex { get; private set; }
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            PageIndex = pageIndex;                                        // current page index
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);     // total page count; Ceiling example: Math.Ceiling(11/10)=2; Math.Ceiling(-11/10)=-1;

            this.AddRange(items);         // add the items to PaginatedList
        }

        public bool HasPreviousPage => PageIndex > 1;  //if pageIndex >1, HasPreviousPage = true, else false;

        public bool HasNextPage => PageIndex < TotalPages; //if HasNextPage < TotalPages, HasNextPage = true, else false;

        //get the items of current page
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync();
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync(); //get the items of current page
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }

}
