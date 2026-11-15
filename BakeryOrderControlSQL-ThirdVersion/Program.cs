using BakeryOrderControl.UI;

try
{
    new App().Start();
}
catch (Exception ex)
{
    Console.WriteLine($"The application could not start: {ex.Message}");
    Console.WriteLine("Make sure SQL Server is running and that the connection string in UI/App.cs is correct.");
    Console.WriteLine("Press Enter to close...");
    Console.ReadLine();
}
