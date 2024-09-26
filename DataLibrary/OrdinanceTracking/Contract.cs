using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class Contract
    {
        public int ContractID { get; set; }
        public string ContractName { get; set; }
        public string ContractTerms { get; set; }
        public bool ChangeOfScope { get; set; }
        public decimal ContractAmount { get; set; }
        public string ChangeOrder {  get; set; }
        public decimal AdditionalAmount { get; set; }
        public string StaffAnalysis { get; set; }
        public bool OrdinanceRequested { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
