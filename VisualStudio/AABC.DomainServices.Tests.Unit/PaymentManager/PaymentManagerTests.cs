using AABC.Domain2.Payments;
using AABC.DomainServices.Payments;
using AABC.DomainServices.Payments.Gateways;
using AABC.DomainServices.Payments.Repositories;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System;
using System.Collections.Generic;

namespace AABC.DomainServices.Tests.Unit.PaymentTests
{
    [TestClass]
    public class PaymentManagerTests
    {
        [TestMethod]
        [TestCategory("AABC.DomainServices.Payments")]
        [ExpectedException(typeof(ArgumentException))]
        public void Savepayment_throws_exception_when_amount_is_lower_or_equal_zero()
        {
            var services = CreateServices();
            var patientLoginInfo = CreatePatientLoginInfo();
            var paymentParameters = new PaymentCreationParameters
            {
                PaymentType = PaymentType.onetime,
                PatientId = 1,
                Amount = 0,
                OneTimePaymentDate = DateTime.Today,
                CardNumber = "4242424242424242",
                CardExpiryMonth = "05",
                CardExpiryYear = "2018",
                CardSecurityCode = "123"
            };
            var result = services.PaymentManager.SavePayment(patientLoginInfo, paymentParameters, null);
        }


        [TestMethod]
        [TestCategory("AABC.DomainServices.Payments")]
        [ExpectedException(typeof(ArgumentException))]
        public void One_time_payment_throws_exception_when_payment_date_is_null()
        {
            var services = CreateServices();
            var patientLoginInfo = CreatePatientLoginInfo();
            var paymentParameters = new PaymentCreationParameters
            {
                PaymentType = PaymentType.onetime,
                PatientId = 1,
                Amount = 100,
                OneTimePaymentDate = null,
                CardNumber = "4242424242424242",
                CardExpiryMonth = "05",
                CardExpiryYear = "2018",
                CardSecurityCode = "123"
            };
            var result = services.PaymentManager.SavePayment(patientLoginInfo, paymentParameters, null);
        }

        [TestMethod]
        [TestCategory("AABC.DomainServices.Payments")]
        [ExpectedException(typeof(ArgumentException))]
        public void One_time_payment_throws_exception_when_payment_date_is_in_the_past()
        {
            var services = CreateServices();
            var patientLoginInfo = CreatePatientLoginInfo();
            var paymentParameters = new PaymentCreationParameters
            {
                PaymentType = PaymentType.onetime,
                PatientId = 1,
                Amount = 100,
                OneTimePaymentDate = DateTime.Today.AddDays(-3),
                CardNumber = "4242424242424242",
                CardExpiryMonth = "05",
                CardExpiryYear = "2018",
                CardSecurityCode = "123"
            };
            var result = services.PaymentManager.SavePayment(patientLoginInfo, paymentParameters, null);
        }

        [TestMethod]
        [TestCategory("AABC.DomainServices.Payments")]
        [ExpectedException(typeof(ArgumentException))]
        public void One_time_payment_throws_exception_when_payment_date_is_outside_time_window()
        {
            var services = CreateServices();
            var patientLoginInfo = CreatePatientLoginInfo();
            var startDate = DateTime.Today.AddDays(services.PaymentManager.Configuration.OneTimeTransactionTimeWindow + 1);
            var paymentParameters = new PaymentCreationParameters
            {
                PaymentType = PaymentType.onetime,
                PatientId = 1,
                Amount = 100,
                OneTimePaymentDate = startDate,
                CardNumber = "4242424242424242",
                CardExpiryMonth = "05",
                CardExpiryYear = "2018",
                CardSecurityCode = "123"
            };
            var result = services.PaymentManager.SavePayment(patientLoginInfo, paymentParameters, null);
        }

