import { Component, OnDestroy, OnInit, Self } from '@angular/core';

import { Subscription, } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { PullRequest } from '../../../../../framework';
import { AllorsFilterService, ContextService, NavigationService, RefreshService, MetaService, UserId } from '../../../../../angular';

import { Notification, Person } from '../../../../../domain';

import { ObjectService } from '../../../../core/services/object';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'notification-link',
  templateUrl: './notification-link.component.html',
  providers: [ContextService, AllorsFilterService]
})
export class NotificationLinkComponent implements OnInit, OnDestroy {

  notifications: Notification[];

  private subscription: Subscription;

  get nrOfNotifications() {
    if (this.notifications) {
      const count = this.notifications.length;
      if (count < 99) {
        return count;
      } else if (count < 1000) {
        return '99+';
      } else {
        return Math.round(count / 1000) + 'k';
      }
    }
  }

  constructor(
    @Self() public allors: ContextService,
    public metaService: MetaService,
    public factoryService: ObjectService,
    public refreshService: RefreshService,
    public navigation: NavigationService,
    private userId: UserId,
    ) {
  }

  ngOnInit(): void {

    const { m, pull, x } = this.metaService;

    this.subscription = this.refreshService.refresh$
      .pipe(
        switchMap(() => {

          const pulls = [
            pull.Person({
              object: this.userId.value,
              include: {
                NotificationList: {
                  UnconfirmedNotifications: x
                }
              }
            })];

          return this.allors.context.load(new PullRequest({ pulls }));
        })
      )
      .subscribe((loaded) => {
        this.allors.context.reset();

        const user = loaded.objects.Person as Person;
        this.notifications = user.NotificationList.UnconfirmedNotifications;
      });
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  toNotifications() {
    this.navigation.list(this.metaService.m.Notification);
  }
}
