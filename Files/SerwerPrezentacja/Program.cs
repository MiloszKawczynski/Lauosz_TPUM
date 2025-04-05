using SerwerLogika;
using SerwerPrezentacja;

var server = new WebSocketServer();
var discountNotifier = new DiscountWebSocketNotifier(server);
var logic = AbstractLogicAPI.CreateAPI(
    dataApi: null,
    discountNotifier: discountNotifier
);


server.Start();

Console.WriteLine("Serwer działa. Naciśnij dowolny klawisz, aby zakończyć...");
Console.ReadKey();