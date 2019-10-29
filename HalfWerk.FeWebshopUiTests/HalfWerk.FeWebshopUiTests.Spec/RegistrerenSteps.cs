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
    public class RegistrerenSteps
    {
        private IWebDriver Browser { get; set; }
        private ProductsPage _productsPage;
        private RegisterPage _registerPage;

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

        [Given(@"ik ben op de productpagina")]
        public void GivenIkBenOpDeProductPagina()
        {
            _productsPage = Browser.GotoProductsPage();
        }

        [Given(@"ik klik op de Registreren knop")]
        public void GivenIkKlikOpDeRegistrerenKnop()
        {
            _productsPage.ClickRegistrerenButton();
        }

        [Given(@"dan ik ben op de registratie pagina")]
        public void GivenDanBenIkBenOpDeRegistratiePagina()
        {
            _registerPage = Browser.GotoRegisterPage();
            Assert.AreEqual(_registerPage.GetPageUrl(), Browser.Url);
        }

        [When(@"ik mijn gegevens invul ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)"", ""(.*)""")]
        public void WhenIkMijnGegevensInvul(string voornaam, string achternaam, string staatnaam, int huisnummer, string postcode, string plaats, string land, string telefoonnummer, string email, string wachtwoord)
        {
            _registerPage.SetVoornaam(voornaam);
            _registerPage.SetAchternaam(achternaam);
            _registerPage.SetStraatnaam(staatnaam);
            _registerPage.SetHuisnummer(huisnummer.ToString());
            _registerPage.SetPostcode(postcode);
            _registerPage.SetPlaats(plaats);
            _registerPage.SetLand(land);
            _registerPage.SetTelefoonnummer(telefoonnummer);
            _registerPage.SetEmail(email);
            _registerPage.SetWachtwoord(wachtwoord);
            _registerPage.SetBevestigWachtwoord(wachtwoord);
            _registerPage.ClickRegistrerenButton();
        }

        [Then(@"ben wordt ik succesvol geregistreerd")]
        public void ThenBenWordtIkSuccesvolGeregistreerd()
        {
            var message = _registerPage.GetToastMessage();
            Assert.AreEqual("Registratie succesvol! U wordt nu ingelogd.", message);
        }

        [Then(@"wordt ik doorgestuurd naar de webshop")]
        public void ThenWordtIkDoorgestuurdNaarDeWebshop()
        {
            _productsPage = Browser.GotoProductsPage();
            Assert.AreEqual(_productsPage.GetPageUrl(), Browser.Url);
        }
    }
}
