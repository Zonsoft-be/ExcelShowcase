using Allors.Excel;
using Application.Ui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
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
            this.Sheet.SetNamedRange("Bing_Image", new Allors.Excel.Range(1, 0, 20, 20, this.Sheet));           

            this.Controls.Bind();

            await this.Sheet.Flush().ConfigureAwait(false);

            await Task.CompletedTask;
        }

        public async void InsertPicture()
        {
            BingDailyImage Data = await this.program.Services.HttpService.Load<BingDailyImage>("https://www.bing.com", "/HPImageArchive.aspx?format=js&idx=0&n=1&mkt=en-US");

            if (Data.images.Any())
            {
                var imageJson = Data.images[0];

                if (!string.IsNullOrEmpty(imageJson.url))
                {
                    var imageUrl = string.Concat("https://www.bing.com", imageJson.url);
                
                    // Some static text
                    this.Controls.Static(0, 0, imageJson.copyright);

                    // Insert the image in the namedRange location
                    var rectangle = this.Sheet.GetRectangle("Bing_Image");
                                        
                    this.Sheet.AddPicture(imageUrl, rectangle);
                }
            }

            this.Controls.Bind();

            await this.Sheet.Flush().ConfigureAwait(false);

            await Task.CompletedTask;

        }
    }
}
