﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.OrdinanceTracking
{
    public class OrdinanceAccounting
    {
        public int OrdinanceAccountingID { get; set; }
        public int OrdinanceID { get; set; }
        public int AccountingID { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}