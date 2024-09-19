using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLibrary
{
    internal class Request
    {
        public int RequestID { get; set; }
        public string OfficeName { get; set; }
        public int UserID { get; set; }
        public DateTime DateFirstReading { get; set; }
        public string EmergencyPassage { get; set; }
        public string SuggestedTitle { get; set; }
        public string LastUpdateBy { get; set; }
        public DateTime LastUpdateDate { get; set; }
        public DateTime EffectiveDate { get; set; }
        public DateTime ExpirationDate { get; set; }
    }
}
