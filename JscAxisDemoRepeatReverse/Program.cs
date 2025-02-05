using JscAxisLib;
using System.Net;

Console.WriteLine("The Axis will move back and forth between 0 and 30'000");
IPAddress? ipAddress;
var axis = new JScAxis();
//axis.OnReceive += (message) => Console.WriteLine(message);

do {
    Console.Write("IP-Address: ");
} while(!IPAddress.TryParse(Console.ReadLine()?.Trim() ?? "", out ipAddress));

Console.WriteLine("Connecting");
axis.Connect(ipAddress);

Console.WriteLine("Power on");
axis.PowerContinuous();

Console.WriteLine("Refrence axis");
axis.Reference();

Console.WriteLine("Set speed to 10'000 inc/s");
axis.SendCommand("SP10000");

Console.WriteLine("Move axis, press any key to close");
while (!Console.KeyAvailable)
{
    axis.GoPosition(30000);
    axis.GoPosition(0);
}

axis.StopMoition();
axis.PowerQuit();
axis.Disconnect();