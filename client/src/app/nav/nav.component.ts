import { Component, OnInit } from '@angular/core';
import { AccountService } from '../_services/account.service';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
import { Router } from '@angular/router';

@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model:any = {};
  //Using accountservice directly in the templateurl - html instead of using it in the .ts file.
  // Commenting the currentUser$ observable code here in .ts file and making the accountService public from private 
  // so we can have access in the templateurl - html
  // currentUser$: Observable<User | null> = of(null);

  constructor(public accountService: AccountService, private router: Router) { }

  ngOnInit(): void {
    // this.currentUser$ = this.accountService.currentUser$
  }

  login() {
    this.accountService.login(this.model).subscribe({
      next: () => this.router.navigateByUrl('/members'),
      error: err => console.log(err) 
    })

  }


  logout() {
    this.accountService.logout();
    this.router.navigateByUrl('/');
  }

}
