using Allors.Excel;
using Application.Models;
using Application.Services;
using Application.Sheets;
using Application.Ui;
using Application.Services;
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
            this.SheetByWorksheet = new ConcurrentDictionary<IWorksheet, ISheet>();
        }
        
        public IAddIn AddIn { get; private set; }

        public ConcurrentDictionary<Guid, IWorkbook> Workbooks { get; } = new ConcurrentDictionary<Guid, IWorkbook>();

        public ConcurrentDictionary<Guid, IWorksheet> Worksheets { get; } = new ConcurrentDictionary<Guid, IWorksheet>();

        public IWorkbook ActiveWorkbook => this.AddIn.Workbooks.FirstOrDefault(v => v.IsActive);

        public IWorksheet ActiveWorksheet => this.ActiveWorkbook.Worksheets.FirstOrDefault(v => v.IsActive);

        public IDictionary<IWorksheet, ISheet> SheetByWorksheet { get; private set; }

        public IServiceLocator Services { get; }

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

            if (controlId == "SaveAsPDFInvoiceSheet")
            {
                if (this.ActiveWorksheet == null)
                {
                    return false;
                }
                else
                {
                    this.SheetByWorksheet.TryGetValue(this.ActiveWorksheet, out var isheet);
                    return isheet is InvoiceSheet;
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

                case "InvoicesSheet":
                    {
                        var kvp = this.SheetByWorksheet.FirstOrDefault(v => Equals(v.Key.Workbook, this.ActiveWorkbook) && v.Value is InvoicesSheet);

                        InvoicesSheet invoicesSheet;

                        if (kvp.Value == null)
                        {
                            var iWorksheet = this.ActiveWorkbook.AddWorksheet(0);
                            iWorksheet.Name = KnownNames.InvoicesSheetName;
                            invoicesSheet = new InvoicesSheet(this, iWorksheet);                            

                            this.SheetByWorksheet.Add(iWorksheet, invoicesSheet);
                        }
                        else
                        {
                            invoicesSheet = (InvoicesSheet)this.SheetByWorksheet[kvp.Key];
                        }

                        await invoicesSheet.Refresh().ConfigureAwait(false);

                        invoicesSheet.Sheet.IsActive = true;
                    }

                    break;
                case "AddInvoiceSheet":
                    {
                        var wsCount = this.Services.Database.Count<Invoice>();

                        var iWorksheet = this.ActiveWorksheet;

                        var invoiceSheet = new InvoiceSheet(this, iWorksheet);
                       
                        this.SheetByWorksheet.Add(iWorksheet, invoiceSheet);

                        await invoiceSheet.Refresh().ConfigureAwait(false);

                        //invoiceSheet.Sheet.IsActive = true;
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

                case "SaveInvoiceSheet":
                    {
                        if (this.SheetByWorksheet.TryGetValue(this.ActiveWorksheet, out var iSheet))
                        {
                            if (iSheet is InvoiceSheet invoiceSheet)
                            {
                                await invoiceSheet.Save();
                            }
                        }

                        foreach(ISheet sheet in this.SheetByWorksheet.Where(v => v.Value is InvoicesSheet).Select(v => v.Value))
                        {
                            sheet.IsWorksheetUpToDate = false;
                        }
                    }
                    break;
                                    
                case "SaveOrganisationsSheet":
                    {
                        if (this.SheetByWorksheet.TryGetValue(this.ActiveWorksheet, out var iSheet))
                        {
                            if (iSheet is OrganisationsSheet organisationsSheet)
                            {
                                await organisationsSheet.Save();
                            }
                        }

                        foreach (ISheet sheet in this.SheetByWorksheet.Where(v => v.Value is OrganisationsSheet).Select(v => v.Value))
                        {
                            sheet.IsWorksheetUpToDate = false;
                        }
                    }
                    break;
                case "SaveAsPDFInvoiceSheet":
                    {
                        if (this.SheetByWorksheet.TryGetValue(this.ActiveWorksheet, out var iSheet))
                        {
                            if (iSheet is InvoiceSheet invoiceSheet)
                            {
                                invoiceSheet.SaveAsPDF();


                            }
                        }
                    }
                    break;

                case "OrganisationsSheet":
                    {
                        var kvp = this.SheetByWorksheet.FirstOrDefault(v => Equals(v.Key.Workbook, this.ActiveWorkbook) && v.Value is OrganisationsSheet);

                        OrganisationsSheet organisationsSheet;

                        if (kvp.Value == null)
                        {
                            var iWorksheet = this.ActiveWorkbook.AddWorksheet(0);
                            iWorksheet.Name = "Organisations";
                            organisationsSheet = new OrganisationsSheet(this, iWorksheet);                          

                            this.SheetByWorksheet.Add(iWorksheet, organisationsSheet);
                        }
                        else
                        {
                            organisationsSheet = (OrganisationsSheet)this.SheetByWorksheet[kvp.Key];
                        }

                        await organisationsSheet.Refresh().ConfigureAwait(false);

                        organisationsSheet.Sheet.IsActive = true;
                    }

                    break;

                case "PaymentTermsSheet":
                    {
                        var kvp = this.SheetByWorksheet.FirstOrDefault(v => Equals(v.Key.Workbook, this.ActiveWorkbook) && v.Value is PaymentTermsSheet);

                        PaymentTermsSheet paymentTermsSheet;

                        if (kvp.Value == null)
                        {
                            var iWorksheet = this.ActiveWorkbook.AddWorksheet(0);
                            iWorksheet.Name = KnownNames.PaymentTermsSheetName;
                            paymentTermsSheet = new PaymentTermsSheet(this, iWorksheet);

                            this.SheetByWorksheet.Add(iWorksheet, paymentTermsSheet);
                        }
                        else
                        {
                            paymentTermsSheet = (PaymentTermsSheet)this.SheetByWorksheet[kvp.Key];
                        }

                        await paymentTermsSheet.Refresh().ConfigureAwait(false);

                        paymentTermsSheet.Sheet.IsActive = true;
                    }

                    break;

            }
        }

        public async Task AddAppConfigSheet()
        {
            var kvp = this.SheetByWorksheet.FirstOrDefault(v => Equals(v.Key.Workbook, this.ActiveWorkbook) && v.Value is AppConfigSheet);

            AppConfigSheet appConfigSheet;

            if (kvp.Value == null)
            {
                var iWorksheet = this.ActiveWorkbook.AddWorksheet(0);
                iWorksheet.Name = KnownNames.AppConfigSheetName;
                appConfigSheet = new AppConfigSheet(this, iWorksheet);

                this.SheetByWorksheet.Add(iWorksheet, appConfigSheet);
            }
            else
            {
                appConfigSheet = (AppConfigSheet)this.SheetByWorksheet[kvp.Key];
            }

            await appConfigSheet.Refresh().ConfigureAwait(false);

            appConfigSheet.Sheet.IsVisible = false;
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
