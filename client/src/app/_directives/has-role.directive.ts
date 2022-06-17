import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]'
})
export class HasRoleDirective implements OnInit {
  @Input() appHasRole: string[];
  user: User;

  constructor(private viewContainer: ViewContainerRef, private templateRef: TemplateRef<any>, private acountService: AccountService) {
    this.acountService.currentUser$.pipe(take(1)).subscribe(data => {
      this.user = data;
    })
   }
  ngOnInit(): void {
    if(!this.user?.roles || this.user == null){
      this.viewContainer.clear();
      return;
    }
    if(this.user?.roles.some(r => this.appHasRole.includes(r))){
      this.viewContainer.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainer.clear();
    }
  }

}
