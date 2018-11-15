namespace AABC.DomainServices.Payments.Gateways
{
    public interface IPaymentGateway
    {
        string GetId();
        dynamic Charge(string Description, int AmountInCents, int CustomerId, string SourceId);
        dynamic Charge(string Description, int AmountInCents, string Cardholder, string CardNumber, string ExpiryMonth, string ExpiryYear, string CVC);
        dynamic CreateCard(int CustomerId, string Cardholder, string CardNumber, string ExpiryMonth, string ExpiryYear, string CVC);
        dynamic Customer(int Id, string Description, string Email);
    }
}