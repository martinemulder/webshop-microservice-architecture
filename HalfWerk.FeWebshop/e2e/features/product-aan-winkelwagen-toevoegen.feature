Feature: ProductAanWinkelwagenToevoegen
    Omdat ik producten wil bestellen
    Als klant
    Wil ik een lijst van alle producten kunnen zien

Scenario: Product aan winkelwagen kunnen toevoegen
  Given ik ben op de productenpagina
   When ik een product aan mijn winkelwagen wil toevoegen
   Then zie ik een knop om het product aan mijn winkelwagen toevoegen

Scenario: Product aan winkelwagen toegevoegd
  Given ik ben op de productenpagina
   When ik een product toevoeg aan mijn winkelwagen
   Then zie ik dat het product is toegevoegd aan een lijst binnen mijn winkelwagen

# Example:
#   Given ik ben op de producten pagina
#    Then zie ik een lijst van drie producten met ieder een naam, afbeelding en prijs
