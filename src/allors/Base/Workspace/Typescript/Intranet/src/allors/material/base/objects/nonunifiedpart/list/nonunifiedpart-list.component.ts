import { Component, OnDestroy, OnInit, Self } from '@angular/core';
import { Title } from '@angular/platform-browser';

import { Subscription, combineLatest } from 'rxjs';
import { switchMap, scan } from 'rxjs/operators';
import * as moment from 'moment/moment';

import { PullRequest, And, Like, Equals, Contains, ContainedIn, Filter, Or, Sort, GreaterThan } from '../../../../../framework';
import { AllorsFilterService, MediaService, ContextService, NavigationService, Action, RefreshService, MetaService, SearchFactory, SingletonId, UserId, InternalOrganisationId, FetcherService } from '../../../../../angular';
import { Sorter, TableRow, Table, OverviewService, DeleteService, FiltersService, PrintService, SaveService } from '../../../..';

import { Part, ProductIdentificationType, ProductIdentification, Facility, Organisation, Brand, Model, InventoryItemKind, ProductType, NonUnifiedPart, PartCategory, NonUnifiedPartBarcodePrint, Singleton, NonSerialisedInventoryItem, Person } from '../../../../../domain';

import { ObjectService } from '../../../../../material/core/services/object';

interface Row extends TableRow {
  object: Part;
  name: string;
  partNo: string;
  categories: string;
  qoh: string;
  localQoh: string;
  brand: string;
  model: string;
  kind: string;
  lastModifiedDate: string;
}

@Component({
  templateUrl: './nonunifiedpart-list.component.html',
  providers: [ContextService, AllorsFilterService]
})
export class NonUnifiedPartListComponent implements OnInit, OnDestroy {

  public title = 'Parts';

  table: Table<Row>;

  edit: Action;
  delete: Action;
  print: Action;

  private subscription: Subscription;
  goodIdentificationTypes: ProductIdentificationType[];
  parts: NonUnifiedPart[];
  nonUnifiedPartBarcodePrint: NonUnifiedPartBarcodePrint;
  facilities: Facility[];
  user: Person;
  internalOrganisation: Organisation;

  constructor(
    @Self() public allors: ContextService,
    @Self() private filterService: AllorsFilterService,
    public metaService: MetaService,
    public factoryService: ObjectService,
    public refreshService: RefreshService,
    public overviewService: OverviewService,
    public deleteService: DeleteService,
    public navigation: NavigationService,
    public mediaService: MediaService,
    public printService: PrintService,
    private filtersService: FiltersService,
    private saveService: SaveService,
    private singletonId: SingletonId,
    private fetcher: FetcherService,
    private internalOrganisationId: InternalOrganisationId,
    private userId: UserId,
    titleService: Title) {

    titleService.setTitle(this.title);

    this.print = printService.print();

    this.delete = deleteService.delete(allors.context);
    this.delete.result.subscribe(() => {
      this.table.selection.clear();
    });

    this.table = new Table({
      selection: true,
      columns: [
        { name: 'name', sort: true },
        { name: 'partNo', sort: true },
        { name: 'type' },
        { name: 'categories' },
        { name: 'qoh' },
        { name: 'localQoh' },
        { name: 'brand' },
        { name: 'model' },
        { name: 'kind' },
        { name: 'lastModifiedDate', sort: true },
      ],
      actions: [
        overviewService.overview(),
        this.delete
      ],
      defaultAction: overviewService.overview(),
      pageSize: 50,
    });
  }

