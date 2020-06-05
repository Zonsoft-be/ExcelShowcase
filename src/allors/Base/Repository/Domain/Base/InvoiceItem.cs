// <copyright file="InvoiceItem.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Repository
{
    using Allors.Repository.Attributes;

    #region Allors
    [Id("d79f734d-4434-4710-a7ea-7d6306f3064f")]
    #endregion
    public partial interface InvoiceItem : DelegatedAccessControlledObject, Priceable, Deletable
    {
        #region Allors
        [Id("39CB3BE2-2E0D-4124-8241-866860C2BDC0")]
        [AssociationId("1A2D792C-1453-458A-80EE-5EDE6FA5663C")]
        [RoleId("1D6A2E2B-EE3D-4F9E-9F08-905D2F6E09B9")]
        #endregion
        [Multiplicity(Multiplicity.OneToOne)]
        [Size(-1)]
        [Workspace]
        string InternalComment { get; set; }

        #region Allors
        [Id("067674d0-6d9b-4a7e-b0c6-62c24f3a4815")]
        [AssociationId("72cdddb8-711d-491c-9965-cef190a10913")]
        [RoleId("5f894db7-f9ed-47d0-a438-c2e00446edbf")]
        #endregion
        [Multiplicity(Multiplicity.OneToMany)]
        [Indexed]
        [Workspace]
        SalesTerm[] SalesTerms { get; set; }

        #region Allors
        [Id("1f92aed8-8a8f-4eb6-8102-83a6395788d6")]
        [AssociationId("b65ecb61-b074-47fc-aac7-74119295c827")]
        [RoleId("e8c62a38-a856-4db6-a971-575d7971689c")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal TotalInvoiceAdjustment { get; set; }

        #region Allors
        [Id("33caab05-ec61-4cf9-b903-b5d5a8d7eef9")]
        [AssociationId("77489b35-b46a-4540-8359-005adbd9d1f9")]
        [RoleId("cf9b4f4a-b867-4a47-919e-2cb90be72980")]
        #endregion
        [Multiplicity(Multiplicity.OneToMany)]
        [Indexed]
        [Workspace]
        InvoiceVatRateItem[] InvoiceVatRateItems { get; set; }

        #region Allors
        [Id("475d7a79-27a1-4d5a-90c1-3896fa2e892e")]
        [AssociationId("ad65733c-6d3d-4e90-97d5-ca91bc4505d9")]
        [RoleId("651b29f8-644d-4588-ac56-0d51f2068ebd")]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Indexed]
        [Workspace]
        InvoiceItem AdjustmentFor { get; set; }

        #region Allors
        [Id("7eed800d-c2b5-4837-a288-150803578b27")]
        [AssociationId("9dbf4d82-0d36-42a0-81a7-49f59e5cd226")]
        [RoleId("f3b11549-8cf9-4ade-8465-111536b00171")]
        #endregion
        [Size(-1)]
        [Workspace]
        string Message { get; set; }

        #region Allors
        [Id("8fd19791-85ed-44c9-8580-a6768578ca3a")]
        [AssociationId("72e1379d-a9c3-41d5-8ae4-a9a82c88ad01")]
        [RoleId("1ca3573f-812f-41ca-a5e8-ec13ea6168aa")]
        #endregion
        [Derived]
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal AmountPaid { get; set; }

        #region Allors
        [Id("ba90acfe-0d55-4854-a046-35279f872e0b")]
        [AssociationId("d231d38a-2e1e-4e21-8622-d5b30199f857")]
        [RoleId("b525a1c4-5f1f-402f-9f40-105e711bf45d")]
        #endregion
        [Required]
        [Precision(19)]
        [Scale(2)]
        [Workspace]
        decimal Quantity { get; set; }

        #region Allors
        [Id("fb202916-1a87-439e-b2d8-b3f3ed4f681a")]
        [AssociationId("13dda3fd-6011-4876-9860-158d86024dbd")]
        [RoleId("50ab8ac2-daca-4e66-861d-4134fcaa0e98")]
        #endregion
        [Size(-1)]
        [Workspace]
        string Description { get; set; }

        #region Allors
        [Id("4B19B32A-1B6F-478A-8376-779A32AB6386")]
        [AssociationId("663FD9FF-E112-40F6-80A0-05AFE613AA3D")]
        [RoleId("3D14D8FD-E189-4CDC-8A1F-00203B0BE7E0")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Synced]
        Invoice SyncedInvoice { get; set; }
    }
}
