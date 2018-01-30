using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;

namespace Notifications.WebAPI.UiTests
{
    [TestClass]
    public class Tests
    {
        [TestMethod]
        public void Test1()
        {
            IWebDriver driver = new FirefoxDriver();
            driver.Url = "http://google.com";
            driver.Close();
            driver.Quit();
            Assert.IsTrue(true);
        }
    }
}