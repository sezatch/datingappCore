import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { Observable, of } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { ConfirmService } from '../_services/confirm.service';

@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChangesGuard implements CanDeactivate<MemberEditComponent> {
  
  constructor(private confrimService: ConfirmService) {}

  canDeactivate(component: MemberEditComponent): Observable<boolean> {
    if(component.editForm?.dirty) {
      return this.confrimService.confirm();
    }
    return of(true);
  }
  
}
