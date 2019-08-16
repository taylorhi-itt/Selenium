using System;
using System.Collections.Generic;
using System.Text;
using Test.Framework.Selenium;
using Test.UI.Actions.PageObjects;

namespace Test.UI.Actions.PageActions
{
    public class HomePage
    {
        #region Private Fields
        private HomePageObject pageObject = new HomePageObject();
        private BciWebBrowser webBrowser;
        #endregion Private Fields

        #region Public Methods
        public HomePage(BciWebBrowser selenium)
        {
            this.webBrowser = selenium;
           // this.js = (IJavaScriptExecutor)selenium.Driver;
        }

        public void GoTo()
        {
            webBrowser.NavigateToUrl("http://newtours.demoaut.com/");
          
        }
        public void TypeInSearch(string searchString)
        {
            webBrowser.EnterText(pageObject.SearchField, searchString);
        }
        #endregion Public Methods
    }
}
