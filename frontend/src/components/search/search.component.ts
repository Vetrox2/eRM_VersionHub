import { Component, EventEmitter, Output } from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { SearchService } from '../../services/search-service.service';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [MatInputModule, MatIconModule, MatFormFieldModule, FormsModule],
  template: `
    <div class="search">
      <mat-icon (click)="searchInput.focus()">search</mat-icon>
      <input #searchInput type="text" placeholder="Version" (input)="updateSearch($event)">
    </div>
  `,
  styles: [
    `
      .search {
        display: flex;
        border: 1px solid #d4d4d4;
        width: min-content;
        height: 56px;
        position: relative;
        border-radius: 2px;
        margin-right: 10px;
        cursor: pointer;
      }
      mat-icon {
        position: absolute;
        height: 55px;
        width: 35px;
        display: flex;
        align-items: center;
        justify-content: center;
      }
      input {
        border: none;
        outline: none;
        padding-inline: 35px;
        cursor: pointer;
      }
    `,
  ],
})
export class SearchComponent {
  constructor(private searchService: SearchService) {}

  updateSearch(event: Event) {
    const target = event.target as HTMLInputElement;
    const searchText = target.value;
    this.searchService.setSearchText(searchText);
  }
}
