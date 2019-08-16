using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Text;

namespace Test.Framework.Selenium
{
    public static class TakeScreenshot
    {

        public static void Action(BrowserActionType actionType, BciWebBrowser webBrowser, IWebDriver driver, IWebElement element)
        {
            Screenshot ss = ((ITakesScreenshot)driver).GetScreenshot();
            string dateString = DateTime.Now.ToString("MM-dd-yyyy-HH.mm.ss");
            
            var path = "C://Selenium";
            ss.SaveAsFile($"{path}//{dateString}-{actionType}-{element.Text}.png", ScreenshotImageFormat.Png);
        }
    }
}
