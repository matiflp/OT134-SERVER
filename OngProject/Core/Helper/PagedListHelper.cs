using System;
using System.Collections.Generic;
using System.Linq;

namespace OngProject.Core.Helper
{
    public class PagedList<T> : List<T>
    {
        public int CurrentPage { get; private set; }
        public int TotalPages { get; private set; }
        public int PageSize { get; private set; }
        public int TotalCount { get; private set; }
        public bool HasPrevius => (CurrentPage > 1 && CurrentPage <= TotalPages);
        public bool HasNext => (CurrentPage < TotalPages);

        public PagedList(ICollection<T> source, int totalCount, int pageNumber, int pageSize)
        {
            this.TotalCount = totalCount;            
            this.PageSize = pageSize;
            this.TotalPages = (int)Math.Ceiling(this.TotalCount / (double)this.PageSize);            
            this.CurrentPage = pageNumber;            
            
            AddRange(source);
        }

        public static PagedList<T> Create(ICollection<T> source, int totalCount, int pageNumber, int pageSize)
        {
            return new PagedList<T>(source, totalCount, pageNumber, pageSize);
        }
    }
}
