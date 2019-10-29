Feature: MagazijnMedewerker
	Om bestellingen te kunnen inpakken
	Als magazijnmedewerker
	Wil ik een bestelling zien om in te kunnen pakken

@mytag
Scenario: Volgende bestelling inzien
	Given ik ben ingelogd als magazijnmedewerker
	And ik geef aan een volgende bestelling te willen inpakken
	Then opent de eerstvolgende bestelling met de status Goedgekeurd

Scenario: Factuur printen
	Given ik heb een in te pakken bestelling geopend
	And ik druk op de knop factuur printen
	Then print er een factuur

Scenario: Adreslabel printen
	Given ik heb een in te pakken bestelling geopend
	And ik druk op de knop adreslabel printen
	Then print er een adreslabel

Scenario: Bestelling klaarmelden
	Given ik heb alle artikelen in de bestelling verzameld
	And ik heb een factuur geprint
	And ik heb een adreslabel geprint
	When ik de bestelling klaarmeld
	Then wordt de status van de bestelling verzonden
	And ga ik terug naar het nieuwe bestelling inzien scherm
