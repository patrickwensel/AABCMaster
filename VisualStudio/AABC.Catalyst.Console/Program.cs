using System;
using System.Net;
using System.Text;

namespace AABC.Catalyst.Console
{
    class Program
    {
        static void Main(string[] args) {



            //APISandbox();

            //EPPSandbox();

            DocuSignSandbox();





        }


        private static void DocuSignSandbox() {

            Dymeng.DocuSign.DocuSignClient client = new Dymeng.DocuSign.DocuSignClient();

            byte[] fileBytes = System.IO.File.ReadAllBytes("E:\\Projects\\AABC\\Environment\\DocuSign\\DSTest.pdf");

            //client.LegacyAuthRequest("signerID", "Jack Leach", "jleach@dymeng.com", "test.pdf", fileBytes, "localhost");


        }




        private static void EPPSandbox() {

            string filepath = "E:\\temp\\catalyst_timesheet_test_import.xlsx";

            var importer = new DomainServices.Integrations.Catalyst.TimesheetImporter(filepath);

            importer.Import();

        }


        private static void APISandbox() {



            //const string endpoint = "https://sandbox.datafinch.com";
            const string endpoint = "https://secure.datafinch.com";
            const string Username = "5052C4FB-8EE7-4FFF-8EBD-13D296C6E33B";
            const string Password = "598F93AA-19BD-4D9D-A3BE-2DD5149296C7";

            System.Net.WebClient client = new System.Net.WebClient();

            //WebRequest request = WebRequest.Create(endpoint + "/api/students");
            //WebRequest request = WebRequest.Create(endpoint + "/api/reports?reportId=9F46FCA2-916F-45EC-820C-099783FD597F&startDate=2017-12-01&endDate=2017-12-31");
            WebRequest request = WebRequest.Create(endpoint + "/api/reports?reportId=0c1ec0a0-3b7d-44b1-b215-b4a0297c5d43&startDate=2017-12-01");



            string authInfo = Username + ":" + Password;
            authInfo = Convert.ToBase64String(Encoding.Default.GetBytes(authInfo));
            request.Headers["Authorization"] = "Basic " + authInfo;
            ((HttpWebRequest)request).UserAgent = "AABC Catalyst Api Integration Client";
            request.ContentType = "application/json";


            request.Method = "GET";

            WebResponse response = request.GetResponse();

            var data = response.GetResponseStream();
            string content = "";

            using (System.IO.StreamReader reader = new System.IO.StreamReader(data)) {
                content = reader.ReadToEnd();
            }

            System.Console.WriteLine(content);
            System.Console.ReadKey();


        }
    }
}
