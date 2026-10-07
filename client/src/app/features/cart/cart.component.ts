import { Component, inject, ChangeDetectionStrategy } from '@angular/core';
import { CartService } from '../../core/services/cart.service';
import { CartItemComponent } from './cart-item/cart-item.component';
import { OrderSummary } from '../../shared/components/order-summary/order-summary';
import { EmptyState } from '../../shared/components/empty-state/empty-state';
import { Router } from '@angular/router';
@Component({
  selector: 'app-cart.component',
  imports: [CartItemComponent, OrderSummary, EmptyState],
  templateUrl: './cart.component.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './cart.component.css',
})
export class CartComponent {
  private router = inject(Router);
  cartService = inject(CartService);


  onAction(){
    this.router.navigateByUrl('/shop');
   }
}
