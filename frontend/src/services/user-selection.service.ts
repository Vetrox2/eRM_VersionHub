import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable } from 'rxjs';

interface UserWithApps {
  Username: string;
  AppsNames: string[];
}

@Injectable({
  providedIn: 'root',
})
export class UserSelectionService {
  private selectedUserSubject = new BehaviorSubject<UserWithApps | null>(null);

  setSelectedUser(user: UserWithApps | null) {
    this.selectedUserSubject.next(user);
  }

  getSelectedUser(): Observable<UserWithApps | null> {
    return this.selectedUserSubject.asObservable();
  }
}
