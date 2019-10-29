using System;
using OpenQA.Selenium;

namespace Halfwerk.FeWebshopUiTests.PageObjects
{
    public static class WebDriverExtensions
    {
        private static readonly string Baseurl;

        static WebDriverExtensions()
        {
#if DEBUG
            Baseurl = Constants.LocalBaseUrl;
#else
            Baseurl = Constants.ReleaseBaseUrl;
#endif
        }

        public static ProductsPage GotoProductsPage(this IWebDriver browser)
        {
            var page = new ProductsPage(browser, Baseurl);
            return page.NavigateHere();
        }

        public static CartPage GotoCart(this IWebDriver browser)
        {
            var page = new CartPage(browser, Baseurl);
            return page.NavigateHere();
        }

        public static CheckoutPage GotoCheckoutPage(this IWebDriver browser)
        {
            var page = new CheckoutPage(browser, Baseurl);
            return page.NavigateHere();
        }

        public static LoginPage GotoLoginPage(this IWebDriver browser)
        {
            var page = new LoginPage(browser, Baseurl);
            return page.NavigateHere();
        }

        public static RegisterPage GotoRegisterPage(this IWebDriver browser)
        {
            var page = new RegisterPage(browser, Baseurl);
            return page.NavigateHere();
        }

        public static SalesPage GotoSalesPage(this IWebDriver browser)
        {
            var page = new SalesPage(browser, Baseurl);
            return page.NavigateHere();
        }

        public static WarehousePage GotoWarehousePage(this IWebDriver browser)
        {
            var page = new WarehousePage(browser, Baseurl);
            return page.NavigateHere();
        }
    }
}
