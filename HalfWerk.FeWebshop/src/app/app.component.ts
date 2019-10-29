import {
  Component, Renderer2
} from '@angular/core';
import { Router, NavigationStart } from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss'],
})
export class AppComponent {

  title = '';
  previousUrl: string;

  constructor(private renderer: Renderer2, private router: Router) {
    this.addComponentNameClassToBody();
  }

  private addComponentNameClassToBody() {
    this.router.events
    .subscribe((event) => {
      if (event instanceof NavigationStart) {
        if (this.previousUrl) {
          this.renderer.removeClass(document.body, this.previousUrl);
        }
        const currentUrlSlug = event.url.slice(1);
        if (currentUrlSlug) {
          this.renderer.addClass(document.body, currentUrlSlug);
        } else {
          this.renderer.addClass(document.body, 'home');
        }
        this.previousUrl = currentUrlSlug;
      }
    });
  }

}
