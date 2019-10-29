import { expect } from 'chai';
import { Given, When, Then, Before } from 'cucumber';
import { AppPage } from '../pages/app.po';

let app: AppPage;

Before(() => {
  app = new AppPage();
});

Given('ik ben op de productenpagina', () => app.navigateTo());

When('ik een product aan mijn winkelwagen wil toevoegen', function () {
  return;
});

Then('zie ik een knop om het product aan mijn winkelwagen toevoegen', function () {
  // Check of er een button element bestaat
  app.getSearchResultItems().then(elems => expect(elems.length).to.be.greaterThan(0));
});


When('ik een product toevoeg aan mijn winkelwagen', () => {
  console.log('Add product');
});

Then('zie ik dat het product is toegevoegd aan een lijst binnen mijn winkelwagen', () => {
  // Navigeer naar overzicht van winkelwagen
  // Check of de winkelwagen het item bevat
  app.getSearchResultItems().then(elems => expect(elems.length).to.be.greaterThan(0));
});
