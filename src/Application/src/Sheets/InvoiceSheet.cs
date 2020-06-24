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

            this.Sheet.Name = $"{nameof(InvoiceSheet)}.{this.Sheet.Index}"; // Single Quotes to always allow spaces or special chars

            // Fetch after we changed the name.
            this.NamedRanges = this.Sheet.GetNamedRanges();

        }

        public int Index => this.Sheet.Index;

        private async void Sheet_CellsChanged(object sender, CellChangedEvent e)
        {
            var addCells = e.Cells.Where(c => c.Column.Index == this.InvoiceLinesFirstColumn + 1).ToArray();

            // When a new detail line is added or deleted.
            if (addCells.Length > 0)
            {
                // Cell with an detail invoice Line
                foreach(ICell cell in addCells)
                {
                    // Add
                    if(!string.IsNullOrEmpty(cell.ValueAsString) && !this.Invoice.InvoiceLines.Any(v => v.Index == cell.Row.Index))
                    {
                        var invoiceLine = (InvoiceLine)this.program.Services.Database.Create<InvoiceLine>(typeof(InvoiceLine), cell.Row.Index);                       
                        this.Invoice.AddInvoiceLine(invoiceLine);

                        var columnIndex = cell.Column.Index - 1;
                        this.Controls.Label<InvoiceLine>(cell.Row.Index, columnIndex++, invoiceLine, "Index");
                        this.Controls.TextBox<InvoiceLine>(cell.Row.Index, columnIndex++, invoiceLine, "Description");
                        this.Controls.TextBox<InvoiceLine>(cell.Row.Index, columnIndex++, invoiceLine, "Quantity");
                        this.Controls.TextBox<InvoiceLine>(cell.Row.Index, columnIndex++, invoiceLine, "UnitPrice");
                        this.Controls.Label<InvoiceLine>(cell.Row.Index, columnIndex++, invoiceLine, "TaxRate");

                        invoiceLine.Description = cell.ValueAsString;
                                                

                    }

                    // Delete
                    if (string.IsNullOrEmpty(cell.ValueAsString))
                    {
                        var invoiceLine = this.Invoice.InvoiceLines.FirstOrDefault(v => v.Index == cell.Row.Index);
                        this.Invoice.RemoveInvoiceLine(invoiceLine);

                        //TODO: cleanup
                        var controls = this.Controls.ControlByCell.Select(v => Equals(v.Key.Tag, invoiceLine));

                    }
                }

                this.Controls.Bind();

                await this.Sheet.Flush().ConfigureAwait(false);
            }
        }        

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
        public bool IsWorksheetUpToDate { get; set; }

        public async Task Refresh()
        {
            if(this.Invoice == null)
            {
                this.Invoice = (Invoice)this.program.Services.Database.Create<Invoice>(null);
                this.Invoice.InvoiceDate = DateTime.Now;
                this.Invoice.DeriveDueDate(Convert.ToInt32(this.program.Services.Configuration["InvoiceDueDate"]), this.program.Services.Configuration["InvoiceDueDateScheme"]);                
            }

            this.InvoiceLinesRange = this.NamedRanges.FirstOrDefault(r => $"{this.Sheet.Name}!Invoice_Lines".Equals(r.Name, StringComparison.OrdinalIgnoreCase));
            this.InvoiceLinesFirstColumn = this.InvoiceLinesRange.Column;
            
            var range = this.NamedRanges.FirstOrDefault(r => $"{this.Sheet.Name}!Invoice_Number".Equals(r.Name, StringComparison.OrdinalIgnoreCase));
            this.Controls.Label<Invoice>(range.Row, range.Column, this.Invoice, "InvoiceNumber");

            range = this.NamedRanges.FirstOrDefault(r => $"{this.Sheet.Name}!Invoice_Date".Equals(r.Name, StringComparison.OrdinalIgnoreCase));
            var cell = this.Controls.Label<Invoice>(range.Row, range.Column, this.Invoice, "InvoiceDate");
            cell.NumberFormat = "dd-MM-YYYY";

            range = this.NamedRanges.FirstOrDefault(r => $"{this.Sheet.Name}!Invoice_Duedate".Equals(r.Name, StringComparison.OrdinalIgnoreCase));
            cell = this.Controls.Label<Invoice>(range.Row, range.Column, this.Invoice, "InvoiceDueDate");
            cell.NumberFormat = "dd-MM-YYYY";

            if (this.Invoice.InvoiceLines.Any())
            {
                foreach(var invoiceLine in this.Invoice.InvoiceLines.OrderBy(v => v.Index))
                {
                    var columnIndex = this.InvoiceLinesRange.Column;
                    cell = this.Controls.Label<InvoiceLine>(this.InvoiceLinesRange.Row, columnIndex++, invoiceLine, "Index");
                    cell = this.Controls.TextBox<InvoiceLine>(this.InvoiceLinesRange.Row, columnIndex++, invoiceLine, "Description");
                    cell = this.Controls.TextBox<InvoiceLine>(this.InvoiceLinesRange.Row, columnIndex++, invoiceLine, "Quantity");
                    cell = this.Controls.TextBox<InvoiceLine>(this.InvoiceLinesRange.Row, columnIndex++, invoiceLine, "UnitPrice");
                    cell = this.Controls.Label<InvoiceLine>(this.InvoiceLinesRange.Row, columnIndex++, invoiceLine, "TaxRate");                                       
                }
            }

            this.Controls.Bind();

            await this.Sheet.Flush().ConfigureAwait(false);

            await Task.CompletedTask;
        }

        internal void SaveAsPDF()
        {
            var fileName = $"{this.program.Services.Configuration["Company_name"]}_{this.Invoice.InvoiceNumber}.pdf";

            var path = this.program.Services.Configuration["OutputDirectory"];

            var file = new FileInfo(Path.Combine(path, fileName));

            if (file.Exists)
            {
                if(this.program.Services.MessageService.Confirm($"{file.Name} already exist. Do you want to overwrite this file?"))
                {
                    this.Sheet.SaveAsPDF(file, overwriteExistingFile: true, false, ignorePrintAreas: false);
                }
            }
            else
            {
                this.Sheet.SaveAsPDF(file, false, false, ignorePrintAreas: false);
            }
        }

        internal async Task Save()
        {
            if (this.Invoice != null)
            {
                this.program.Services.Database.Save(this.Invoice);
            }

            await this.Refresh().ConfigureAwait(false);           
        }
    }
}
