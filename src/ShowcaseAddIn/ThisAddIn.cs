using System;
using Application;
using ProductManager.Services;
using InteropWorkbook = Microsoft.Office.Interop.Excel.Workbook;
using InteropWorksheet = Microsoft.Office.Interop.Excel.Worksheet;
using System.Linq;
using Application.Sheets;
using Application.Models;
using Allors.Excel;
using Microsoft.Office.Core;
using Allors.Excel.Interop;
using Application.Services;

namespace ProductManager
{
    public partial class ThisAddIn
    {
        public Ribbon Ribbon { get; set; }

        public Allors.Excel.Interop.AddIn AddIn { get; private set; }

        protected override IRibbonExtensibility CreateRibbonExtensibilityObject()
        {
            this.Ribbon = new Ribbon();

            return this.Ribbon;
        }

        private async void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            var services = new ServiceLocator();
            
            var office = new OfficeDecorator(this);

            var program = new Program(services);

            this.AddIn = new Allors.Excel.Interop.AddIn(this.Application, program, office);

            this.Ribbon.AddIn = this.AddIn;
            this.Ribbon.Services = services;

            ((Microsoft.Office.Interop.Excel.AppEvents_Event)this.Application).NewWorkbook += ThisAddIn_NewWorkbook;

            ((Microsoft.Office.Interop.Excel.AppEvents_Event)this.Application).SheetActivate += ThisAddIn_SheetActivate;

            ((Microsoft.Office.Interop.Excel.AppEvents_Event)this.Application).WorkbookOpen += ThisAddIn_WorkbookOpen;

            ((Microsoft.Office.Interop.Excel.AppEvents_Event)this.Application).WorkbookBeforeSave += ThisAddIn_WorkbookBeforeSave;     

            await program.OnStart(this.AddIn);
        }

        private void ThisAddIn_WorkbookBeforeSave(InteropWorkbook Wb, bool SaveAsUI, ref bool Cancel)
        {
            if (this.AddIn.WorkbookByInteropWorkbook.TryGetValue(Wb, out Workbook iworkbook))
            {
                iworkbook.TrySetCustomProperty(AppConstants.KeyWorkbook, true);

                foreach (Worksheet iworkSheet in iworkbook.Worksheets)
                {
                    var paymentTermsSheet = ((Program)this.AddIn.Program).SheetByWorksheet.FirstOrDefault(w => Equals(iworkSheet, w.Key) && w.Value is PaymentTermsSheet).Value;

                    if (paymentTermsSheet != null)
                    {
                        ((PaymentTermsSheet)paymentTermsSheet).SaveTo(iworkbook);
                    }

                    var invoicesSheet = ((Program)this.AddIn.Program).SheetByWorksheet.FirstOrDefault(w => Equals(iworkSheet, w.Key) && w.Value is InvoicesSheet).Value;

                    if (invoicesSheet != null)
                    {
                        ((InvoicesSheet)invoicesSheet).SaveTo(iworkbook);
                    }

                    var organisationsSheet = ((Program)this.AddIn.Program).SheetByWorksheet.FirstOrDefault(w => Equals(iworkSheet, w.Key) && w.Value is OrganisationsSheet).Value;

                    if (organisationsSheet != null)
                    {
                        ((OrganisationsSheet)organisationsSheet).SaveTo(iworkbook);
                    }
                  
                }
            }
        }      

