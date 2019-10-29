Feature: Registreren
	Omdat ik producten wil bestellen
	Als klant
	Wil ik een account aan kunnen maken

@mytag
Scenario: Registreren
	Given ik ben op de productpagina
	And ik klik op de Registreren knop
	And dan ik ben op de registratie pagina
	When ik mijn gegevens invul "Klant", "de Koning", "St. Jacobsstraat", "12", "3511 BS", "Utrecht", "Nederland", "06 12 345 678", "kees@dekoning.com", "wachtwoord"
	Then ben wordt ik succesvol geregistreerd
	And wordt ik doorgestuurd naar de webshop
