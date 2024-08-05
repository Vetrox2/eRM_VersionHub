import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';
import { AppPermission } from '../models/app-permissions.model';

@Injectable({
  providedIn: 'root',
})
export class UserSelectionService {
  selectedUserSubject = new BehaviorSubject<string>('');

  setSelectedUser(user: string) {
    this.selectedUserSubject.next(user);
  }

  getSelectedUser(): string {
    const selectedUser = this.selectedUserSubject.getValue();
    return selectedUser;
  }
}
