using Allors.Excel;
using Application.Models;
using Application.Ui;
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

namespace Application.Sheets
{
    public class InvoiceSheet : ISheet
    {
        private Program program;

        public IWorksheet Sheet { get; }

        public InvoiceSheet(Program program, IWorksheet worksheet)
        {
            this.program = program;
            this.Sheet = worksheet;

            this.Controls = new Controls(worksheet);

            this.Binder = new Binder(this.Sheet, Constants.ChangedStyle);
            this.Binder.ToDomained += this.Binder_ToDomained;

            this.Sheet.SheetActivated += this.Sheet_SheetActivated;
            this.Sheet.CellsChanged += Sheet_CellsChanged;
        }

        private void Sheet_CellsChanged(object sender, CellChangedEvent e)
        {
           
        }

        public bool IsWorksheetUpToDate { get; set; }

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


        private Invoice Invoice {get; set;}
               

        public async Task Refresh()
        {
            var sheetRanges = this.Sheet.GetNamedRanges();

            this.Invoice = (Invoice) this.program.Services.Database.Create<Invoice>();
            this.Invoice.InvoiceDate = DateTime.Now;
            this.Invoice.InvoiceNumber = this.program.Services.Database.Get<Invoice>().Length + 1;

            var range = sheetRanges.FirstOrDefault(r => $"'{this.Sheet.Name}'!company_name".Equals(r.Name, StringComparison.OrdinalIgnoreCase));
            this.Controls.Static(range.Row, range.Column, this.program.Services.Configuration["Company_name"]);

            range = sheetRanges.FirstOrDefault(r => $"'{this.Sheet.Name}'!company_street".Equals(r.Name, StringComparison.OrdinalIgnoreCase));
            this.Controls.Static(range.Row, range.Column, this.program.Services.Configuration["Company_Street"]);

            range = sheetRanges.FirstOrDefault(r => $"'{this.Sheet.Name}'!company_city".Equals(r.Name, StringComparison.OrdinalIgnoreCase));
            this.Controls.Static(range.Row, range.Column, this.program.Services.Configuration["Company_City"]);

            range = sheetRanges.FirstOrDefault(r => $"'{this.Sheet.Name}'!company_country".Equals(r.Name, StringComparison.OrdinalIgnoreCase));
            this.Controls.Static(range.Row, range.Column, this.program.Services.Configuration["Company_Country"]);

            range = sheetRanges.FirstOrDefault(r => $"'{this.Sheet.Name}'!invoice_date".Equals(r.Name, StringComparison.OrdinalIgnoreCase));
            var cell = this.Controls.Label<Invoice>(range.Row, range.Column, this.Invoice, "InvoiceDate");
            cell.NumberFormat = "dd-MM-YYYY";

            range = sheetRanges.FirstOrDefault(r => $"'{this.Sheet.Name}'!invoice_number".Equals(r.Name, StringComparison.OrdinalIgnoreCase));
            this.Controls.Label<Invoice>(range.Row, range.Column, this.Invoice, "InvoiceNumber");

            range = sheetRanges.FirstOrDefault(r => $"'{this.Sheet.Name}'!invoice_currency".Equals(r.Name, StringComparison.OrdinalIgnoreCase));
            this.Controls.Static(range.Row, range.Column, this.program.Services.Configuration["Company_Currency"]);

            this.Controls.Bind();

            await this.Sheet.Flush().ConfigureAwait(false);

            await Task.CompletedTask;
        }

        internal void SaveAsPDF()
        {
            var fileName = $"{this.program.Services.Configuration["Company_name"]}_{this.Invoice.InvoiceNumber}.pdf";

            var path = this.program.Services.Configuration["OutputDirectory"];

            var file = new FileInfo(Path.Combine(path, fileName));

            this.Sheet.SaveAsPDF(file, false, false, ignorePrintAreas: false);
        }

        internal async Task Save()
        {
            if (this.Invoice != null)
            {
                this.program.Services.Database.Save(this.Invoice);
            }

            await Task.CompletedTask;
        }
    }
}
