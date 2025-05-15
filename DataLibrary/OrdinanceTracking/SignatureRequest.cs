using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class SignatureRequest
    {
        public int SignatureRequestID { get; set; }
        public int OrdinanceID { get; set; }
        public string FundsCheckBy { get; set; }
        public string DirectorSupervisor { get; set; }
        public string CityPurchasingAgent { get; set; }
        public string OBMDirector { get; set; }
        public string Mayor { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
