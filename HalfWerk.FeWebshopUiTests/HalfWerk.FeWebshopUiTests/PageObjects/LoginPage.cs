using OpenQA.Selenium;

namespace Halfwerk.FeWebshopUiTests.PageObjects
{
    public class LoginPage : PageObject<LoginPage>
    {
        public LoginPage(IWebDriver browser, string baseUrl) : base(browser, baseUrl)
        {
            Path = "/login";
        }

        public LoginPage ClickInloggenButton()
        {
            Browser.FindElement(By.Id("login")).Click();
            return this;
        }

        public LoginPage SetEmail(string email)
        {
            return SetFieldById("email-input", email);
        }

        public LoginPage SetWachtwoord(string wachtwoord)
        {
            return SetFieldById("password-input", wachtwoord);
        }
    }
}
