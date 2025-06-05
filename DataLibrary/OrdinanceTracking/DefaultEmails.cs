using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.OrdinanceTracking
{
    public class DefaultEmails
    {
        public int DefaultEmailsID { get; set; }
        public string EmailAddress { get; set; }
        public string Department { get; set; }
        public string Division { get; set; }
        public string DefaultType { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
