using Allors.Excel;
using Application.Models;
using Application.Ui;
using Application.Ui.GenericControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Application.Sheets
{
    public class ProductSheet : ISheet
    {
        private Program program;

        public IWorksheet Sheet { get; }

        public ProductSheet(Program program, IWorksheet worksheet)
        {
            this.program = program;
            this.Sheet = worksheet;

            this.Controls = new Controls(worksheet);

            this.Binder = new Binder(this.Sheet, Constants.ChangedStyle);
            this.Binder.ToDomained += this.Binder_ToDomained;

            this.Sheet.SheetActivated += this.Sheet_SheetActivated;
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

        public Product[] Products { get; set; } 

        public async Task Refresh()
        {
            this.Products = this.program.Services.Database.Get<Product>();

            //
            this.Controls.Static(0, 0, "Product ID");
            this.Controls.Static(0, 1, "Name");
            this.Controls.Static(0, 2, "Qty");
            this.Controls.Static(0, 3, "Price/Unit");
            this.Controls.Static(0, 4, "Unit");

            this.Sheet.FreezePanes(new Range(0, -1, 0, 0));

            var rowIndex = 1;
                       
            foreach (var product in this.Products)
            {
                this.Controls.Static(rowIndex, 0, product.Id.ToString());
                this.Controls.Static(rowIndex, 1, product.Name);                        
                this.Controls.Static(rowIndex, 2, product.Quantity);
                var icell = this.Controls.Static(rowIndex, 3, product.UnitPrice);
                icell.NumberFormat = "##0.00";

                this.Controls.Static(rowIndex, 4, product.Unit);

                rowIndex++;
            }

            {
                rowIndex++;

                var icell = this.Controls.Static(rowIndex, 2, this.Products.Sum(v => v.Quantity ));
                icell.Comment = "Total Quantity";

                icell = this.Controls.Static(rowIndex, 3, this.Products.Average(v => v.UnitPrice));
                icell.Comment = "Average Price";
            }        
           
           
            this.Controls.Bind();

            await this.Sheet.Flush().ConfigureAwait(false);

            await Task.CompletedTask;
        }
    }
}
