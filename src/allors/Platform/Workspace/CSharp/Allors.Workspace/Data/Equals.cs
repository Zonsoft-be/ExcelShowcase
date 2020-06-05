// <copyright file="Equals.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Workspace.Data
{
    using System.Collections.Generic;
    using Allors.Protocol.Data;
    using Allors.Workspace.Meta;

    public class Equals : IPropertyPredicate
    {
        public string[] Dependencies { get; set; }

        public Equals(IPropertyType propertyType = null) => this.PropertyType = propertyType;

        /// <inheritdoc/>
        public IPropertyType PropertyType { get; set; }

        public ISessionObject Object { get; set; }

        public object Value { get; set; }

        public string Parameter { get; set; }

        public Predicate ToJson() =>
            new Predicate
            {
                Kind = PredicateKind.Equals,
                Dependencies = this.Dependencies,
                PropertyType = this.PropertyType.Id,
                Object = this.Object?.Id.ToString(),
                Value = UnitConvert.ToString(this.Value),
                Parameter = this.Parameter,
            };
    }
}
