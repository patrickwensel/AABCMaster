using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System;

namespace AABC.Web.Tests.UI
{
    [TestClass]
    [TestCategory("_UI")]
    [TestCategory("_UI_AABC.Web")]
    public class WebTests
    {

        public const string basePath = "http://localhost:58997";

        private static IWebDriver driver;
        private static OpenQA.Selenium.Support.UI.WebDriverWait defaultWait;

        [ClassInitialize]
        public static void  Setup(TestContext context)
        {

            driver = new FirefoxDriver();
            defaultWait = new OpenQA.Selenium.Support.UI.WebDriverWait(driver, TimeSpan.FromSeconds(15));

            driver.Navigate().GoToUrl(basePath);
            var userName = driver.FindElement(By.Name("UserName"));
            userName.SendKeys("kmccarty2");
            var password = driver.FindElement(By.Name("Password"));
            password.SendKeys("2LSH7DX8");
            
            var now = DateTime.Now;
            defaultWait.PollingInterval = TimeSpan.FromMilliseconds(500);
            defaultWait.Until(x => (DateTime.Now - now) - TimeSpan.FromMilliseconds(500) > TimeSpan.Zero);

            password.Submit();

            defaultWait.Until(drv => drv.FindElement(By.Name("CurrentUser")));
        }

        [TestMethod]
        public void CaseManager_SummaryView()
        {
            driver.Navigate().GoToUrl(basePath + "/Case/418/Manage/Summary");
            var element = defaultWait.Until(drv => drv.FindElement(By.Id("PatientName")));
            Assert.AreEqual("David Protovin", element.Text);
        }

        [TestMethod]
        public void CaseManager_DischargeAndCancel()
        {
            driver.Navigate().GoToUrl(basePath + "/Case/418/Manage/Summary");
            var dischargeButton = defaultWait.Until(drv => drv.FindElement(By.Id("btnDischarge_CD")));
            dischargeButton.Click();

            var element = defaultWait.Until(drv => drv.FindElement(By.Id("DischargePopupHeader")));
            Assert.IsTrue(element.Displayed);

            var cancelButton = driver.FindElement(By.Id("btnDischargeCancel"));
            cancelButton.Click();
            Assert.IsFalse(element.Displayed);
        }

        [ClassCleanup]
        public static void EndTest()
        {
            driver.Close();
        }
    }
}
