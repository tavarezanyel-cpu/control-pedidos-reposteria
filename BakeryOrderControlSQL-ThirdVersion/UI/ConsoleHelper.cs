using System.Globalization;
using BakeryOrderControl.Exceptions;

namespace BakeryOrderControl.UI;

/// <summary>Input/output helpers with validation so the program does not crash.</summary>
public static class ConsoleHelper
{
    public static string ReadText(string message, bool required = true)
    {
        while (true)
        {
            Console.Write(message);
            var text = (Console.ReadLine() ?? "").Trim();
            if (!required || text.Length > 0) return text;
            ShowError("This field cannot be empty.");
        }
    }

    public static int ReadInt(string message, int min = 1, int max = int.MaxValue)
    {
        while (true)
        {
            Console.Write(message);
            if (int.TryParse(Console.ReadLine(), out var number) && number >= min && number <= max) return number;
            ShowError($"Enter a whole number between {min} and {(max == int.MaxValue ? "infinity" : max.ToString())}.");
        }
    }

    public static decimal ReadDecimal(string message)
    {
        while (true)
        {
            Console.Write(message);
            if (TryParseDecimal(Console.ReadLine(), out var value) && value > 0) return value;
            ShowError("Enter a valid amount greater than 0.");
        }
    }

    /// <summary>Empty = null (keep the value). An invalid entry throws a BakeryException.</summary>
    public static decimal? ReadOptionalDecimal(string message)
    {
        Console.Write(message);
        var text = (Console.ReadLine() ?? "").Trim();
        if (text.Length == 0) return null;
        if (TryParseDecimal(text, out var value) && value > 0) return value;
        throw new BakeryException("Invalid price; no changes were made.");
    }

    public static bool Confirm(string message)
    {
        Console.Write($"{message} (Y/N): ");
        return (Console.ReadLine() ?? "").Trim().Equals("Y", StringComparison.OrdinalIgnoreCase);
    }

    public static void ShowAll<T>(IEnumerable<T> items, string emptyMessage = "No results.")
    {
        List<T> list = [.. items];
        if (list.Count == 0) { Console.WriteLine(emptyMessage); return; }
        foreach (var item in list) Console.WriteLine(item);
    }

    public static void ShowError(string message) => Console.WriteLine($"  ! {message}");

    public static void Pause()
    {
        Console.Write("\nPress Enter to continue...");
        Console.ReadLine();
    }

    private static bool TryParseDecimal(string? text, out decimal value) =>
        decimal.TryParse((text ?? "").Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out value);
}
