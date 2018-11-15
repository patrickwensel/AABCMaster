using AABC.Data.V2;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Linq;


namespace AABC.Data.Tests.Integration
{
    [TestClass]
    public class ModelInsertionTests
    {


        CoreContext context = new CoreContext();

        [TestInitialize]
        public void Initialize() {
            context.Database.Log = (s) => System.Diagnostics.Debug.WriteLine(s);
        }


        //[TestMethod]
        //[TestCategory("Integration")]
        //public void EF6CreatesNewLogin() {

        //    var login = new PatientPortal.Login();

        //    login.FirstName = randomString();
        //    login.LastName = randomString();
        //    login.Email = randomString();
        //    login.Password = "unused";

        //    context.Entry(login).State = System.Data.Entity.EntityState.Added;
        //    //context.PatientPortalLogins(login);
        //    context.SaveChanges();

        //    var detail = new PatientPortal.WebMembershipDetail();
        //    detail.ID = login.ID;
        //    detail.Password = "test password";
        //    login.WebMembershipDetail = detail;

        //    context.SaveChanges();

        //    Assert.IsTrue(login.ID > 0);
        //    Assert.IsTrue(detail.ID > 0);
        //    Assert.IsTrue(login.ID == detail.ID);

        //}





        private static Random random = new Random();
        string randomString(int charCount = 8) {

            const string chars = "abcdefghijklmnopqrstuvwxyz";
            return new string(Enumerable.Repeat(chars, charCount)
                .Select(s => s[random.Next(s.Length)]).ToArray());

        }

    }
}
