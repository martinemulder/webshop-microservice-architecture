Feature: SalesMedewerkerOverzicht
	Om bestellingen boven de 500 euro te kunnen goedkeuren
	Als salesmedewerkers
	Wil ik een overzicht van alle bestellingen boven de 500 euro zien

@mytag
Scenario: Haal aangemaakte bestellingen boven 500 euro op
	Given de volgende bestellingen staan in de database
	| Id | Klantnummer | FactuurTotaalInclBtw | BestelStatus             |
	| 1  | 1           | 290.00M              | BestelStatus.Verzonden   |
	| 2  | 2           | 140.95M              | BestelStatus.Goedgekeurd |
	| 3  | 3           | 590.00M              | BestelStatus.Geplaatst   |
	| 4  | 4           | 500.00M              | BestelStatus.Geplaatst   |
	| 5  | 5           | 500.01M              | BestelStatus.Geplaatst   |
	| 6  | 6           | 59.00M               | BestelStatus.Geplaatst   |
	| 7  | 7           | 1590.00M             | BestelStatus.Geplaatst   |
	When het sales medewerker overzicht is opgevraagd
	Then zie ik alleen bestellingen boven de 500 euro
	And alleen bestellingen met de status Geplaatst

Scenario: Klant doet twee bestellingen met een totaalbedrag boven de 500 euro
	Given de volgende bestellingen staan in de database
	| Id | Klantnummer | FactuurTotaalInclBtw | BestelStatus             |
	| 1  | 1           | 290.00M              | BestelStatus.Goedgekeurd |
	| 2  | 1           | 340.95M              | BestelStatus.Geplaatst   |
	| 3  | 2           | 265.95M              | BestelStatus.Betaald     |
	| 4  | 2           | 265.95M              | BestelStatus.Geplaatst   |
	When het sales medewerker overzicht is opgevraagd
	Then zie ik de bestelling met id 2 in het overzicht

Scenario: Sales medewerker vraagt om aanbetaling
	Given de volgende bestelling staat in het sales medewerker overzicht
	| Id | Klantnummer | FactuurTotaalInclBtw | BestelStatus             |
	| 1  | 1           | 590.00M              | BestelStatus.Geplaatst |
	When de sales medewerker vraagt om een aanbetaling
	Then verandert de status van bestelling met id 1 in BestalStatus.AfwachtenOpAanbetaling
