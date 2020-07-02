// <copyright file="ComboBox.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Application.Ui.GenericControls
{
    using System;
    using Allors.Excel;
    using System.Globalization;
    using Application.Models;
    using System.Reflection;
    using Application.Ui;

    public class ComboBox<T> : IControl where T : Identifiable
    {
        public ComboBox(ICell cell)
        {
            this.Cell = cell;
        }

        public Range Options => this.Cell.Options;

        public T SessionObject { get; internal set; }

        public string RoleType { get; internal set; }

        public string DisplayRoleType { get; internal set; }

        public Func<object, dynamic> ToDomain { get; internal set; }

        public ICell Cell { get; set; }

        public string RelationType { get; internal set; }

        public void Bind()
        {
            var propertyInfo = this.SessionObject.GetType().GetProperty(this.DisplayRoleType ?? this.RoleType);

            if (propertyInfo.CanRead)
            {
                this.SetCellValue(this.SessionObject, propertyInfo);
            }

            this.Cell.Style = propertyInfo.CanWrite
                 ? Constants.WriteStyle
                 : Constants.ReadOnlyStyle;
        }

        bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null;

        public void OnCellChanged()
        {
            var propertyInfo = this.SessionObject.GetType().GetProperty(this.RoleType);

            if (this.Cell.Value == null && IsNullable(propertyInfo.PropertyType))
            {
                propertyInfo.SetValue(this.SessionObject, null);
            }
            else if (propertyInfo.PropertyType == typeof(decimal) || propertyInfo.PropertyType == typeof(decimal?))
            {
                decimal.TryParse(this.Cell.ValueAsString, out decimal result);
                propertyInfo.SetValue(this.SessionObject, result);
            }
            else if (propertyInfo.PropertyType == typeof(int) || propertyInfo.PropertyType == typeof(int?))
            {
                int.TryParse(this.Cell.ValueAsString, out int result);
                propertyInfo.SetValue(this.SessionObject, result);
            }
            else if (propertyInfo.PropertyType == typeof(DateTime) || propertyInfo.PropertyType == typeof(DateTime?))
            {
                var dt = DateTime.FromOADate(Convert.ToDouble(this.Cell.Value));
                propertyInfo.SetValue(this.SessionObject, dt);
            }
            else if (propertyInfo.PropertyType == typeof(bool))
            {
                propertyInfo.SetValue(this.SessionObject, Constants.YES.Equals(this.Cell.ValueAsString, StringComparison.OrdinalIgnoreCase) ? true : false);
            }
            else
            {
                if(this.ToDomain != null)
                {
                    var relation = this.ToDomain(this.Cell.Value);
                    propertyInfo.SetValue(this.SessionObject, relation);
                }
                else
                {
                    propertyInfo.SetValue(this.SessionObject, this.Cell.ValueAsString);
                }
            }

            this.Cell.Style = Constants.ChangedStyle;
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
                if(this.RelationType != null)
                {
                    var relation = propertyInfo.GetValue(obj);
                    var relationPropertyInfo = relation?.GetType().GetProperty(this.RelationType);

                    this.Cell.Value = relationPropertyInfo?.GetValue(relation);

                }
                else
                {
                    this.Cell.Value = propertyInfo.GetValue(obj);

                }
            }
        }
    }
}