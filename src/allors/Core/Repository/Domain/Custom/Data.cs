// <copyright file="Data.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Repository
{
    using System;

    using Allors.Repository.Attributes;

    #region Allors
    [Id("0E82B155-208C-41FD-B7D0-731EADBB5338")]
    #endregion
    public partial class Data : Object
    {
        #region inherited properties
        public Permission[] DeniedPermissions { get; set; }

        public SecurityToken[] SecurityTokens { get; set; }
        #endregion

        #region Allors
        [Id("36FA4EB8-5EA9-4F56-B5AA-9908EF2B417F")]
        [AssociationId("C0CA43A1-9C16-42BA-B83B-5E6C72DCB605")]
        [RoleId("5F26C1A3-BD24-465B-A4F9-D7A5D79A5C80")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Workspace]
        public Person AutocompleteFilter { get; set; }

        #region Allors
        [Id("C1C4D5D9-EEC0-44B5-9317-713E9AB2277E")]
        [AssociationId("9ED53BA6-6B03-448D-B2E7-42AD045BEEC3")]
        [RoleId("4B25DD13-A74D-483C-A0C4-7A5491B9D955")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Workspace]
        public Person AutocompleteOptions { get; set; }

        #region Allors
        [Id("46964F62-AF12-4450-83DA-C695C4A0ECE8")]
        [AssociationId("4E112908-E5B4-448C-B6A6-58094165522B")]
        [RoleId("BA0EA6C5-E62F-487B-B57C-D7412A6BF958")]
        #endregion
        [Workspace]
        public bool Checkbox { get; set; }

        #region Allors
        [Id("7E098B17-2ECB-4D1C-AA73-80684394BD9B")]
        [AssociationId("D13FDDE0-8817-4B13-BE41-D54ED349813F")]
        [RoleId("903F0C58-0867-49D8-B3F7-EA1A6F89EA35")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.OneToMany)]
        [Workspace]
        public Person[] Chips { get; set; }

        #region Allors
        [Id("46C310DE-8E36-412E-8068-A9D734734E74")]
        [AssociationId("7F157FB6-8F06-4C71-B0B9-FD3B2E6237AD")]
        [RoleId("3E57DB60-D0C5-4748-8095-31FD10A9DD50")]
        #endregion
        [Size(-1)]
        [Workspace]
        public string String { get; set; }

        #region Allors
        [Id("31D0A290-2637-452D-8462-4BBB744E3065")]
        [AssociationId("0551F665-4510-4CAC-AB4E-C4B67B0C6099")]
        [RoleId("A77403DD-E597-4372-8BC4-61F9F0BA4615")]
        #endregion
        [Workspace]
        public DateTime Date { get; set; }

        #region Allors
        [Id("487A0EF5-C987-4064-BF6B-0B7354EC4315")]
        [AssociationId("49FCDC52-8093-4972-A6E7-0CA9302853F0")]
        [RoleId("4285B1D9-5697-4345-B18C-8EF746F82FB5")]
        #endregion
        [Workspace]
        public DateTime DateTime { get; set; }

        #region Allors
        [Id("940DAD46-78C6-44B3-93A2-4AE0137C2839")]
        [AssociationId("93C8A47F-1069-4565-9EED-D4D612EDF422")]
        [RoleId("924808AC-861B-4000-89A9-F0D1EC98F8FB")]
        #endregion
        [Workspace]
        public DateTime DateTime2 { get; set; }

        #region Allors
        [Id("BA910E25-0D71-43E1-8311-7C9620AC0CDE")]
        [AssociationId("55675B9D-6226-45F1-9DE2-ED92263212D9")]
        [RoleId("90FAB888-FA1A-4DAE-809A-0AC1D4618A30")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToOne)]
        [Workspace]
        public Media File { get; set; }

        #region Allors
        [Id("68515CCE-3E87-4D21-B5E5-2136CC3D4F5C")]
        [AssociationId("C07CDDB3-7D31-416E-B4DE-D9197BD5FA25")]
        [RoleId("65195AEF-8A73-40F6-B8DF-98A1C1B6B54D")]
        [Indexed]
        #endregion
        [Multiplicity(Multiplicity.ManyToMany)]
        [Workspace]
        public Media[] MultipleFiles { get; set; }

        #region Allors
        [Id("3AA7FE12-F9DC-43A8-ACA7-3EADAEE0D05D")]
        [AssociationId("C0EFDE58-8A46-48B1-8742-AEDB5970A2E5")]
        [RoleId("735F03A0-8F3A-486F-AD7A-55EEFD2671E8")]
        #endregion
        [Size(256)]
        [Workspace]
        public string RadioGroup { get; set; }

        #region Allors
        [Id("C5061BAE-0B3B-474D-ABAA-DDAD638B8DA1")]
        [AssociationId("D4B26BEE-AF21-4DA4-8C18-4A0E835C2FBD")]
        [RoleId("F0DCBF23-FCDE-4661-87EC-857D3E983000")]
        #endregion
        [Workspace]
        public int Slider { get; set; }

        #region Allors
        [Id("753E6310-B943-48E8-A9F6-306D2A5DB6E4")]
        [AssociationId("522AA21D-A52B-4F3D-B734-062A63EF4E75")]
        [RoleId("73109BBE-1CE4-4677-8CBB-E9375166200A")]
        #endregion
        [Workspace]
        public bool SlideToggle { get; set; }

        #region Allors
        [Id("7B18C411-5414-4E28-A7C1-5749347B673B")]
        [AssociationId("A964B0D0-3D8D-4D1A-9EBE-36BDF1AC0EB2")]
        [RoleId("3AF4AA7C-27F4-490C-B0F4-434FB2D981F1")]
        #endregion
        [Workspace]
        [MediaType("text/plain")]
        public string PlainText { get; set; }

        #region Allors
        [Id("A01C4AD6-A07E-48D0-B3FB-A35ADEDC9050")]
        [AssociationId("053E9EDC-ABFC-474E-8D6B-827DEC42DBFB")]
        [RoleId("CC1FB0B7-1AF7-4C71-AAD7-4076DEFCC3AE")]
        #endregion
        [Workspace]
        [Size(-1)]
        [MediaType("text/markdown")]
        public string Markdown { get; set; }

        #region Allors
        [Id("BF21BDD8-07D8-460B-B8BF-4B69E5B96725")]
        [AssociationId("9C908B88-D4B6-401C-B39E-05CCF6F1BB76")]
        [RoleId("9A93E93E-57D9-4002-AB5F-C54C8BBF88F4")]
        #endregion
        [Workspace]
        [Size(-1)]
        [MediaType("text/html")]
        public string Html { get; set; }

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
