using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopRestApi.Application.Common.Models
{
    public class PaginationParameters
    {
        private const int MaxPageSize = 50;

        private int _pageNumber = 1;

        private int _pageSize = 10;

        public int PageNumber
        {
            get => _pageNumber;
            set => _pageNumber =
                value <= 0 ? 1 : value;
        }

        public int PageSize
        {
            get => _pageSize;
            set
            {
                if (value <= 0)
                    _pageSize = 10;
                else
                    _pageSize =
                        value > MaxPageSize
                            ? MaxPageSize
                            : value;
            }
        }
    }
}
