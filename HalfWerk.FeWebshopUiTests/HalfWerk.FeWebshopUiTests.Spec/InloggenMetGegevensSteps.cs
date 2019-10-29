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
    public class InloggenMetGegevensSteps
    {
        private IWebDriver Browser { get; set; }
        private ProductsPage _productsPage;
        private SalesPage _salesPage;
        private WarehousePage _warehousePage;
        private LoginPage _loginPage;

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

        [Given(@"ik ben op de inlogpagina")]
        public void GivenIkBenOpDeInlogpagina()
        {
            _loginPage = Browser.GotoLoginPage();
        }

        [When(@"ik mijn emailadres ""(.*)"" invoer")]
        public void WhenIkMijnGebruikersnaamInvoer(string emailadres)
        {
            _loginPage.SetEmail(emailadres);
        }

        [When(@"mijn wachtwoord ""(.*)"" invoer")]
        public void WhenMijnWachtwoordInvoer(string wachtwoord)
        {
            _loginPage.SetWachtwoord(wachtwoord);
        }

        [When(@"op de inlogknop klik")]
        public void WhenOpDeInlogknopKlik()
        {
            _loginPage.ClickInloggenButton();
        }

        [Then(@"wordt ik succesvol ingelogd")]
        public void ThenWordtIkSuccesvolIngelogdAlsKlant()
        {
            var message = _loginPage.GetToastMessage();
            Assert.AreEqual("U bent succesvol ingelogd!", message);
        }

        [Then(@"doorverwezen naar de webshop")]
        public void ThenDoorverwezenNaarDeWebshop()
        {
            _productsPage = Browser.GotoProductsPage();
            Assert.AreEqual(_productsPage.GetPageUrl(), Browser.Url);
        }

        [Then(@"doorverwezen naar de salespagina")]
        public void ThenDoorverwezenNaarDeSalespagina()
        {
            _salesPage = Browser.GotoSalesPage();
            Assert.AreEqual(_salesPage.GetPageUrl(), Browser.Url);
        }

        [Then(@"doorverwezen naar de magazijnpagina")]
        public void ThenDoorverwezenNaarDeMagazijnpagina()
        {
            _warehousePage = Browser.GotoWarehousePage();
            Assert.AreEqual(_warehousePage.GetPageUrl(), Browser.Url);
        }

        [When(@"ik naar de salespagina navigeer")]
        public void WhenIkNaarDeSalespaginaNavigeer()
        {
            _salesPage = Browser.GotoSalesPage();
        }

        [Then(@"wordt ik niet ingelogd")]
        public void ThenWordtIkNietIngelogd()
        {
            var message = _loginPage.GetToastMessage();
            Assert.AreEqual("Geen toegang, probeer opnieuw in te loggen.", message);
        }

        [Then(@"blijf ik op de inlogpagina")]
        public void ThenBlijfIkOpDeInlogpagina()
        {
            Assert.AreEqual(_loginPage.GetPageUrl(), Browser.Url);
        }

        [Then(@"wordt ik teruggestuurd naar de inlogpagina")]
        public void ThenWordtIkTeruggestuurdNaarDeInlogpagina()
        {
            string redirectParameter = "?returnUrl=%2Fsales%2Forders";
            Assert.AreEqual(_loginPage.GetPageUrl() + redirectParameter, Browser.Url);
        }
    }
}
