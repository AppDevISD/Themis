using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class Audit
    {
        public int AuditID { get; set; }
        public int OrdinanceAuditID { get; set; }
        public string Type { get; set; }
        public string Label { get; set; }
        public string OldValue { get; set; }
        public string NewValue { get; set; }
        public string DataType { get; set; }
    }
}
