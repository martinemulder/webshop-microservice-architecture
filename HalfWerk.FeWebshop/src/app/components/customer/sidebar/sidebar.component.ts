import { Component, OnInit, Input, Output, EventEmitter } from '@angular/core';
import {
  trigger,
  state,
  style,
  transition,
  animate
} from '@angular/animations';
import { Route } from '@angular/router';
import { FormGroup, FormBuilder } from '@angular/forms';
import { ShoppingCartService } from '@services/shopping-cart/shopping-cart.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-sidebar',
  templateUrl: './sidebar.component.html',
  styleUrls: ['./sidebar.component.scss'],
  animations: [
    trigger('onSideNavChange', [
      state(
        'close',
        style({
          width: '60px'
        })
      ),
      state(
        'open',
        style({
          width: '200px'
        })
      ),
      transition('close => open', animate('175ms ease-in')),
      transition('open => close', animate('500ms ease-in'))
    ])
  ]
})
export class SidebarComponent implements OnInit {
  @Input('sideNavState') sideNavState: string;

  @Output('sideNavStateChanged') sideNavStateEmitter = new EventEmitter<boolean>();

  options: FormGroup;

  expanded = false;

  routerLinks: IRouterLink[] = [];

  private subscription: Subscription;

  constructor(fb: FormBuilder, private shoppingCartService: ShoppingCartService) {
    this.options = fb.group({
      bottom: 0,
      fixed: true,
      top: 72
    });
  }

  ngOnInit() {
    this.subscription = this.shoppingCartService.cart$.subscribe(() => {
      this.updateShoppingCartCount();
    });
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  toggleExpanded(event: Event) {
    this.expanded = !this.expanded;
    this.sideNavStateEmitter.emit(this.expanded);
  }

  private updateShoppingCartCount(): void {
    this.routerLinks = [
      {
        name: 'Producten',
        icon: 'local_mall',
        route: { path: '/shop' }
      },
      {
        name: 'Winkelmandje',
        // tslint:disable-next-line:max-line-length
        icon: this.shoppingCartService.getTotalItemCount() < 10 ? 'shopping_basket' : this.shoppingCartService.getTotalItemCount() < 99 ? 'shopping_cart' : 'local_shipping',
        route: { path: '/shop/cart' },
        badge: this.shoppingCartService.getTotalItemCount()
      }
    ];
  }
}

export interface IRouterLink {
  name: string;
  icon: string;
  route: Route;
  badge?: number;
}
