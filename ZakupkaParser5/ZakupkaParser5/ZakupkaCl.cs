using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ZakupkaParser5
{
    public class ZakupkaCl
    {
        public string NumberOfCheck { get; set; }
        public string CheckHref { get; set; }
        public string NumberOfPurchase { get; set; }
        public string PurchaseHref { get; set; }
        public string DateOfAccommodation { get; set; }
        public string DateOfCheck { get; set; }
        public string Backgr { get; set; } = "White";
    }
}
