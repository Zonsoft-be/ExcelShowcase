using Allors.Excel;
using Application.Sheets;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application
{
    public class Program : IProgram
    {

        public Program()
        {
            this.BookByWorkbook = new ConcurrentDictionary<IWorkbook, IBook>();
            this.SheetByWorksheet = new ConcurrentDictionary<IWorksheet, ISheet>();
        }

       

        public IAddIn AddIn { get; private set; }

        public ConcurrentDictionary<Guid, IWorkbook> Workbooks { get; } = new ConcurrentDictionary<Guid, IWorkbook>();

        public ConcurrentDictionary<Guid, IWorksheet> Worksheets { get; } = new ConcurrentDictionary<Guid, IWorksheet>();

        public IWorkbook ActiveWorkbook => this.AddIn.Workbooks.FirstOrDefault(v => v.IsActive);

        public IWorksheet ActiveWorksheet => this.ActiveWorkbook.Worksheets.FirstOrDefault(v => v.IsActive);

        public IDictionary<IWorksheet, ISheet> SheetByWorksheet { get; private set; }

        public IDictionary<IWorkbook, IBook> BookByWorkbook { get; private set; }

        /// <summary>
        /// Sets the Enabled value of control Id in the Ribbon. You should use some authentication and authorization infrastructure
        /// </summary>
        /// <param name="controlId"></param>
        /// <param name="controlTag"></param>
        /// <returns></returns>
        public bool IsEnabled(string controlId, string controlTag)
        {
            if (this.AddIn == null)
            {
                return false;
            }

            return true;
        }

        public async Task OnBeforeDelete(IWorksheet worksheet)
        {
            var kvp = this.Worksheets.FirstOrDefault(v => Equals(v.Value, worksheet));

            if (kvp.Key != null)
            {
                this.Worksheets.TryRemove(kvp.Key, out var result);
            }

            await Task.CompletedTask;
        }

        public void OnClose(IWorkbook workbook, ref bool cancel)
        {
            var kvp = this.Workbooks.FirstOrDefault(v => Equals(v.Value, workbook));

            if (kvp.Key != null)
            {
                this.Workbooks.TryRemove(kvp.Key, out var result);
            }
        }

        public async Task OnHandle(string handle, params object[] argument)
        {
            switch (handle)
            {
                case "AddProductSheet":

                    var ws = this.ActiveWorkbook.AddWorksheet();
                    ws.Name = "Product Sheet";
                    var productSheet = new ProductSheet(this, ws);
                    this.SheetByWorksheet.Add(ws, productSheet);

                    await productSheet.Refresh().ConfigureAwait(false);

                    break;

            }            
        }

        public async Task OnLogin()
        {
            await Task.CompletedTask;
        }

        public async Task OnLogout()
        {
            await Task.CompletedTask;
        }

        public async Task OnNew(IWorkbook workbook)
        {
            this.Workbooks.TryAdd(Guid.NewGuid(), workbook);

            await Task.CompletedTask;
        }

        public async Task OnNew(IWorksheet worksheet)
        {
            this.Worksheets.TryAdd(Guid.NewGuid(), worksheet);

            await Task.CompletedTask;
        }

        public async Task OnStart(IAddIn addIn)
        {
            await Task.Run( () => { this.AddIn = addIn; });            
        }

        public async Task OnStop()
        {
            this.Worksheets?.Clear();
            this.Workbooks?.Clear();

            // Place for cleaning up your stuff.
            await Task.CompletedTask;
        }
    }
}
