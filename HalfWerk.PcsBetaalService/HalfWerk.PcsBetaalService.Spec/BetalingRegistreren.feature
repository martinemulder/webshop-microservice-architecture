Feature: BetalingRegistreren
	Omdat ik een bestelling wil kunnen goedkeuren
	Als een Salesmedewerker
	Wil ik betalingen kunnen invoeren


Scenario: Betaling toevoegen en klanttotaal is onder de 500 euro
	Given Er is een bestelling:
	| Factuurnummer | Klantnummer | FactuurTotaalInclBtw |
	| 1             | 1           | 1000,00              |
	And ik heb een betaling ingevoerd
	| Factuurnummer | Bedrag |
	| 1             | 500,00 | 
	When ik op betaling invoeren druk
	Then de bestelling wordt goedgekeurd

Scenario: Betaling toevoegen en klanttotaal is boven de 500 euro
	Given Er is een bestelling:
	| Factuurnummer | Klantnummer | FactuurTotaalInclBtw |
	| 1             | 1           | 1100,00              |
	And ik heb een betaling ingevoerd
	| Factuurnummer | Bedrag |
	| 1             | 500,00 | 
	When ik op betaling invoeren druk
	Then de bestelstatus is ongewijzigd door het openstaande restbedrag

Scenario: Betaling toevoegen en het klanttotaal is lager dan 500 euro
	Given Er is een bestelling:
	| Factuurnummer | Klantnummer | FactuurTotaalInclBtw |
	| 1             | 1           | 600,00               |
	| 2             | 1           | 200,00               |
	And ik heb een betaling ingevoerd
	| Factuurnummer | Bedrag |
	| 1             | 600,00 | 
	When ik op betaling invoeren druk
	Then de bestellingen moeten beide goedgekeurd zijn

Scenario: Bestellingen toevoegen en klanttotaal blijft onder de 500 euro
	Given Er is een bestelling:
	| Factuurnummer | Klantnummer | FactuurTotaalInclBtw |
	| 1             | 1           | 500,00               |
	| 2             | 1           | 200,00               |
	And ik heb een betaling ingevoerd
	| Factuurnummer | Bedrag |
	| 1             | 100,00 | 
	When ik op betaling invoeren druk
	Then de tweede bestelling is niet goedgekeurd door het openstaande restbedrag