  ngOnInit(): void {
    const { m, pull, x } = this.metaService;

    const predicate = new And([
      new Or([
        new Like({ roleType: m.Part.Name, parameter: 'name' }),
        new ContainedIn({
          propertyType: m.Part.LocalisedNames,
          extent: new Filter({
            objectType: m.LocalisedText,
            predicate: new Like({
              roleType: m.LocalisedText.Text,
              parameter: 'name'
            })
          })
        }),
      ]),
      new Like({ roleType: m.Part.Keywords, parameter: 'keyword' }),
      new Like({ roleType: m.Part.HsCode, parameter: 'hsCode' }),
      new Contains({ propertyType: m.Part.ProductIdentifications, parameter: 'identification' }),
      new Contains({ propertyType: m.Part.SuppliedBy, parameter: 'supplier' }),
      new ContainedIn({
        propertyType: m.Part.SupplierOfferingsWherePart,
        extent: new Filter({
          objectType: m.SupplierOffering,
          predicate: new Like({
            roleType: m.SupplierOffering.SupplierProductId,
            parameter: 'supplierReference'
          })
        })
      }),
      new Equals({ propertyType: m.Part.ManufacturedBy, parameter: 'manufacturer' }),
      new Equals({ propertyType: m.Part.Brand, parameter: 'brand' }),
      new Equals({ propertyType: m.Part.Model, parameter: 'model' }),
      new Equals({ propertyType: m.Part.InventoryItemKind, parameter: 'kind' }),
      new Equals({ propertyType: m.Part.ProductType, parameter: 'type' }),
      new Contains({ propertyType: m.NonUnifiedPart.PartCategoriesWherePart, parameter: 'category' }),
      new ContainedIn({
        propertyType: m.Part.InventoryItemsWherePart,
        extent: new Filter({
          objectType: m.NonSerialisedInventoryItem,
          predicate: new Equals({
            propertyType: m.InventoryItem.Facility,
            parameter: 'inStock',
          }),
        }),
      }),
      new ContainedIn({
        propertyType: m.Part.InventoryItemsWherePart,
        extent: new Filter({
          objectType: m.NonSerialisedInventoryItem,
          predicate: new Equals({
            propertyType: m.InventoryItem.Facility,
            parameter: 'outOfStock',
          }),
        }),
      }),
    ]);

    const typeSearch = new SearchFactory({
      objectType: m.ProductType,
      roleTypes: [m.ProductType.Name],
    });

    const kindSearch = new SearchFactory({
      objectType: m.InventoryItemKind,
      predicates: [new Equals({ propertyType: m.Enumeration.IsActive, value: true })],
      roleTypes: [m.InventoryItemKind.Name],
    });

    const categorySearch = new SearchFactory({
      objectType: m.PartCategory,
      roleTypes: [m.PartCategory.Name],
    });

    const brandSearch = new SearchFactory({
      objectType: m.Brand,
      roleTypes: [m.Brand.Name],
    });

    const modelSearch = new SearchFactory({
      objectType: m.Model,
      roleTypes: [m.Model.Name],
    });

    const manufacturerSearch = new SearchFactory({
      objectType: m.Organisation,
      predicates: [new Equals({ propertyType: m.Organisation.IsManufacturer, value: true })],
      roleTypes: [m.Organisation.PartyName],
    });

    const idSearch = new SearchFactory({
      objectType: m.ProductIdentification,
      roleTypes: [m.ProductIdentification.Identification],
    });

    const facilitySearch = new SearchFactory({
      objectType: m.Facility,
      roleTypes: [m.Facility.Name],
    });

    this.filterService.init(predicate,
      {
        supplier: { search: this.filtersService.suppliersFilter, display: (v: Organisation) => v && v.PartyName },
        manufacturer: { search: manufacturerSearch, display: (v: Organisation) => v && v.PartyName },
        brand: { search: brandSearch, display: (v: Brand) => v && v.Name },
        model: { search: modelSearch, display: (v: Model) => v && v.Name },
        kind: { search: kindSearch, display: (v: InventoryItemKind) => v && v.Name },
        type: { search: typeSearch, display: (v: ProductType) => v && v.Name },
        category: { search: categorySearch, display: (v: PartCategory) => v && v.Name },
        identification: { search: idSearch, display: (v: ProductIdentification) => v && v.Identification },
        inStock: { search: facilitySearch, display: (v: Facility) => v && v.Name },
        outOfStock: { search: facilitySearch, display: (v: Facility) => v && v.Name },
      });

    const sorter = new Sorter(
      {
        name: m.NonUnifiedPart.Name,
        partNo: m.NonUnifiedPart.ProductNumber,
        lastModifiedDate: m.UnifiedProduct.LastModifiedDate,
      }
    );

    this.subscription = combineLatest(this.refreshService.refresh$, this.filterService.filterFields$, this.table.sort$, this.table.pager$, this.internalOrganisationId.observable$)
      .pipe(
        scan(([previousRefresh, previousFilterFields], [refresh, filterFields, sort, pageEvent, internalOrganisationId]) => {
          return [
            refresh,
            filterFields,
            sort,
            (previousRefresh !== refresh || filterFields !== previousFilterFields) ? Object.assign({ pageIndex: 0 }, pageEvent) : pageEvent,
            internalOrganisationId,
          ];
        }, [, , , , ,]),
        switchMap(([, filterFields, sort, pageEvent]) => {

          const pulls = [
            this.fetcher.internalOrganisation,
            pull.NonUnifiedPart({
              predicate,
              sort: sorter.create(sort),
              include: {
                Brand: x,
                Model: x,
                ProductType: x,
                PrimaryPhoto: x,
                InventoryItemKind: x,
                InventoryItemsWherePart: {
                  Facility: x,
                },
                ProductIdentifications: {
                  ProductIdentificationType: x,
                },
              },
              parameters: this.filterService.parameters(filterFields),
              skip: pageEvent.pageIndex * pageEvent.pageSize,
              take: pageEvent.pageSize,
            }),
            pull.NonUnifiedPart({
              predicate,
              sort: sorter.create(sort),
              fetch: {
                PartCategoriesWherePart: {
                  include: {
                    Parts: x,
                    PrimaryAncestors: x
                  }
                },
              }
            }),
            pull.Singleton({
              object: this.singletonId.value,
              fetch: {
                NonUnifiedPartBarcodePrint: {
                  include: {
                    PrintDocument: {
                      Media: x
                    }
                  }
                }
              }
            }),
            pull.ProductIdentificationType(),
            pull.BasePrice(),
            pull.Person({
              object: this.userId.value,
              include: { Locale: x }
            }),
          ];

          return this.allors.context
            .load(new PullRequest({ pulls }));
        })
      )
      .subscribe((loaded) => {
        this.allors.context.reset();

        this.user = loaded.objects.Person as Person;
        this.internalOrganisation = loaded.objects.InternalOrganisation as Organisation;
        this.facilities = loaded.collections.Facilities as Facility[];
        this.nonUnifiedPartBarcodePrint = loaded.objects.NonUnifiedPartBarcodePrint as NonUnifiedPartBarcodePrint;

        this.parts = loaded.collections.NonUnifiedParts as NonUnifiedPart[];

        const inStockSearch = this.filterService.filterFields.find(v => v.definition.fieldName === 'In Stock');
        let facilitySearchId = inStockSearch?.value;
        if (inStockSearch !== undefined)
        {
          this.parts = this.parts.filter(v => {
            return v.InventoryItemsWherePart.filter((i: NonSerialisedInventoryItem) => i.Facility.id === inStockSearch.value && Number(i.QuantityOnHand) > 0).length > 0;
          });
        }

        const outOStockSearch = this.filterService.filterFields.find(v => v.definition.fieldName === 'Out Of Stock');
        if (facilitySearchId === undefined) {
          facilitySearchId = outOStockSearch?.value;
        }

        if (outOStockSearch !== undefined)
        {
          this.parts = this.parts.filter(v => {
            return v.InventoryItemsWherePart.filter((i: NonSerialisedInventoryItem) => i.Facility.id === outOStockSearch.value && Number(i.QuantityOnHand) === 0).length > 0;
          });
        }

        this.goodIdentificationTypes = loaded.collections.ProductIdentificationTypes as ProductIdentificationType[];
        const partCategories = loaded.collections.PartCategories as PartCategory[];
        const partNumberType = this.goodIdentificationTypes.find((v) => v.UniqueId === '5735191a-cdc4-4563-96ef-dddc7b969ca6');

        const partNumberByPart = this.parts.reduce((map, obj) => {
          map[obj.id] = obj.ProductIdentifications.filter(v => v.ProductIdentificationType === partNumberType).map(w => w.Identification);
          return map;
        }, {});

        this.table.total = loaded.values.NonUnifiedParts_total;

        this.table.data = this.parts.map((v) => {
          return {
            object: v,
            name: v.Name,
            partNo: v.ProductNumber,
            qoh: v.QuantityOnHand,
            localQoh: facilitySearchId && (v.InventoryItemsWherePart as NonSerialisedInventoryItem[]).find(i => i.Facility.id === facilitySearchId).QuantityOnHand,
            categories: partCategories.filter(w => w.Parts.includes(v)).map((w) => w.displayName).join(', '),
            brand: v.Brand ? v.Brand.Name : '',
            model: v.Model ? v.Model.Name : '',
            kind: v.InventoryItemKind.Name,
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

  public printBarcode(parts: any): void {
    const { context } = this.allors;

    this.nonUnifiedPartBarcodePrint.Parts = parts;
    this.nonUnifiedPartBarcodePrint.Facility = this.internalOrganisation.FacilitiesWhereOwner[0];
    this.nonUnifiedPartBarcodePrint.Locale = this.user.Locale;

    context
      .save()
      .subscribe(() => {

        const { pull, x } = this.metaService;

        const pulls = [
          pull.Singleton({
            object: this.singletonId.value,
            fetch: {
              NonUnifiedPartBarcodePrint: {
                include: {
                  PrintDocument: {
                    Media: x,
                  }
                }
              }
            }
          }),
        ];

        this.allors.context
          .load(new PullRequest({ pulls }))
          .subscribe((loaded) => {
            this.allors.context.reset();

            this.nonUnifiedPartBarcodePrint = loaded.objects.NonUnifiedPartBarcodePrint as NonUnifiedPartBarcodePrint;

            this.print.execute(this.nonUnifiedPartBarcodePrint);
            this.refreshService.refresh();
          });
      },
        this.saveService.errorHandler
      );
  }
}
