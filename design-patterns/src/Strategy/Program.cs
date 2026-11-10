// Strategy Pattern: interchangeable payment algorithms.
// Run with: dotnet new console -n StrategyDemo && replace Program.cs with this file.

public interface IPaymentStrategy
{
    void Pay(decimal amount);
}

public sealed class UpiPayment : IPaymentStrategy
{
    public void Pay(decimal amount) =>
        Console.WriteLine($"Paid ₹{amount} using UPI");
}

public sealed class CardPayment : IPaymentStrategy
{
    public void Pay(decimal amount) =>
        Console.WriteLine($"Paid ₹{amount} using Credit Card");
}

public sealed class PaymentService(IPaymentStrategy strategy)
{
    public void Checkout(decimal amount) => strategy.Pay(amount);
}

public static class Program
{
    public static void Main()
    {
        IPaymentStrategy strategy = new UpiPayment();
        var paymentService = new PaymentService(strategy);
        paymentService.Checkout(500);

        // Swap algorithm without changing PaymentService:
        var cardService = new PaymentService(new CardPayment());
        cardService.Checkout(1200);
    }
}

/*
Interview answer:
"Strategy defines a family of algorithms, encapsulates each one, and makes them
interchangeable. The client depends on an abstraction rather than conditional
logic. I would use it for payment methods, discount rules, or shipping
calculations. It improves extensibility and testing, but introduces extra types."
*/
