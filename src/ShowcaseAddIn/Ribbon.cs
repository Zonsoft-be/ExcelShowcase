using Application;
using ProductManager.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Policy;
using System.Text;
using System.Windows.Forms.VisualStyles;
using Office = Microsoft.Office.Core;

// TODO:  Follow these steps to enable the Ribbon (XML) item:

// 1: Copy the following code block into the ThisAddin, ThisWorkbook, or ThisDocument class.

//  protected override Microsoft.Office.Core.IRibbonExtensibility CreateRibbonExtensibilityObject()
//  {
//      return new Ribbon();
//  }

// 2. Create callback methods in the "Ribbon Callbacks" region of this class to handle user
//    actions, such as clicking a button. Note: if you have exported this Ribbon from the Ribbon designer,
//    move your code from the event handlers to the callback methods and modify the code to work with the
//    Ribbon extensibility (RibbonX) programming model.

// 3. Assign attributes to the control tags in the Ribbon XML file to identify the appropriate callback methods in your code.  

// For more information, see the Ribbon XML documentation in the Visual Studio Tools for Office Help.


namespace ProductManager
{
    [ComVisible(true)]
    public class Ribbon : Office.IRibbonExtensibility
    {
        private Office.IRibbonUI ribbon;

        public Ribbon()
        {
        }

        public Allors.Excel.Interop.AddIn AddIn { get; set; }
        public ServiceLocator Services { get; internal set; }

        public async void OnClick(Office.IRibbonControl control)
        {
            if (this.AddIn != null)
            {
                await this.AddIn.Program.OnHandle(control.Id);

                this.Invalidate();
            }
        }

        public async void AddInvoiceSheet(Office.IRibbonControl control)
        {
            if (this.AddIn != null)
            {
                string fileName = this.Services.Configuration["TemplateFile"];

                var templateFile = new FileInfo(fileName);

                if (templateFile.Exists)
                {
                    var targetWorkbook = this.AddIn.Application.ActiveWorkbook;
                    var index = targetWorkbook.Sheets.Count;

                    var template = this.AddIn.Application.Workbooks.Open(templateFile.FullName);
                    var invoiceTemplate = (Microsoft.Office.Interop.Excel.Worksheet)template.Worksheets[1];
                    invoiceTemplate.Copy(After: targetWorkbook.Sheets[index]);
                    template.Close(SaveChanges: false);

                    var copied = (Microsoft.Office.Interop.Excel.Worksheet)targetWorkbook.Sheets[index + 1];                                       
                    
                    var wb = this.AddIn.WorkbookByInteropWorkbook[targetWorkbook];
                    wb.New(copied);
                }

                await this.AddIn.Program.OnHandle(control.Id);

                this.Invalidate();
            }
        }

        /// <summary>
        /// Invalidate will have excel call the GetEnabled handler. Either for all controls, or for the controls you pass as a parameter
        /// </summary>
        /// <param name="controlIds"></param>
        public void Invalidate(params string[] controlIds)
        {
            if(!controlIds.Any())
            {
                this.ribbon.Invalidate();
            }
            else
            {
                foreach (var controlId in controlIds)
                {
                    this.ribbon.InvalidateControl(controlId);
                }
            }
        }

        public bool GetEnabled(Office.IRibbonControl control)
        {
            return this.AddIn?.Program?.IsEnabled(control.Id, control.Tag) ?? false;
        }

        #region IRibbonExtensibility Members

        public string GetCustomUI(string ribbonID)
        {
            return GetResourceText("ProductManager.Ribbon.xml");
        }

        #endregion

        #region Ribbon Callbacks
        //Create callback methods here. For more information about adding callback methods, visit https://go.microsoft.com/fwlink/?LinkID=271226

        public void Ribbon_Load(Office.IRibbonUI ribbonUI)
        {
            this.ribbon = ribbonUI;
        }

        #endregion

        #region Helpers

        private static string GetResourceText(string resourceName)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            string[] resourceNames = asm.GetManifestResourceNames();
            for (int i = 0; i < resourceNames.Length; ++i)
            {
                if (string.Compare(resourceName, resourceNames[i], StringComparison.OrdinalIgnoreCase) == 0)
                {
                    using (StreamReader resourceReader = new StreamReader(asm.GetManifestResourceStream(resourceNames[i])))
                    {
                        if (resourceReader != null)
                        {
                            return resourceReader.ReadToEnd();
                        }
                    }
                }
            }
            return null;
        }

        #endregion
    }
}
