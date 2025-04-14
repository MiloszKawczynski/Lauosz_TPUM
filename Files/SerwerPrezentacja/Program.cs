using SerwerLogika;
using SerwerPrezentacja;

var server = new WebSocketServer();
var discountNotifier = new DiscountWebSocketNotifier(server);
var logic = AbstractLogicAPI.CreateAPI(
    dataApi: null,
    discountNotifier: discountNotifier
);
var requestHandler = new WebSocketRequestHandler(logic);
server.SetRequestHandler(requestHandler);


server.Start();
logic.AddNewPlant("Kwiatek", 23.09f);
logic.AddNewPlant("Krzaczek", 23.09f);
Console.WriteLine("Serwer działa. Naciśnij dowolny klawisz, aby zakończyć...");
Console.ReadKey();