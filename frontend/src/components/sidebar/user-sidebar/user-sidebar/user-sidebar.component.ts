import { Component, OnInit } from '@angular/core';
import { Observable, of } from 'rxjs';
import { map } from 'rxjs/operators';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AsyncPipe, NgClass, NgFor, NgIf, NgStyle } from '@angular/common';
import { AdminService } from '../../../../services/admin.service';
import { UserSelectionService } from '../../../../services/user-selection.service';
import { SearchComponent } from '../../../search/search.component';
import { UserApps } from '../../../../models/user-apps.model';

@Component({
  selector: 'app-user-sidebar',
  standalone: true,
  imports: [
    MatListModule,
    MatTooltipModule,
    NgClass,
    NgFor,
    NgIf,
    NgStyle,
    AsyncPipe,
    SearchComponent,
    MatDividerModule,
  ],
  templateUrl: './user-sidebar.component.html',
  styleUrls: ['./user-sidebar.component.scss'],
})
export class UserSidebarComponent implements OnInit {
  users$: Observable<UserApps[]>;
  filteredUsers$: Observable<UserApps[]>;
  selectedUser$: Observable<UserApps | null>;
  loading = true;
  error = '';

  constructor(
    private adminService: AdminService,
    private userSelectionService: UserSelectionService
  ) {
    this.users$ = this.adminService.users$;
    this.filteredUsers$ = this.users$;
    this.selectedUser$ = this.userSelectionService.getSelectedUser();
  }

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.loading = true;
    this.adminService.loadUsers();
    this.users$.subscribe(
      (users) => {
        this.loading = false;
        if (users.length === 0) {
          this.error = 'No users found.';
        }
        this.userSelectionService.setSelectedUser(users[0]);
      },
      (error) => {
        this.loading = false;
        this.error = 'Error loading users. Please try again.';
        console.error('Error loading users:', error);
      }
    );
  }

  onSearchValueChanged(value: string) {
    this.filteredUsers$ = this.users$.pipe(
      map((users) =>
        users.filter((user) =>
          user.Username.toLowerCase().includes(value.toLowerCase())
        )
      )
    );
  }

  selectUser(user: UserApps) {
    this.userSelectionService.setSelectedUser(user);
  }

  trackByUsername(index: number, user: UserApps): string {
    return user.Username;
  }
}
