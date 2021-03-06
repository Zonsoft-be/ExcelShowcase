// <copyright file="SalesOrderTransfer.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Repository
{
    using System;
    using Allors.Repository.Attributes;

    #region Allors
    [Id("0d8a85a8-4e76-457d-9594-84ee38c4e66f")]
    #endregion
    public partial class SalesOrderTransfer: Object
    {
        #region inherited properties
        public Permission[] DeniedPermissions { get; set; }

        public SecurityToken[] SecurityTokens { get; set; }

        #endregion

        #region Allors
        [Id("907e7a93-eaef-4cfb-83f7-c7c84cb88ed7")]
        [AssociationId("d72b69ae-f211-4c37-a445-088cd343a7f7")]
        [RoleId("ec7b3ed3-53b0-4b52-9cac-a1b4cf9a230d")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.OneToOne)]
        [Workspace]
        public SalesOrder From { get; set; }

        #region Allors
        [Id("b83ce113-d73a-442e-8535-a084620bfbb5")]
        [AssociationId("2d22238d-4cf4-4438-b10d-f4d83c1e0636")]
        [RoleId("fe17c614-cc15-47ac-8a71-eb9074388cd8")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.OneToOne)]
        [Required]
        [Workspace]
        public SalesOrder To { get; set; }

        #region inherited methods

        public void OnBuild() { }

        public void OnPostBuild() { }

        public void OnInit() { }

        public void OnPreDerive() { }

        public void OnDerive() { }

        public void OnPostDerive() { }

        #endregion
    }
}
