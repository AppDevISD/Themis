﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class PurchaseAgentApproval
    {
        public int PurchaseAgentApprovalID { get; set; }
        public string ApprovalName { get; set; }
        public byte[] ApprovalImage { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
