using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class OrdinanceAudit
    {
        public int OrdinanceAuditID { get; set; }
        public int OrdinanceID { get; set; }
        public string UpdateType { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
