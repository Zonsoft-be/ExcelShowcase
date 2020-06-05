// <copyright file="StatementOfWorkVersion.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Repository
{
    using System;
    using Allors.Repository.Attributes;

    #region Allors
    [Id("9C5784FB-6217-4C5D-8B87-D458DD4A00EE")]
    #endregion
    public partial class StatementOfWorkVersion : QuoteVersion
    {
        #region inherited properties
        public Permission[] DeniedPermissions { get; set; }

        public SecurityToken[] SecurityTokens { get; set; }

        public string InternalComment { get; set; }

        public DateTime RequiredResponseDate { get; set; }

        public DateTime ValidFromDate { get; set; }

        public QuoteTerm[] QuoteTerms { get; set; }

        public DateTime ValidThroughDate { get; set; }

        public string Description { get; set; }

        public Party Receiver { get; set; }

        public ContactMechanism FullfillContactMechanism { get; set; }

        public VatRegime VatRegime { get; set; }

        public VatClause AssignedVatClause { get; set; }

        public VatClause DerivedVatClause { get; set; }

        public decimal TotalExVat { get; set; }

        public decimal TotalVat { get; set; }

        public decimal TotalIncVat { get; set; }

        public decimal TotalSurcharge { get; set; }

        public decimal TotalDiscount { get; set; }

        public decimal TotalShippingAndHandling { get; set; }

        public decimal TotalFee { get; set; }

        public decimal TotalBasePrice { get; set; }

        public decimal TotalListPrice { get; set; }

        public ShippingAndHandlingCharge ShippingAndHandlingCharge { get; set; }

        public DiscountAdjustment DiscountAdjustment { get; set; }

        public SurchargeAdjustment SurchargeAdjustment { get; set; }

        public Fee Fee { get; set; }

        public Currency Currency { get; set; }

        public DateTime IssueDate { get; set; }

        public QuoteItem[] QuoteItems { get; set; }

        public string QuoteNumber { get; set; }

        public QuoteState QuoteState { get; set; }

        public Request Request { get; set; }

        public Guid DerivationId { get; set; }

        public DateTime DerivationTimeStamp { get; set; }

        public User LastModifiedBy { get; set; }

        #endregion

        #region inherited methods

        public void OnBuild() { }

        public void OnPostBuild() { }

        public void OnInit()
        {
        }

        public void OnPreDerive() { }

        public void OnDerive() { }

        public void OnPostDerive() { }

        #endregion
    }
}
