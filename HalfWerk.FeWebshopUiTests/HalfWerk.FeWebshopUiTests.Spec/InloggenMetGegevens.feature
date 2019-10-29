Feature: Inloggen met gegevens
	Omdat ik producten wil bestellen
	Als klant
	Wil ik in kunnen loggen
	
@mytag
Scenario: Inloggen als klant
	Given ik ben op de inlogpagina
	When ik mijn emailadres "klant" invoer
	And mijn wachtwoord "klant" invoer
	And op de inlogknop klik
	Then wordt ik succesvol ingelogd
	And doorverwezen naar de webshop
	
Scenario: Inloggen als salasmedewerker
	Given ik ben op de inlogpagina
	When ik mijn emailadres "sales" invoer
	And mijn wachtwoord "sales" invoer
	And op de inlogknop klik
	Then wordt ik succesvol ingelogd
	And doorverwezen naar de salespagina
	
Scenario: Inloggen als magazijnmedewerker
	Given ik ben op de inlogpagina
	When ik mijn emailadres "magazijn1" invoer
	And mijn wachtwoord "magazijn1" invoer
	And op de inlogknop klik
	Then wordt ik succesvol ingelogd
	And doorverwezen naar de magazijnpagina
	
Scenario: Inloggen als niet bestaand account
	Given ik ben op de inlogpagina
	When ik mijn emailadres "bestaatniet" invoer
	And mijn wachtwoord "bestaatniet" invoer
	And op de inlogknop klik
	Then wordt ik niet ingelogd
	And blijf ik op de inlogpagina

Scenario: Als niet ingelogde gebruiker kan ik niet op de sales pagina komen
	Given ik ben op de inlogpagina
	When ik naar de salespagina navigeer
	Then wordt ik teruggestuurd naar de inlogpagina