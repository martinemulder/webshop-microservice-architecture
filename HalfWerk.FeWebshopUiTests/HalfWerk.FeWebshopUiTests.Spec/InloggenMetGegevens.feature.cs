﻿// ------------------------------------------------------------------------------
//  <auto-generated>
//      This code was generated by SpecFlow (http://www.specflow.org/).
//      SpecFlow Version:1.9.0.77
//      SpecFlow Generator Version:1.9.0.0
//      Runtime Version:4.0.30319.42000
// 
//      Changes to this file may cause incorrect behavior and will be lost if
//      the code is regenerated.
//  </auto-generated>
// ------------------------------------------------------------------------------
#region Designer generated code
#pragma warning disable
namespace HalfWerk.FeWebshopUiTests.Spec
{
    using TechTalk.SpecFlow;
    
    
    [System.CodeDom.Compiler.GeneratedCodeAttribute("TechTalk.SpecFlow", "1.9.0.77")]
    [System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    [Microsoft.VisualStudio.TestTools.UnitTesting.TestClassAttribute()]
    public partial class InloggenMetGegevensFeature
    {
        
        private static TechTalk.SpecFlow.ITestRunner testRunner;
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassInitializeAttribute()]
        public static void FeatureSetup(Microsoft.VisualStudio.TestTools.UnitTesting.TestContext testContext)
        {
            testRunner = TechTalk.SpecFlow.TestRunnerManager.GetTestRunner();
            TechTalk.SpecFlow.FeatureInfo featureInfo = new TechTalk.SpecFlow.FeatureInfo(new System.Globalization.CultureInfo("en-US"), "Inloggen met gegevens", "Omdat ik producten wil bestellen\r\nAls klant\r\nWil ik in kunnen loggen", ProgrammingLanguage.CSharp, ((string[])(null)));
            testRunner.OnFeatureStart(featureInfo);
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.ClassCleanupAttribute()]
        public static void FeatureTearDown()
        {
            testRunner.OnFeatureEnd();
            testRunner = null;
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestInitializeAttribute()]
        public virtual void TestInitialize()
        {
            if (((TechTalk.SpecFlow.FeatureContext.Current != null) 
                        && (TechTalk.SpecFlow.FeatureContext.Current.FeatureInfo.Title != "Inloggen met gegevens")))
            {
                HalfWerk.FeWebshopUiTests.Spec.InloggenMetGegevensFeature.FeatureSetup(null);
            }
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCleanupAttribute()]
        public virtual void ScenarioTearDown()
        {
            testRunner.OnScenarioEnd();
        }
        
        public virtual void ScenarioSetup(TechTalk.SpecFlow.ScenarioInfo scenarioInfo)
        {
            testRunner.OnScenarioStart(scenarioInfo);
        }
        
        public virtual void ScenarioCleanup()
        {
            testRunner.CollectScenarioErrors();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Inloggen als klant")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Inloggen met gegevens")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestCategoryAttribute("mytag")]
        public virtual void InloggenAlsKlant()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Inloggen als klant", new string[] {
                        "mytag"});
            this.ScenarioSetup(scenarioInfo);
            testRunner.Given("ik ben op de inlogpagina", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
            testRunner.When("ik mijn emailadres \"klant\" invoer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
            testRunner.And("mijn wachtwoord \"klant\" invoer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
            testRunner.And("op de inlogknop klik", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
            testRunner.Then("wordt ik succesvol ingelogd", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
            testRunner.And("doorverwezen naar de webshop", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Inloggen als salasmedewerker")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Inloggen met gegevens")]
        public virtual void InloggenAlsSalasmedewerker()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Inloggen als salasmedewerker", ((string[])(null)));
            this.ScenarioSetup(scenarioInfo);
            testRunner.Given("ik ben op de inlogpagina", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
            testRunner.When("ik mijn emailadres \"sales\" invoer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
            testRunner.And("mijn wachtwoord \"sales\" invoer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
            testRunner.And("op de inlogknop klik", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
            testRunner.Then("wordt ik succesvol ingelogd", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
            testRunner.And("doorverwezen naar de salespagina", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Inloggen als magazijnmedewerker")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Inloggen met gegevens")]
        public virtual void InloggenAlsMagazijnmedewerker()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Inloggen als magazijnmedewerker", ((string[])(null)));
            this.ScenarioSetup(scenarioInfo);
            testRunner.Given("ik ben op de inlogpagina", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
            testRunner.When("ik mijn emailadres \"magazijn1\" invoer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
            testRunner.And("mijn wachtwoord \"magazijn1\" invoer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
            testRunner.And("op de inlogknop klik", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
            testRunner.Then("wordt ik succesvol ingelogd", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
            testRunner.And("doorverwezen naar de magazijnpagina", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Inloggen als niet bestaand account")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Inloggen met gegevens")]
        public virtual void InloggenAlsNietBestaandAccount()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Inloggen als niet bestaand account", ((string[])(null)));
            this.ScenarioSetup(scenarioInfo);
            testRunner.Given("ik ben op de inlogpagina", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
            testRunner.When("ik mijn emailadres \"bestaatniet\" invoer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
            testRunner.And("mijn wachtwoord \"bestaatniet\" invoer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
            testRunner.And("op de inlogknop klik", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
            testRunner.Then("wordt ik niet ingelogd", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
            testRunner.And("blijf ik op de inlogpagina", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "And ");
            this.ScenarioCleanup();
        }
        
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestMethodAttribute()]
        [Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute("Als niet ingelogde gebruiker kan ik niet op de sales pagina komen")]
        [Microsoft.VisualStudio.TestTools.UnitTesting.TestPropertyAttribute("FeatureTitle", "Inloggen met gegevens")]
        public virtual void AlsNietIngelogdeGebruikerKanIkNietOpDeSalesPaginaKomen()
        {
            TechTalk.SpecFlow.ScenarioInfo scenarioInfo = new TechTalk.SpecFlow.ScenarioInfo("Als niet ingelogde gebruiker kan ik niet op de sales pagina komen", ((string[])(null)));
            this.ScenarioSetup(scenarioInfo);
            testRunner.Given("ik ben op de inlogpagina", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Given ");
            testRunner.When("ik naar de salespagina navigeer", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "When ");
            testRunner.Then("wordt ik teruggestuurd naar de inlogpagina", ((string)(null)), ((TechTalk.SpecFlow.Table)(null)), "Then ");
            this.ScenarioCleanup();
        }
    }
}
#pragma warning restore
#endregion
