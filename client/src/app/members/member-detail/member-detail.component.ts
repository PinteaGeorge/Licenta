import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { Member } from 'src/app/_models/member';
import { Message } from 'src/app/_models/message';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';
import { MessageService } from 'src/app/_services/message.service';
import { PressenceService } from 'src/app/_services/pressence.service';

@Component({
  selector: 'app-member-detail',
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css']
})
export class MemberDetailComponent implements OnInit, OnDestroy {
  @ViewChild('memberTabs', {static: true}) memberTabs: TabsetComponent;
  memberTemplate: Member;
  galleryOptions:  NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  activeTab: TabDirective;
  messages: Message[] = [];
  user: User;

  constructor(public presence: PressenceService,private route: ActivatedRoute, private toastr: ToastrService,
    private messageService: MessageService, private accountService: AccountService, private router: Router, private memberService: MembersService) {
      this.accountService.currentUser$.pipe(take(1)).subscribe(data => this.user = data);
      this.router.routeReuseStrategy.shouldReuseRoute = () => false;
     }

  ngOnInit(): void {
    this.route.data.subscribe(data => {
      this.memberTemplate = data.member;
    })
    this.route.queryParams.subscribe(params => {
      params.tab ? this.selectTab(params.tab) : this.selectTab(0);
    })
    this.galleryOptions = [
      {
        width: '500px',
        height: '500px',
        imagePercent: 100,
        thumbnailsColumns: 4,
        imageAnimation: NgxGalleryAnimation.Slide,
        preview: false
      }
    ]
    this.galleryImages = this.getImages();
  }

  getImages(): NgxGalleryImage[]{
    const imageUrls = [];
    for(let item of this.memberTemplate.photos){
      imageUrls.push({
        small: item?.url,
        medium: item?.url,
        big: item?.url
      })
    }
    return imageUrls;
  }

  loadMessages(){
    this.messageService.getMessageThread(this.memberTemplate.username).subscribe(data => {
      this.messages = data;
    })
  }

  selectTab(tabId: number){
    this.memberTabs.tabs[tabId].active = true;
  }

  onTabActivated(data: TabDirective){
    this.activeTab = data;
    if(this.activeTab.heading === 'Messages' && this.messages.length === 0){
      this.messageService.createHubConnection(this.user, this.memberTemplate.username);
    }else{
      this.messageService.stopHubConnection();
    }
  }

  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }

  addLike(member: Member){
    this.memberService.addLike(member.username).subscribe(() => {
      this.toastr.success('You have liked ' + member.knownAs);
    })
  }
}
