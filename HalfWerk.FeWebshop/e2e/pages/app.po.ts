import { browser, by, element, until } from 'protractor';

export class AppPage {

  public navigateTo() {
    return browser.get('/');
  }

  public enterSearchInput(text: string) {
    return element(by.css('input[aria-label="search"]'))
      .sendKeys(text);
  }

  public getSearchResultItems() {
    const condition = until.elementsLocated(by.css('.product'));

    return browser.wait(condition, 5000);
  }

}
