import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of, take } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { MembersService } from '../_services/members.service';
import { UserParams } from '../_models/userParams';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model:any = {};
  // user: User | undefined;
  //Using accountservice directly in the templateurl - html instead of using it in the .ts file.
  // Commenting the currentUser$ observable code here in .ts file and making the accountService public from private 
  // so we can have access in the templateurl - html
  // currentUser$: Observable<User | null> = of(null);

  constructor(public accountService: AccountService, private router: Router,
     private toastr: ToastrService, private memberService: MembersService) { }

  ngOnInit(): void {
    // this.currentUser$ = this.accountService.currentUser$
  }

  login() {
    this.accountService.login(this.model).subscribe({
      next: () => {
        this.router.navigateByUrl('/members');
        this.model = {};
        // this.accountService.currentUser$.pipe(take(1)).subscribe({
        //   next: user => {
        //     if(user){
        //       this.memberService.setUserParams(new UserParams(user));
        //     }
        //   }
        // });
      }
    })

  }


  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
