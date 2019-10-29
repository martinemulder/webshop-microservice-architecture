using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace Halfwerk.FeWebshopUiTests.PageObjects
{
    public class SalesPage : PageObject<SalesPage>
    {
        public SalesPage(IWebDriver browser, string baseUrl) : base(browser, baseUrl)
        {
            Path = "/sales/orders";
        }

        public SalesPage GetOrdersToVerify()
        {
            return this;
        }

        public SalesPage ClickVerifyButtonOnFirstOrder()
        {
            return this;
        }
    }
}
