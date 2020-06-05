// <copyright file="PurchaseInvoiceItem.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using System;

    using Allors.Meta;

    public partial class PurchaseInvoiceItem
    {
        public static readonly TransitionalConfiguration[] StaticTransitionalConfigurations =
            {
                new TransitionalConfiguration(M.PurchaseInvoiceItem, M.PurchaseInvoiceItem.PurchaseInvoiceItemState),
            };

        public TransitionalConfiguration[] TransitionalConfigurations => StaticTransitionalConfigurations;

        public bool IsValid => !(this.PurchaseInvoiceItemState.IsCancelledByInvoice || this.PurchaseInvoiceItemState.IsRejected);

        internal bool IsDeletable =>
            !this.ExistOrderItemBillingsWhereInvoiceItem &&
            !this.ExistShipmentItemBillingsWhereInvoiceItem &&
            !this.ExistWorkEffortBillingsWhereInvoiceItem &&
            !this.ExistServiceEntryBillingsWhereInvoiceItem;

        public decimal PriceAdjustment => this.TotalSurcharge - this.TotalDiscount;

        public void BaseDelegateAccess(DelegatedAccessControlledObjectDelegateAccess method)
        {
            if (method.SecurityTokens == null)
            {
                method.SecurityTokens = this.SyncedInvoice?.SecurityTokens.ToArray();
            }

            if (method.DeniedPermissions == null)
            {
                method.DeniedPermissions = this.SyncedInvoice?.DeniedPermissions.ToArray();
            }
        }

        public void BaseOnBuild(ObjectOnBuild method)
        {
            if (!this.ExistPurchaseInvoiceItemState)
            {
                this.PurchaseInvoiceItemState = new PurchaseInvoiceItemStates(this.Strategy.Session).Received;
            }

            if (this.ExistPart && !this.ExistInvoiceItemType)
            {
                this.InvoiceItemType = new InvoiceItemTypes(this.Strategy.Session).PartItem;
            }
        }

        public void BaseOnPreDerive(ObjectOnPreDerive method)
        {
            var (iteration, changeSet, derivedObjects) = method;
            var invoice = this.PurchaseInvoiceWherePurchaseInvoiceItem;

            if (invoice != null && iteration.ChangeSet.Associations.Contains(this.Id))
            {
                iteration.AddDependency(invoice, this);
                iteration.Mark(invoice);
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
        }

        public void BaseOnDerivePrices()
        {
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
                var invoice = this.PurchaseInvoiceWherePurchaseInvoiceItem;
                if (this.ExistPart)
                {
                    this.UnitBasePrice = new SupplierOfferings(this.Strategy.Session).PurchasePrice(invoice.BilledFrom, invoice.InvoiceDate, this.Part);
                }
            }

            if (this.ExistUnitBasePrice)
            {
                var discountAdjustment = this.GetDiscountAdjustment();

                if (discountAdjustment != null)
                {
                    if (discountAdjustment.Percentage.HasValue)
                    {
                        this.UnitDiscount += Math.Round(this.UnitBasePrice * discountAdjustment.Percentage.Value / 100, 2);
                    }
                    else
                    {
                        if (discountAdjustment.Amount.HasValue)
                        {
                            this.UnitDiscount += discountAdjustment.Amount.Value;
                        }
                    }
                }

                var surchargeAdjustment = this.GetSurchargeAdjustment();

                if (surchargeAdjustment != null)
                {
                    if (surchargeAdjustment.Percentage.HasValue)
                    {
                        this.UnitSurcharge += Math.Round(this.UnitBasePrice * surchargeAdjustment.Percentage.Value / 100, 2);
                    }
                    else
                    {
                        if (surchargeAdjustment.Amount.HasValue)
                        {
                            this.UnitSurcharge += surchargeAdjustment.Amount.Value;
                        }
                    }
                }

                this.VatRegime = this.AssignedVatRegime ?? this.PurchaseInvoiceWherePurchaseInvoiceItem.VatRegime;
                this.VatRate = this.VatRegime?.VatRate;

                this.UnitVat = this.ExistVatRate ? this.UnitPrice * this.VatRate.Rate / 100 : 0;
                this.TotalBasePrice = this.UnitBasePrice * this.Quantity;
                this.TotalDiscount = this.UnitDiscount * this.Quantity;
                this.TotalSurcharge = this.UnitSurcharge * this.Quantity;
                this.UnitPrice = this.UnitBasePrice - this.UnitDiscount + this.UnitSurcharge;
                this.TotalVat = this.UnitVat * this.Quantity;
                this.TotalExVat = this.UnitPrice * this.Quantity;
                this.TotalIncVat = this.TotalExVat + this.TotalVat;
            }
        }

        public void CancelFromInvoice() => this.PurchaseInvoiceItemState = new PurchaseInvoiceItemStates(this.Strategy.Session).CancelledByinvoice;

        public void BaseDelete(DeletableDelete method)
        {
            if (this.PurchaseInvoiceWherePurchaseInvoiceItem.PurchaseInvoiceState.IsCreated)
            {
                this.PurchaseInvoiceWherePurchaseInvoiceItem.RemovePurchaseInvoiceItem(this);
                foreach (OrderItemBilling orderItemBilling in this.OrderItemBillingsWhereInvoiceItem)
                {
                    orderItemBilling.Delete();
                }
            }
        }

        public void BaseReject(PurchaseInvoiceItemReject method) => this.PurchaseInvoiceItemState = new PurchaseInvoiceItemStates(this.Strategy.Session).Rejected;

        public void Sync(Invoice invoice) => this.SyncedInvoice = invoice;

        private SurchargeAdjustment GetSurchargeAdjustment()
        {
            var surchargeAdjustment = this.ExistSurchargeAdjustment ? this.SurchargeAdjustment : null;
            if (surchargeAdjustment == null && this.ExistPurchaseInvoiceWherePurchaseInvoiceItem)
            {
                surchargeAdjustment = this.PurchaseInvoiceWherePurchaseInvoiceItem.ExistSurchargeAdjustment ? this.PurchaseInvoiceWherePurchaseInvoiceItem.SurchargeAdjustment : null;
            }

            return surchargeAdjustment;
        }

        private DiscountAdjustment GetDiscountAdjustment()
        {
            var discountAdjustment = this.ExistDiscountAdjustment ? this.DiscountAdjustment : null;
            if (discountAdjustment == null && this.ExistPurchaseInvoiceWherePurchaseInvoiceItem)
            {
                discountAdjustment = this.PurchaseInvoiceWherePurchaseInvoiceItem.ExistDiscountAdjustment ? this.PurchaseInvoiceWherePurchaseInvoiceItem.DiscountAdjustment : null;
            }

            return discountAdjustment;
        }
    }
}
