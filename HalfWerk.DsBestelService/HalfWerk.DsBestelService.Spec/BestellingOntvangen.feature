Feature: Bestelling ontvangen
	Omdat ik producten wil verkopen aan consumenten
	Als Kantilever
	Wil ik bestellingen kunnen ontvangen en verwerken

@mytag
Scenario: Verwerk binnengekomen bestelling
	Given er is bestelling binnen gekomen:
	| Klantnummer | 
	| 1           | 
	When de bestelling bestelregels bevat:
	| Artikelnummer | LeverancierCode | Naam                      | Prijs  | Aantal |
	| 1             | BAT-001-H       | Batavus Heren Fiets Blauw | 799,00 | 1      |
	| 2             | POC-BL-009      | POC Blauwe Fietshelm      | 117,95 | 2      |
	Then verwerk een bestelling in het systeem met het klantnummer, besteldatum en bestelregels
	And genereer een factuurnummer
