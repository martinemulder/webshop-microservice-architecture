import { browser, by, element } from 'protractor';

export class ShopPage {
  navigateTo() {
    return browser.get('/');
  }

  getTitleText() {
    return element(by.css('app-shop h1')).getText();
  }
}
