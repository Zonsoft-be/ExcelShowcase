import * as moment from 'moment/moment';
import { Component, OnDestroy, OnInit, Self } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { Subscription, combineLatest } from 'rxjs';
import { switchMap, scan } from 'rxjs/operators';

import { PullRequest, And, Like } from '../../../../../framework';
import { AllorsFilterService, MediaService, ContextService, NavigationService, Action, RefreshService, MetaService, TestScope } from '../../../../../angular';
import { Sorter, TableRow, Table, DeleteService, EditService } from '../../../..';

import { CommunicationEvent } from '../../../../../domain';

interface Row extends TableRow {
  object: CommunicationEvent;
  name: string;
  type: string;
  state: string;
  subject: string;
  involved: string;
  started: string;
  ended: string;
  lastModifiedDate: string;
}

@Component({
  templateUrl: './communicationevent-list.component.html',
  providers: [ContextService, AllorsFilterService]
})
export class CommunicationEventListComponent extends TestScope implements OnInit, OnDestroy {

  public title = 'Communications';

  table: Table<Row>;

  delete: Action;
  edit: Action;

  private subscription: Subscription;

  constructor(
    @Self() public allors: ContextService,
    @Self() private filterService: AllorsFilterService,
    public metaService: MetaService,
    public refreshService: RefreshService,
    public deleteService: DeleteService,
    public editService: EditService,
    public navigation: NavigationService,
    public mediaService: MediaService,
    titleService: Title
  ) {
    super();

    titleService.setTitle(this.title);

    this.delete = deleteService.delete(allors.context);
    this.edit = editService.edit();

    this.delete.result.subscribe((v) => {
      this.table.selection.clear();
    });

    this.table = new Table({
      selection: true,
      columns: [
        { name: 'type' },
        { name: 'state' },
        { name: 'subject', sort: true },
        { name: 'involved' },
        { name: 'started' },
        { name: 'ended' },
        { name: 'lastModifiedDate', sort: true },
      ],
      actions: [
        this.edit,
        this.delete
      ],
      defaultAction: this.edit,
      pageSize: 50,
    });
  }

  ngOnInit(): void {

    const { m, pull, x } = this.metaService;

    const predicate = new And([
      new Like({ roleType: m.CommunicationEvent.Subject, parameter: 'subject' }),
    ]);

    // this.filterService.init(predicate);

    const sorter = new Sorter(
      {
        subject: m.CommunicationEvent.Subject,
        lastModifiedDate: m.CommunicationEvent.LastModifiedDate,
      }
    );

    this.subscription = combineLatest([this.refreshService.refresh$, this.filterService.filterFields$, this.table.sort$, this.table.pager$])
      .pipe(
        scan(([previousRefresh, previousFilterFields], [refresh, filterFields, sort, pageEvent]) => {
          return [
            refresh,
            filterFields,
            sort,
            (previousRefresh !== refresh || filterFields !== previousFilterFields) ? Object.assign({ pageIndex: 0 }, pageEvent) : pageEvent,
          ];
        }, [, , , , ]),
        switchMap(([refresh, filterFields, sort, pageEvent]) => {

          const pulls = [
            pull.CommunicationEvent({
              predicate,
              sort: sorter.create(sort),
              include: {
                CommunicationEventState: x,
                InvolvedParties: x,
              },
              parameters: this.filterService.parameters(filterFields),
              skip: pageEvent.pageIndex * pageEvent.pageSize,
              take: pageEvent.pageSize,
            })];

          return this.allors.context.load(new PullRequest({ pulls }));
        })
      )
      .subscribe((loaded) => {
        this.allors.context.reset();
        const communicationEvents = loaded.collections.CommunicationEvents as CommunicationEvent[];
        this.table.total = loaded.values.CommunicationEvents_total;
        this.table.data = communicationEvents.map((v) => {
          return {
            object: v,
            type: v.objectType.name,
            state: v.CommunicationEventState && v.CommunicationEventState.Name,
            subject: v.Subject,
            involved: v.InvolvedParties.map((w) => w.displayName).join(', '),
            started: v.ActualStart && moment(v.ActualStart).format('MMM Do YY'),
            ended: v.ActualEnd && moment(v.ActualEnd).format('MMM Do YY'),
            lastModifiedDate: moment(v.LastModifiedDate).fromNow()
          } as Row;
        });
      });
  }

  public ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }
}
