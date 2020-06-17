using Allors.Excel;
using Application.Ui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Dynamic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;

namespace Application.Sheets
{
    public class Covid19Sheet : ISheet
    {
        private Program program;

        public IWorksheet Sheet { get; }

        public Covid19Sheet(Program program, IWorksheet worksheet)
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
            Covid19Data Data = await this.program.Services.HttpService.Load<Covid19Data>("https://corona-api.com", "/countries");
            
            var columnIndex = 0;
            this.Controls.Static(0, columnIndex++, "Code");
            this.Controls.Static(0, columnIndex++, "Name");
            this.Controls.Static(0, columnIndex++, "Population");
            this.Controls.Static(0, columnIndex++, "Updated at");
            this.Controls.Static(0, columnIndex++, "Deaths");
            this.Controls.Static(0, columnIndex++, "Confirmed");
            this.Controls.Static(0, columnIndex++, "Recoverd");
            this.Controls.Static(0, columnIndex++, "Critical");
            this.Controls.Static(0, columnIndex++, "Today Confirmed");
            this.Controls.Static(0, columnIndex++, "Today Deaths");

            // Freeze topRow
            this.Sheet.FreezePanes(new Range(0, -1, 0,0));

            var row = 1;
            foreach (var data in Data.data.OrderBy(v => v.name))
            {
                columnIndex = 0;
                this.Controls.Static(row, columnIndex++, data.code);
                this.Controls.Static(row, columnIndex++, data.name);
                this.Controls.Static(row, columnIndex++, data.population);
                this.Controls.Static(row, columnIndex++, data.updated_at.ToString("d-M-yy h:mm"));
                this.Controls.Static(row, columnIndex++, data.latest_data.deaths);
                this.Controls.Static(row, columnIndex++, data.latest_data.confirmed);
                this.Controls.Static(row, columnIndex++, data.latest_data.recovered);
                this.Controls.Static(row, columnIndex++, data.latest_data.critical);
                this.Controls.Static(row, columnIndex++, data.today.confirmed);
                this.Controls.Static(row, columnIndex++, data.today.deaths);

                row++;
            }

            this.Controls.Bind();

            await this.Sheet.Flush().ConfigureAwait(false);

            await Task.CompletedTask;
        }
    }
}
