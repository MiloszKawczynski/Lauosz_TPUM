using SerwerLogika;
using SerwerPrezentacja;

var logic = AbstractLogicAPI.CreateAPI();
var server = new WebSocketServer(logic);
server.Start();

Console.WriteLine("Naciśnij dowolny klawisz, aby zakończyć...");
Console.ReadKey();