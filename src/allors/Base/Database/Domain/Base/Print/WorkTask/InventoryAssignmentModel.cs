// <copyright file="InventoryAssignmentModel.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain.Print.WorkTaskModel
{
    using System;

    public class InventoryAssignmentModel
    {
        public InventoryAssignmentModel(WorkEffortInventoryAssignment assignment)
        {
            this.PartId = assignment.InventoryItem.Part?.PartIdentification();
            this.PartName = assignment.InventoryItem.Part?.Name;
            this.Quantity = assignment.BillableQuantity ?? assignment.Quantity;
            this.UnitOfMeasure = assignment.InventoryItem.Part?.UnitOfMeasure?.Abbreviation?.ToUpperInvariant() ??
                                 assignment.InventoryItem.Part?.UnitOfMeasure?.Name?.ToUpperInvariant() ??
                                 "EA";
            this.UnitSellingPrice = assignment.UnitSellingPrice;
            this.SellingPrice = Math.Round(this.Quantity * assignment.UnitSellingPrice, 2);
        }

        public string PartId { get; }

        public string PartName { get; }

        public decimal Quantity { get; }

        public string UnitOfMeasure { get; }

        public decimal UnitSellingPrice { get; }

        public decimal SellingPrice { get; }
    }
}
