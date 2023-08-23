import { Inject, inject } from '@angular/core';
import { ActivatedRouteSnapshot, CanActivateFn, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable, map, pipe } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { ToastrService } from 'ngx-toastr';


export const adminGuard: CanActivateFn = (route,state) => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);

  return accountService.currentUser$.pipe(
    map(user  => {
      if(!user) return false;

      if(user.roles.includes('Admin') || user.roles.includes('Moderator')){
        return true;
      }
      else{
        toastr.error('This is not permmitable');
        return false;
      }

    })
  )

  }
  

