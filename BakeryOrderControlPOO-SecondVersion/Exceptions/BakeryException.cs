namespace BakeryOrderControl.Exceptions;

/// <summary>Business error of the application (validations and rules).</summary>
public class BakeryException(string message) : Exception(message);
