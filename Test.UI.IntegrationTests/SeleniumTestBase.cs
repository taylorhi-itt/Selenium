using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Test.Framework;
using Test.Framework.Selenium;
using Test.UI.Actions;

namespace Test.UI.IntegrationTests
{
    public class SeleniumTestBase 
    {
        public TestUI Ui { get; set; }

        public BciDriverOptions BciDriverOptions { get; set; } = new BciDriverOptions()
        {
            Before = new List<Action<BrowserActionType, BciWebBrowser, OpenQA.Selenium.IWebDriver, OpenQA.Selenium.IWebElement>> { TakeScreenshot.Action
    },
            After = new List<Action<BrowserActionType, BciWebBrowser, OpenQA.Selenium.IWebDriver, OpenQA.Selenium.IWebElement>> { TakeScreenshot.Action
}
        };

        [SetUp]
        public void Setup()
        {
            Ui = new TestUI(new BciWebBrowser(BciDriverOptions));

        }

        //[TearDown]
        //public void Close()
        //{
        //    Ui.CloseWebBrowser();
        //}
    
    }
}
