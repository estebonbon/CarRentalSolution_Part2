using Microsoft.EntityFrameworkCore;

namespace CarRental.Utilities
{
    //This is a generic class, use for 
    public class PaginatedList<T> : List<T>
    {
        //Current Page number
        public int PageIndex { get; private set; }

        //Total Page numbers available
        public int TotalPages { get; private set; }

        public PaginatedList(List<T> items, int count, int pageIndex, int pageSize)
        {
            //Item is how many there are
            //Page indexx that page we are on
            //pagesize how big the pages are

            PageIndex = pageIndex;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);

            this.AddRange(items);
        }

        public bool HasPreviousPage
        {
            get
            {
                return (PageIndex > 1);
            }
        }

        public bool HasNextPage
        {
            get
            {
                return (PageIndex < TotalPages);
            }
        }

        //No instance needs to be created, that is why it is static
        //Parameters, DB-Query, current page, number of records per page
        public static async Task<PaginatedList<T>> CreateAsync(IQueryable<T> source, int pageIndex, int pageSize)
        {
            var count = await source.CountAsync(); //Total Records
            var items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            if (items.Count() == 0 && count > 0 && pageIndex > 1)
            {
                //If there are no items to show on a page, but there is data and you are not already 
                //on the first page, then move back a page and get the data again.
                //Decrease page number by 1
                pageIndex--;
                items = await source.Skip((pageIndex - 1) * pageSize).Take(pageSize).ToListAsync();
            }
            return new PaginatedList<T>(items, count, pageIndex, pageSize);
        }
    }
}
