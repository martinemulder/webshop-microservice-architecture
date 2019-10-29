using System.Collections.ObjectModel;
using OpenQA.Selenium;

namespace Halfwerk.FeWebshopUiTests.PageObjects
{
    public class ProductsPage : PageObject<ProductsPage>
    {
        public ProductsPage(IWebDriver browser, string baseUrl) : base(browser, baseUrl)
        {
            Path = "/shop";
        }

        public string GetPageTitle()
        {
            return Browser.FindElement(By.Id("site-title")).Text;
        }

        public ProductsPage ClickRegistrerenButton()
        {
            ClickButtonById("register-button");
            return this;
        }

        public ReadOnlyCollection<IWebElement> GetAllProductsOnPage()
        {
            ReadOnlyCollection<IWebElement> producten = Browser.FindElements(By.ClassName("product"));
            return producten;
        }

        public ProductsPage AddFirstProductToCart()
        {
            var product = Browser.FindElements(By.ClassName("product"));
            var title = product[0].FindElement(By.ClassName("title")).Text;
            product[0].FindElement(By.ClassName("add-to-cart")).Click();
            return this;
        }

        public string GetNameFromFirstProduct()
        {
            var product = Browser.FindElements(By.ClassName("product"));
            return product[0].FindElement(By.ClassName("title")).Text;
        }

        public double GetPrijsFromFirstProduct()
        {
            var product = Browser.FindElements(By.ClassName("product"));
            var prijsString = product[0].FindElement(By.ClassName("price")).Text;
            double.TryParse(prijsString, out double prijs);
            return prijs;
        }
    }
}
