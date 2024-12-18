using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class Template
    {
        public int FormID { get; set; }
        public int FormTypeID { get; set; }
        public int FormTypeDescID { get; set; }
        public string FormTypeDesc { get; set; }
        public string ContactName { get; set; }
        public string EmployeeName { get; set; }
        public string Comments { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
