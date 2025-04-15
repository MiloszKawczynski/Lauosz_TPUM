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
logic.AddNewPlant("Kaktus", 21.37f);
logic.AddNewPlant("Róża", 69.69f);
logic.AddNewPlant("Tulipan", 42.00f);
logic.AddNewPlant("Gerber", 51.50f);
logic.AddNewPlant("Monstera", 38.17f);
logic.AddNewPlant("Fiołek", 12.34f);
Console.WriteLine("Serwer działa. Naciśnij dowolny klawisz, aby zakończyć...");
Console.ReadKey();