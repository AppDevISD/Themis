using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class AccountingAudit
    {
        public int AccountingAuditID { get; set; }
        public int AuditID { get; set; }
        public int OrdinanceAccountingID { get; set; }
        public string AccountingDesc { get; set; }
        public string FundCode { get; set; }
        public string DepartmentCode { get; set; }
        public string UnitCode { get; set; }
        public string ActivityCode { get; set; }
        public string ObjectCode { get; set; }
        public string Amount { get; set; }
    }
}
