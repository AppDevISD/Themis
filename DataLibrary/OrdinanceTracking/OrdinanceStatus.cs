using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class OrdinanceStatus
    {
        public string StatusDescription { get; set; }
        public int OrdinanceStatusID { get; set; }
        public int OrdinanceID { get; set; }
        public int StatusID { get; set; }
        public string Signature { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
