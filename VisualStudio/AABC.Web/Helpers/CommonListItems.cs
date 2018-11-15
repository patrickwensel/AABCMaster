using System;
using System.Collections.Generic;
using System.Linq;


namespace AABC.Web.Helpers
{
    public class CommonListItems
    {
        
        private static Data.V2.CoreContext context {
            get
            {
                return AppService.Current.DataContextV2;
            }
        }




        public static List<AuthCode> GetAuthCodes() {

            var auths = context.AuthorizationCode.OrderBy(x => x.Code).ToList();

            var items = new List<AuthCode>();

            foreach (var auth in auths) {

                items.Add(new AuthCode()
                {
                    ID = auth.ID,
                    Code = auth.Code,
                    Name = auth.Description
                });
            }

            return items;
        }



        public static List<ProviderType> GetProviderTypes() {

            var types = context.ProviderTypes.OrderBy(x => x.Code).ToList();

            var items = new List<ProviderType>();

            foreach (var type in types) {
                items.Add(new ProviderType()
                {
                    ID = type.ID,
                    Code = type.Code,
                    Name = type.Name
                });
            }

            return items;
        }

        public static List<Service> GetServices() {

            var sp = new DomainServices.Services.ServiceProvider(context);

            var services = sp.GetAllServices().OrderBy(x => x.Code).ToList();

            var items = new List<Service>();

            foreach (var service in services) {
                items.Add(new Service()
                {
                    ID = service.ID,
                    Code = service.Code,
                    Name = service.Name
                });
            }

            return items;
        }

        public static string GetCommonName(string firstName, string lastName)
        {
            string s = "";
            if (firstName != null)
            {
                s = firstName;
            }
            if (lastName != null)
            {
                if (s == "")
                {
                    s = lastName;
                }
                else
                {
                    s += " " + lastName;
                }
            }
            return s;
        }

        public static string GetAuthBreakdown(ICollection<Domain2.Hours.AuthorizationBreakdown> authBreakdowns)
        {
            if(authBreakdowns == null)
            {
                return "";
            }

            return String.Join(
                ", ",
                authBreakdowns.Select(x => x.Authorization?.AuthorizationCode.Code));
        }



        public class ProviderType
        {
            public int ID { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
        }

        public class Service
        {
            public int ID { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
        }

        public class AuthCode
        {
            public int ID { get; set; }
            public string Code { get; set; }
            public string Name { get; set; }
        }

    }
}