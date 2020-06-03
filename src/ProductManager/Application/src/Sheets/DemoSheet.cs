using Allors.Excel;
using Application.Ui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Application.Sheets
{
    public class DemoSheet : ISheet
    {
        private Program program;

        public IWorksheet Sheet { get; }

        public DemoSheet(Program program, IWorksheet worksheet)
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

        public async Task Refresh()
        {
            this.Sheet.SetNamedRange($"{this.Sheet.Name}_Image", new Range(5, 10, 20, 10, this.Sheet));

            ////
            //this.Controls.Static(0, 0, "Product ID");
            //this.Controls.Static(0, 1, "Name");
            //this.Controls.Static(0, 2, "Qty");
            //this.Controls.Static(0, 3, "Price");

            //var randomQty = new Random(124578);
            //foreach(int index in Enumerable.Range(1, 100000))
            //{
            //    this.Controls.Static(index, 0, $"ID_{index}");
            //    this.Controls.Static(index, 1, $"Name {index}");
            //    this.Controls.Static(index, 2, randomQty.Next(10000));
            //    var icell = this.Controls.Static(index, 3, new decimal(randomQty.Next(0, 1000) * randomQty.NextDouble()));
            //    icell.NumberFormat = "##0.00";
            //}

            this.Controls.Bind();

            await this.Sheet.Flush().ConfigureAwait(false);

            await Task.CompletedTask;
        }

        public void InsertPicture()
        {
            string urlName = null;
            using (var dlg = new OpenFileDialog())
            {
                dlg.CheckFileExists = false;

                dlg.ShowDialog();
                urlName = dlg.FileName;
                urlName = Path.GetFileName(urlName);
            }

            if (!string.IsNullOrEmpty(urlName))
            {
                var rectangle = this.Sheet.GetRectangle($"{this.Sheet.Name}_Image");
                this.Sheet.AddPicture(urlName, rectangle);
            }
        }
    }
}
