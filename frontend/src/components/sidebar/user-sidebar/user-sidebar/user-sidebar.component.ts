import { Component, OnInit } from '@angular/core';
import { Observable } from 'rxjs';
import { MatDividerModule } from '@angular/material/divider';
import { MatListModule } from '@angular/material/list';
import { MatTooltipModule } from '@angular/material/tooltip';
import { AsyncPipe, NgClass, NgFor, NgIf, NgStyle } from '@angular/common';
import { AdminService } from '../../../../services/admin.service';
import { UserSelectionService } from '../../../../services/user-selection.service';
import { SearchComponent } from '../../../search/search.component';

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
  users: string[] = [];
  filteredUsers: string[] = [];
  selectedUser: string = '';
  loading = true;
  error = '';

  constructor(
    private adminService: AdminService,
    private userSelectionService: UserSelectionService
  ) {}

  ngOnInit() {
    this.adminService.loadUsers();
    this.adminService.users$.subscribe(
      (users) => {
        this.users = users;
        this.filteredUsers = users;
        if (users.length > 0) {
          this.selectUser(users[0]);
        }
        this.loading = false;
      },
      (error) => {
        this.error = 'Error loading users';
        this.loading = false;
      }
    );
  }

  onSearchValueChanged(value: string) {
    this.filteredUsers = this.users.filter((user) =>
      user.toLowerCase().includes(value.toLowerCase())
    );
  }

  selectUser(user: string) {
    this.selectedUser = user;
    this.userSelectionService.setSelectedUser(user);
  }
}
