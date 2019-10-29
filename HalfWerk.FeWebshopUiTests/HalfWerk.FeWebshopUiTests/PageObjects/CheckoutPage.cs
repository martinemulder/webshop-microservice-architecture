using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace Halfwerk.FeWebshopUiTests.PageObjects
{
    public class CheckoutPage : PageObject<CheckoutPage>
    {
        public CheckoutPage(IWebDriver browser, string baseUrl) : base(browser, baseUrl)
        {
            Path = "/shop/cart/checkout";
        }

        public CheckoutPage ClickVolgendeButton()
        {
            ClickButtonById("next-checkout-button");
            return this;
        }

        public CheckoutPage ClickBestellingBevestigenButton()
        {
            ClickButtonById("place-order-button");
            return this;
        }

        public string GetBestellingBevestigdMessage()
        {
            var text = Browser.FindElement(By.Id("order-success")).Text;
            return text;
        }

        public CheckoutPage SetVoornaam(string voornaam)
        {
            return SetFieldById("voornaam-input", voornaam);
        }

        public CheckoutPage SetAchternaam(string achternaam)
        {
            return SetFieldById("achternaam-input", achternaam);
        }

        public CheckoutPage SetTelefoonnummer(string telefoonnummer)
        {
            return SetFieldById("telefoonnummer-input", telefoonnummer);
        }

        public CheckoutPage SetEmail(string email)
        {
            return SetFieldById("email-input", email);
        }

        public CheckoutPage SetStraatnaam(string straatnaam)
        {
            return SetFieldById("straat-input", straatnaam);
        }

        public CheckoutPage SetHuisnummer(string huisnummer)
        {
            return SetFieldById("huisnummer-input", huisnummer);
        }

        public CheckoutPage SetPostcode(string postcode)
        {
            return SetFieldById("postcode-input", postcode);
        }

        public CheckoutPage SetPlaats(string plaats)
        {
            return SetFieldById("plaats-input", plaats);
        }

        public CheckoutPage SetLand(string land)
        {
            return SetFieldById("land-input", land);
        }
    }
}
