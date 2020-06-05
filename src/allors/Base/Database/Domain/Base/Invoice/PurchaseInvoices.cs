// <copyright file="PurchaseInvoices.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using Allors.Meta;

    public partial class PurchaseInvoices
    {
        protected override void BasePrepare(Setup setup) => setup.AddDependency(this.ObjectType, M.PurchaseInvoiceState);

        protected override void BaseSecure(Security config)
        {
            var created = new PurchaseInvoiceStates(this.Session).Created;
            var awaitingApproval = new PurchaseInvoiceStates(this.Session).AwaitingApproval;
            var received = new PurchaseInvoiceStates(this.Session).Received;
            var notPaid = new PurchaseInvoiceStates(this.Session).NotPaid;
            var partiallyPaid = new PurchaseInvoiceStates(this.Session).PartiallyPaid;
            var paid = new PurchaseInvoiceStates(this.Session).Paid;
            var cancelled = new PurchaseInvoiceStates(this.Session).Cancelled;
            var rejected = new PurchaseInvoiceStates(this.Session).Rejected;

            var approve = this.Meta.Approve;
            var reject = this.Meta.Reject;
            var confirm = this.Meta.Confirm;
            var cancel = this.Meta.Cancel;
            var reopen = this.Meta.Reopen;
            var delete = this.Meta.Delete;
            var setPaid = this.Meta.SetPaid;

            config.Deny(this.ObjectType, created, approve, reject, reopen, setPaid);
            config.Deny(this.ObjectType, cancelled, approve, reject, confirm, cancel, setPaid, delete);
            config.Deny(this.ObjectType, rejected, approve, reject, confirm, cancel, setPaid, delete);
            config.Deny(this.ObjectType, awaitingApproval, confirm, cancel, reopen, setPaid, delete);
            config.Deny(this.ObjectType, notPaid, approve, confirm, reopen, delete);
            config.Deny(this.ObjectType, partiallyPaid, approve, confirm, reopen, delete);
            config.Deny(this.ObjectType, received, delete);

            config.Deny(this.ObjectType, notPaid, Operations.Write);
            config.Deny(this.ObjectType, partiallyPaid, Operations.Write);
            config.Deny(this.ObjectType, paid, Operations.Write, Operations.Execute);
            config.Deny(this.ObjectType, cancelled, Operations.Write);
            config.Deny(this.ObjectType, rejected, Operations.Write, Operations.Execute);
        }
    }
}
