import { HttpClient, HttpClientModule } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { AccountService } from './_services/account.service';
import { User } from './_models/user';
import { retry } from 'rxjs';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'client';
  users: any;
  constructor(private http: HttpClient, private accountService: AccountService){

  }
  ngOnInit(): void {
    this.getUsers();
    this.setCurrentUser();
  }

  getUsers(){
    this.http.get('https://localhost:5001/api/users').subscribe({
      next: res => this.users = res,
      error: err => console.log(err), 
      complete: () => console.log('Request Completedd!')
    })
  }

  setCurrentUser(){
    const userString = localStorage.getItem('user');
    if(!userString) return;

    if(userString){
      const user: User = JSON.parse(userString);
      this.accountService.setCurrentUser(user);
    }
  }


  
}
