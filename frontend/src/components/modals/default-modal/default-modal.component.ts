import { CommonModule } from '@angular/common';
import { Output, EventEmitter } from '@angular/core';
import { Component, Inject } from '@angular/core';
import { MatButtonModule } from '@angular/material/button';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { MatButtonToggleModule } from '@angular/material/button-toggle';
import { FormsModule } from '@angular/forms';
import { TagSelectorComponent } from "../../tag-selector/tag-selector.component";

@Component({
  selector: 'app-modal',
  standalone: true,
  imports: [CommonModule, MatDialogModule, MatButtonModule, MatButtonToggleModule, FormsModule, TagSelectorComponent],
  template: `
    <div class="container">
      <h2 mat-dialog-title>{{ data.title }}</h2>
      <mat-dialog-content>{{ data.message }}</mat-dialog-content>
      <app-tag-selector [selectedOption]="selectedTag" (tagSelected)="onTagSelected($event)"></app-tag-selector>
      <mat-dialog-actions>
        <button mat-button (click)="onCancel()">Cancel</button>
        <button mat-button (click)="onConfirm()" cdkFocusInitial>Confirm</button>
      </mat-dialog-actions>
    </div>

  `,
  styles: [`
    .container {
      display: flex;
      flex-direction: column;
      align-items: center;
      justify-content: center;
      text-align: center;
      padding: 20px;
    }
    mat-dialog-content { margin-bottom: 20px; }
    mat-button-toggle-group { margin-bottom: 20px; }
    mat-dialog-actions { justify-content: center; margin-top: 20px; }
    button { margin: 0 10px; }
  `]
})
export class DefaultModalComponent {
  selectedTag: string = 'none';

  constructor(
    public dialogRef: MatDialogRef<DefaultModalComponent>,
    @Inject(MAT_DIALOG_DATA) public data: any
  ) {
    this.selectedTag = data.selectedTag || 'none';
  }

  onTagSelected(tag: string) {
    this.selectedTag = tag;
  }

  onConfirm(): void {
    this.dialogRef.close({ selectedTag: this.selectedTag });
  }

  onCancel(): void {
    this.dialogRef.close();
  }
}

