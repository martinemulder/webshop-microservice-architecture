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
    public class ProductToevoegenAanHetWinkelmandjeSteps
    {
        private IWebDriver Browser { get; set; }
        private ProductsPage _productsPage;
        private CartPage _cartPage;

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

        private double _productPrijs;
        private string _productNaam;

        [Given(@"ik ben op de productenpagina en wil een product aan mijn winkelwagen toevoegen")]
        public void GivenIkBenOpDeProductenpaginaEnWilEenProductAanMijnWinkelwagenToevoegen()
        {
            _productsPage = Browser.GotoProductsPage();
        }

        [Given(@"ik ben op de winkelwagenpagina en heb een product erin zitten met aantal (.*)")]
        public void GivenIkBenOpDeWinkelwagenpaginaEnHebEenProductErinZittenMetAantal(int givenAantal)
        {
            _productsPage = Browser.GotoProductsPage();
            _productsPage.AddFirstProductToCart();
            _cartPage = Browser.GotoCart();
            var aantal = _cartPage.GetAmountOfProductsInCart();
            Assert.AreEqual(givenAantal, aantal);
        }

        [When(@"ik mijn product heb gevonden en klik op de Toevoegen knop")]
        public void WhenIkMijnProductHebGevondenEnKlikOpDeToevoegenKnop()
        {
            _productsPage.AddFirstProductToCart();
            _productNaam = _productsPage.GetNameFromFirstProduct();
            _productPrijs = _productsPage.GetPrijsFromFirstProduct();
        }

        [When(@"navigeer naar de winkelwagenpagina")]
        public void WhenNavigeerNaarDeWinkelwagenpagina()
        {
            _cartPage = Browser.GotoCart();
        }

        [When(@"ik klik op de plus bij een product")]
        public void WhenIkKlikOpDePlusBijEenProduct()
        {
            _cartPage.Add1AmountFromFirstCartItem();
        }

        [When(@"ik klik op de min bij een product")]
        public void WhenIkKlikOpDeMinBijEenProduct()
        {
            _cartPage.Remove1AmountFromFirstCartItem();
        }

        [Then(@"zie ik het net toegevoegde product staan in het overzicht")]
        public void ThenZieIkHetNetToegevoegdeProductStaanInHetOverzicht()
        {
            var naam = _cartPage.GetNaamFromFirstOrderItem();
            Assert.AreEqual(_productNaam, naam);
        }

        [Then(@"zie ik dat het aantal bij het product (.*) is")]
        public void ThenZieIkDatHetAantalBijHetProductIs(int givenAantalProducten)
        {
            var aantal = _cartPage.GetAantalFromFirstOrderItem();
            Assert.AreEqual(givenAantalProducten, aantal);
        }

        [Then(@"de totaalprijs met (.*) vermenigvuldigt is")]
        public void ThenDeTotaalprijsMetVermenigvuldigtIs(int vermenigvuldiging)
        {
            var prijs = _cartPage.GetPrijsFromFirstOrderItem();
            Assert.AreEqual(_productPrijs * vermenigvuldiging, prijs);
        }

        [Given(@"ik wil een bestelling gaan doen")]
        public void GivenIkWilEenBestellingGaanDoen()
        {
            _productsPage = Browser.GotoProductsPage();
        }

        [When(@"ik navigaar naar de productpagina")]
        public void WhenIkNavigaarNaarDeProductpagina()
        {
            _productsPage = Browser.GotoProductsPage();
        }

        [Then(@"zie ik als titel bij de pagina ""(.*)""")]
        public void ThenZieIkAlsTitelBijDePagina(string givenTitle)
        {
            var title = _productsPage.GetPageTitle();
            Assert.AreEqual(title, givenTitle);
        }

        [Then(@"zie ik standaard (.*) producten")]
        public void ThenZieIkStandaardProducten(int givenAantalProducten)
        {
            var aantal = _productsPage.GetAllProductsOnPage().Count;
            Assert.AreEqual(givenAantalProducten, aantal);
        }
    }
}
