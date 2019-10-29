import {
  Component, OnInit
} from '@angular/core';
import { Router, NavigationStart, NavigationEnd, NavigationCancel, NavigationError, RouterEvent } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-sales',
  templateUrl: './sales.component.html',
  styleUrls: ['./sales.component.scss'],
})
export class SalesComponent implements OnInit {

  expanded: boolean = false;
  loading: boolean = true;
  subscription: Subscription;

  public get sideNavState(): string {
    return this.expanded ? 'open' : 'close';
  }

  constructor(private router: Router) {
    this.subscription = router.events.subscribe((routerEvent: any) => {
      this.checkEventFromRouter(routerEvent);
    });
  }

  ngOnInit() {

  }

  ngOnDestroy() {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  checkEventFromRouter(routerEvent: RouterEvent): void {
    if (routerEvent instanceof NavigationStart) {
      this.loading = true;
    }

    if (routerEvent instanceof NavigationEnd ||
      routerEvent instanceof NavigationCancel ||
      routerEvent instanceof NavigationError) {
      this.loading = false;
    }
  }

}
