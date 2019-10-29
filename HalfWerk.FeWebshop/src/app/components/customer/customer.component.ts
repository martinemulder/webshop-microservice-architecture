import {
  Component, OnInit
} from '@angular/core';
import {
  trigger,
  state,
  style,
  transition,
  animate
} from '@angular/animations';
import { Router, NavigationStart, NavigationEnd, NavigationCancel, NavigationError, RouterEvent } from '@angular/router';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-customer',
  templateUrl: './customer.component.html',
  styleUrls: ['./customer.component.scss'],
  animations: [
    trigger('onSideNavContainerChange', [
      state(
        'close',
        style({
          marginLeft: '60px',
          width: 'calc(100vw - 60px)'
        })
      ),
      state(
        'open',
        style({
          marginLeft: '200px',
          width: 'calc(100vw - 200px)'
        })
      ),
      transition('close => open', animate('175ms ease-in')),
      transition('open => close', animate('500ms ease-in'))
    ])
  ]
})
export class CustomerComponent implements OnInit {

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

  onSideNavStateChanged(sideNavExpanded: boolean) {
    this.expanded = sideNavExpanded;
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
