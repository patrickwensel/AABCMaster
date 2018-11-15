using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Net;
using System.Net.Http;
using System.Text;
namespace AABC.DomainServices.PaymentGateways
{
    public static class Stripe
    {
        public static dynamic Charge(string Description, int AmountInCents, string Cardholder, string CardNumber, int ExpiryMonth, int ExpiryYear, int CVC)
        {
            var charge = new FormUrlEncodedContent(new[]
                 {
                    new KeyValuePair<string, string>("description", Description),
                    new KeyValuePair<string, string>("amount", AmountInCents.ToString()),
                    new KeyValuePair<string, string>("currency", "USD"),
                    new KeyValuePair<string, string>("source[name]", Cardholder),
                    new KeyValuePair<string, string>("source[number]", CardNumber),
                    new KeyValuePair<string, string>("source[object]", "card"),
                    new KeyValuePair<string, string>("source[exp_month]", ExpiryMonth.ToString()),
                    new KeyValuePair<string, string>("source[exp_year]", ExpiryYear.ToString()),
                    new KeyValuePair<string, string>("source[cvc]", CVC.ToString())

                });

            return PostForm("charges", charge);

        }
        public static dynamic Charge(string Description, int AmountInCents, int CustomerId, string SourceId)
        {
            var charge = new FormUrlEncodedContent(new[]
                 {
                    new KeyValuePair<string, string>("description", Description),
                    new KeyValuePair<string, string>("amount", AmountInCents.ToString()),
                    new KeyValuePair<string, string>("currency", "USD"),
                    new KeyValuePair<string, string>("customer", CustomerId.ToString()),
                    new KeyValuePair<string, string>("source", SourceId),
                });

            return PostForm("charges", charge);

        }
        public static dynamic Customer(int Id, string Description, string Email)
        {
            string route = "customers/" + Id;
            var r = GetUrl(route);
            if (r["error"]!=null)
            {
                if (r.error.message.ToString().IndexOf("No such customer") >= 0)
                {

                }
                else
                {
                    throw new Exception(r.error.message.ToString());
                }
            }

            var customer = new List<KeyValuePair<string, string>>();
            customer.Add(new KeyValuePair<string, string>("description", Description));
            customer.Add(new KeyValuePair<string, string>("email", Email));

            if (r["id"] != null)
            {
                return PostForm(route, new FormUrlEncodedContent(customer.ToArray())); //edit

            }
            else
            {
                customer.Add(new KeyValuePair<string, string>("id", Id.ToString()));

                return PostForm("customers", new FormUrlEncodedContent(customer.ToArray())); //create new
            }

        }
        public static dynamic CreateCard(int CustomerId, string Cardholder, string CardNumber, int ExpiryMonth, int ExpiryYear, int CVC)
        {
            var card = new List<KeyValuePair<string, string>>();
            card.Add(new KeyValuePair<string, string>("source[name]", Cardholder));
            card.Add(new KeyValuePair<string, string>("source[number]", CardNumber));
            card.Add(new KeyValuePair<string, string>("source[object]", "card"));
            card.Add(new KeyValuePair<string, string>("source[exp_month]", ExpiryMonth.ToString()));
            card.Add(new KeyValuePair<string, string>("source[exp_year]", ExpiryYear.ToString()));
            card.Add(new KeyValuePair<string, string>("source[cvc]", CVC.ToString()));
            return PostForm("customers/" + CustomerId + "/sources", new FormUrlEncodedContent(card.ToArray())); //create new

        }


        public static dynamic Subscription(int CustomerId, string PlanId, int AmountInCents, string CardNumber, int ExpiryMonth, int ExpiryYear, int CVC, DateTime DateStart)
        {
            var subscription = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("customer", CustomerId.ToString()),
                    new KeyValuePair<string, string>("quantity", AmountInCents.ToString()),
                    new KeyValuePair<string, string>("plan", PlanId),
                    new KeyValuePair<string, string>("start", ConvertToTimestamp(DateStart).ToString()),
                    new KeyValuePair<string, string>("source[number]", CardNumber),
                    new KeyValuePair<string, string>("source[object]", "card"),
                    new KeyValuePair<string, string>("source[exp_month]", ExpiryMonth.ToString()),
                    new KeyValuePair<string, string>("source[exp_year]", ExpiryYear.ToString()),
                    new KeyValuePair<string, string>("source[cvc]", CVC.ToString())

                });

            return PostForm("subscriptions", subscription);

        }
        public static dynamic GetUrl(string route)
        {
            // specify to use TLS 1.2 as default connection
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            using (HttpClient client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["PaymentGateways.Stripe.ApiKey"].ToString());
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                using (HttpResponseMessage response = client.GetAsync(ConfigurationManager.AppSettings["PaymentGateways.Stripe.ApiEndpoint"].ToString() + "/" + route).Result)
                using (HttpContent content = response.Content)
                {
                    string result = content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject(result);
                }
            }
        }
        public static dynamic PostForm(string route, FormUrlEncodedContent form)
        {
            // specify to use TLS 1.2 as default connection
            System.Net.ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;


            using (HttpClient client = new HttpClient())
            {
                var byteArray = Encoding.ASCII.GetBytes(ConfigurationManager.AppSettings["PaymentGateways.Stripe.ApiKey"].ToString());
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteArray));

                using (HttpResponseMessage response = client.PostAsync(ConfigurationManager.AppSettings["PaymentGateways.Stripe.ApiEndpoint"].ToString() + "/" + route, form).Result)
                using (HttpContent content = response.Content)
                {
                    string result = content.ReadAsStringAsync().Result;
                    return JsonConvert.DeserializeObject(result);
                }
            }
        }
        public static bool HasProperty(dynamic settings, string name)
        {
            return settings.GetType().GetProperty(name) != null;
        }
        private static long ConvertToTimestamp(DateTime value)
        {
            long epoch = (value.Ticks - 621355968000000000) / 10000000;
            return epoch;
        }
    }
    
}