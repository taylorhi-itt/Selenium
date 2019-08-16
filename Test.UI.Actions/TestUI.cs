using System;
using System.Collections.Generic;
using System.Text;
using Test.Framework.Selenium;
using Test.UI.Actions.PageActions;

namespace Test.UI.Actions
{
    public class TestUI
    {
        private BciWebBrowser webBrowser;
        public TestUI(BciWebBrowser webBrowser)
        {
            this.webBrowser = webBrowser;
        }

        public HomePage HomePage { get { return new HomePage(webBrowser);  } }

        public void CloseWebBrowser()
        {
            webBrowser.CloseWebBrowser();
        }
    }
}
