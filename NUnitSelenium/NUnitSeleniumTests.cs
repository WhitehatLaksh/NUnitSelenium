using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Remote;
using NUnit.Framework;
using System.Threading;
using System.Collections.Generic;

namespace NUnitSelenium
{
    [TestFixture("chrome", "99", "Windows 11")]
   // [TestFixture("internet explorer", "11", "Windows 7")]
  //  [TestFixture("firefox", "60", "Windows 7")]
  //  [TestFixture("chrome", "71", "Windows 7")]
  //  [TestFixture("internet explorer", "11", "Windows 10")]
  //  [TestFixture("firefox", "58", "Windows 7")]
  //  [TestFixture("chrome", "67", "Windows 7")]
  //  [TestFixture("internet explorer", "10", "Windows 7")]
  //  [TestFixture("firefox", "55", "Windows 7")]
    [Parallelizable(ParallelScope.Children)]
    public class NUnitSeleniumSample
    {
        public static string LT_USERNAME = Environment.GetEnvironmentVariable("LT_USERNAME") == null ? "your username" : Environment.GetEnvironmentVariable("LT_USERNAME");
        public static string LT_ACCESS_KEY = Environment.GetEnvironmentVariable("LT_ACCESS_KEY") == null ? "your accessKey" : Environment.GetEnvironmentVariable("LT_ACCESS_KEY");
        public static bool tunnel = Boolean.Parse(Environment.GetEnvironmentVariable("LT_TUNNEL") == null ? "false" : Environment.GetEnvironmentVariable("LT_TUNNEL"));
        public static string build = Environment.GetEnvironmentVariable("LT_BUILD") == null ? "your build name" : Environment.GetEnvironmentVariable("LT_BUILD");
        public static string seleniumUri = "https://hub.lambdatest.com:443/wd/hub/";


        ThreadLocal<IWebDriver> driver = new ThreadLocal<IWebDriver>();
        private String browser;
        private String version;
        private String os;

        public NUnitSeleniumSample(String browser, String version, String os)
        {
            this.browser = browser;
            this.version = version;
            this.os = os;
        }

        [SetUp]
        public void Init()
        {

            DesiredCapabilities capabilities = new DesiredCapabilities();
            capabilities.SetCapability(CapabilityType.BrowserName, browser);
            capabilities.SetCapability(CapabilityType.Version, version);
            capabilities.SetCapability(CapabilityType.Platform, os);
            capabilities.SetCapability(CapabilityType.SupportsLocationContext, "GB");

            //Requires a named tunnel.
            if (tunnel)
            {
                capabilities.SetCapability("tunnel", tunnel);
            }
            if (build != null)
            {
                capabilities.SetCapability("build", "Download a file & Azure Pipeline");
            }

            capabilities.SetCapability("user", "lakshaysaini");
            capabilities.SetCapability("accessKey", "5dW8X6si7Fb3v8uXVQviU2iEXRrwXLCmgKk97SheWzxXuEFh9A");

            capabilities.SetCapability("name",
            String.Format("{0}:{1}",
            TestContext.CurrentContext.Test.ClassName,
            TestContext.CurrentContext.Test.MethodName));
            driver.Value = new RemoteWebDriver(new Uri("http://lakshaysaini:5dW8X6si7Fb3v8uXVQviU2iEXRrwXLCmgKk97SheWzxXuEFh9A@hub.lambdatest.com/wd/hub"), capabilities, TimeSpan.FromSeconds(600));
            Console.Out.WriteLine(driver);
        }

        [Test]
        public void Todotest()
        {
            {
                
                try
                {
                    Console.WriteLine("Navigating to provided web app to download pdf");
                    driver.Value.Navigate().GoToUrl("https://the-internet.herokuapp.com/download");
                    Thread.Sleep(1000);
                    driver.Value.FindElement(By.XPath("/html/body/div[2]/div/div/a[3]")).Click();
                    Thread.Sleep(5000);
                  //  Console.WriteLine(((IJavaScriptExecutor)driver.Value).ExecuteScript("lambda-file-stats=>sample.pdf"));​
                    String base64EncodedFile = ((IJavaScriptExecutor)driver.Value).ExecuteScript("lambda-file-content=>sample.pdf").ToString();
                    Console.WriteLine(base64EncodedFile);
                    byte[] data = System.Convert.FromBase64String(base64EncodedFile);
                    base64EncodedFile = System.Text.ASCIIEncoding.ASCII.GetString(data);
                    Console.WriteLine(data);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }
        }

        [TearDown]
        public void Cleanup()
        {
            bool passed = TestContext.CurrentContext.Result.Outcome.Status == NUnit.Framework.Interfaces.TestStatus.Passed;
            try
            {
                // Logs the result to LambdaTest
                ((IJavaScriptExecutor)driver.Value).ExecuteScript("lambda-status=" + (passed ? "passed" : "failed"));
            }
            finally
            {

                // Terminates the remote webdriver session
                driver.Value.Quit();
            }
        }
    }
}
