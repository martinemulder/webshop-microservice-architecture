using Halfwerk.FeWebshopUiTests.PageObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.IO;
using System.Reflection;
using TechTalk.SpecFlow;

namespace HalfWerk.FeWebshopUiTests.Spec
{
    [Binding]
    public class BestellingPlaatsenSteps
    {
        private IWebDriver Browser { get; set; }
        private ProductsPage _productsPage;
        private CartPage _cartPage;
        private LoginPage _loginPage;
        private CheckoutPage _checkoutPage;

        [BeforeScenario]
        public void BeforeScenario()
        {
            SpecFlowTestLock.Lock();
            var chromeOptions = new ChromeOptions();
            //chromeOptions.AddArgument("--headless");
            Browser = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), chromeOptions);

            Browser.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(5);
        }

        [AfterScenario]
        public void AfterScenario()
        {
            Browser.Close();
            Browser.Dispose();
            SpecFlowTestLock.Unlock();
        }

        [Given(@"ik ben ingelogd met gebruikersnaam ""(.*)"" en wachtwoord ""(.*)""")]
        public void GivenIkBenIngelogdMetGebruikersnaamEnWachtwoord(string gebruikersnaam, string wachtwoord)
        {
            _loginPage = Browser.GotoLoginPage();
            _loginPage.SetEmail(gebruikersnaam);
            _loginPage.SetWachtwoord(wachtwoord);
            _loginPage.ClickInloggenButton();
            var message = _loginPage.GetToastMessage();
            Assert.AreEqual("U bent succesvol ingelogd!", message);
        }

        [Given(@"ik ben op de winkelmand pagina met items in mijn winkelmandje")]
        public void GivenIkBenOpDeWinkelmandPaginaMetItemsInMijnWinkelmandje()
        {
            _productsPage = Browser.GotoProductsPage();
            _productsPage.AddFirstProductToCart();
            _cartPage = Browser.GotoCart();
            Assert.IsTrue(_cartPage.GetAmountOfProductsInCart() > 0);
        }

        [Given(@"ik klik op Bestelling afronden")]
        public void GivenIkKlikOpBestellingAfronden()
        {
            _cartPage.ClickButtonById("checkout-button");
            _checkoutPage = Browser.GotoCheckoutPage();
        }

        [Then(@"vul ik mijn klantgegevens in: ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)""")]
        public void ThenVulIkMijnKlantgegevensIn(string voornaam, string achternaam, string email, string telefoonnummer)
        {
            _checkoutPage.SetVoornaam(voornaam);
            _checkoutPage.SetAchternaam(achternaam);
            _checkoutPage.SetEmail(email);
            _checkoutPage.SetTelefoonnummer(telefoonnummer);
        }

        [Then(@"ik klik op volgende")]
        public void ThenIkKlikOpVolgende()
        {
            _checkoutPage.ClickVolgendeButton();
        }

        [Then(@"ik vul mijn adresgegevens: ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)""")]
        public void ThenIkVulMijnAdresgegevens(string straatnaam, int huisnummer, string postcode, string plaats, string land)
        {
            _checkoutPage.SetStraatnaam(straatnaam);
            _checkoutPage.SetHuisnummer(huisnummer.ToString());
            _checkoutPage.SetPostcode(postcode);
            _checkoutPage.SetPlaats(plaats);
            _checkoutPage.SetLand(land);
        }

        [Then(@"ik klik op Plaats Bestelling")]
        public void ThenIkBevestigMijnGegevens()
        {
            _checkoutPage.ClickBestellingBevestigenButton();
        }

        [Then(@"is mijn bestelling succesvol geplaatst")]
        public void ThenIsMijnBestellingSuccesvolGeplaatst()
        {
            var message = _checkoutPage.GetBestellingBevestigdMessage();
            var splitMessage = message.Split(' ');
            var finalMessage = splitMessage[0] + " " + splitMessage[2] + " " + splitMessage[3];
            Assert.AreEqual(finalMessage, "Bestelling is geplaatst");
        }
    }
}
