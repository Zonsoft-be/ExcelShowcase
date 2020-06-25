// <copyright file="Label.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Application.Ui
{
    using System;
    using System.Reflection;
    using Allors.Excel;
    using Application.Models;

    public class Label<T> : IControl where T : Identifiable
    {
        public Label(ICell cell)
        {
            this.Cell = cell;
        }

        public T SessionObject { get; internal set; }

        public string RoleType { get; internal set; }

        public string DisplayRoleType { get; internal set; }
        
        public ICell Cell { get; set; }

        public void Bind()
        {
            var propertyInfo = this.SessionObject.GetType().GetProperty(this.DisplayRoleType ?? this.RoleType);

            if (propertyInfo.CanRead)
            {
                this.SetCellValue(this.SessionObject, propertyInfo);               
            }
        }

        public void OnCellChanged()
        {
            var propertyInfo = this.SessionObject.GetType().GetProperty(this.RoleType);

            // Restore the object value
            this.SetCellValue(this.SessionObject, propertyInfo);
        }

        public void Unbind()
        {
            // TODO
        }

        private void SetCellValue(T obj, PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType == typeof(bool))
            {                
                if (propertyInfo.GetValue(obj) is bool boolvalue && boolvalue)
                {
                    this.Cell.Value = Constants.YES;
                }
                else
                {
                    this.Cell.Value = Constants.NO;
                }
            }
            else if (propertyInfo.PropertyType == typeof(DateTime))
            {
                var dt = (DateTime?)propertyInfo.GetValue(obj);
                this.Cell.Value = dt?.ToOADate();
            }
            else
            {
                this.Cell.Value = propertyInfo.GetValue(obj);
            }
        }
    }
}