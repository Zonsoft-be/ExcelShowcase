import { Component, OnDestroy, Self, Injector, AfterViewInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute } from '@angular/router';
import { Subscription, combineLatest } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { NavigationService, NavigationActivatedRoute, PanelManagerService, RefreshService, MetaService, ContextService, InternalOrganisationId, TestScope } from '../../../../../angular';
import { Good } from '../../../../../domain';
import { PullRequest, Pull } from '../../../../../framework';

@Component({
  templateUrl: './nonunifiedgood-overview.component.html',
  providers: [PanelManagerService, ContextService]
})
export class NonUnifiedGoodOverviewComponent extends TestScope implements AfterViewInit, OnDestroy {

  title = 'Good';

  good: Good;

  subscription: Subscription;

  constructor(
    @Self() public panelManager: PanelManagerService,
    public metaService: MetaService,
    public refreshService: RefreshService,
    public navigation: NavigationService,
    private route: ActivatedRoute,
    public injector: Injector,
    private internalOrganisationId: InternalOrganisationId,
    titleService: Title,
  ) {
    super();

    titleService.setTitle(this.title);
  }

  public ngAfterViewInit(): void {

    this.subscription = combineLatest(this.route.url, this.route.queryParams, this.refreshService.refresh$, this.internalOrganisationId.observable$)
      .pipe(
        switchMap(([urlSegments, queryParams, date, internalOrganisationId]) => {

          const { m, pull, x } = this.metaService;

          const navRoute = new NavigationActivatedRoute(this.route);
          this.panelManager.objectType = m.NonUnifiedGood;
          this.panelManager.id = navRoute.id();
          this.panelManager.expanded = navRoute.panel();

          this.panelManager.on();

          const pulls = [
            pull.NonUnifiedGood({
              object: this.panelManager.id,
              include: {
                ProductIdentifications: {
                  ProductIdentificationType: x
                },
                Part: {
                  Brand: x,
                  Model: x
                }
              }
            })
          ];

          this.panelManager.onPull(pulls);

          return this.panelManager.context
            .load(new PullRequest({ pulls }));
        })
      )
      .subscribe((loaded) => {

        this.panelManager.context.session.reset();
        this.panelManager.onPulled(loaded);

        this.good = loaded.objects.NonUnifiedGood as Good;
      });
  }

  public ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
