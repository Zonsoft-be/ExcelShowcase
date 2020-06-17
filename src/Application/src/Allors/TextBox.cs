namespace Application.Ui
{
    using System;
    using Allors.Excel;
    using System.Globalization;
    using Application.Models;
    using System.Reflection;
    using System.Net.Http.Headers;

    public class TextBox<T> : IControl where T : Identifiable
    {
        /// <summary>
        /// TextBox is a two-way binding object for excel cell value.
        /// </summary>
        /// <param name="cell"></param>
        public TextBox(ICell cell)
        {
            this.Cell = cell;
        }

        public T SessionObject { get; internal set; }

        public string RoleType { get; internal set; }

        public string DisplayRoleType { get; internal set; }

        public Func<object, dynamic> ToDomain { get; internal set; }

        /// <summary>
        /// Func called just before writing to the excel cell. Last chance to change the value.
        /// </summary>
        public Func<T, dynamic> ToCell { get; internal set; }

        public ICell Cell { get; set; }

        public string RelationType { get; internal set; }

        /// <summary>
        /// Factory must provide a new SessionObject when the OnCellChanged event is handled.
        /// </summary>
        public Func<ICell, T> Factory { get; internal set; }

        public void Bind()
        {
            var propertyInfo = this.SessionObject.GetType().GetProperty(this.DisplayRoleType ?? this.RoleType);

            if (propertyInfo.CanRead)
            {
                this.SetCellValue(this.SessionObject, propertyInfo);
            }

            this.Cell.Style = propertyInfo.CanWrite || this.Factory != null 
                ? Constants.WriteStyle 
                : Constants.ReadOnlyStyle;
        }

        public void OnCellChanged()
        {
            if (this.SessionObject == null && this.Factory != null) 
            {
                this.SessionObject = this.Factory(this.Cell);
            }

            var propertyInfo = this.SessionObject.GetType().GetProperty(this.RoleType);
            System.TypeCode typeCode = Type.GetTypeCode(propertyInfo.PropertyType);

            if (propertyInfo.PropertyType == typeof(decimal))
            {                
                if(this.Cell.Value == null)
                {
                    propertyInfo.SetValue(this.SessionObject, default(decimal));
                }
                else
                {
                    decimal.TryParse(this.Cell.ValueAsString, out decimal result);
                    propertyInfo.SetValue(this.SessionObject, result);
                }
            } 
            else if (propertyInfo.PropertyType == typeof(decimal?))
            {
                if (this.Cell.Value == null)
                {
                    propertyInfo.SetValue(this.SessionObject, default(decimal?));

                }
                else
                {
                    propertyInfo.SetValue(this.SessionObject, Convert.ToDecimal(this.Cell.Value));
                }
            }
            else if (propertyInfo.PropertyType == typeof(int))
            {
                if (this.Cell.Value == null)
                {
                    propertyInfo.SetValue(this.SessionObject, default(int));

                }
                else
                {
                    propertyInfo.SetValue(this.SessionObject, Convert.ToDecimal(this.Cell.Value));
                }
            }
            else if (propertyInfo.PropertyType == typeof(int?))
            {
                if (this.Cell.Value == null)
                {
                    propertyInfo.SetValue(this.SessionObject, default(int?));

                }
                else
                {
                    propertyInfo.SetValue(this.SessionObject, Convert.ToInt32(this.Cell.Value));
                }
            }
            else if (propertyInfo.PropertyType == typeof(DateTime))
            {
                if (this.Cell.Value == null)
                {
                    propertyInfo.SetValue(this.SessionObject, default(DateTime));

                }
                else
                {
                    var dt = DateTime.FromOADate(Convert.ToDouble(this.Cell.Value));
                    propertyInfo.SetValue(this.SessionObject, dt);
                }
            }
            else if (propertyInfo.PropertyType == typeof(DateTime?))
            {
                if (this.Cell.Value == null)
                {
                    propertyInfo.SetValue(this.SessionObject, default(DateTime?));

                }
                else
                {
                    var dt = DateTime.FromOADate(Convert.ToDouble(this.Cell.Value));
                    propertyInfo.SetValue(this.SessionObject, dt);
                }
            }
            else if (propertyInfo.PropertyType == typeof(bool))
            {
                if (this.Cell.Value == null)
                {
                    propertyInfo.SetValue(this.SessionObject, default(bool));

                }
                else
                {
                    propertyInfo.SetValue(this.SessionObject, Constants.YES.Equals(this.Cell.ValueAsString, StringComparison.OrdinalIgnoreCase) ? true : false  );
                }
            }
            else if (propertyInfo.PropertyType == typeof(bool?))
            {
                if (this.Cell.Value == null)
                {
                    propertyInfo.SetValue(this.SessionObject, default(bool?));

                }
                else
                {
                    propertyInfo.SetValue(this.SessionObject, Constants.YES.Equals(this.Cell.ValueAsString, StringComparison.OrdinalIgnoreCase) ? true : false);
                }
            }
            else
            {
                if (this.Cell.Value == null)
                {
                    propertyInfo.SetValue(this.SessionObject, default(string));

                }
                else
                {
                    propertyInfo.SetValue(this.SessionObject, this.Cell.ValueAsString);
                }
            }

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