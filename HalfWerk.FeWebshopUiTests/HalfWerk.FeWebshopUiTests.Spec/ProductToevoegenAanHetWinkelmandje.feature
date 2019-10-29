Feature: Product toevoegen aan het winkelmandje
  Omdat ik een product wil bestellen
  Als een klant
  Wil ik een product kunnen toevoegen aan mijn winkelmandje

@mytag
Scenario: Product aan winkelwagen kunnen toevoegen
	Given ik ben op de productenpagina en wil een product aan mijn winkelwagen toevoegen
    When ik mijn product heb gevonden en klik op de Toevoegen knop
    And navigeer naar de winkelwagenpagina
    Then zie ik het net toegevoegde product staan in het overzicht

Scenario: Aantal in winkelwagen aanpassen
    Given ik ben op de winkelwagenpagina en heb een product erin zitten met aantal 1
    When ik klik op de plus bij een product
	And ik klik op de plus bij een product
	And ik klik op de min bij een product
    Then zie ik dat het aantal bij het product 2 is
    And de totaalprijs met 2 vermenigvuldigt is

Scenario: Productpagina openen
	Given ik wil een bestelling gaan doen
    When ik navigaar naar de productpagina
    Then zie ik als titel bij de pagina "Kantilever Webshop"
	And zie ik standaard 10 producten
