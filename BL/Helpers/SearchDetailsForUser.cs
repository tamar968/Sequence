using DAL;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Helpers
{
    public class SearchDetailsForUser:SearchDTO
    {
        public int CodeSearch { get; set; }
        public string NameProduct { get; set; }
        public string nameCategory { get; set; }
        public EStatus Status { get; set; }
        public string nameShop { get; set; }
        public bool? IsFavoriteShop { get; set; }

    }
}
