// <copyright file="PurchaseOrderItem.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using Allors.Meta;

    public partial class PurchaseOrderItem
    {
        #region Transitional

        public static readonly TransitionalConfiguration[] StaticTransitionalConfigurations =
            {
                new TransitionalConfiguration(M.PurchaseOrderItem, M.PurchaseOrderItem.PurchaseOrderItemState),
                new TransitionalConfiguration(M.PurchaseOrderItem, M.PurchaseOrderItem.PurchaseOrderItemShipmentState),
                new TransitionalConfiguration(M.PurchaseOrderItem, M.PurchaseOrderItem.PurchaseOrderItemPaymentState),
            };

        public TransitionalConfiguration[] TransitionalConfigurations => StaticTransitionalConfigurations;

        #endregion Transitional

        public bool IsValid => !(this.PurchaseOrderItemState.IsCancelled || this.PurchaseOrderItemState.IsRejected);

        internal bool IsDeletable =>
            (this.PurchaseOrderItemState.Equals(new PurchaseOrderItemStates(this.Strategy.Session).Created)
                || this.PurchaseOrderItemState.Equals(new PurchaseOrderItemStates(this.Strategy.Session).Cancelled)
                || this.PurchaseOrderItemState.Equals(new PurchaseOrderItemStates(this.Strategy.Session).Rejected))
            && !this.ExistOrderItemBillingsWhereOrderItem
            && !this.ExistOrderShipmentsWhereOrderItem
            && !this.ExistOrderRequirementCommitmentsWhereOrderItem
            && !this.ExistWorkEffortsWhereOrderItemFulfillment;

        public string SupplierReference
        {
            get
            {
                Extent<SupplierOffering> offerings = null;

                if (this.ExistPart)
                {
                    offerings = this.Part.SupplierOfferingsWherePart;
                }

                if (offerings != null)
                {
                    offerings.Filter.AddEquals(M.SupplierOffering.Supplier, this.PurchaseOrderWherePurchaseOrderItem.TakenViaSupplier);
                    foreach (SupplierOffering offering in offerings)
                    {
                        if (offering.FromDate <= this.PurchaseOrderWherePurchaseOrderItem.OrderDate &&
                            (!offering.ExistThroughDate || offering.ThroughDate >= this.PurchaseOrderWherePurchaseOrderItem.OrderDate))
                        {
                            return offering.SupplierProductId;
                        }
                    }
                }

                return string.Empty;
            }
        }

        public void BaseOnBuild(ObjectOnBuild method)
        {
            if (!this.ExistPurchaseOrderItemState)
            {
                this.PurchaseOrderItemState = new PurchaseOrderItemStates(this.Strategy.Session).Created;
            }

            if (!this.ExistPurchaseOrderItemShipmentState)
            {
                this.PurchaseOrderItemShipmentState = new PurchaseOrderItemShipmentStates(this.Strategy.Session).NotReceived;
            }

            if (!this.ExistPurchaseOrderItemPaymentState)
            {
                this.PurchaseOrderItemPaymentState = new PurchaseOrderItemPaymentStates(this.Strategy.Session).NotPaid;
            }
        }

        public void BaseOnPreDerive(ObjectOnPreDerive method)
        {
            var (iteration, changeSet, derivedObjects) = method;

            if (iteration.IsMarked(this) || changeSet.IsCreated(this) || changeSet.HasChangedRoles(this))
            {
                iteration.AddDependency(this.PurchaseOrderWherePurchaseOrderItem, this);
                iteration.Mark(this.PurchaseOrderWherePurchaseOrderItem);

                iteration.AddDependency(this.SerialisedItem, this);
                iteration.Mark(this.SerialisedItem);
            }
        }

        public void BaseOnDerive(ObjectOnDerive method)
        {
            var derivation = method.Derivation;

            // States
            var states = new PurchaseOrderItemStates(this.Session());

            var purchaseOrderState = this.PurchaseOrderWherePurchaseOrderItem.PurchaseOrderState;
            if (purchaseOrderState.IsCreated
                && !this.PurchaseOrderItemState.IsCancelled
                && !this.PurchaseOrderItemState.IsRejected)
            {
                this.PurchaseOrderItemState = states.Created;
            }

            if (purchaseOrderState.IsInProcess &&
                (this.PurchaseOrderItemState.IsCreated || this.PurchaseOrderItemState.IsOnHold))
            {
                this.PurchaseOrderItemState = states.InProcess;
            }

            if (purchaseOrderState.IsOnHold && this.PurchaseOrderItemState.IsInProcess)
            {
                this.PurchaseOrderItemState = states.OnHold;
            }

            if (purchaseOrderState.IsSent && this.PurchaseOrderItemState.IsInProcess)
            {
                this.PurchaseOrderItemState = states.Sent;
            }

            if (this.IsValid && purchaseOrderState.IsFinished)
            {
                this.PurchaseOrderItemState = states.Finished;
            }

            if (this.IsValid && purchaseOrderState.IsCancelled)
            {
                this.PurchaseOrderItemState = states.Cancelled;
            }

            if (this.IsValid && purchaseOrderState.IsRejected)
            {
                this.PurchaseOrderItemState = states.Rejected;
            }

            if (this.IsValid)
            {
                if (this.AssignedDeliveryDate.HasValue)
                {
                    this.DeliveryDate = this.AssignedDeliveryDate.Value;
                }
                else if (this.PurchaseOrderWherePurchaseOrderItem.DeliveryDate.HasValue)
                {
                    this.DeliveryDate = this.PurchaseOrderWherePurchaseOrderItem.DeliveryDate.Value;
                }

                this.UnitBasePrice = 0;
                this.UnitDiscount = 0;
                this.UnitSurcharge = 0;

                if (this.AssignedUnitPrice.HasValue)
                {
                    this.UnitBasePrice = this.AssignedUnitPrice.Value;
                    this.UnitPrice = this.AssignedUnitPrice.Value;
                }
                else
                {
                    var order = this.PurchaseOrderWherePurchaseOrderItem;
                    this.UnitBasePrice = new SupplierOfferings(this.Strategy.Session).PurchasePrice(order.TakenViaSupplier, order.OrderDate, this.Part);
                }

                this.VatRegime = this.AssignedVatRegime ?? this.PurchaseOrderWherePurchaseOrderItem.VatRegime;
                this.VatRate = this.VatRegime?.VatRate;

                this.UnitVat = this.ExistVatRate ? this.UnitPrice * this.VatRate.Rate / 100 : 0;
                this.TotalBasePrice = this.UnitBasePrice * this.QuantityOrdered;
                this.TotalDiscount = this.UnitDiscount * this.QuantityOrdered;
                this.TotalSurcharge = this.UnitSurcharge * this.QuantityOrdered;
                this.UnitPrice = this.UnitBasePrice - this.UnitDiscount + this.UnitSurcharge;
                this.TotalVat = this.UnitVat * this.QuantityOrdered;
                this.TotalExVat = this.UnitPrice * this.QuantityOrdered;
                this.TotalIncVat = this.TotalExVat + this.TotalVat;
            }

            if (this.ExistPart && this.Part.InventoryItemKind.Serialised)
            {
                derivation.Validation.AssertAtLeastOne(this, M.PurchaseOrderItem.SerialisedItem, M.PurchaseOrderItem.SerialNumber);
                derivation.Validation.AssertExistsAtMostOne(this, M.PurchaseOrderItem.SerialisedItem, M.PurchaseOrderItem.SerialNumber);
            }

            var purchaseOrderItemShipmentStates = new PurchaseOrderItemShipmentStates(derivation.Session);
            var purchaseOrderItemPaymentStates = new PurchaseOrderItemPaymentStates(derivation.Session);
            var purchaseOrderItemStates = new PurchaseOrderItemStates(derivation.Session);

            if (this.IsValid)
            {
                // ShipmentState
                if (this.ExistPart)
                {
                    var quantityReceived = 0M;
                    foreach (ShipmentReceipt shipmentReceipt in this.ShipmentReceiptsWhereOrderItem)
                    {
                        quantityReceived += shipmentReceipt.QuantityAccepted;
                    }

                    this.QuantityReceived = quantityReceived;
                }

                if (this.QuantityReceived == 0)
                {
                    this.PurchaseOrderItemShipmentState = new PurchaseOrderItemShipmentStates(this.Strategy.Session).NotReceived;
                }
                else
                {
                    this.PurchaseOrderItemShipmentState = this.QuantityReceived < this.QuantityOrdered ?
                        purchaseOrderItemShipmentStates.PartiallyReceived :
                        purchaseOrderItemShipmentStates.Received;
                }

                // PaymentState
                var orderBilling = this.OrderItemBillingsWhereOrderItem.Select(v => v.InvoiceItem).OfType<PurchaseInvoiceItem>().ToArray();

                if (orderBilling.Any())
                {
                    if (orderBilling.All(v => v.PurchaseInvoiceWherePurchaseInvoiceItem.PurchaseInvoiceState.IsPaid))
                    {
                        this.PurchaseOrderItemPaymentState = purchaseOrderItemPaymentStates.Paid;
                    }
                    else if (orderBilling.All(v => !v.PurchaseInvoiceWherePurchaseInvoiceItem.PurchaseInvoiceState.IsPaid))
                    {
                        this.PurchaseOrderItemPaymentState = purchaseOrderItemPaymentStates.NotPaid;
                    }
                    else
                    {
                        this.PurchaseOrderItemPaymentState = purchaseOrderItemPaymentStates.PartiallyPaid;
                    }
                }

                // PurchaseOrderItem States
                if (this.PurchaseOrderItemShipmentState.IsReceived)
                {
                    this.PurchaseOrderItemState = purchaseOrderItemStates.Completed;
                }

                if (this.PurchaseOrderItemState.IsCompleted && this.PurchaseOrderItemPaymentState.IsPaid)
                {
                    this.PurchaseOrderItemState = purchaseOrderItemStates.Finished;
                }
            }

            if (this.PurchaseOrderItemState.Equals(states.InProcess) ||
                this.PurchaseOrderItemState.Equals(states.Cancelled) ||
                this.PurchaseOrderItemState.Equals(states.Rejected))
            {
                NonSerialisedInventoryItem inventoryItem = null;

                if (this.ExistPart)
                {
                    var inventoryItems = this.Part.InventoryItemsWherePart;
                    inventoryItems.Filter.AddEquals(M.InventoryItem.Facility, this.PurchaseOrderWherePurchaseOrderItem.Facility);
                    inventoryItem = inventoryItems.First as NonSerialisedInventoryItem;
                }

                if (this.PurchaseOrderItemState.Equals(new PurchaseOrderItemStates(this.Strategy.Session).InProcess))
                {
                    if (!this.ExistPreviousQuantity || !this.QuantityOrdered.Equals(this.PreviousQuantity))
                    {
                        // TODO: Remove OnDerive
                        inventoryItem?.OnDerive(x => x.WithDerivation(derivation));
                    }
                }

                if (this.PurchaseOrderItemState.Equals(new PurchaseOrderItemStates(this.Strategy.Session).Cancelled) ||
                    this.PurchaseOrderItemState.Equals(new PurchaseOrderItemStates(this.Strategy.Session).Rejected))
                {
                    // TODO: Remove OnDerive
                    inventoryItem?.OnDerive(x => x.WithDerivation(derivation));
                }
            }

            if ((this.IsValid && !this.ExistOrderItemBillingsWhereOrderItem &&
                this.PurchaseOrderItemShipmentState.IsReceived) || this.PurchaseOrderItemShipmentState.IsPartiallyReceived || (!this.ExistPart && this.QuantityReceived == 1))
            {
                this.CanInvoice = true;
            }
            else
            {
                this.CanInvoice = false;
            }
        }

        public void BaseOnPostDerive(ObjectOnPostDerive method)
        {
            var deletePermission = new Permissions(this.Strategy.Session).Get(this.Meta.ObjectType, this.Meta.Delete, Operations.Execute);
            if (this.IsDeletable)
            {
                this.RemoveDeniedPermission(deletePermission);
            }
            else
            {
                this.AddDeniedPermission(deletePermission);
            }

            if (!this.PurchaseOrderItemShipmentState.IsNotReceived)
            {
                var deniablePermissionByOperandTypeId = new Dictionary<Guid, Permission>();

                foreach (Permission permission in this.Session().Extent<Permission>())
                {
                    if (permission.ConcreteClassPointer == this.strategy.Class.Id
                        && (permission.Operation == Operations.Write || permission.Operation == Operations.Execute))
                    {
                        deniablePermissionByOperandTypeId.Add(permission.OperandTypePointer, permission);
                    }
                }

                foreach (var keyValuePair in deniablePermissionByOperandTypeId)
                {
                    this.AddDeniedPermission(keyValuePair.Value);
                }
            }
        }

        public void BaseDelete(PurchaseOrderItemDelete method)
        {
            if (this.ExistSerialisedItem)
            {
                this.SerialisedItem.DerivationTrigger = Guid.NewGuid();
            }
        }

        public void BaseApprove(OrderItemApprove method) => this.PurchaseOrderItemState = new PurchaseOrderItemStates(this.Strategy.Session).InProcess;

        public void BaseQuickReceive(PurchaseOrderItemQuickReceive method)
        {
            var session = this.Session();
            var order = this.PurchaseOrderWherePurchaseOrderItem;

            if (this.ExistPart)
            {
                var shipment = new PurchaseShipmentBuilder(session)
                    .WithShipmentMethod(new ShipmentMethods(session).Ground)
                    .WithShipToParty(order.OrderedBy)
                    .WithShipFromParty(order.TakenViaSupplier)
                    .WithShipToFacility(order.Facility)
                    .Build();

                var shipmentItem = new ShipmentItemBuilder(session)
                    .WithPart(this.Part)
                    .WithQuantity(this.QuantityOrdered)
                    .WithUnitPurchasePrice(this.UnitPrice)
                    .WithContentsDescription($"{this.QuantityOrdered} * {this.Part.Name}")
                    .Build();

                shipment.AddShipmentItem(shipmentItem);

                new OrderShipmentBuilder(session)
                    .WithOrderItem(this)
                    .WithShipmentItem(shipmentItem)
                    .WithQuantity(this.QuantityOrdered)
                    .Build();

                new ShipmentReceiptBuilder(session)
                    .WithQuantityAccepted(this.QuantityOrdered)
                    .WithShipmentItem(shipmentItem)
                    .WithOrderItem(this)
                    .Build();

                if (this.Part.InventoryItemKind.Serialised)
                {
                    var serialisedItem = this.SerialisedItem;
                    if (!this.ExistSerialisedItem)
                    {
                        serialisedItem = new SerialisedItemBuilder(session)
                            .WithSerialNumber(this.SerialNumber)
                            .Build();

                        this.Part.AddSerialisedItem(serialisedItem);
                    }

                    shipmentItem.SerialisedItem = serialisedItem;

                    // HACK: DerivedRoles (WIP)
                    var serialisedItemDeriveRoles = (SerialisedItemDerivedRoles)serialisedItem;
                    serialisedItemDeriveRoles.PurchaseOrder = order;
                    serialisedItemDeriveRoles.SuppliedBy = order.TakenViaSupplier;
                    serialisedItem.RemoveAssignedPurchasePrice();
                    serialisedItemDeriveRoles.PurchasePrice = this.TotalExVat;

                    serialisedItem.OwnedBy = order.OrderedBy;
                    serialisedItem.Buyer = order.OrderedBy;
                    serialisedItem.SerialisedItemAvailability = new SerialisedItemAvailabilities(this.Session()).Available;

                    var inventoryItem = serialisedItem.SerialisedInventoryItemsWhereSerialisedItem
                        .FirstOrDefault(v => v.SerialisedItem.Equals(serialisedItem) && v.Facility.Equals(order.Facility));

                    if (inventoryItem == null)
                    {
                        new SerialisedInventoryItemBuilder(this.Session())
                            .WithSerialisedItem(serialisedItem)
                            .WithSerialisedInventoryItemState(new SerialisedInventoryItemStates(this.Session()).Good)
                            .WithPart(this.Part)
                            .WithUnitOfMeasure(new UnitsOfMeasure(this.Session()).Piece)
                            .WithFacility(order.Facility)
                            .Build();
                    }
                }

                if (shipment.ShipToParty is InternalOrganisation internalOrganisation)
                {
                    if (internalOrganisation.IsAutomaticallyReceived)
                    {
                        shipment.Receive();
                    }
                }
            }
            else
            {
                this.QuantityReceived = 1;
            }
        }

        public void BaseCancel(OrderItemCancel method) => this.PurchaseOrderItemState = new PurchaseOrderItemStates(this.Strategy.Session).Cancelled;

        public void BaseReject(OrderItemReject method) => this.PurchaseOrderItemState = new PurchaseOrderItemStates(this.Strategy.Session).Rejected;

        public void BaseReopen(OrderItemReopen method) => this.PurchaseOrderItemState = this.PreviousPurchaseOrderItemState;

    }
}
