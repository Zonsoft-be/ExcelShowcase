// <copyright file="RequestItem.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

using Resources;

namespace Allors.Domain
{
    using Allors.Meta;

    public partial class RequestItem
    {
        public static readonly TransitionalConfiguration[] StaticTransitionalConfigurations =
            {
                new TransitionalConfiguration(M.RequestItem, M.RequestItem.RequestItemState),
            };

        public TransitionalConfiguration[] TransitionalConfigurations => StaticTransitionalConfigurations;

        internal bool IsDeletable =>
            (this.RequestItemState.Equals(new RequestItemStates(this.Strategy.Session).Draft)
                || this.RequestItemState.Equals(new RequestItemStates(this.Strategy.Session).Submitted)
                || this.RequestItemState.Equals(new RequestItemStates(this.Strategy.Session).Cancelled))
            && !this.ExistQuoteItemsWhereRequestItem;

        public void BaseDelegateAccess(DelegatedAccessControlledObjectDelegateAccess method)
        {
            if (method.SecurityTokens == null)
            {
                method.SecurityTokens = this.SyncedRequest?.SecurityTokens.ToArray();
            }

            if (method.DeniedPermissions == null)
            {
                method.DeniedPermissions = this.SyncedRequest?.DeniedPermissions.ToArray();
            }
        }

        public void BaseOnBuild(ObjectOnBuild method)
        {
            if (!this.ExistRequestItemState)
            {
                this.RequestItemState = new RequestItemStates(this.Strategy.Session).Submitted;
            }
        }

        public void BaseOnDerive(ObjectOnDerive method)
        {
            var derivation = method.Derivation;

            derivation.Validation.AssertAtLeastOne(this, M.RequestItem.Product, M.RequestItem.ProductFeature, M.RequestItem.SerialisedItem, M.RequestItem.Description, M.RequestItem.NeededSkill, M.RequestItem.Deliverable);
            derivation.Validation.AssertExistsAtMostOne(this, M.RequestItem.Product, M.RequestItem.ProductFeature, M.RequestItem.Description, M.RequestItem.NeededSkill, M.RequestItem.Deliverable);
            derivation.Validation.AssertExistsAtMostOne(this, M.RequestItem.SerialisedItem, M.RequestItem.ProductFeature, M.RequestItem.Description, M.RequestItem.NeededSkill, M.RequestItem.Deliverable);

            if (!this.ExistUnitOfMeasure)
            {
                this.UnitOfMeasure = new UnitsOfMeasure(this.Strategy.Session).Piece;
            }

            if (this.ExistRequestWhereRequestItem && new RequestStates(this.Strategy.Session).Cancelled.Equals(this.RequestWhereRequestItem.RequestState))
            {
                this.Cancel();
            }

            if (this.ExistSerialisedItem && this.Quantity != 1)
            {
                derivation.Validation.AddError(this, this.Meta.Quantity, ErrorMessages.SerializedItemQuantity);
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

        public void BaseCancel(RequestItemCancel method) => this.RequestItemState = new RequestItemStates(this.Strategy.Session).Cancelled;

        public void Sync(Request request) => this.SyncedRequest = request;
    }
}
