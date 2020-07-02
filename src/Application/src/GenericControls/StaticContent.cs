// <copyright file="Label.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Application.Ui.GenericControls
{
    using global::Allors.Excel;
    using System;

    public class StaticContent<T> : IControl
    {
        public StaticContent(ICell cell)
        {
            this.Cell = cell;
        }

        public ICell Cell { get; set; }

        public T Value { get; set; }

        public void Bind()
        {
            this.Cell.Value = this.Value;
        }

        public void OnCellChanged()
        {
            this.Cell.Value = this.Value;
        }

        public void Unbind()
        {
            // TODO
        }
    }
}