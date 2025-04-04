using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SerwerDane
{
    public static class WebSocketCommands
    {
        public const string Purchase = "PURCHASE";
        public const string PurchaseResponse = "PURCHASE_RESPONSE";
    }

    public class PurchaseRequest
    {
        public int PlantId { get; set; }
    }
}
