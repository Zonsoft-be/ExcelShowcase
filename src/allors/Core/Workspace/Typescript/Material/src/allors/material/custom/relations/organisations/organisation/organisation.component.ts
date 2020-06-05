import { AfterViewInit, Component, OnDestroy, OnInit, Self } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, UrlSegment } from '@angular/router';

import { BehaviorSubject, Observable, Subscription, combineLatest } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { RoleField, SearchFactory, Loaded, Saved, WorkspaceService, ContextService, MetaService, TestScope } from '../../../../../angular';
import { Organisation, Person } from '../../../../../domain';
import { PullRequest, assert } from '../../../../../framework';
import { Meta } from '../../../../../meta';
import { getLocaleDateTimeFormat } from '@angular/common';

@Component({
  templateUrl: './organisation.component.html',
  providers: [ContextService]
})
export class OrganisationComponent extends TestScope implements OnInit, AfterViewInit, OnDestroy {

  title: string;

  field: RoleField;

  m: Meta;
  people: Person[];

  organisation: Organisation;

  peopleFilter: SearchFactory;

  private refresh$: BehaviorSubject<Date>;
  private subscription: Subscription;

  constructor(
    @Self() public allors: ContextService,
    private metaService: MetaService,
    private titleService: Title,
    private route: ActivatedRoute,
  ) {
    super();

    this.title = 'Organisation';
    this.titleService.setTitle(this.title);

    this.m = this.metaService.m;

    this.peopleFilter = new SearchFactory({ objectType: this.m.Person, roleTypes: [this.m.Person.UserName] });

    this.refresh$ = new BehaviorSubject<Date>(new Date());
  }

  public ngOnInit(): void {
    const route$: Observable<UrlSegment[]> = this.route.url;
    const combined$: Observable<[UrlSegment[], Date]> = combineLatest([route$, this.refresh$]);

    const { pull } = this.metaService;

    this.subscription = combined$
      .pipe(
        switchMap(([]: [UrlSegment[], Date]) => {

          const id = this.route.snapshot.paramMap.get('id');

          const pulls = [
            pull.Organisation({
              object: id ?? ''
            }),
            pull.Person()
          ];

          return this.allors.context
            .load(new PullRequest({ pulls }));
        })
      )
      .subscribe((loaded: Loaded) => {

        this.allors.context.reset();

        this.organisation = loaded.objects.Organisation as Organisation || this.allors.context.create('Organisation') as Organisation;
        this.people = loaded.collections.People as Person[];
      });
  }

  public ngAfterViewInit(): void {
  }

  public ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  public refresh(): void {
    this.refresh$.next(new Date());
  }

  public toggleCanWrite() {
    this.allors.context
      .invoke(this.organisation.ToggleCanWrite)
      .subscribe(() => {
        this.refresh();
      });
  }

  public save(): void {

    this.allors.context
      .save()
      .subscribe(() => {
        this.goBack();
      });
  }

  public goBack(): void {
    window.history.back();
  }

  public ownerSelected(field: RoleField): void {
    this.field = field;
  }
}
