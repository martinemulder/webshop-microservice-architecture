Feature: BestellingPlaatsen
	Omdat ik een bestelling wil afronden
	Als een klant
	Wil ik een bestelling bevestigen
	
@mytag
Scenario: Bestelling Plaatsen
	Given ik ben ingelogd met gebruikersnaam "klant" en wachtwoord "klant"
	And ik ben op de winkelmand pagina met items in mijn winkelmandje
	And ik klik op Bestelling afronden
	Then vul ik mijn klantgegevens in: "Kees", "de Koning", "kees@dekoning.com", "06 12 345 678"
	And ik klik op volgende
	And ik vul mijn adresgegevens: "St. Jacobsstraat", "12", "3511 BS", "Utrecht", "Nederland" 
	And ik klik op Plaats Bestelling
	Then is mijn bestelling succesvol geplaatst
