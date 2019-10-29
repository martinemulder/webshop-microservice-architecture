using System;
using System.Collections.Generic;
using System.Text;
using OpenQA.Selenium;

namespace Halfwerk.FeWebshopUiTests.PageObjects
{
    public class WarehousePage : PageObject<WarehousePage>
    {
        public WarehousePage(IWebDriver browser, string baseUrl) : base(browser, baseUrl)
        {
            Path = "/warehouse";
        }

        public WarehousePage GetPackableItems()
        {
            return this;
        }

        public WarehousePage ClickPrintFactuurButton()
        {
            return this;
        }

        public WarehousePage ClickPrintAdreslabelButton()
        {
            return this;
        }

        public WarehousePage ClickBestellingAfrondenButton()
        {
            return this;
        }

        public bool CheckAfrondenButtonIsEnabled()
        {
            return false;
        }
    }
}
