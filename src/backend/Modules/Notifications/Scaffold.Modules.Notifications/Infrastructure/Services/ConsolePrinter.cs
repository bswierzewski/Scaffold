namespace Scaffold.Modules.Notifications.Infrastructure.Services;

public sealed class ConsolePrinter : IPrinter
{
    public void Print(string message)
    {
        Console.WriteLine(message);
    }
}