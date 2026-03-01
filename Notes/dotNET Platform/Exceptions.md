### *What an exception is and how it is used for error handling.*

An exception is an event that occurs during the execution of a program and disrupts its normal flow, often indicating an error or unexpected behavior. Exception handling in C# is done using try, catch, and finally blocks, where the try block contains code that might throw an exception, the catch block handles the exception(can have multiple), and the finally block executes code regardless of whether an exception occurred.

```csharp
try
{
    int divisor = 0;
    int result = 10 / divisor; // throws DivideByZeroException
}
catch (DivideByZeroException ex)
{
    Console.WriteLine("Error: " + ex.Message); // handle the exception
}
finally
{
    Console.WriteLine("Finally block executed."); // always runs
}
```

### *Basic classes in the exception hierarchy in .NET, such as Exception, SystemException, ApplicationException, and others, and their properties.*

The exception hierarchy is a structured way of representing exceptions, with System.Exception being the base class.

_Exception (System.Exception)_
Base class for all exceptions in .NET.
Key Properties:

- Message: Describes the error.
- StackTrace: Provides details of where the error occurred.
- InnerException: Captures any inner exceptions.
    
_SystemException_
Inherits from Exception.
Base class for exceptions that are thrown by the CLR (Common Language Runtime).
Examples include IndexOutOfRangeException, NullReferenceException, etc.

_CustomException_ (Application)
```csharp
// ❌ Outdated pattern
public class OrderException : ApplicationException { }

// ✅ Correct — derive from Exception directly
public class OrderNotFoundException : Exception
{
    public int OrderId { get; }
    
    public OrderNotFoundException(int orderId)
        : base($"Order {orderId} not found.")
    {
        OrderId = orderId;
    }
}
```

**Custom exception best practices:**
- Inherit from `Exception` (not `ApplicationException`).
- Provide at minimum: default constructor, message constructor, message + inner exception constructor.
- Add domain-specific properties (e.g., `OrderId`) for programmatic handling.
- Name them `*Exception` (e.g., `InsufficientFundsException`).

_ArgumentException_: Thrown when an argument passed to a method is invalid.

_ArgumentNullException_: Thrown when a null argument is passed to a method that does not accept it.

_ArgumentOutOfRangeException_: Thrown when an argument is outside the range of valid values.

_InvalidOperationException_: Thrown when a method call is invalid for the object's current state.

_IOException_: Thrown for errors during input/output operations.

_NullReferenceException_: Thrown when attempting to access a member on a type whose value is null.

### *The call stack, how exceptions propagate through the call stack, and how they can be caught and handled at different levels of code.*

The call stack is a record of function calls in a program, storing information about each active function's execution. If an exception is thrown, it travels up the call stack from the point of origin until it is caught by an appropriate catch block; if uncaught, it continues to the top of the stack, potentially leading to program termination. Exceptions can be intercepted and managed at various levels in the code, allowing for flexible and robust error handling.

call-stack:
ASP.NET pipeline       ← GlobalExceptionHandler catches it here
  ↓ no catch
TournamentController   
  ↓ no catch
RegisterTeam()         
  ↓ no catch
ValidateSlot()         ← throws ValidationException


Exception filters are a feature in C# that allow you to specify a condition within a catch clause.

```csharp
try
{
    // code that might throw
}
catch (Exception ex) when (ex.Message.Contains("specific condition"))
{
    // only catches exceptions where the condition is true
}
