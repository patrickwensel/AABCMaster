using AABC.Data.V2;
using AABC.Domain2.Payments;
using AABC.DomainServices.Payments.Gateways;
using AABC.DomainServices.Payments.Repositories;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AABC.DomainServices.Payments
{
    public class PaymentManager
    {
        private readonly IPaymentGateway PaymentGateway;
        private readonly IRepository<CreditCard> CreditCardRepository;
        private readonly IPaymentScheduleRepository PaymentScheduleRepository;
        private readonly DatesGenerator DatesGenerator;
        public IPaymentRepository PaymentRepository { get; set; }
        public IPaymentChargeRepository PaymentChargeRepository { get; set; }
        public PaymentManagerConfiguration Configuration { get; private set; }

        public PaymentManager(
            IPaymentGateway paymentGateway,
            IPaymentRepository paymentRepository,
            IRepository<CreditCard> cardRepository,
            IPaymentScheduleRepository scheduleRepository,
            IPaymentChargeRepository chargeRepository,
            DatesGenerator datesGenerator,
            PaymentManagerConfiguration configuration
        )
        {
            PaymentGateway = paymentGateway;
            PaymentRepository = paymentRepository;
            CreditCardRepository = cardRepository;
            PaymentScheduleRepository = scheduleRepository;
            PaymentChargeRepository = chargeRepository;
            DatesGenerator = datesGenerator;
            Configuration = configuration;
        }

        public PaymentManager(CoreContext context, PaymentManagerConfiguration configuration) : this(
                new Stripe(),
                new PaymentRepository(context),
                new CreditCardRepository(context),
                new PaymentScheduleRepository(context),
                new PaymentChargeRepository(context),
                new DatesGenerator(),
                configuration
            )
        { }


        public OperationResult SavePayment(PatientLoginInfo patientLoginInfo, PaymentCreationParameters paymentCreationParameters, int? managementUserId)
        {
            if (patientLoginInfo == null) throw new ArgumentNullException(nameof(patientLoginInfo));
            if (paymentCreationParameters == null) throw new ArgumentNullException(nameof(paymentCreationParameters));
            ValidatePaymentCreationParameters(paymentCreationParameters);

            try
            {
                //save payment data locally
                var paymentDb = PaymentRepository.Create();
                Mapper.ToPayment(paymentCreationParameters, paymentDb);
                paymentDb.Active = true;
                paymentDb.LoginId = patientLoginInfo.ID;
                paymentDb.ManagementUserId = managementUserId;
                PaymentRepository.Insert(paymentDb);

                //create the customer in stripe
                var r = PaymentGateway.Customer(patientLoginInfo.ID, patientLoginInfo.FirstName + " " + patientLoginInfo.LastName, patientLoginInfo.Email);
                if (r["error"] != null)
                {
                    throw new Exception(r.error.message.ToString());
                }

                //create the card in stripe
                var c = PaymentGateway.CreateCard(patientLoginInfo.ID, paymentCreationParameters.CardHolder, paymentCreationParameters.CardNumber, paymentCreationParameters.CardExpiryMonth, paymentCreationParameters.CardExpiryYear, paymentCreationParameters.CardSecurityCode);
                if (c["error"] != null)
                {
                    throw new Exception(r.error.message.ToString());
                }

                //save cc data locally
                var card = CreditCardRepository.Create();
                Mapper.ToCreditCard(paymentCreationParameters, card);
                card.LoginId = patientLoginInfo.ID;
                card.GatewayType = PaymentGateway.GetId();
                card.GatewayCardId = c["id"];
                card.CardType = c["brand"];
                CreditCardRepository.Insert(card);

                //update payment with cc data
                paymentDb.CreditCardId = card.Id;
                PaymentRepository.Save(paymentDb);

                if (paymentCreationParameters.PaymentType == PaymentType.recurring)
                {
                    //create instance of a recurring payment
                    var start = paymentCreationParameters.RecurringDateStart.Value;
                    var end = paymentCreationParameters.RecurringDateEnd.Value;
                    var dates = DatesGenerator.GetDates(start, end, paymentCreationParameters.RecurringFrequency.Value).Select((m, i) => new { date = m, index = i });
                    if (dates.Count() == 0)
                    {
                        throw new InvalidOperationException("No payments to schedule.");
                    }
                    var schedules = new List<PaymentSchedule>();
                    foreach (var d in dates)
                    {
                        var schedule = PaymentScheduleRepository.Create();
                        schedule.PaymentId = paymentDb.Id;
                        schedule.ScheduledDate = d.date;
                        schedule.ScheduleNumber = d.index + 1;
                        schedules.Add(schedule);
                    }
                    PaymentScheduleRepository.Insert(schedules);
                }
                else
                {
                    if (paymentCreationParameters.OneTimePaymentDate.Value.Date == DateTime.Today)
                    {
                        //charge to cc since this is a onetime payment scheduled for today
                        var chargeParameters = new PaymentChargeParameters
                        {
                            Description = "One Time Charge -  Payment Id " + paymentDb.Id.ToString("D5"),
                            Amount = paymentCreationParameters.Amount,
                            CustomerId = patientLoginInfo.ID,
                            GatewayCardId = card.GatewayCardId,
                            PaymentId = paymentDb.Id,
                            CardId = card.Id,
                            ReferenceType = "Payment",
                            ReferenceId = paymentDb.Id
                        };
                        return Charge(chargeParameters);
                    }
                    else
                    {
                        var schedule = PaymentScheduleRepository.Create();
                        schedule.PaymentId = paymentDb.Id;
                        schedule.ScheduledDate = paymentCreationParameters.OneTimePaymentDate.Value.Date;
                        schedule.ScheduleNumber = 1;
                        PaymentScheduleRepository.Insert(schedule);
                    }
                }
            }
            catch (Exception e)
            {
                return new OperationResult { Success = false, Message = e.Message };
            }
            return new OperationResult { Success = true };
        }

        public ChargeOperationResult Charge(PaymentChargeParameters paymentChargeParameters)
        {
            if (paymentChargeParameters == null) throw new ArgumentNullException(nameof(paymentChargeParameters));
            ValidatePaymentChargeParameters(paymentChargeParameters);
            var ch = PaymentGateway.Charge(
                paymentChargeParameters.Description,
                (int)(paymentChargeParameters.Amount * 100),
                paymentChargeParameters.CustomerId,
                paymentChargeParameters.GatewayCardId);
            if (ch["error"] != null)
            {
                return new ChargeOperationResult { Success = false, Message = ch.error.message.ToString() };
            }

            //log transaction details
            var charge = PaymentChargeRepository.Create();
            charge.ChargeDate = DateTime.Now;
            //LoginId is redundant, can be obtained through the payment, but we still save the FK here
            charge.LoginId = paymentChargeParameters.CustomerId;
            charge.PaymentId = paymentChargeParameters.PaymentId;
            charge.CreditCardId = paymentChargeParameters.CardId;
            charge.Description = paymentChargeParameters.Description;
            charge.Amount = paymentChargeParameters.Amount;
            charge.ReferenceId = paymentChargeParameters.PaymentId;
            charge.ReferenceType = paymentChargeParameters.ReferenceType;
            charge.GatewayChargeId = ch["id"];
            charge.Result = ch["outcome"]["type"].ToString();
            charge.ResultDetails = JsonConvert.SerializeObject(ch["outcome"], Formatting.None);
            PaymentChargeRepository.Insert(charge);
            return new ChargeOperationResult { Success = true, PaymentChargeId = charge.Id };
        }

        public IEnumerable<RecurringChargeOperationResult> PayUnpaidSchedules(DateTime date)
        {
            var results = new List<RecurringChargeOperationResult>();
            var schedules = PaymentScheduleRepository.GetUnpaidSchedules(date);
            foreach (var s in schedules)
            {
                var chargeParameters = new PaymentChargeParameters
                {
                    Description = "Recurring Charge -  Payment Id " + s.PaymentId.ToString("D5") + " - Schedule No. " + s.ScheduleNumber.ToString("D2"),
                    Amount = s.Payment.Amount,
                    CustomerId = s.Payment.LoginId,
                    GatewayCardId = s.Payment.CreditCard.GatewayCardId,
                    CardId = s.Payment.CreditCardId.Value,
                    PaymentId = s.PaymentId,
                    ReferenceType = "Payment Schedule",
                    ReferenceId = s.Id
                };
                var result = Charge(chargeParameters);
                if (result.Success)
                {
                    s.PaymentChargeId = result.PaymentChargeId;
                    PaymentScheduleRepository.Save(s);
                    results.Add(new RecurringChargeOperationResult { Success = true, Message = result.Message, PaymentScheduleId = s.Id, PaymentChargeId = result.PaymentChargeId });
                }
                else
                {
                    results.Add(new RecurringChargeOperationResult { Success = false, Message = result.Message, PaymentScheduleId = s.Id });
                }
            }
            return results;
        }

        public OperationResult StopScheduledPayment(int id)
        {
            var payment = PaymentRepository.GetById(id);
            if (payment != null && payment.PaymentSchedules.Any())
            {
                payment.Active = false;
                PaymentRepository.Save(payment);
                return new OperationResult { Success = true };
            }
            return new OperationResult { Success = false };
        }

        private void ValidatePaymentCreationParameters(PaymentCreationParameters p)
        {
            if (p.PatientId == default(int)) throw new ArgumentException("PatientId must be provided.");
            if (p.Amount <= 0) throw new ArgumentException("Amount should be greater than zero.");
            switch (p.PaymentType)
            {
                case PaymentType.onetime:
                    if (!p.OneTimePaymentDate.HasValue) throw new ArgumentException(nameof(p.OneTimePaymentDate));
                    if (p.OneTimePaymentDate.Value.Date < DateTime.Today) throw new ArgumentException("OneTimePaymentDate cannot contain a date in the past.");
                    if (p.OneTimePaymentDate.Value.Date > DateTime.Today.AddDays(Configuration.OneTimeTransactionTimeWindow)) throw new ArgumentException($"One time transactions can only be scheduled within {Configuration.OneTimeTransactionTimeWindow} days.");
                    break;
                case PaymentType.recurring:
                    if (!p.RecurringDateStart.HasValue) throw new ArgumentException(nameof(p.RecurringDateStart));
                    if (!p.RecurringDateEnd.HasValue) throw new ArgumentException(nameof(p.RecurringDateEnd));
                    if (p.RecurringDateStart.Value.Date < DateTime.Today) throw new ArgumentException("RecurringDateStart cannot contain a date in the past.");
                    if (p.RecurringDateStart.Value.Date > DateTime.Today.AddMonths(Configuration.RecurringTransactionTimeWindow)) throw new ArgumentException($"Recurring transactions can only be scheduled within {Configuration.RecurringTransactionTimeWindow} months.");
                    if (p.RecurringDateStart.Value.Date > p.RecurringDateEnd.Value.Date) throw new ArgumentException("RecurringDateEnd cannot be greater than RecurringDateStart.");
                    break;
            }
        }

        private void ValidatePaymentChargeParameters(PaymentChargeParameters paymentChargeParameters)
        {
            if (string.IsNullOrEmpty(paymentChargeParameters.Description)) throw new ArgumentNullException(nameof(paymentChargeParameters.Description));
            if (paymentChargeParameters.Amount <= 0) throw new ArgumentException("Amount should be greater than zero.");
            if (string.IsNullOrEmpty(paymentChargeParameters.GatewayCardId)) throw new ArgumentNullException(nameof(paymentChargeParameters.GatewayCardId));
        }
    }
}
