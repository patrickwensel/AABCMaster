using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;

namespace AABC.DomainServices.PaymentGateways
{
    public class StripeForRBT
    {

        private readonly string ApiKey;
        private readonly string ApiEndPoint;

        public StripeForRBT(string apiKey, string apiEndPoint)
        {
            if (string.IsNullOrEmpty(apiKey)) throw new ArgumentNullException(nameof(apiKey));
            if (string.IsNullOrEmpty(apiEndPoint)) throw new ArgumentNullException(nameof(apiEndPoint));
            ApiKey = apiKey;
            ApiEndPoint = apiEndPoint;
        }

        public PaymentTransactionResult Charge(string Description, int AmountInCents, string Cardholder, string CardNumber, string ExpiryMonth, string ExpiryYear, string CVC, string Email)
        {
            var charge = new FormUrlEncodedContent(new[]
                 {
                    new KeyValuePair<string, string>("description", Description),
                    new KeyValuePair<string, string>("amount", AmountInCents.ToString()),
                    new KeyValuePair<string, string>("currency", "USD"),
                    new KeyValuePair<string, string>("source[name]", Cardholder),
                    new KeyValuePair<string, string>("source[number]", CardNumber),
                    new KeyValuePair<string, string>("source[object]", "card"),
                    new KeyValuePair<string, string>("source[exp_month]", ExpiryMonth),
                    new KeyValuePair<string, string>("source[exp_year]", ExpiryYear),
                    new KeyValuePair<string, string>("source[cvc]", CVC),
                    new KeyValuePair<string, string>("receipt_email", Email)
                });
            return PostForm("charges", charge);
        }

        private PaymentTransactionResult PostForm(string route, FormUrlEncodedContent form)
        {
            // specify to use TLS 1.2 as default connection
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            using (HttpClient client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(ApiKey);
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                using (HttpResponseMessage response = client.PostAsync(ApiEndPoint + "/" + route, form).Result)
                using (HttpContent content = response.Content)
                {
                    string result = content.ReadAsStringAsync().Result;
                    if (response.StatusCode == HttpStatusCode.OK)
                    {
                        var s = JsonConvert.DeserializeObject<StripeEntityWithId>(result);
                        var ret = new PaymentTransactionResult
                        {
                            Success = true,
                            TransactionId = s.Id
                        };
                        return ret;
                    }
                    else
                    {
                        var s = JsonConvert.DeserializeObject<StripeErrorWrapper>(result);
                        var ret = new PaymentTransactionResult
                        {
                            Success = false,
                            Error = s.Error.Message
                        };
                        return ret;
                    }
                }
            }
        }
    }
    public class PaymentTransactionResult
    {
        public bool Success { get; set; }
        public string Error { get; set; }
        public string TransactionId { get; set; }
    }

    public class StripeEntityWithId
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }

    public class StripeErrorWrapper
    {

        [JsonProperty("error")]
        public StripeError Error { get; set; }
    }

    public class StripeError
    {
        [JsonProperty("type")]
        public string ErrorType { get; set; }

        [JsonProperty("message")]
        public string Message { get; set; }

        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("param")]
        public string Parameter { get; set; }

        [JsonProperty("error")]
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }

        [JsonProperty("charge")]
        public string ChargeId { get; set; }

        [JsonProperty("decline_code")]
        public string DeclineCode { get; set; }
    }
}
