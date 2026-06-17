using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopRestApi.Application.Common.Models
{
    public class ProductQueryParameters : PaginationParameters
    {
        public string? Search { get; set; }
    }
}
