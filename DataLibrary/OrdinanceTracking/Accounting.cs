using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class Accounting
    {
        public int AccountingID { get; set; }
        public string AccountingDesc { get; set; }
        public string FundCode { get; set; }
        public string DepartmentCode { get; set; }
        public string UnitCode { get; set; }
        public string ActivityCode { get; set; }
        public string ObjectCode { get; set; }
        public decimal Amount { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
