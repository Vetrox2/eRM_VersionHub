import {
  Component,
  EventEmitter,
  Input,
  OnDestroy,
  OnInit,
  Output,
} from '@angular/core';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { FormsModule } from '@angular/forms';
import { SearchService } from '../../services/search-service.service';
import { MatButtonModule } from '@angular/material/button';
import { debounceTime, distinctUntilChanged, Subject, takeUntil } from 'rxjs';

@Component({
  selector: 'app-search',
  standalone: true,
  imports: [
    MatInputModule,
    MatIconModule,
    MatFormFieldModule,
    FormsModule,
    MatButtonModule,
  ],
  template: `
    <mat-form-field class="search-field" appearance="fill">
      @if (showIcons) {
      <mat-icon matIconPrefix>search</mat-icon>
      }
      <input
        matInput
        [placeholder]="placeholder"
        name="search"
        [(ngModel)]="value"
        (ngModelChange)="onInputChange()"
      />
      @if (showIcons && value.length > 0) {
      <button
        matSuffix
        aria-label="Clear"
        (click)="clearSearch()"
        class="clear-button"
      >
        <mat-icon>close</mat-icon>
      </button>
      }
    </mat-form-field>
  `,
  styleUrls: ['./search.component.scss'],
})
export class SearchComponent implements OnInit, OnDestroy {
  @Input() showIcons: boolean = true;
  @Input() placeholder: string = 'Search';
  @Output() valueChanged = new EventEmitter<string>();

  value: string = '';
  private searchSubject = new Subject<string>();
  private destroy$ = new Subject<void>();

  constructor(private searchService: SearchService) {}

  ngOnInit() {
    this.searchSubject
      .pipe(debounceTime(300), distinctUntilChanged(), takeUntil(this.destroy$))
      .subscribe((value) => {
        this.valueChanged.emit(value);
      });
  }

  onInputChange() {
    this.searchSubject.next(this.value);
  }

  clearSearch() {
    this.value = '';
    this.searchSubject.next(this.value);
  }

  ngOnDestroy() {
    this.destroy$.next();
    this.destroy$.complete();
  }
}
