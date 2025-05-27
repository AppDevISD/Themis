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
        public string StatusDescription { get; set; }
        public string OrdinanceNumber { get; set; }
        public string AgendaNumber { get; set; }
        public string RequestDepartment { get; set; }
        public string RequestDivision { get; set; }
        public string RequestContact { get; set; }
        public string RequestPhone { get; set; }
        public string RequestEmail { get; set; }
        public DateTime FirstReadDate { get; set; }
        public bool EmergencyPassage { get; set; }
        public string EmergencyPassageReason { get; set; }
        public decimal OrdinanceFiscalImpact { get; set; }
        public string OrdinanceTitle { get; set; }                
        public string ContractVendorName { get; set; }
        public string ContractVendorNumber { get; set; }
        public string ContractStartDate { get; set; }
        public string ContractEndDate { get; set; }
        public string ContractTerm { get; set; }
        public decimal ContractAmount { get; set; }
        public bool ScopeChange { get; set; }
        public string ChangeOrderNumber { get; set; }
        public decimal AdditionalAmount { get; set; }
        public string ContractMethod { get; set; }
        public string OtherException { get; set; }
        public string PreviousOrdinanceNumbers { get; set; }
        public string CodeProvision { get; set; }
        public bool PAApprovalRequired { get; set; }
        public bool PAApprovalIncluded { get; set; }
        public string OrdinanceAnalysis { get; set; }        
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
