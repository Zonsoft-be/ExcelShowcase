// <copyright file="Controls.cs" company="Allors bvba">
// Copyright (c) Allors bvba. All rights reserved.
// Licensed under the LGPL license. See LICENSE file in the project root for full license information.
// </copyright>

namespace Application.Ui
{
    using System;
    using System.Collections.Concurrent;
    using System.Collections.Generic;
    using System.Linq;
    using Allors.Excel;
    using System.Globalization;
    using System.Drawing;
    using Application.Models;

    public class Controls
    {
        public Controls(IWorksheet worksheet)
        {
            this.Worksheet = worksheet ?? throw new ArgumentNullException(nameof(worksheet));
            this.Worksheet.CellsChanged += this.Worksheet_CellsChanged;
            this.ControlByCell = new ConcurrentDictionary<ICell, IControl>();
            this.ActiveControls = new HashSet<IControl>();
        }

        ~Controls()
        {
            this.Worksheet.CellsChanged -= this.Worksheet_CellsChanged;
        }

        public IWorksheet Worksheet { get; }

        public ConcurrentDictionary<ICell, IControl> ControlByCell { get; private set; }

        private HashSet<IControl> ActiveControls { get; }

        public string ExcelColumnFromNumber(int column)
        {
            string columnString = string.Empty;
            decimal columnNumber = column;
            while (columnNumber > 0)
            {
                decimal currentLetterNumber = (columnNumber - 1) % 26;
                char currentLetter = (char)(currentLetterNumber + 65);
                columnString = currentLetter + columnString;
                columnNumber = (columnNumber - (currentLetterNumber + 1)) / 26;
            }

            return columnString;
        }

        public int ExcelColumnIndexFromName(string name)
        {
            if(!string.IsNullOrWhiteSpace(name))
            {
                name = name.ToUpper(CultureInfo.CurrentCulture);
                int number = 0;
                int pow = 1;
                for (int i = name.Length - 1; i >= 0; i--)
                {
                    number += (name[i] - 'A' + 1) * pow;
                    pow *= 26;
                }

                return number;
            }

            return 0;
        }

        /// <summary>
        /// Shows a value of T in the provided cell.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="row"></param>
        /// <param name="column"></param>
        /// <param name="value"></param>
        internal ICell Static<T>(int row, int column, T value)
        {
            var cell = this.Worksheet[row, column];
            
            if (!this.ControlByCell.TryGetValue(cell, out var control))
            {
                control = new StaticContent<T>(cell);
                this.ControlByCell.TryAdd(cell, control);
            }

            var staticContent = (StaticContent<T>)control;
            staticContent.Value = value;

            this.ActiveControls.Add(staticContent);

            return cell;
        }
              

        /// <summary>
        /// Sets a Formula in the row, column. Formula is a string, starting with '='
        /// </summary>
        /// <param name="cell"></param>
        /// <param name="formula"></param>
        internal ICell Formula(int row, int column, string formula)
        {
            var cell = this.Worksheet[row, column];

            if (!this.ControlByCell.TryGetValue(cell, out var control))
            {
                control = new FormulaControl(cell);
                this.ControlByCell.TryAdd(cell, control);
            }

            var formulaControl = (FormulaControl)control;
            formulaControl.Formula = formula;

            this.ActiveControls.Add(formulaControl);

            return cell;
        }


        ///// <summary>
        ///// Sets a readonly value in the cell. Changes are not handled.
        ///// </summary>
        ///// <param name="cell"></param>
        ///// <param name="sessionObject"></param>
        ///// <param name="roleType"></param>
        ///// <param name="relationType"></param>
        internal ICell Label<T>(int row, int column, T sessionObject, string roleType, string relationType = null) where T : Identifiable
        {
            if (sessionObject != null)
            {
                var cell = this.Worksheet[row, column];

                if (!this.ControlByCell.TryGetValue(cell, out var control))
                {
                    control = new Label<T>(cell);
                    this.ControlByCell.TryAdd(cell, control);
                }

                var label = (Label<T>)control;
                label.SessionObject = sessionObject;
                label.RoleType = roleType;
               

                this.ActiveControls.Add(control);

                return cell;
            }

            return null;
        }

