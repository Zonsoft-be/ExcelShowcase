import { Component, Self } from '@angular/core';
import { PanelService, NavigationService, MetaService, Invoked, RefreshService, Action } from '../../../../../../angular';
import { PurchaseOrder, PurchaseInvoice, Shipment } from '../../../../../../domain';
import { Meta } from '../../../../../../meta';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PrintService, SaveService } from '../../../../../../material';

@Component({
  // tslint:disable-next-line:component-selector
  selector: 'purchaseorder-overview-summary',
  templateUrl: './purchaseorder-overview-summary.component.html',
  providers: [PanelService]
})
export class PurchaseOrderOverviewSummaryComponent {

  m: Meta;

  order: PurchaseOrder;
  purchaseInvoices: PurchaseInvoice[] = [];

  print: Action;
  shipments: Shipment[];

  constructor(
    @Self() public panel: PanelService,
    public metaService: MetaService,
    public navigation: NavigationService,
    public printService: PrintService,
    private saveService: SaveService,

    public refreshService: RefreshService,
    public snackBar: MatSnackBar) {

    this.m = this.metaService.m;

    this.print = printService.print();

    panel.name = 'summary';

    const puchaseOrderPullName = `${panel.name}_${this.m.PurchaseOrder.name}`;
    const shipmentPullName = `${panel.name}_${this.m.Shipment.name}`;
    const purchaseInvoicePullName = `${panel.name}_${this.m.PurchaseInvoice.name}`;

    panel.onPull = (pulls) => {
      const { m, pull, x } = this.metaService;

      pulls.push(

        pull.PurchaseOrder({
          name: puchaseOrderPullName,
          object: this.panel.manager.id,
          include: {
            TakenViaSupplier: x,
            PurchaseOrderState: x,
            PurchaseOrderShipmentState: x,
            PurchaseOrderPaymentState: x,
            CreatedBy: x,
            LastModifiedBy: x,
            PrintDocument: {
              Media: x
            },
          }
        }),
        pull.PurchaseOrder({
          name: shipmentPullName,
          object: this.panel.manager.id,
          fetch: {
            PurchaseOrderItems: {
              OrderShipmentsWhereOrderItem: {
                ShipmentItem: {
                  ShipmentWhereShipmentItem: x
                }
              }
            }
          }
        }),
        pull.PurchaseOrder({
          name: purchaseInvoicePullName,
          object: this.panel.manager.id,
          fetch: { PurchaseInvoicesWherePurchaseOrder: x }
        }),
      );
    };

    panel.onPulled = (loaded) => {
      this.order = loaded.objects[puchaseOrderPullName] as PurchaseOrder;
      this.purchaseInvoices = loaded.collections[purchaseInvoicePullName] as PurchaseInvoice[];
      this.shipments = loaded.collections[shipmentPullName] as Shipment[];
    };
  }

  public approve(): void {

    this.panel.manager.context.invoke(this.order.Approve)
      .subscribe((invoked: Invoked) => {
        this.refreshService.refresh();
        this.snackBar.open('Successfully approved.', 'close', { duration: 5000 });
      },
        this.saveService.errorHandler);
  }

  public cancel(): void {

    this.panel.manager.context.invoke(this.order.Cancel)
      .subscribe((invoked: Invoked) => {
        this.refreshService.refresh();
        this.snackBar.open('Successfully cancelled.', 'close', { duration: 5000 });
      },
        this.saveService.errorHandler);
  }

  public reject(): void {

    this.panel.manager.context.invoke(this.order.Reject)
      .subscribe((invoked: Invoked) => {
        this.refreshService.refresh();
        this.snackBar.open('Successfully rejected.', 'close', { duration: 5000 });
      },
        this.saveService.errorHandler);
  }

  public hold(): void {

    this.panel.manager.context.invoke(this.order.Hold)
      .subscribe((invoked: Invoked) => {
        this.refreshService.refresh();
        this.snackBar.open('Successfully put on hold.', 'close', { duration: 5000 });
      },
        this.saveService.errorHandler);
  }

  public continue(): void {

    this.panel.manager.context.invoke(this.order.Continue)
      .subscribe((invoked: Invoked) => {
        this.refreshService.refresh();
        this.snackBar.open('Successfully removed from hold.', 'close', { duration: 5000 });
      },
        this.saveService.errorHandler);
  }

  public setReadyForProcessing(): void {

    this.panel.manager.context.invoke(this.order.SetReadyForProcessing)
      .subscribe((invoked: Invoked) => {
        this.refreshService.refresh();
        this.snackBar.open('Successfully set ready for processing.', 'close', { duration: 5000 });
      },
        this.saveService.errorHandler);
  }

  public reopen(): void {

    this.panel.manager.context.invoke(this.order.Reopen)
      .subscribe((invoked: Invoked) => {
        this.refreshService.refresh();
        this.snackBar.open('Successfully reopened.', 'close', { duration: 5000 });
      },
        this.saveService.errorHandler);
  }

  public revise(): void {

    this.panel.manager.context.invoke(this.order.Revise)
      .subscribe((invoked: Invoked) => {
        this.refreshService.refresh();
        this.snackBar.open('Successfully revised.', 'close', { duration: 5000 });
      },
        this.saveService.errorHandler);
  }

  public send(): void {

    this.panel.manager.context.invoke(this.order.Send)
      .subscribe((invoked: Invoked) => {
        this.refreshService.refresh();
        this.snackBar.open('Successfully send.', 'close', { duration: 5000 });
      },
        this.saveService.errorHandler);
  }

  public invoice(): void {

    this.panel.manager.context.invoke(this.order.Invoice)
      .subscribe((invoked: Invoked) => {
        this.refreshService.refresh();
        this.snackBar.open('Successfully created purchase invoice', 'close', { duration: 5000 });
      },
        this.saveService.errorHandler);
  }

  public quickReceive(): void {

    this.panel.manager.context.invoke(this.order.QuickReceive)
      .subscribe((invoked: Invoked) => {
        this.panel.toggle();
        this.snackBar.open('inventory created for all items', 'close', { duration: 5000 });
        this.refreshService.refresh();
      },
        this.saveService.errorHandler);
  }
}
