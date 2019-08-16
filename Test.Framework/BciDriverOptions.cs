using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;
using Test.Framework.Selenium;
using Test.Framework.Selenium.Driver.Browsers;

namespace Test.Framework
{
    public class BciDriverOptions
    {
        public List<Action<BrowserActionType, BciWebBrowser, IWebDriver, IWebElement>> Before { get; set; }

        public List<Action<BrowserActionType, BciWebBrowser, IWebDriver, IWebElement>> After { get; set; }
        public Func<IWebDriver> CreateDriver { get; set; } = WebDriverInitializer.Start;
        
    }
    
}
