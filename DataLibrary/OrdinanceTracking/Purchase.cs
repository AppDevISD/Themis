using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class Purchase
    {
        public int PurchaseID {  get; set; }
        public string PurchaseName { get; set; }
        public int ContractID { get; set; }
        public bool PurchaseAgentRequired { get; set; }
        public bool PurchaseAgentApproval {  get; set; }
        public int AgentApprovalID { get; set; }
        public int PurchaseTypeID { get; set; }
        public string PurchaseDescription { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
