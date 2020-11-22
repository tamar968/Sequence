﻿using DAL;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Helpers
{
    public class SearchDetailsForUser
    {
        public int codeSearch { get; set; }
        public string nameProduct { get; set; }
        public string nameCategory { get; set; }
        public EStatus status { get; set; }
        public string nameShop { get; set; }
    }
}
