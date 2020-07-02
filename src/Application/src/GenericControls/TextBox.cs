namespace Application.Ui.GenericControls
{
    using System;
    using Allors.Excel;
    using System.Globalization;
    using Application.Models;
    using System.Reflection;
    using System.Net.Http.Headers;
    using Application.Ui;

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

        public T SessionObject { get; set; }

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

        bool IsNullable(Type type) => Nullable.GetUnderlyingType(type) != null;

        public void OnCellChanged()
        {
            if (this.SessionObject == null && this.Factory != null) 
            {
                this.SessionObject = this.Factory(this.Cell);
            }

            var propertyInfo = this.SessionObject.GetType().GetProperty(this.RoleType);
          
            if(this.Cell.Value == null && IsNullable(propertyInfo.PropertyType))
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
                propertyInfo.SetValue(this.SessionObject, this.Cell.ValueAsString);              
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
                this.Cell.Value = propertyInfo.GetValue(obj);
            }
        }
    }
}