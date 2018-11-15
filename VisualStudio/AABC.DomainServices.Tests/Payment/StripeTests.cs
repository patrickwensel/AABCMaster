using AABC.DomainServices.Payments.Gateways;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace AABC.DomainServices.Tests.Payment
{

    [TestClass]
    public class StripeTests
    {

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("AABC.DomainServices.Payments.Stripe")]
        public void StripeGatewayCreatesCard()
        {
            var paymentGateway = new Stripe();
            var card = paymentGateway.CreateCard(1, "alain rex rivas", "4242424242424242", 12.ToString(), 2018.ToString(), 123.ToString());
            Assert.IsNotNull(card);
            Assert.IsNotNull(card["id"]);
            Assert.IsNull(card["error"]);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("AABC.DomainServices.Payments.Stripe")]
        public void StripeGatewayProcessesCharge()
        {
            var paymentGateway = new Stripe();
            var charge = paymentGateway.Charge("Order No. 1234", 1234, "Jack D. Leach", "4242424242424242", 12.ToString(), 2018.ToString(), 123.ToString());
            Assert.IsNotNull(charge);
            Assert.IsNull(charge["error"]);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("AABC.DomainServices.Payments.Stripe")]
        public void StripeGatewayCreatesCustomer()
        {
            var paymentGateway = new Stripe();
            int Id = 1;
            var customer = paymentGateway.Customer(Id, "alain rivas", "alainrivas@gmail.com");
            Assert.IsNotNull(customer);
            Assert.IsNull(customer["error"]);
        }

        [TestMethod]
        [TestCategory("Integration")]
        [TestCategory("AABC.DomainServices.Payments.Stripe")]
        public void StripeGatewayUpdatesCustomer()
        {
            var paymentGateway = new Stripe();
            var customer = paymentGateway.Customer(1, "alain rex rivas", "alainrivas@gmail.com");
            Assert.AreEqual("alain rex rivas", (string)customer.description);
        }

    }
}
