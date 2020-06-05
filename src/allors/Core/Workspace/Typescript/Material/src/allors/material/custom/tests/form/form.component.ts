import * as moment from 'moment/moment';
import { AfterViewInit, Component, OnDestroy, OnInit, Self } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { ActivatedRoute, Router, UrlSegment } from '@angular/router';

import { BehaviorSubject, combineLatest, Observable, Subscription } from 'rxjs';
import { switchMap } from 'rxjs/operators';

import { Meta } from '../../../../meta';
import { Person, Data, Organisation } from '../../../../domain';
import { PullRequest } from '../../../../framework';
import { SearchFactory, Loaded, WorkspaceService, ContextService, MetaService, TestScope } from '../../../../angular';
import { RadioGroupOption } from '../../../../material';
import { SaveService } from 'src/allors/material/core/services/save';

@Component({
  templateUrl: './form.component.html',
  providers: [ContextService]
})
export class FormComponent extends TestScope implements OnInit, AfterViewInit, OnDestroy {

  public title: string;
  public m: Meta;

  public organisations: Organisation[];
  public people: Person[];

  public peopleFilter: SearchFactory;

  public data: Data | null;

  public radioGroupOptions: RadioGroupOption[] = [
    { label: 'One', value: 'one' },
    { label: 'Two', value: 'two' },
    { label: 'Three', value: 'three' },
  ];

  private refresh$: BehaviorSubject<Date>;
  private subscription: Subscription;

  constructor(
    @Self() public allors: ContextService,
    public metaService: MetaService,
    private workspaceService: WorkspaceService,
    private titleService: Title,
    private route: ActivatedRoute,
    private saveService: SaveService,
  ) {

    super();

    this.title = 'Form';
    this.titleService.setTitle(this.title);

    this.m = this.metaService.m;

    this.peopleFilter = new SearchFactory({ objectType: this.m.Person, roleTypes: [this.m.Person.FirstName, this.m.Person.LastName, this.m.Person.UserName] });

    this.refresh$ = new BehaviorSubject<Date>(new Date());
  }

  ngOnInit(): void {
    const route$: Observable<UrlSegment[]> = this.route.url;
    const combined$: Observable<[UrlSegment[], Date]> = combineLatest(route$, this.refresh$);

    const { m, pull, x } = this.metaService;

    this.subscription = combined$
      .pipe(
        switchMap(([]: [UrlSegment[], Date]) => {

          const pulls = [
            pull.Data(
              {
                include: {
                  AutocompleteFilter: x,
                  AutocompleteOptions: x,
                  Chips: x,
                  File: x,
                  MultipleFiles: x
                }
              }),
            pull.Organisation({
              include: {
                OneData: x,
                ManyDatas: x,
              }
            }),
            pull.Person(),
          ];

          return this.allors.context
            .load(new PullRequest({ pulls }));
        }))
      .subscribe((loaded: Loaded) => {

        this.allors.context.reset();

        this.organisations = loaded.collections.organisations as Organisation[];
        this.people = loaded.collections.People as Person[];
        const datas = loaded.collections.Datas as Data[];

        if (datas && datas.length > 0) {
          this.data = datas[0];
        } else {
          this.data = this.allors.context.create(this.m.Data) as Data;
        }
      });
  }

  ngAfterViewInit(): void {
  }

  ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  reset() {
    this.allors.context.reset();
    this.data = this.allors.context.create(this.m.Data) as Data;
  }


  newDate() {
    if (this.data) {
      this.data.Date = moment.utc().toISOString();
    }
  }

  newDateTime() {
    if (this.data) {
      this.data.DateTime = moment.utc().toISOString();
    }
  }

  newDateTime2() {
    if (this.data) {
      this.data.DateTime2 = moment.utc().toISOString();
    }
  }

  refresh(): void {
    this.refresh$.next(new Date());
  }

  save(): void {

    console.log('save');

    this.allors.context
      .save()
      .subscribe(() => {
        this.data = null;
        this.refresh();
      },
        this.saveService.errorHandler
      );
  }

  public goBack(): void {
  }
}