        /// <inheritdoc cref="Application.Excel.TextBox"/>
        internal ICell TextBox<T>(
            int row, int column, T sessionObject, string roleType, string relationType = null,
            string displayRoleType = null,
            string numberFormat = null,
            Func<object, dynamic> toDomain = null,
            Func<T, dynamic> toCell = null,
            Func<ICell, T> factory = null) where T: Identifiable
        {
            if (sessionObject != null || factory != null)
            {
                var cell = this.Worksheet[row, column];
                cell.NumberFormat = numberFormat;

                if (!this.ControlByCell.TryGetValue(cell, out var control))
                {
                    control = new TextBox<T>(cell);
                    this.ControlByCell.TryAdd(cell, control);
                }

                var textBox = (TextBox<T>)control;
                textBox.SessionObject = sessionObject;
                textBox.RoleType = roleType;
                textBox.RelationType = relationType;
                textBox.DisplayRoleType = displayRoleType;
                textBox.ToDomain = toDomain;
                textBox.ToCell = toCell;
                textBox.Factory = factory;

                this.ActiveControls.Add(control);

                return cell;
            }

            return null;
        }

        internal void Select<T>(int row, int column, Range options, T sessionObject, string roleType, string relationType = null,
            string displayRoleType = null,
            Func<object, dynamic> toDomain = null,
            string numberFormat = null,
            bool hideInCellDropDown = false) where T : Identifiable
        {
            if (sessionObject != null)
            {
                var cell = this.Worksheet[row, column];
                cell.Options = options ?? throw new ArgumentNullException(nameof(options));
                cell.NumberFormat = numberFormat;
                //cell.IsRequired = roleType.IsRequired;
                cell.HideInCellDropdown = hideInCellDropDown;

                if (!this.ControlByCell.TryGetValue(cell, out var control))
                {
                    control = new ComboBox<T>(cell);
                    this.ControlByCell.TryAdd(cell, control);
                }

                var comboBox = (ComboBox<T>)control;

                comboBox.SessionObject = sessionObject;
                comboBox.RoleType = roleType;
                comboBox.RelationType = relationType;
                comboBox.DisplayRoleType = displayRoleType;
                comboBox.ToDomain = toDomain;

                this.ActiveControls.Add(control);
            }
        }

        internal CompositeControl<T> Composite<T>(int row, int column) where T : Identifiable
        {
            var cell = this.Worksheet[row, column];

            if (!this.ControlByCell.TryGetValue(cell, out var control))
            {
                control = new CompositeControl<T>(this, cell);

                this.ControlByCell.TryAdd(cell, control);
            }

            this.ActiveControls.Add(control);

            return (CompositeControl<T>)control;
        }

        internal void Bind()
        {
            var obsoleteControls = this.ControlByCell.Values.Except(this.ActiveControls);
            foreach (var control in this.ActiveControls)
            {
                control.Bind();
            }

            foreach (var control in obsoleteControls)
            {
                control.Unbind();
            }

            this.ControlByCell = new ConcurrentDictionary<ICell, IControl>(this.ActiveControls.ToDictionary(v => v.Cell));
        }

        private async void Worksheet_CellsChanged(object sender, CellChangedEvent e)
        {
            var changesReverted = false;

            foreach (var cell in e.Cells)
            {
                if (this.ControlByCell.TryGetValue(cell, out var control))
                {
                    control.OnCellChanged();

                    if (IsGenericStaticContent(control))
                    {                        
                        changesReverted = true;
                    }
                }
            }

            if (changesReverted)
            {
                // a single message to the user should be done here:
            }

            await this.Worksheet.Flush().ConfigureAwait(false);
        }

        private bool IsGenericStaticContent(IControl icontrol)
        {
            var type = icontrol.GetType();
            bool result1 = type.IsGenericType && type.FullName.Contains("StaticContent");

            return result1;
        }
    }
}