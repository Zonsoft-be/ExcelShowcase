// <copyright file="Client.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Allors.Excel
{
    using Allors.Workspace;

    public partial class Client
    {
        public Client(IDatabase database, IWorkspace workspace)
        {
            this.Database = database;
            this.Workspace = workspace;
        }

        public IDatabase Database { get; }

        public IWorkspace Workspace { get; }

        public bool IsLoggedIn { get; set; }

        public string UserName { get; set; }
    }
}
