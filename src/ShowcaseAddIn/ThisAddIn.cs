using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Excel = Microsoft.Office.Interop.Excel;
using Office = Microsoft.Office.Core;
using Microsoft.Office.Tools.Excel;
using Microsoft.Office.Core;
using Application;
using Allors.Excel.Embedded;
using ProductManager.Services;

namespace ProductManager
{
    public partial class ThisAddIn
    {
        public Ribbon Ribbon { get; set; }

        public AddIn AddIn { get; private set; }

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

            this.AddIn = new Allors.Excel.Embedded.AddIn(this.Application, program, office);

            this.Ribbon.AddIn = this.AddIn;

            ((Microsoft.Office.Interop.Excel.AppEvents_Event)this.Application).NewWorkbook += ThisAddIn_NewWorkbook;

            ((Microsoft.Office.Interop.Excel.AppEvents_Event)this.Application).SheetActivate += ThisAddIn_SheetActivate;

            await program.OnStart(this.AddIn);
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
