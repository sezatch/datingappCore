import { Component, OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent implements OnInit {

  registerMode: boolean = false;
  users: any;

  constructor() { }
  // constructor(private http: HttpClient) { }

  ngOnInit(): void {
    // this.getUsers();
  }

  registerToggle(){
    this.registerMode = !this.registerMode;
  }

    // getUsers(){
    //   this.http.get('https://localhost:5001/api/users').subscribe({
    //     next: res => this.users = res,
    //     error: err => console.log(err), 
    //     complete: () => console.log('Request Completedd!')
    //   })
    // }


  cancelRegisterMode(event: boolean){
    this.registerMode = event;
  }

}
