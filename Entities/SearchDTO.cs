using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public enum EStatus
    {
        NotFound,
        Found,
        Deleted,
        TimeWait,
        TimeOver
    }

    public class SearchDTO
    {
        public int codeSearch { get; set; }
        public int codeUser { get; set; }
        public string nameProduct { get; set; }
        public EStatus status { get; set; }
        public int? codeShop { get; set; }
        public int codeCategory { get; set; }
        public int? distance { get; set; }
        public DateTime? dateStart { get; set; }
        public DateTime? dateEnd { get; set; }
    }
}
