using Allors.Excel;
using Application.Models;
using Application.Ui;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Application.Sheets
{
    public class InvoicesSheet : ISheet
    {
        private Program program;

        public IWorksheet Sheet { get; }

        public InvoicesSheet(IProgram program, IWorksheet worksheet)
        {
            this.program = (Program)program;
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

        public Invoice[] Invoices { get; set; } 

        public async Task Refresh()
        {
            this.Invoices = this.program.Services.Database.Get<Invoice>();

            await RefreshSheet().ConfigureAwait(false);
        }

        private async Task RefreshSheet()
        {
            //
            this.Controls.Static(0, 0, "Invoice ID");
            this.Controls.Static(0, 1, "Number");
            this.Controls.Static(0, 2, "Date");
            this.Controls.Static(0, 3, "Customer");
            this.Controls.Static(0, 4, "Net");
            this.Controls.Static(0, 5, "VAT");
            this.Controls.Static(0, 6, "Total");

            this.Sheet.FreezePanes(new Range(0, -1, 0, 0));

            var rowIndex = 1;

            foreach (var invoice in this.Invoices)
            {
                this.Controls.Static(rowIndex, 0, invoice.Id.ToString());
                this.Controls.Static(rowIndex, 1, invoice.InvoiceNumber);
                this.Controls.Static(rowIndex, 2, invoice.InvoiceDate.ToShortDateString());
                this.Controls.Static(rowIndex, 3, invoice.Customer?.Name);
                this.Controls.Static(rowIndex, 4, invoice.NetAmount);
                this.Controls.Static(rowIndex, 5, invoice.Tax);
                this.Controls.Static(rowIndex, 6, invoice.Total);

                rowIndex++;
            }

            this.Controls.Bind();

            await this.Sheet.Flush().ConfigureAwait(false);

            await Task.CompletedTask;
        }

        public async Task Load(IWorkbook iWorkbook)
        {
            object tagId = null;

            if( iWorkbook.TryGetCustomProperty(KnownNames.InvoiceTag, ref tagId))
            {
                var xmlDocument = iWorkbook.GetCustomXMLById(Convert.ToString(tagId));
                if(xmlDocument != null)
                {
                    var root = xmlDocument.DocumentElement.Name;
                    XmlSerializer serializer = new XmlSerializer(typeof(Invoice[]), new XmlRootAttribute(root));

                    StringReader stringReader = new StringReader(xmlDocument.OuterXml);

                    var existingInvoices = (Invoice[])serializer.Deserialize(stringReader);
                    this.program.Services.Database.Store<Invoice>(existingInvoices);

                    await Refresh().ConfigureAwait(false);
                }
            }          
        }


        public void SaveTo(IWorkbook iWorkbook)
        {
            object tagId = null;

            // Create the XML Document that we will save in the XML parts of the workbook
            string outputXml = null;       
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(typeof(Invoice[]));
                serializer.Serialize(stringwriter, this.Invoices);

                outputXml = stringwriter.ToString();

            }

            // Check if there is already an xml part present for invoices
            if (iWorkbook.TryGetCustomProperty(KnownNames.InvoiceTag, ref tagId))
            {
                // Delete the existing xml part
                iWorkbook.TryDeleteCustomXMLById(Convert.ToString(tagId));               
            }

            // Create the new XmlPart
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(outputXml);

            tagId = iWorkbook.SetCustomXML(xmlDoc);

            iWorkbook.TrySetCustomProperty(KnownNames.InvoiceTag, Convert.ToString(tagId));            
        }
    }
}
