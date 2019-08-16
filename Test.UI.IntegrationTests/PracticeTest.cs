using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Test.UI.IntegrationTests
{
    [TestFixture]
    public class PracticeTest : SeleniumTestBase
    {
        [Test]
        public void FirstTest()
        {
            Ui.HomePage.GoTo();
            Ui.HomePage.TypeInSearch("test");
        }
    }
}
