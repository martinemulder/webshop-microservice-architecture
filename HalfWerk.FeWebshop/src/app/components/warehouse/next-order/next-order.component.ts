import { Component } from '@angular/core';
import { Router } from '@angular/router';

@Component({
  selector: 'app-orders',
  templateUrl: './next-order.component.html',
  styleUrls: ['./next-order.component.scss']
})
export class NextOrderComponent {

  constructor(private router: Router) {}

  nextOrder() {
    this.router.navigate(['/warehouse/nextorder']);
  }

}
