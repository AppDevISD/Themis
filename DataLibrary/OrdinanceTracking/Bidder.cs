using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class Bidder
    {
        public int BidderID { get; set; }
        public int VendorID { get; set; }
        public string VendorName { get; set; }
        public string VendorNumber { get; set; }
        public string VendorDescription { get; set; }
        public int BidderTypeID { get; set; }
        public string BidderName { get; set; }
        public string BidderDescription { get; set; }
        public DateTime InsertDate { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
