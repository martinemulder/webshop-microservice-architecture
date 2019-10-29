using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Halfwerk.FeWebshopUiTests.PageObjects
{
    public class CartPage : PageObject<CartPage>
    {
        public CartPage(IWebDriver browser, string baseUrl) : base(browser, baseUrl)
        {
            Path = "/shop/cart";
        }

        public int GetAantalFromFirstOrderItem()
        {
            var aantalString = Browser.FindElement(By.ClassName("amount-value")).Text;
            int.TryParse(aantalString, out int aantal);
            return aantal;
        }

        public string GetNaamFromFirstOrderItem()
        {
            var cartRow = Browser.FindElement(By.ClassName("mat-row"));
            return cartRow.FindElement(By.ClassName("cdk-column-naam")).Text;
        }

        public double GetPrijsFromFirstOrderItem()
        {
            var cartRow = Browser.FindElement(By.ClassName("mat-row"));
            string prijsString = cartRow.FindElement(By.ClassName("cdk-column-prijs")).Text;
            double.TryParse(prijsString, out double prijs);
            return prijs;
        }

        public int GetAmountOfProductsInCart()
        {
            var rows = Browser.FindElements(By.ClassName("mat-row"));
            return rows.Count;
        }

        public CartPage Add1AmountFromFirstCartItem()
        {
            WebDriverWait wait = new WebDriverWait(Browser, System.TimeSpan.FromSeconds(5));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("btn-increase-quantity")));
            Browser.FindElement(By.ClassName("btn-increase-quantity")).Click();
            return this;
        }

        public CartPage Remove1AmountFromFirstCartItem()
        {
            WebDriverWait wait = new WebDriverWait(Browser, System.TimeSpan.FromSeconds(5));
            wait.Until(ExpectedConditions.ElementToBeClickable(By.ClassName("btn-decrease-quantity")));
            Browser.FindElement(By.ClassName("btn-decrease-quantity")).Click();
            return this;
        }

        public CartPage ClickBestellingAfrondenButton()
        {
            return this;
        }
    }
}
