using Allors.Excel;
using Application.Models;
using Application.Services;
using Application.Ui;
using Application.Ui.GenericControls;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using System.Xml.Serialization;

namespace Application.Sheets
{
    public class AppConfigSheet : ISheet
    {
        private Program program;

        public IWorksheet Sheet { get; }

        public AppConfigSheet(IProgram program, IWorksheet worksheet)
        {
            this.program = (Program)program;
            this.Sheet = worksheet;

            this.Controls = new Controls(worksheet);

            this.Binder = new Binder(this.Sheet, Constants.ChangedStyle);
            this.Binder.ToDomained += this.Binder_ToDomained;

            this.Sheet.SheetActivated += this.Sheet_SheetActivated;

            this.Sheet.Name = $"{nameof(AppConfigSheet)}";

            // Fetch after we changed the name.
            this.NamedRanges = this.Sheet.GetNamedRanges();

            // Save so we can re-instate it as an invoicesSheet on startup
            var customProperties = new CustomProperties();
            customProperties.Add(AppConstants.KeySheet, nameof(AppConfigSheet));
            customProperties.Add(AppConstants.KeyCreated, DateTime.Now);
            customProperties.Add(AppConstants.KeyCreatedBy, this.program.Services.Configuration["Username"]);
            this.Sheet.SetCustomProperties(customProperties);
        }

        public int Index => this.Sheet.Index;

        private async void Sheet_SheetActivated(object sender, string e)
        {
            if (!this.IsWorksheetUpToDate)
            {
                await this.Refresh().ConfigureAwait(false);

                this.IsWorksheetUpToDate = true;
            }
        }

        private async void Binder_ToDomained(object sender, EventArgs e)
        {
            await this.Sheet.Flush().ConfigureAwait(false);
        }

        public Binder Binder { get; set; }

        private Controls Controls { get; }
             

        public Range[] NamedRanges { get; }

        public bool IsWorksheetUpToDate { get; set; }

        public async Task Refresh()
        {
            await RefreshSheet().ConfigureAwait(false);
        }

        private async Task RefreshSheet()
        {
            var colIndex = 0;
            var rowIndex = 0;

            //
            this.Controls.Static(rowIndex++, colIndex, "Booleans");
            this.Controls.Static(rowIndex++, colIndex, "True");
            this.Controls.Static(rowIndex, colIndex, "False");

            rowIndex = 1;
            colIndex = 1;
            this.Controls.Static(rowIndex++, colIndex, this.program.Services.Configuration["true"]);
            this.Controls.Static(rowIndex, colIndex, this.program.Services.Configuration["false"]);

            this.Sheet.Workbook.SetNamedRange(KnownNames.ValidationRangeBooleans, new Range(1, 1, 2, 1, this.Sheet));

            this.Controls.Bind();

            await this.Sheet.Flush().ConfigureAwait(false);           
        }
    }
}
