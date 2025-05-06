using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary.OrdinanceTracking
{
    public class OrdinanceSignature
    {
        public int SignatureID { get; set; }
        public int OrdinanceID { get; set; }
        public string SignatureType { get; set; }
        public string Signature { get; set; }
        public DateTime DateSigned { get; set; }
        public bool SignatureCertified { get; set; }
        public int SortOrder { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
    }
}
