import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {
  username = '';
  availableRoles: any[] = [];
  selectedRoles: any[] = [];

  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit(): void {
  }

  updateChecked(checkedValue: string){
    const idx = this.selectedRoles.indexOf(checkedValue);

    idx !== -1 ? this.selectedRoles.splice(idx, 1) : this.selectedRoles.push(checkedValue);
  }

}
