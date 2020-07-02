using Allors.Excel;
using Application.Models;
using Application.Services;
using Application.Ui;
using Application.Ui.GenericControls;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;
using System.Xml;
using System.Xml.Serialization;

namespace Application.Sheets
{
    public class PaymentTermsSheet : ISheet
    {
        private Program program;

        public IWorksheet Sheet { get; }

        public PaymentTermsSheet(IProgram program, IWorksheet worksheet)
        {
            this.program = (Program)program;
            this.Sheet = worksheet;

            this.Controls = new Controls(worksheet);

            this.Binder = new Binder(this.Sheet, Constants.ChangedStyle);
            this.Binder.ToDomained += this.Binder_ToDomained;

            this.Sheet.SheetActivated += this.Sheet_SheetActivated;

            this.Sheet.Name = $"{nameof(PaymentTermsSheet)}.{this.Sheet.Index}"; // Single Quotes to always allow spaces or special chars

            // Fetch after we changed the name.
            this.NamedRanges = this.Sheet.GetNamedRanges();

            // Save so we can re-instate it as an invoicesSheet on startup
            var customProperties = new CustomProperties();
            customProperties.Add(AppConstants.KeySheet, nameof(PaymentTermsSheet));
            customProperties.Add(AppConstants.KeyCreated, DateTime.Now);
            customProperties.Add(AppConstants.KeyCreatedBy, this.program.Services.Configuration["Username"]);
            this.Sheet.SetCustomProperties(customProperties);
        }

        public int Index => this.Sheet.Index;

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
             

        public Range[] NamedRanges { get; }

        public bool IsWorksheetUpToDate { get; set; }

        public List<PaymentTerm> PaymentTerms { get; private set; }

        public async Task Refresh()
        {
            this.PaymentTerms = this.program.Services.Database.Get<PaymentTerm>()?.ToList();

            await RefreshSheet().ConfigureAwait(false);
        }

        private async Task RefreshSheet()
        {
            var colIndex = 0;
            //
            this.Controls.Static(0, colIndex++, "Name");
            this.Controls.Static(0, colIndex++, "Days");
            this.Controls.Static(0, colIndex++, "Description");           

            this.Sheet.FreezePanes(new Range(0, -1, 0, 0));

            var rowIndex = 1;

            foreach (var paymentTerm in this.PaymentTerms.OrderBy(o => o.Name))
            {
                colIndex = 0;

                this.Controls.TextBox(rowIndex, colIndex++, paymentTerm, "Name");
                this.Controls.TextBox(rowIndex, colIndex++, paymentTerm, "Days");
                this.Controls.TextBox(rowIndex, colIndex++, paymentTerm, "Description");              

                rowIndex++;
            }

            this.Sheet.Workbook.SetNamedRange(KnownNames.ValidationRangePaymentTerms, new Range(1, 2, this.PaymentTerms.Count, 1, this.Sheet));

            this.Controls.Bind();

            await this.Sheet.Flush().ConfigureAwait(false);

            await Task.CompletedTask;
        }

        internal async Task Save()
        {
            if (this.PaymentTerms != null)
            {
                this.program.Services.Database.Save(this.PaymentTerms.ToArray());
            }

            await this.Refresh().ConfigureAwait(false);           
        }

        public async Task Load(IWorkbook iWorkbook)
        {
            object tagId = null;

            if (iWorkbook.TryGetCustomProperty(KnownNames.PaymentTermTag, ref tagId))
            {
                var xmlDocument = iWorkbook.GetCustomXMLById(Convert.ToString(tagId));
                if (xmlDocument != null)
                {
                    var root = xmlDocument.DocumentElement.Name;
                    XmlSerializer serializer = new XmlSerializer(typeof(PaymentTerm[]), new XmlRootAttribute(root));

                    StringReader stringReader = new StringReader(xmlDocument.OuterXml);

                    var existingPaymentTerms = (PaymentTerm[])serializer.Deserialize(stringReader);
                    this.program.Services.Database.Store<PaymentTerm>(existingPaymentTerms);

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
                var serializer = new XmlSerializer(typeof(PaymentTerm[]));
                serializer.Serialize(stringwriter, this.PaymentTerms.ToArray());

                outputXml = stringwriter.ToString();

            }

            // Check if there is already an xml part present for invoices
            if (iWorkbook.TryGetCustomProperty(KnownNames.PaymentTermTag, ref tagId))
            {
                // Delete the existing xml part
                iWorkbook.TryDeleteCustomXMLById(Convert.ToString(tagId));
            }

            // Create the new XmlPart
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(outputXml);

            tagId = iWorkbook.SetCustomXML(xmlDoc);

            iWorkbook.TrySetCustomProperty(KnownNames.PaymentTermTag, Convert.ToString(tagId));
        }
    }
}
