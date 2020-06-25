using System;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.Office.Core;
using Application;
using ProductManager.Services;
using Allors.Excel.Interop;
using InteropWorkbook = Microsoft.Office.Interop.Excel.Workbook;
using InteropWorksheet = Microsoft.Office.Interop.Excel.Worksheet;
using System.Linq;
using Application.Sheets;

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

                foreach (IWorksheet iworkSheet in iworkbook.Worksheets)
                {
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

        private async void ThisAddIn_WorkbookOpen(Excel.Workbook Wb)
        {
            // this has been marked as a showCase workbook. So threat it as one we know.
            var iWorkbook = this.AddIn.New(Wb);

            object result = null;
            if(iWorkbook.TryGetCustomProperty(AppConstants.KeyWorkbook, ref result))
            {
                if (Convert.ToBoolean(result))
                {
                    foreach (InteropWorksheet interopWorksheet in Wb.Sheets)
                    {
                        var worksheet = new Allors.Excel.Interop.Worksheet(iWorkbook, interopWorksheet);

                        var customProperties = worksheet.GetCustomProperties();
                        if (customProperties.Any(v =>Equals(AppConstants.KeySheet, v.Key) &&  Equals(nameof(InvoicesSheet), v.Value)))
                        {
                            var invoicesSheet = new InvoicesSheet(this.AddIn.Program, worksheet);

                            await invoicesSheet.Load(iWorkbook);

                            ((Program)this.AddIn.Program).SheetByWorksheet.Add(worksheet, invoicesSheet);
                        }

                        if (customProperties.Any(v => Equals(AppConstants.KeySheet, v.Key) && Equals(nameof(OrganisationsSheet), v.Value)))
                        {
                            var organisationsSheet = new OrganisationsSheet(this.AddIn.Program, worksheet);

                            await organisationsSheet.Load(iWorkbook);

                            ((Program)this.AddIn.Program).SheetByWorksheet.Add(worksheet, organisationsSheet);
                        }
                    }                    
                }
            }
            else
            {
               //TODO: remove the iWorkbook
            }
        }

        private void ThisAddIn_SheetActivate(object Sh)
        {
            this.Ribbon.Invalidate();
        }

        private void ThisAddIn_NewWorkbook(Excel.Workbook Wb)
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
