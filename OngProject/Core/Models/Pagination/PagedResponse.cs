using OngProject.Core.Helper;
using System;
using System.Collections.Generic;

namespace OngProject.Core.Models.Paged
{
    public class PagedResponse<T>
    {
        /// <summary>
        ///     If exist, url of the previus page.
        /// </summary>
        /// <example>https://yourdomain.com/resource/PageNumber=1&PageSize=2</example>
        public string PreviusPage { get; set; }

        /// <summary>
        ///     If exist, url of the nex page.
        /// </summary>
        /// <example>https://yourdomain.com/resource/PageNumber=3&PageSize=2</example>
        public string NextPage { get; set; }

        /// <summary>
        ///     Number of the current page.
        /// </summary>
        /// <example>2</example>
        public int PageNumber { get; set; }

        /// <summary>
        ///     Number of the total pages.
        /// </summary>
        /// <example>5</example>
        public int TotalPages { get; set; }

        /// <summary>
        ///     Items per page.
        /// </summary>
        /// <example>10</example>
        public int PageSize { get; set; }

        /// <summary>
        ///     Results.
        /// </summary>
        public List<T> Items { get; set; }
        public PagedResponse(PagedList<T> pagedList, string url)
        {            
            this.Items = pagedList;
            this.TotalPages = pagedList.TotalPages;
            this.PageSize = pagedList.PageSize;
            this.PageNumber = pagedList.CurrentPage;
            if (pagedList.HasNext) this.NextPage = $"{url}?PageNumber={pagedList.CurrentPage + 1}&PageSize={pagedList.PageSize}";
            if (pagedList.HasPrevius) this.PreviusPage = $"{url}?PageNumber={pagedList.CurrentPage - 1}&PageSize={pagedList.PageSize}";
        }
    }
}
