import { Component, OnDestroy, OnInit, ViewChild, Self } from '@angular/core';
import { Location } from '@angular/common';
import { Title } from '@angular/platform-browser';
import { MatDialog } from '@angular/material/dialog';
import { PageEvent } from '@angular/material/paginator';
import { MatSnackBar } from '@angular/material/snack-bar';
import { Sort } from '@angular/material/sort';
import { MatTableDataSource } from '@angular/material/table';
import { SelectionModel } from '@angular/cdk/collections';

import { BehaviorSubject, Subscription, combineLatest } from 'rxjs';
import { switchMap, scan } from 'rxjs/operators';

import { PullRequest, SessionObject, And, Like } from '../../../../framework';
import { Invoked, MediaService, ContextService, NavigationService, AllorsFilterService, MetaService, TestScope } from '../../../../angular';
import { Person } from '../../../../domain';
import { TableRow, Sorter, AllorsMaterialDialogService } from '../../../../material';

interface Row extends TableRow {
  person: Person;
  firstName: string;
  lastName: string;
  email: string;
}

@Component({
  templateUrl: './people.component.html',
  providers: [ContextService, AllorsFilterService]
})
export class PeopleComponent extends TestScope implements OnInit, OnDestroy {

  public title = 'People';

  public displayedColumns = ['select', 'firstName', 'lastName', 'email', 'menu'];
  public selection = new SelectionModel<Row>(true, []);

  public total: number;
  public dataSource = new MatTableDataSource<Row>();

  private sort$: BehaviorSubject<Sort | null>;
  private refresh$: BehaviorSubject<Date>;
  private pager$: BehaviorSubject<PageEvent>;

  private subscription: Subscription;

  constructor(
    @Self() public allors: ContextService,
    @Self() private filterService: AllorsFilterService,
    public metaService: MetaService,
    public navigation: NavigationService,
    public mediaService: MediaService,
    private dialog: MatDialog,
    private snackBar: MatSnackBar,
    private dialogService: AllorsMaterialDialogService,
    private location: Location,
    titleService: Title) {

    super();

    titleService.setTitle(this.title);

    this.sort$ = new BehaviorSubject<Sort | null>(null);
    this.refresh$ = new BehaviorSubject<Date>(new Date());
    this.pager$ = new BehaviorSubject<PageEvent>(Object.assign(new PageEvent(), { pageIndex: 0, pageSize: 50 }));
  }

  public ngOnInit(): void {

    const { x, m, pull } = this.metaService;

    const predicate = new And([
      new Like({ roleType: m.Person.FirstName, parameter: 'firstName' }),
      new Like({ roleType: m.Person.LastName, parameter: 'lasttName' }),
      new Like({ roleType: m.Person.UserEmail, parameter: 'email' }),
    ]);

    this.filterService.init(predicate);

    const sorter = new Sorter(
      {
        firstName: m.Person.FirstName,
        lastName: m.Person.LastName,
        email: m.Person.UserEmail,
      }
    );

    this.subscription = combineLatest([this.refresh$, this.filterService.filterFields$, this.sort$, this.pager$])
      .pipe(
        scan(([previousRefresh, previousFilterFields], [refresh, filterFields, sort, pageEvent]) => {
          return [
            refresh,
            filterFields,
            sort,
            (previousRefresh !== refresh || filterFields !== previousFilterFields) ? Object.assign({ pageIndex: 0 }, pageEvent) : pageEvent,
          ];
        }),
        switchMap(([, filterFields, sort, pageEvent]) => {

          const pulls = [
            pull.Person({
              predicate,
              sort: sort ? sorter.create(sort) : null,
              include: {
                Pictures: x
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
        this.total = loaded.values.People_total;
        const people = loaded.collections.People as Person[];

        this.dataSource.data = people.map((v) => {
          return {
            person: v,
            firstName: v.FirstName,
            lastName: v.LastName,
            email: v.UserEmail,
          } as Row;
        });
      });
  }

  public ngOnDestroy(): void {
    if (this.subscription) {
      this.subscription.unsubscribe();
    }
  }

  public get hasSelection() {
    return !this.selection.isEmpty();
  }

  public get hasDeleteSelection() {
    return this.selectedPeople.filter((v) => v.CanExecuteDelete).length > 0;
  }

  public get selectedPeople() {
    return this.selection.selected.map(v => v.person);
  }

  public isAllSelected() {
    const numSelected = this.selection.selected.length;
    const numRows = this.dataSource.data.length;
    return numSelected === numRows;
  }

  public masterToggle() {
    this.isAllSelected() ?
      this.selection.clear() :
      this.dataSource.data.forEach(row => this.selection.select(row));
  }

  public goBack(): void {
    this.location.back();
  }

  public refresh(): void {
    this.refresh$.next(new Date());
  }

  public sort(event: Sort): void {
    this.sort$.next(event);
  }

  public page(event: PageEvent): void {
    this.pager$.next(event);
  }

  public delete(person: Person | Person[]): void {

    const people = person instanceof SessionObject ? [person as Person] : person instanceof Array ? person : [];
    const methods = people.filter((v) => v.CanExecuteDelete).map((v) => v.Delete);

    if (methods.length > 0) {
      this.dialogService
        .confirm(
          methods.length === 1 ?
            { message: 'Are you sure you want to delete this person?' } :
            { message: 'Are you sure you want to delete these people?' })
        .subscribe((confirm: boolean) => {
          if (confirm) {
            this.allors.context.invoke(methods)
              .subscribe((invoked: Invoked) => {
                this.snackBar.open('Successfully deleted.', 'close', { duration: 5000 });
                this.refresh();
              });
          }
        });
    }
  }
}