        private async void ThisAddIn_WorkbookOpen(InteropWorkbook Wb)
        {
            // this has been marked as a showCase workbook. So threat it as one we know.
            var iWorkbook = this.AddIn.New(Wb);

            object result = null;
            if(iWorkbook.TryGetCustomProperty(AppConstants.KeyWorkbook, ref result))
            {
                if (Convert.ToBoolean(result))
                {
                    // Check the Custom Properties for existing data, and if so, instantiate those sheets.
                    object tagId = null;

                    {
                        //// We need to have an InvoicesSheet
                        //var interopWorksheet = this.GetWorkSheet(Wb, iWorkbook, nameof(AppConfigSheet));
                        //if (interopWorksheet == null)
                        //{
                        //    var iWorksheet = iWorkbook.AddWorksheet(0);
                        //    iWorksheet.Name = KnownNames.AppConfigSheetName;
                        //    interopWorksheet = Wb.ActiveSheet;
                        //}

                        //var worksheet = new Allors.Excel.Interop.Worksheet(iWorkbook, interopWorksheet);
                        //var appConfigSheet = new AppConfigSheet(this.AddIn.Program, worksheet);

                        //await appConfigSheet.Refresh();

                        //((Program)this.AddIn.Program).SheetByWorksheet.Add(worksheet, appConfigSheet);                      
                    }

                    if (iWorkbook.TryGetCustomProperty(KnownNames.PaymentTermTag, ref tagId))
                    {
                        // We need to have an InvoicesSheet
                        var interopWorksheet = this.GetWorkSheet(Wb, iWorkbook, nameof(PaymentTermsSheet));
                        if (interopWorksheet == null)
                        {
                            var iWorksheet = iWorkbook.AddWorksheet(0);
                            iWorksheet.Name = KnownNames.PaymentTermsSheetName;
                            interopWorksheet = Wb.ActiveSheet;
                        }

                        var worksheet = new Allors.Excel.Interop.Worksheet(iWorkbook, interopWorksheet);
                        var paymentTermsSheet = new PaymentTermsSheet(this.AddIn.Program, worksheet);

                        await paymentTermsSheet.Load(iWorkbook);

                        ((Program)this.AddIn.Program).SheetByWorksheet.Add(worksheet, paymentTermsSheet);
                    }

                    if(iWorkbook.TryGetCustomProperty(KnownNames.InvoiceTag, ref tagId))
                    {
                        // We need to have an OrganisationsSheet
                        var interopWorksheet = this.GetWorkSheet(Wb, iWorkbook, nameof(InvoicesSheet));

                        if (interopWorksheet == null)
                        {
                            var iWorksheet = iWorkbook.AddWorksheet(0);
                            iWorksheet.Name = KnownNames.InvoicesSheetName;
                            interopWorksheet = Wb.ActiveSheet;

                        }

                        var worksheet = new Allors.Excel.Interop.Worksheet(iWorkbook, interopWorksheet);
                        var invoicesSheet = new InvoicesSheet(this.AddIn.Program, worksheet);

                        await invoicesSheet.Load(iWorkbook);

                        ((Program)this.AddIn.Program).SheetByWorksheet.Add(worksheet, invoicesSheet);                       
                    }

                    if (iWorkbook.TryGetCustomProperty(KnownNames.OrganisationTag, ref tagId))
                    {
                        // We need to have an InvoicesSheet
                        var interopWorksheet = this.GetWorkSheet(Wb, iWorkbook, nameof(OrganisationsSheet));
                        if (interopWorksheet == null)
                        {
                            var iWorksheet = iWorkbook.AddWorksheet(0);
                            iWorksheet.Name = KnownNames.OrganisationsSheetName;
                            interopWorksheet = Wb.ActiveSheet;
                        }

                        var worksheet = new Allors.Excel.Interop.Worksheet(iWorkbook, interopWorksheet);
                        var organisationsSheet = new OrganisationsSheet(this.AddIn.Program, worksheet);

                        await organisationsSheet.Load(iWorkbook);

                        ((Program)this.AddIn.Program).SheetByWorksheet.Add(worksheet, organisationsSheet);
                    }
                }
            }
            else
            {
               //TODO: remove the iWorkbook
            }
        }

        private InteropWorksheet GetWorkSheet(InteropWorkbook workBook, Workbook workbook, string nameOfsheet)
        {
            foreach (InteropWorksheet interopWorksheet in workBook.Sheets)
            {
                var ws = new Allors.Excel.Interop.Worksheet(workbook, interopWorksheet);

                var customProperties = ws.GetCustomProperties();
                if (customProperties.Any(v => Equals(AppConstants.KeySheet, v.Key) && Equals(nameOfsheet, v.Value)))
                {
                    return interopWorksheet;
                }            
            }

            return null;
        }

        private void ThisAddIn_SheetActivate(object Sh)
        {
            this.Ribbon.Invalidate();
        }

        private void ThisAddIn_NewWorkbook(InteropWorkbook Wb)
        {
            // Do stuff when a new workbook is added.
        }

        private async void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            if (this.AddIn?.Program != null)
            {
                await System.Threading.Tasks.Task.Run(async () =>
                {
                    await this.AddIn.Program.OnStop();
                });
            }
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
