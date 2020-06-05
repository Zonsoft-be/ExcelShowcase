// <copyright file="ExtentKind.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Protocol.Data
{
    // TODO: Make enumeration lik PredicateKind
    public static class ExtentKind
    {
        public const string Filter = "Filter";

        public const string Union = "Union";

        public const string Intersect = "Intersect";

        public const string Except = "Except";
    }
}
