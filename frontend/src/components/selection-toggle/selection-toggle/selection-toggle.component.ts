import { Component } from '@angular/core';
import { MatButtonToggleModule } from '@angular/material/button-toggle';

@Component({
  selector: 'selection-toggle',
  standalone: true,
  imports: [MatButtonToggleModule],
  template: `
    <mat-button-toggle-group
      style="margin-bottom: 1em;"
      name="accountView"
      aria-label="Project selection"
    >
      <mat-button-toggle value="my" [checked]="true">All</mat-button-toggle>
      <mat-button-toggle value="shared">Favorites</mat-button-toggle>
    </mat-button-toggle-group>
  `,
  styles: [``],
})
export class SelectionToggleComponent {
  // You can add any necessary logic here
}
