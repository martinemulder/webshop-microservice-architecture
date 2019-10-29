using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using SeleniumExtras.WaitHelpers;

namespace Halfwerk.FeWebshopUiTests.PageObjects
{
    public abstract class PageObject<T>
        where T : PageObject<T>
    {
        protected readonly IWebDriver Browser;
        protected readonly string BaseUrl;

        public string Path { get; protected set; }

        public PageObject(IWebDriver browser, string baseUrl)
        {
            Browser = browser;
            BaseUrl = baseUrl;
        }

        public T NavigateHere()
        {
            Browser.Navigate().GoToUrl(BaseUrl + Path);
            return (T)this;
        }

        public string GetPageUrl()
        {
            return BaseUrl + Path;
        }

        public T SetFieldById(string elementId, string value)
        {
            var element = Browser.FindElement(By.Id(elementId));
            element.Clear();
            element.SendKeys(value);
            return (T)this;
        }

        public T ClickButtonById(string id)
        {
            Browser.FindElement(By.Id(id)).Click();
            return (T)this;
        }

        public string GetToastMessage()
        {
            WebDriverWait wait = new WebDriverWait(Browser, System.TimeSpan.FromSeconds(10));
            wait.Until(ExpectedConditions.ElementExists(By.ClassName("toast-message")));
            var toaster = Browser.FindElement(By.ClassName("toast-message"));
            return toaster.Text;
        }
    }
}
