import { Component, OnInit } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Message } from '../_models/message';
import { Pagination } from '../_models/pagination';
import { ConfirmService } from '../_services/confirm.service';
import { MessageService } from '../_services/message.service';

@Component({
  selector: 'app-messages',
  templateUrl: './messages.component.html',
  styleUrls: ['./messages.component.css']
})
export class MessagesComponent implements OnInit {
  messages: Message[] = [];
  pagination: Pagination;
  container = 'Unread';
  pageNo = 1;
  pageSize = 5;
  loading = false;
  constructor(private messageService: MessageService, private confirmService: ConfirmService, private toastr: ToastrService) { }

  ngOnInit(): void {
    this.loadMessages();
  }

  loadMessages(){
    this.loading = true;
    this.messageService.getMessages(this.pageNo, this.pageSize, this.container).subscribe(data => {
      this.messages = data.result;
      this.pagination = data.pagination;
      this.loading = false;
    })
  }

  pageChanged(event: any){
    if(this.pageNo !== event.page){
      this.pageNo = event.page;
    this.loadMessages();
    }
  }

  deleteMessage(id: number){
    this.confirmService.confirm('Confirm delete message', 
    'Are you sure you want to delete this message? This cannot be undone!').subscribe(result =>{
      if(result){
        this.messageService.deleteMessage(id).subscribe(() => {
          this.messages.splice(this.messages.findIndex(x => x.id === id), 1);
          this.toastr.success('Message has been deleted!');
        });
      }
    });
  }

}
