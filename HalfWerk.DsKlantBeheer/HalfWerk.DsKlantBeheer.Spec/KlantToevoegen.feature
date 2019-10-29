Feature: KlantToevoegen
	Omdat ik producten wil kunnen leveren
	Als Kantilever
	Wil ik het adres van mijn klanten kunnen registreren

@mytag
Scenario: Voeg nieuwe klant toe
	Given er is een klant binnen gekomen met de volgende gegevens:
	| Voornaam | Achternaam | Telefoonnummer  | Email                               |
	| Hans     | Worst      | 0625411258      | hans@worst.nl                       |
	| Hannah   | Nayan      | +316 41 598 774 | hannah.nayan-is_supercool@worst.com |
	And de klant heeft het adres:
	| Straatnaam                | Postcode | Huisnummer | Plaats                                                     | Land      |
	| St. Jacobsstraat          | 3511BS   | 12         | Utrecht                                                    | Nederland |
	| Longest Place Name Avenue | 8934 AS  | 12 K235    | Llanfairpwllgwyngyllgogerychwyrndrobwllllantysiliogogogoch | België    |
	Then voeg een klant toe aan het systeem met voornaam, achternaam, telefoonnummer, email en adres
	And genereer een klantnummer