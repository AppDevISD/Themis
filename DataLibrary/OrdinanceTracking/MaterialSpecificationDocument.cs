using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    public class MaterialSpecificationDocument
    {
        public int SpecDocID { get; set; }
        public string DocDescription { get; set; }
        public byte[] DocImage { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
