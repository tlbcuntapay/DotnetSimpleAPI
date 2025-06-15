using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace api.Helpers
{
    public class QueryObject
    {
        public string? Symbol { get; set; } = null;
        public string? CompanyName { get; set; } = null;
        public string? SortBy { get; set; } = null;
        public bool IsDescending { get; set; } = false;

        public int PageNo { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        public int CommentPageNo { get; set; } = 1;
        public int CommentPageSize { get; set; } = 10;

    }
}