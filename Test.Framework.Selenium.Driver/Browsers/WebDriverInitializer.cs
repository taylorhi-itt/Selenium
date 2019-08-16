using System;
using System.IO;
using System.Threading;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Test.Framework.Selenium.Driver.Browsers
{
    public static class WebDriverInitializer
    {

        public static IWebDriver Start()
        {
            ChromeOptions options = new ChromeOptions();
            options.AddArguments("chrome.switches", "--disable-extensions");
            options.AddArgument("--no-sandbox");
            IWebDriver driver = new ChromeDriver("."); 
            driver.Manage().Window.Maximize();
            driver.Manage().Cookies.DeleteAllCookies();

            return driver;
        }

    }
}