using BakeryOrderControl.Exceptions;

namespace BakeryOrderControl.UI;

/// <summary>Generic menu: each submenu inherits and defines its own options (polymorphism).</summary>
public abstract class MenuBase
{
    protected abstract string Title { get; }
    protected abstract string[] Options { get; }
    protected abstract void Execute(int option);

    public void Show()
    {
        var goBack = false;
        while (!goBack)
        {
            Console.Clear();
            Console.WriteLine($"--- {Title} ---");
            for (var i = 0; i < Options.Length; i++)
                Console.WriteLine($"{i + 1}. {Options[i]}");
            Console.WriteLine("0. Back");

            var option = ConsoleHelper.ReadInt("Option: ", 0, Options.Length);
            if (option == 0) { goBack = true; continue; }

            try
            {
                Execute(option);
            }
            catch (BakeryException ex)
            {
                ConsoleHelper.ShowError(ex.Message);
            }
            ConsoleHelper.Pause();
        }
    }
}
