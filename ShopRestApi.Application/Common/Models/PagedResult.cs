using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopRestApi.Application.Common.Models
{
    public class PagedResult<T>
    {
        public IReadOnlyList<T> Items { get; set; } = [];

        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages =>
            (int)Math.Ceiling((double)TotalCount / PageSize);

        public bool HasPrevious =>
            PageNumber > 1;

        public bool HasNext =>
            PageNumber < TotalPages;
    }
}
