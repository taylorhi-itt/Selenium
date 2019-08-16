using log4net;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;

namespace Test.Framework.Selenium
{
    public class BciWebBrowser
    {
        #region Public Properties

        public BciDriverOptions DriverOptions { get; private set; }

        public IWebDriver Driver
        {
            get
            {
                if (driverCache == null)
                {
                    lock (locker)
                    {
                        if (driverCache == null)
                        {
                            driverCache = DriverOptions.CreateDriver.Invoke();
                        }
                    }
                }

                return driverCache;
            }
        }
        #endregion

        #region Private Fields
        private Object locker = "";
        private IWebDriver driverCache = null;

        /// <summary>
        /// Logger instance
        /// </summary>
        private readonly ILog Log = LogManager.GetLogger(typeof(BciWebBrowser));

        #endregion


        #region Public Methods

        public BciWebBrowser(BciDriverOptions driverOptions)
        {
            DriverOptions = driverOptions;
        }

        /// <summary>
        /// This method will navigate to given url on any browser.
        /// </summary>
        public void NavigateToUrl(string url)
        {
            Driver.Manage().Window.Maximize();
            Driver.Url = url;
        }

        /// <summary>
        /// This method will use to find any Element on any web page.
        /// </summary>
        public IWebElement FindElement(By locator)
        {
            try
            {
                Thread.Sleep(1000);
                Log.DebugFormat("Finding {0} Element", locator);
                WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
                wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException), typeof(NoSuchElementException));
                var element = wait.Until(c => c.FindElement(locator));
                Log.DebugFormat("Found Element: {0}", element);
                return element;
            }
            catch (Exception e)
            {
                Log.Debug("Element not found." + e.Message + "\n" + e.StackTrace);
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// This method will use to find any Element on any web page.
        /// </summary>
        public ReadOnlyCollection<IWebElement> FindElements(By locator)
        {
            try
            {
                Log.DebugFormat("Finding {0} Element", locator);
                WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
                wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));
                var element = wait.Until(c => c.FindElements(locator));
                Log.DebugFormat("Found Element: {0}", element);
                return element;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// This method will use to enter text in any text field.
        /// </summary>
        public void EnterText(By locator, string text)
        {
            try
            {
                WaitForElementToBeClickable(locator);
                var element = FindElement(locator);
                element.Clear();
                Thread.Sleep(100);
                UIActions(DriverOptions.Before, BrowserActionType.KeyboardType, element);
                element.SendKeys(text);
                UIActions(DriverOptions.After, BrowserActionType.KeyboardType, element);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// This method will use click on any web Element.
        /// </summary>
        public void ClickOnElement<T>(T locator)
        {
            IWebElement Element = null;
            if (locator.GetType() == typeof(By))
            {
                By newLocator = (By)(object)locator;
                Element = FindElement(newLocator);
            }
            else if (locator.GetType() == typeof(OpenQA.Selenium.Remote.RemoteWebElement))
            {
                Element = (IWebElement)(object)locator;
            }

            try
            {
                UIActions(DriverOptions.Before, BrowserActionType.Click, Element);
                WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
                wait.Until(c => !StalenessOf(Element));
                Element.Click();
                UIActions(DriverOptions.After, BrowserActionType.Click, Element);
            }
            catch (Exception e)
            {
                Log.Debug("Exception occured:" + e.Message);
                Console.WriteLine(e);
            }

        }

        /// <summary>
        /// Double click on Element
        /// </summary>
        /// <param name="driver"></param>
        public void DoubleClickOnElement<T>(T locator)
        {
            IWebElement element = null;
            if (locator.GetType() == typeof(By))
            {
                By newLocator = (By)(object)locator;
                element = FindElement(newLocator);
            }
            else if (locator.GetType() == typeof(OpenQA.Selenium.Remote.RemoteWebElement))
            {
                element = (IWebElement)(object)locator;
            }

            try
            {
                WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
                wait.Until(c => !StalenessOf(element));
                UIActions(DriverOptions.Before, BrowserActionType.Click, element);
                new Actions(Driver).DoubleClick(element).Perform();
                UIActions(DriverOptions.After, BrowserActionType.Click, element);
            }
            catch (Exception e)
            {
                Log.Debug("Exception occured:" + e.Message);
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// This method will verify is web Element is displayed or not on page.
        /// </summary>
        public bool IsElementDisplayed(By locator)
        {
            try
            {
                var element = FindElement(locator);
                return element.Displayed;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }

        }

        /// <summary>
        /// This method will verify the web Element is selected or not
        /// </summary>
        public bool IsElementSelected(By locator)
        {
            try
            {
                var element = FindElement(locator);
                return element.Selected;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return false;
            }
        }

        /// <summary>
        /// This method will wait for complete the page load.
        /// </summary>
        public void WaitForPageLoad()
        {
            try
            {
                var a = new WebDriverWait(Driver, TimeSpan.FromMilliseconds(30000)).Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        /// <summary>
        /// This method will return text from any webElement
        /// </summary>
        public String GetElementText(By locator)
        {
            try
            {
                IsElementDisplayed(locator);
                var element = FindElement(locator);
                return element.Text;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }

        /// <summary>
        /// This method will switch any Iframe
        /// </summary>
        public void SwitchToIframe(By locator)
        {
            Log.Debug("Switching to Iframe...");
            var element = FindElement(locator);
            Driver.SwitchTo().Frame(element);
        }

        /// <summary>
        /// Switch back to Main content
        /// </summary>
        /// <param name="driver"></param>
        public void SwitchBackToMainContent()
        {
            Log.Debug("Switching to main content...");
            Driver.SwitchTo().DefaultContent();
        }

        /// <summary>
        /// This method will press enter key on page
        /// </summary>
        public void PressEnterKey(By locator)
        {
            var element = FindElement(locator);
            element.SendKeys(Keys.Enter);
            Log.Debug("Pressed Enter key...");
        }


        /// <summary>
        /// This method will navigate back from browser
        /// </summary>
        public void NavigateBackFromBrowser()
        {
            Driver.Navigate().Back();
        }

        /// <summary>
        /// This method will highlight the webElement
        /// </summary>
        public void HighlightElement(IWebElement webElement)
        {
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].style.border='3px solid red'", webElement);
        }

        /// <summary>
        /// An expectation for checking whether an Element is visible.
        /// </summary>
        /// <param name="locator">The locator used to find the Element.</param>
        /// <returns>The <see cref="IWebElement"/> once it is located, visible and clickable.</returns>
        public bool IsElementClickable(By locator)
        {
            var element = FindElement(locator);
            return (element != null && element.Displayed && element.Enabled) ? true : false;
        }

        /// <summary>
        /// Wait till the Element becomes clickable
        /// </summary>
        /// <param name="driver"></param>
        /// <param name="locator"></param>
        public void WaitForElementToBeClickable<T>(T locator)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

            if (locator.GetType() == typeof(By))
            {
                By newLocator = (By)(object)locator;
                wait.Until(c => c.FindElement(newLocator).Enabled && c.FindElement(newLocator).Displayed);
            }
            else if (locator.GetType() == typeof(IWebElement))
            {
                IWebElement newLocator = (IWebElement)(object)locator;
                //wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.ElementToBeClickable(newLocator));
            }
        }

        public void WaitForJavaScriptExecution()
        {
            try
            {
                var a = new WebDriverWait(Driver, TimeSpan.FromMilliseconds(30)).Until(d => ((IJavaScriptExecutor)d).ExecuteScript("return document.readyState").Equals("complete"));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public void WaitForElementDisappear(By locator)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
            Type[] type = { typeof(StaleElementReferenceException), typeof(NoSuchElementException) };
            wait.IgnoreExceptionTypes(type);
            wait.Until(c => !c.FindElement(locator).Displayed);
        }

        public void WaitForElementToAppear(By locator)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
            Type[] type = { typeof(StaleElementReferenceException), typeof(NoSuchElementException) };
            wait.IgnoreExceptionTypes(type);
            wait.Until(c => c.FindElement(locator).Displayed);
        }

        public void ScrollToElement(By locator)
        {
            var Element = FindElement(locator);
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", Element);
        }

        public void ScrollToElement(IWebElement webElement)
        {
            Log.Debug("Scrolling to Element..");
            ((IJavaScriptExecutor)Driver).ExecuteScript("arguments[0].scrollIntoView(true);", webElement);
            Thread.Sleep(500);
        }

        internal void HandleBrowserPopUp(string actionToTake)
        {
            try
            {
                IAlert alert = Driver.SwitchTo().Alert();
                if (actionToTake.ToUpper().Equals("Accept") || actionToTake.ToUpper().Equals("OK"))
                {
                    alert.Accept();
                }
                else if (actionToTake.ToUpper().Equals("Close") || actionToTake.ToUpper().Equals("Cancel"))
                {
                    alert.Dismiss();
                }
            }
            catch (NoAlertPresentException e)
            {
                Log.Debug("No alert box was present" + e.Message);
                Console.Write("No alert box was present");
            }
        }

        public void WaitForTextToBeVisible<T>(T locator)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

            if (locator.GetType() == typeof(By))
            {
                By newLocator = (By)(object)locator;
                wait.Until(c => c.FindElement(newLocator).Text != string.Empty);
                //Log.Debug("Text is present " + driver.FindElement(newLocator).Text);
            }
            else if (locator.GetType() == typeof(OpenQA.Selenium.Remote.RemoteWebElement))
            {
                IWebElement newLocator = (IWebElement)(object)locator;
                wait.Until(c => newLocator.Text != string.Empty);
                Log.Debug("Text is present " + newLocator.Text);
            }
        }

        public void WaitForTextToBePresent<T>(T locator, string value)
        {
            WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30));
            wait.IgnoreExceptionTypes(typeof(StaleElementReferenceException));

            if (locator.GetType() == typeof(By))
            {
                By newLocator = (By)(object)locator;
                // wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElement(driver.FindElement(newLocator), value));
                Log.Debug("Text is present " + Driver.FindElement(newLocator).Text);
            }
            else if (locator.GetType() == typeof(OpenQA.Selenium.Remote.RemoteWebElement))
            {
                IWebElement newLocator = (IWebElement)(object)locator;
                //  wait.Until(SeleniumExtras.WaitHelpers.ExpectedConditions.TextToBePresentInElement(newLocator, value));
                Log.Debug("Text is present " + newLocator.Text);
            }
        }

        public void WaitForPageUrl(string expectedUrl)
        {
            try
            {
                WebDriverWait wait = new WebDriverWait(Driver, TimeSpan.FromSeconds(30)); 
                Type[] type = { typeof(StaleElementReferenceException), typeof(NoSuchElementException) };
                wait.IgnoreExceptionTypes(type);
                wait.Until(c => c.Url == expectedUrl);
            }
            catch (Exception e)
            {
                Log.Debug("Url not loaded." + e.Message + "\n" + e.StackTrace);
                Console.WriteLine(e);
            }
        }

        public string GetPageUrl()
        {
            return Driver.Url;
        }

        public bool StalenessOf(IWebElement webElement)
        {
            try
            {
                // Calling any method forces a staleness check
                return webElement == null || !webElement.Enabled;
            }
            catch (StaleElementReferenceException)
            {
                return true;
            }
        }

        public string GetElementTextByValue(By transactionCodeInputElement)
        {
            var element = FindElement(transactionCodeInputElement);
            return element.GetAttribute("value");
        }

        public void CloseWebBrowser()
        {
            Driver.Quit();
        }

        public void UIActions(List<Action<BrowserActionType, BciWebBrowser, IWebDriver, IWebElement>> actions, BrowserActionType actionType, IWebElement element = null)
        {
            foreach (Action<BrowserActionType, BciWebBrowser, IWebDriver, IWebElement> action in actions)
            {
                action.Invoke(actionType, this, Driver, element);
            }
        }



        #endregion
    }
}