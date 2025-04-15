
using System.Net.WebSockets;
using System.Text.Json;
using SerwerLogika;
using SharedModel;

namespace SerwerPrezentacja
{
    public abstract class IRequestHandler
    {
        public abstract Task<string> HandleRequest(string message, WebSocket webSocket);
        public abstract List<IPlant> GetAllPlantsForBroadcast();
    }

    internal class WebSocketRequestHandler : IRequestHandler
    {
        private readonly AbstractLogicAPI _logic;

        public WebSocketRequestHandler(AbstractLogicAPI logic)
        {
            _logic = logic;
        }

        public override List<IPlant> GetAllPlantsForBroadcast()
        {
            return _logic.GetAllPlants();
        }

        public override async Task<string> HandleRequest(string message, WebSocket webSocket)
        {
            if (message == "GET_PLANTS")
            {
                var plants = _logic.GetAllPlants();
                var options = new JsonSerializerOptions { WriteIndented = true };
                return JsonSerializer.Serialize(plants, options);
            }
            else if (message.StartsWith("PURCHASE:"))
            {
                var plantIdStr = message.Substring("PURCHASE:".Length);
                if (int.TryParse(plantIdStr, out int plantId))
                {
                    bool success = await _logic.PurchasePlantAsync(plantId);
                    return success ? "PURCHASE_SUCCESS" : "PURCHASE_FAILED";
                }
                return "ERROR: Invalid plant ID";
            }

            return "ERROR: Unknown command";
        }
    }
}
