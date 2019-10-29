using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace Halfwerk.FeWebshopUiTests.PageObjects
{
    public class RegisterPage : PageObject<RegisterPage>
    {
        public RegisterPage(IWebDriver browser, string baseUrl) : base(browser, baseUrl)
        {
            Path = "/register";
        }

        public RegisterPage ClickRegistrerenButton()
        {
            Browser.FindElement(By.Id("register")).Click();
            return this;
        }

        public RegisterPage SetVoornaam(string voornaam)
        {
            return SetFieldById("voornaam", voornaam);
        }

        public RegisterPage SetAchternaam(string achternaam)
        {
            return SetFieldById("achternaam", achternaam);
        }

        public RegisterPage SetStraatnaam(string straatnaam)
        {
            return SetFieldById("straatnaam", straatnaam);
        }

        public RegisterPage SetHuisnummer(string huisnummer)
        {
            return SetFieldById("huisnummer", huisnummer);
        }

        public RegisterPage SetPostcode(string postcode)
        {
            return SetFieldById("postcode", postcode);
        }

        public RegisterPage SetPlaats(string plaats)
        {
            return SetFieldById("plaats", plaats);
        }

        public RegisterPage SetLand(string land)
        {
            return SetFieldById("land", land);
        }

        public RegisterPage SetTelefoonnummer(string telefoonnummer)
        {
            return SetFieldById("telefoonnummer", telefoonnummer);
        }

        public RegisterPage SetEmail(string email)
        {
            return SetFieldById("email-input", email);
        }

        public RegisterPage SetWachtwoord(string wachtwoord)
        {
            return SetFieldById("password-input", wachtwoord);
        }

        public RegisterPage SetBevestigWachtwoord(string bevestigWachtwoord)
        {
            return SetFieldById("confirm-password-input", bevestigWachtwoord);
        }
    }
}
