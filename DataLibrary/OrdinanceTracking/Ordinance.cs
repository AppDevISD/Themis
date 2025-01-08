using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class Ordinance
    {
        public int OrdinanceID { get; set; }
        public string OrdinanceNumber { get; set; }
        public string OrdinanceTitle { get; set; }
        public string OrdinanceAnalysis { get; set; }
        public decimal OrdinanceFiscalImpact { get; set; }
        public int RequestID { get; set; }
        public string RequestDepartment { get; set; }
        public string RequestContact { get; set; }
        public string RequestPhone { get; set; }
        public DateTime FirstReadDate { get; set; }
        public bool EmergencyPassage { get; set; }
        public string EmergencyPassageReason { get; set; }
        public int ContractVendorID { get; set; }
        public string ContractVendorName { get; set; }
        public int ContractVendorNumber { get; set; }
        public string ContractStartDate { get; set; } // Added
        public string ContractEndDate { get; set; } // Added
        public string ContractTerm { get; set; }
        public decimal ContractAmount { get; set; }
        public bool ScopeChange { get; set; } // Added
        public string ChangeOrderNumber { get; set; } // Added
        public decimal AdditionalAmount { get; set; } // Added
        public string ContractMethod { get; set; }
        //public string OtherException { get; set; } // Added
        //public string PreviousOrdinanceNumbers { get; set; } // Added
        //public string CodeProvision { get; set; } // Added
        public bool PAApprovalRequired { get; set; }
        public bool PAApprovalIncluded { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
