// <copyright file="WorkEffortType.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Domain
{
    using Allors.Meta;

    public partial class WorkEffortType
    {
        public void BaseOnDerive(ObjectOnDerive method)
        {
            var derivation = method.Derivation;

            derivation.Validation.AssertExists(this, M.WorkEffortType.Description);
            derivation.Validation.AssertExists(this, M.WorkEffortType.WorkEffortTypeKind);
        }
    }
}