        [TestMethod]
        [TestCategory("AABC.DomainServices.Payments")]
        public void One_time_payment_gets_charged_immediately()
        {
            var services = CreateServices();
            var patientLoginInfo = CreatePatientLoginInfo();
            var paymentParameters = new PaymentCreationParameters
            {
                PaymentType = PaymentType.onetime,
                PatientId = 1,
                Amount = 100,
                OneTimePaymentDate = DateTime.Today,
                CardNumber = "4242424242424242",
                CardExpiryMonth = "05",
                CardExpiryYear = "2018",
                CardSecurityCode = "123"
            };
            var result = services.PaymentManager.SavePayment(patientLoginInfo, paymentParameters, null);
            services.PaymentGatewayMock.Verify(m => m.Charge(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Once());
            services.PaymentScheduleRepositoryMock.Verify(m => m.Insert(It.IsAny<PaymentSchedule>()), Times.Never());

        }

        [TestMethod]
        [TestCategory("AABC.DomainServices.Payments")]
        public void One_time_payment_is_scheduled()
        {
            var services = CreateServices();
            var patientLoginInfo = CreatePatientLoginInfo();
            var paymentParameters = new PaymentCreationParameters
            {
                PaymentType = PaymentType.onetime,
                PatientId = 1,
                Amount = 100,
                OneTimePaymentDate = DateTime.Today.AddDays(2),
                CardNumber = "4242424242424242",
                CardExpiryMonth = "05",
                CardExpiryYear = "2018",
                CardSecurityCode = "123"
            };
            var result = services.PaymentManager.SavePayment(patientLoginInfo, paymentParameters, null);
            services.PaymentGatewayMock.Verify(m => m.Charge(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>()), Times.Never());
            services.PaymentScheduleRepositoryMock.Verify(m => m.Insert(It.IsAny<PaymentSchedule>()), Times.Once());
        }


        [TestMethod]
        [TestCategory("AABC.DomainServices.Payments")]
        [ExpectedException(typeof(ArgumentException))]
        public void Recurring_payment_throws_exception_when_recurring_start_date_is_null()
        {
            var services = CreateServices();
            var patientLoginInfo = CreatePatientLoginInfo();
            var paymentParameters = new PaymentCreationParameters
            {
                PaymentType = PaymentType.recurring,
                PatientId = 1,
                Amount = 100,
                RecurringDateStart = null,
                CardNumber = "4242424242424242",
                CardExpiryMonth = "05",
                CardExpiryYear = "2018",
                CardSecurityCode = "123"
            };
            var result = services.PaymentManager.SavePayment(patientLoginInfo, paymentParameters, null);
        }

        [TestMethod]
        [TestCategory("AABC.DomainServices.Payments")]
        [ExpectedException(typeof(ArgumentException))]
        public void Recurring_payment_throws_exception_when_recurring_end_date_is_null()
        {
            var services = CreateServices();
            var patientLoginInfo = CreatePatientLoginInfo();
            var paymentParameters = new PaymentCreationParameters
            {
                PaymentType = PaymentType.recurring,
                PatientId = 1,
                Amount = 100,
                RecurringDateStart = DateTime.Today,
                RecurringDateEnd = null,
                CardNumber = "4242424242424242",
                CardExpiryMonth = "05",
                CardExpiryYear = "2018",
                CardSecurityCode = "123"
            };
            var result = services.PaymentManager.SavePayment(patientLoginInfo, paymentParameters, null);
        }

        [TestMethod]
        [TestCategory("AABC.DomainServices.Payments")]
        [ExpectedException(typeof(ArgumentException))]
        public void Recurring_payment_throws_exception_when_recurring_start_date_is_in_the_past()
        {
            var services = CreateServices();
            var patientLoginInfo = CreatePatientLoginInfo();
            var paymentParameters = new PaymentCreationParameters
            {
                PaymentType = PaymentType.recurring,
                PatientId = 1,
                Amount = 100,
                RecurringDateStart = DateTime.Today.AddDays(-3),
                CardNumber = "4242424242424242",
                CardExpiryMonth = "05",
                CardExpiryYear = "2018",
                CardSecurityCode = "123"
            };
            var result = services.PaymentManager.SavePayment(patientLoginInfo, paymentParameters, null);
        }

        [TestMethod]
        [TestCategory("AABC.DomainServices.Payments")]
        [ExpectedException(typeof(ArgumentException))]
        public void Recurring_payment_throws_exception_when_recurring_start_date_is_outside_time_window()
        {
            var services = CreateServices();
            var patientLoginInfo = CreatePatientLoginInfo();
            var startDate = DateTime.Today.AddMonths(services.PaymentManager.Configuration.RecurringTransactionTimeWindow + 1);
            var paymentParameters = new PaymentCreationParameters
            {
                PaymentType = PaymentType.recurring,
                PatientId = 1,
                Amount = 100,
                RecurringDateStart = startDate,
                RecurringDateEnd = startDate.AddDays(10),
                CardNumber = "4242424242424242",
                CardExpiryMonth = "05",
                CardExpiryYear = "2018",
                CardSecurityCode = "123"
            };
            var result = services.PaymentManager.SavePayment(patientLoginInfo, paymentParameters, null);
        }

        [TestMethod]
        [TestCategory("AABC.DomainServices.Payments")]
        [ExpectedException(typeof(ArgumentException))]
        public void Recurring_payment_throws_exception_when_recurring_start_date_is_greater_than_end_date()
        {
            var services = CreateServices();
            var patientLoginInfo = CreatePatientLoginInfo();
            var paymentParameters = new PaymentCreationParameters
            {
                PaymentType = PaymentType.recurring,
                PatientId = 1,
                Amount = 100,
                RecurringDateStart = DateTime.Today.AddDays(3),
                RecurringDateEnd = DateTime.Today.AddDays(2),
                CardNumber = "4242424242424242",
                CardExpiryMonth = "05",
                CardExpiryYear = "2018",
                CardSecurityCode = "123"
            };
            var result = services.PaymentManager.SavePayment(patientLoginInfo, paymentParameters, null);
        }

        [TestMethod]
        [TestCategory("AABC.DomainServices.Payments")]
        public void Recurring_payment_creates_scheduled_payments()
        {
            var services = CreateServices();
            var patientLoginInfo = CreatePatientLoginInfo();
            var paymentParameters = new PaymentCreationParameters
            {
                PaymentType = PaymentType.recurring,
                PatientId = 1,
                Amount = 100,
                RecurringFrequency = RecurringFrequency.weekly,
                RecurringDateStart = DateTime.Today,
                RecurringDateEnd = DateTime.Today.AddDays(30),
                CardNumber = "4242424242424242",
                CardExpiryMonth = "05",
                CardExpiryYear = "2018",
                CardSecurityCode = "123"
            };
            var result = services.PaymentManager.SavePayment(patientLoginInfo, paymentParameters, null);
            services.PaymentScheduleRepositoryMock.Verify(m => m.Insert(It.IsAny<IEnumerable<PaymentSchedule>>()), Times.Once());
        }




        private static PatientLoginInfo CreatePatientLoginInfo()
        {
            var patientLoginInfo = new PatientLoginInfo
            {
                ID = 1,
                LastName = "Smith",
                FirstName = "John",
                Email = "john@smith.net"
            };
            return patientLoginInfo;
        }


        private static Services CreateServices()
        {
            var services = new Services
            {
                PaymentGatewayMock = new Mock<IPaymentGateway>(),
                PaymentRepositoryMock = new Mock<IPaymentRepository>(),
                CreditCardRepositoryMock = new Mock<IRepository<CreditCard>>(),
                PaymentScheduleRepositoryMock = new Mock<IPaymentScheduleRepository>(),
                PaymentChargeRepositoryMock = new Mock<IPaymentChargeRepository>()
            };
            services.PaymentManager = new PaymentManager(
               services.PaymentGatewayMock.Object,
               services.PaymentRepositoryMock.Object,
               services.CreditCardRepositoryMock.Object,
               services.PaymentScheduleRepositoryMock.Object,
               services.PaymentChargeRepositoryMock.Object,
               new DatesGenerator(),
               PaymentManagerConfiguration.CreateDefault()
            );
            services.PaymentRepositoryMock.Setup(m => m.Create()).Returns(new Payment());
            services.PaymentRepositoryMock.Setup(m => m.Insert(It.IsAny<Payment>())).Callback((Payment p) =>
            {
                p.Id = new Random().Next(1, 1000);
            });
            services.CreditCardRepositoryMock.Setup(m => m.Create()).Returns(new CreditCard());
            services.CreditCardRepositoryMock.Setup(m => m.Insert(It.IsAny<CreditCard>())).Callback((CreditCard p) =>
            {
                p.Id = new Random().Next(1, 1000);
            });
            services.PaymentScheduleRepositoryMock.Setup(m => m.Create()).Returns(new PaymentSchedule());
            services.PaymentScheduleRepositoryMock.Setup(m => m.Insert(It.IsAny<PaymentSchedule>())).Callback((PaymentSchedule p) =>
            {
                p.Id = new Random().Next(1, 1000);
            });
            services.PaymentChargeRepositoryMock.Setup(m => m.Create()).Returns(new PaymentCharge());
            services.PaymentChargeRepositoryMock.Setup(m => m.Insert(It.IsAny<PaymentCharge>())).Callback((PaymentCharge p) =>
            {
                p.Id = new Random().Next(1, 1000);
            });
            services.PaymentGatewayMock.Setup(m => m.Customer(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Dictionary<string, string> { { "error", null } });
            services.PaymentGatewayMock.Setup(m => m.CreateCard(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>())).Returns(new Dictionary<string, string> { { "error", null }, { "id", "creditcard" }, { "brand", "visa" } });
            services.PaymentGatewayMock.Setup(m => m.Charge(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<string>())).Returns(new Dictionary<string, object> { { "error", null }, { "id", "transaction_id" }, { "outcome", new Dictionary<string, string> { { "type", "type" } } } });
            return services;
        }

        private class Services
        {
            public Mock<IPaymentGateway> PaymentGatewayMock { get; set; }
            public Mock<IPaymentRepository> PaymentRepositoryMock { get; set; }
            public Mock<IRepository<CreditCard>> CreditCardRepositoryMock { get; set; }
            public Mock<IPaymentScheduleRepository> PaymentScheduleRepositoryMock { get; set; }
            public Mock<IPaymentChargeRepository> PaymentChargeRepositoryMock { get; set; }
            public PaymentManager PaymentManager { get; set; }

        }
    }
}
