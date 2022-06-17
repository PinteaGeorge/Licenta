import { Component, OnInit } from '@angular/core';
import { Member } from '../_models/member';
import { Pagination } from '../_models/pagination';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  members: Partial<Member[]>;
  predicate = 'liked';
  pageNo = 1;
  pageSize = 10;
  pagination: Pagination;
  constructor(private memberService: MembersService) { }

  ngOnInit(): void {
    this.loadLikes();
  }

  loadLikes(){
    this.memberService.getLikes(this.predicate, this.pageNo, this.pageSize).subscribe(data => {
      this.members = data.result;
      this.pagination = data.pagination;
    })
  }

  pageChanged(event: any){
    this.pageNo = event.page;
    this.loadLikes();
  }

}
