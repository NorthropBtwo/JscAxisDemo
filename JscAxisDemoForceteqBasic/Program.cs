using JscAxisLib;
using System.Net;
using System.Runtime.CompilerServices;

const int PSR_BIT_I_FORCE_LIMIT_REACHED = (1 << 15);

Console.WriteLine("The Axis will move from 0 inc and 30'000 inc with limited speed.");
IPAddress? ipAddress;
var axis = new JScAxis();

do
{
    Console.Write("IP-Address: ");
} while (!IPAddress.TryParse(Console.ReadLine()?.Trim() ?? "", out ipAddress));

Console.WriteLine("Connecting");
axis.Connect(ipAddress);

Console.WriteLine("Power on");
axis.PowerContinuous();

Console.WriteLine("Refrence axis");
axis.Reference();

Console.WriteLine("Force calibration from 0 to 30'000 increments");
axis.SendCommand("SP10000");
axis.ForceCalibration(0, 30000);


while (!Console.KeyAvailable)
{
    Console.WriteLine("Move axis forward slowly with limited speed");
    axis.SendCommand("SP10000");
    axis.SendCommand("LIF40");
    CancellationTokenSource cts = new CancellationTokenSource();
    var goPositionTask = Task.Run(() => axis.GoPosition(30000));
    var detectObstacleTask = Task.Run(() => DetectObstacle(cts.Token));
    Task.WaitAny(goPositionTask, detectObstacleTask);
    if (detectObstacleTask.IsCompleted)
    {
        Console.WriteLine("Obstacle detected, move axis back to 0 fast");
        axis.StopMoition();
        axis.SendCommand("SP100000");
    }
    else
    {
        cts.Cancel();
        Console.WriteLine("Move axis back to 0");
    }

    axis.SendCommand("LIF0");
    axis.GoPosition(0);
}

axis.StopMoition();
axis.PowerQuit();
axis.Disconnect();

void DetectObstacle(CancellationToken token)
{
    int psr;
    do
    {
        Thread.Sleep(100);
        try
        {
            psr = axis.TellPSR(token);
        }
        catch (OperationCanceledException)
        {
            return;
        }
    } while ((psr & PSR_BIT_I_FORCE_LIMIT_REACHED) == 0);
}