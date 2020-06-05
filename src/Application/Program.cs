﻿using Allors.Excel;
using Application.Services;
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

        public Program(IServiceLocator services)
        {
            this.Services = services;
            this.BookByWorkbook = new ConcurrentDictionary<IWorkbook, IBook>();
            this.SheetByWorksheet = new ConcurrentDictionary<IWorksheet, ISheet>();
        }
        
        public IAddIn AddIn { get; private set; }

        public ConcurrentDictionary<Guid, IWorkbook> Workbooks { get; } = new ConcurrentDictionary<Guid, IWorkbook>();

        public ConcurrentDictionary<Guid, IWorksheet> Worksheets { get; } = new ConcurrentDictionary<Guid, IWorksheet>();

        public IWorkbook ActiveWorkbook => this.AddIn.Workbooks.FirstOrDefault(v => v.IsActive);

        public IWorksheet ActiveWorksheet => this.ActiveWorkbook.Worksheets.FirstOrDefault(v => v.IsActive);

        public IDictionary<IWorksheet, ISheet> SheetByWorksheet { get; private set; }

        public IServiceLocator Services { get; }

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

            if(controlId == "InsertPicture")
            {
                if(this.ActiveWorksheet == null)
                {
                    return false;
                }
                else
                {
                    this.SheetByWorksheet.TryGetValue(this.ActiveWorksheet, out var isheet);
                    return isheet is DemoSheet;
                }               
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
                    {
                        var kvp = this.SheetByWorksheet.FirstOrDefault(v => Equals(v.Key.Workbook, this.ActiveWorkbook) && v.Value is ProductSheet);
                        
                        ProductSheet productSheet;

                        if (kvp.Value == null)
                        {                      
                            var iWorksheet = this.ActiveWorkbook.AddWorksheet(0);
                            iWorksheet.Name = "Products";
                            productSheet = new ProductSheet(this, iWorksheet);
                            this.SheetByWorksheet.Add(iWorksheet, productSheet);
                        }
                        else
                        {
                            productSheet = (ProductSheet) this.SheetByWorksheet[kvp.Key];
                        }

                        await productSheet.Refresh().ConfigureAwait(false);

                        productSheet.Sheet.IsActive = true;
                    }                   

                    break;

                case "ListCovid19Sheet":
                    {
                        var kvp = this.SheetByWorksheet.FirstOrDefault(v => Equals(v.Key.Workbook, this.ActiveWorkbook) && v.Value is Covid19Sheet);

                        Covid19Sheet covid19Sheet = null;
                        if (kvp.Value == null)
                        {                            
                            var ws = this.ActiveWorkbook.AddWorksheet(0);
                            ws.Name = "Covid19";
                            covid19Sheet = new Covid19Sheet(this, ws);
                            this.SheetByWorksheet.Add(ws, covid19Sheet);
                        }
                        else
                        {
                            covid19Sheet = (Covid19Sheet) this.SheetByWorksheet[kvp.Key];
                        }

                        await covid19Sheet.Refresh().ConfigureAwait(false);

                        covid19Sheet.Sheet.IsActive = true;
                    }                   

                    break;

                case "AddDemoSheet":
                    {
                        var wsCount = this.SheetByWorksheet.Count(v => Equals(v.Key.Workbook, this.ActiveWorkbook) && v.Value is DemoSheet);

                        var iWorksheet = this.ActiveWorkbook.AddWorksheet(0);
                        iWorksheet.Name = $"Demo {wsCount}";
                        var demoSheet = new DemoSheet(this, iWorksheet);
                        this.SheetByWorksheet.Add(iWorksheet, demoSheet);

                        await demoSheet.Refresh().ConfigureAwait(false);

                        demoSheet.Sheet.IsActive = true;
                    }
                    break;

                case "InsertPicture":
                    {
                        if(this.SheetByWorksheet.TryGetValue(this.ActiveWorksheet, out var iSheet))
                        {
                            if(iSheet is DemoSheet demoSheet)
                            {
                                demoSheet.InsertPicture();
                            }
                        }                        
                    }
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