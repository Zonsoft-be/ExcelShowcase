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

            this.NamedRanges = this.Sheet.GetNamedRanges();

        }

        private async void Sheet_CellsChanged(object sender, CellChangedEvent e)
        {
            var addCells = e.Cells.Where(c => c.Column.Index == this.InvoiceLinesFirstColumn).ToArray();

            // When a new detail line is added or deleted.
            if (addCells.Length > 0)
            {
                // Cell with an detail invoice Line
                foreach(ICell cell in addCells)
                {
                    // Add
                    if("A".Equals(cell.ValueAsString, StringComparison.OrdinalIgnoreCase))
                    {
                        var invoiceLine = (InvoiceLine)this.program.Services.Database.Create<InvoiceLine>();
                        invoiceLine.Index = cell.Row.Index;
                        this.Invoice.AddInvoiceLine(invoiceLine);
                    }

                    // Delete
                    if ("D".Equals(cell.ValueAsString, StringComparison.OrdinalIgnoreCase))
                    {
                        var invoiceLine = this.Invoice.InvoiceLines.FirstOrDefault(v => v.Index == cell.Row.Index);
                        this.Invoice.RemoveInvoiceLine(invoiceLine);
                    }
                }

                this.Controls.Bind();

                await this.Sheet.Flush().ConfigureAwait(false);
            }
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

        public Range InvoiceLinesRange { get; private set; }

        public int InvoiceLinesFirstColumn { get; private set; }
        
        private Invoice Invoice {get; set;}
        public Range[] NamedRanges { get; }

        public async Task Refresh()
        {
            if(this.Invoice == null)
            {
                this.Invoice = (Invoice)this.program.Services.Database.Create<Invoice>();
                this.Invoice.InvoiceDate = DateTime.Now;
                this.Invoice.DeriveDueDate(Convert.ToInt32(this.program.Services.Configuration["InvoiceDueDate"]), this.program.Services.Configuration["InvoiceDueDateScheme"]);
                this.Invoice.InvoiceNumber = this.program.Services.Database.Count<Invoice>() + 1;
            }

            this.InvoiceLinesRange = this.NamedRanges.FirstOrDefault(r => $"'{this.Sheet.Name}'!Invoice_Lines".Equals(r.Name, StringComparison.OrdinalIgnoreCase));
            this.InvoiceLinesFirstColumn = this.InvoiceLinesRange.Column;
            
            var range = this.NamedRanges.FirstOrDefault(r => $"'{this.Sheet.Name}'!Invoice_Number".Equals(r.Name, StringComparison.OrdinalIgnoreCase));
            this.Controls.Label<Invoice>(range.Row, range.Column, this.Invoice, "InvoiceNumber");

            range = this.NamedRanges.FirstOrDefault(r => $"'{this.Sheet.Name}'!Invoice_Date".Equals(r.Name, StringComparison.OrdinalIgnoreCase));
            var cell = this.Controls.Label<Invoice>(range.Row, range.Column, this.Invoice, "InvoiceDate");
            cell.NumberFormat = "dd-MM-YYYY";

            range = this.NamedRanges.FirstOrDefault(r => $"'{this.Sheet.Name}'!Invoice_Duedate".Equals(r.Name, StringComparison.OrdinalIgnoreCase));
            cell = this.Controls.Label<Invoice>(range.Row, range.Column, this.Invoice, "InvoiceDueDate");
            cell.NumberFormat = "dd-MM-YYYY";
                        
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
