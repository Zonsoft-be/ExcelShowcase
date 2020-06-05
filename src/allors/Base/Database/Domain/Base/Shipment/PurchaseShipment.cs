// <copyright file="PurchaseShipment.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System.Linq;
    using Allors.Meta;

    public partial class PurchaseShipment
    {
        public static readonly TransitionalConfiguration[] StaticTransitionalConfigurations =
            {
                new TransitionalConfiguration(M.PurchaseShipment, M.PurchaseShipment.ShipmentState),
            };

        public TransitionalConfiguration[] TransitionalConfigurations => StaticTransitionalConfigurations;

        public void BaseOnBuild(ObjectOnBuild method)
        {
            if (!this.ExistShipToParty)
            {
                var internalOrganisations = new Organisations(this.Strategy.Session).InternalOrganisations();
                if (internalOrganisations.Length == 1)
                {
                    this.ShipToParty = internalOrganisations.First();
                }
            }

            if (!this.ExistShipmentState)
            {
                this.ShipmentState = new ShipmentStates(this.Strategy.Session).Created;
            }

            if (!this.ExistEstimatedArrivalDate)
            {
                this.EstimatedArrivalDate = this.Session().Now().Date;
            }
        }

        public void BaseOnPreDerive(ObjectOnPreDerive method)
        {
            var (iteration, changeSet, derivedObjects) = method;

            if (iteration.ChangeSet.Associations.Contains(this.Id))
            {
                foreach (ShipmentItem shipmentItem in this.ShipmentItems)
                {
                    if (shipmentItem.ExistShipmentReceiptWhereShipmentItem
                        && shipmentItem.ShipmentReceiptWhereShipmentItem.ExistInventoryItem)
                    {
                        iteration.AddDependency(shipmentItem.ShipmentReceiptWhereShipmentItem.InventoryItem, this);
                        iteration.Mark(shipmentItem.ShipmentReceiptWhereShipmentItem.InventoryItem);
                    }

                    foreach (OrderShipment orderShipment in shipmentItem.OrderShipmentsWhereShipmentItem)
                    {
                        iteration.AddDependency(orderShipment.OrderItem, this);
                        iteration.Mark(orderShipment.OrderItem);
                    }
                }
            }
        }

        public void BaseOnDerive(ObjectOnDerive method)
        {
            var derivation = method.Derivation;

            derivation.Validation.AssertExists(this, this.Meta.ShipFromParty);

            var internalOrganisations = new Organisations(this.Strategy.Session).Extent().Where(v => Equals(v.IsInternalOrganisation, true)).ToArray();
            var shipToParty = this.ShipToParty as InternalOrganisation;
            if (!this.ExistShipToParty && internalOrganisations.Count() == 1)
            {
                this.ShipToParty = internalOrganisations.First();
                shipToParty = internalOrganisations.First();
            }

            this.ShipToAddress = this.ShipToAddress ?? this.ShipToParty?.ShippingAddress ?? this.ShipToParty?.GeneralCorrespondence as PostalAddress;

            if (!this.ExistShipToFacility && shipToParty != null && shipToParty.StoresWhereInternalOrganisation.Count == 1)
            {
                this.ShipToFacility = shipToParty.StoresWhereInternalOrganisation.Single().DefaultFacility;
            }

            if (!this.ExistShipmentNumber && shipToParty != null)
            {
                this.ShipmentNumber = shipToParty.NextShipmentNumber(this.Strategy.Session.Now().Year);
            }

            if (!this.ExistShipFromAddress && this.ExistShipFromParty)
            {
                this.ShipFromAddress = this.ShipFromParty.ShippingAddress;
            }

            if (this.ShipmentItems.Any()
                && this.ShipmentItems.All(v => v.ExistShipmentReceiptWhereShipmentItem
                &&  v.ShipmentReceiptWhereShipmentItem.QuantityAccepted.Equals(v.ShipmentReceiptWhereShipmentItem.OrderItem?.QuantityOrdered))
                && this.ShipmentItems.All(v => v.ShipmentItemState.Equals(new ShipmentItemStates(this.strategy.Session).Received)))
            {
                this.ShipmentState = new ShipmentStates(this.Strategy.Session).Received;
            }

            this.Sync(this.Session());
        }

        public void BaseReceive(PurchaseShipmentReceive method)
        {
            this.ShipmentState = new ShipmentStates(this.Strategy.Session).Received;
            this.EstimatedArrivalDate = this.Session().Now().Date;

            foreach (ShipmentItem shipmentItem in this.ShipmentItems)
            {
                shipmentItem.ShipmentItemState = new ShipmentItemStates(this.Session()).Received;

                if (shipmentItem.Part.InventoryItemKind.Serialised)
                {
                    new InventoryItemTransactionBuilder(this.Session())
                        .WithSerialisedItem(shipmentItem.SerialisedItem)
                        .WithUnitOfMeasure(shipmentItem.Part.UnitOfMeasure)
                        .WithFacility(shipmentItem.ShipmentWhereShipmentItem.ShipToFacility)
                        .WithReason(new InventoryTransactionReasons(this.Strategy.Session).IncomingShipment)
                        .WithShipmentItem(shipmentItem)
                        .WithSerialisedInventoryItemState(new SerialisedInventoryItemStates(this.Session()).Good)
                        .WithQuantity(1)
                        .Build();
                }
                else
                {
                    new InventoryItemTransactionBuilder(this.Session())
                        .WithPart(shipmentItem.Part)
                        .WithUnitOfMeasure(shipmentItem.Part.UnitOfMeasure)
                        .WithFacility(shipmentItem.ShipmentWhereShipmentItem.ShipToFacility)
                        .WithReason(new InventoryTransactionReasons(this.Strategy.Session).IncomingShipment)
                        .WithShipmentItem(shipmentItem)
                        .WithQuantity(shipmentItem.Quantity)
                        .WithCost(shipmentItem.UnitPurchasePrice)
                        .Build();
                }
            }
        }

        private void Sync(ISession session)
        {
            // session.Prefetch(this.SyncPrefetch, this);
            foreach (ShipmentItem shipmentItem in this.ShipmentItems)
            {
                shipmentItem.Sync(this);
            }
        }
    }
}
