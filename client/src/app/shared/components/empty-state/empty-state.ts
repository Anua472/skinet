import { Component, ChangeDetectionStrategy, input, output, inject } from '@angular/core';
import { MatButton } from '@angular/material/button';
import { MatIcon } from '@angular/material/icon';
import { RouterLink } from '@angular/router';
import { Busy } from '../../../core/services/busy';

@Component({
  selector: 'app-empty-state',
  imports: [
    MatIcon,
    MatButton,
    ],
  templateUrl: './empty-state.html',
  changeDetection: ChangeDetectionStrategy.Eager,
  styleUrl: './empty-state.css',
})
export class EmptyState {
  busyService = inject(Busy)
  message = input.required<string>();
  icon = input.required<string>();
  actionText = input.required<string>();
  action = output<void>();

  onAction(){
    this.action.emit();
  }
}
