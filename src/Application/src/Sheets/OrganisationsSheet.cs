﻿using Allors.Excel;
using Application.Models;
using Application.Ui;
using Application.Ui.GenericControls;
using Application.Services;
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
    public class OrganisationsSheet : ISheet
    {
        private Program program;

        public IWorksheet Sheet { get; }

        public OrganisationsSheet(IProgram program, IWorksheet worksheet)
        {
            this.program = (Program)program;
            this.Sheet = worksheet;
            
            this.Controls = new Controls(worksheet);

            this.Binder = new Binder(this.Sheet, Constants.ChangedStyle);
            this.Binder.ToDomained += this.Binder_ToDomained;

            this.Sheet.SheetActivated += this.Sheet_SheetActivated;

            this.Sheet.CellsChanged += Sheet_CellsChanged;

            // Save so we can re-instate it as an invoicesSheet on startup
            var customProperties = new CustomProperties();
            customProperties.Add(AppConstants.KeySheet, nameof(OrganisationsSheet));
            customProperties.Add(AppConstants.KeyCreated, DateTime.Now);
            customProperties.Add(AppConstants.KeyCreatedBy, this.program.Services.Configuration["Username"]);
            this.Sheet.SetCustomProperties(customProperties);         
        }

        private async void Sheet_CellsChanged(object sender, CellChangedEvent e)
        {
            // First columns will trigger an insert new Organisation
            var addCells = e.Cells
                    .Where(c => c.Column.Index == 0 
                    && c.Value != null 
                    && !this.Controls.ControlByCell.ContainsKey(c)).ToArray();

            if (addCells.Length > 0)
            {
                var options = new Range(0, 0, null, null, null, KnownNames.ValidationRangePaymentTerms);

                // Cell with an detail invoice Line
                foreach (ICell cell in addCells)
                {       
                    var organisation = (Organisation)this.program.Services.Database.Create<Organisation>(typeof(Organisation));
                    organisation.Name = cell.ValueAsString;

                    this.Organisations.Add(organisation);

                    var colIndex = 0;
                    var icell = this.Controls.TextBox(cell.Row.Index, colIndex++, organisation, "Name");
                    icell.Style = Constants.ChangedStyle;

                    this.Controls.TextBox(cell.Row.Index, colIndex++, organisation, "Street");
                    this.Controls.TextBox(cell.Row.Index, colIndex++, organisation, "City");
                    this.Controls.TextBox(cell.Row.Index, colIndex++, organisation, "Country");
                    this.Controls.TextBox(cell.Row.Index, colIndex++, organisation, "VatNumber");
                    this.Controls.TextBox(cell.Row.Index, colIndex++, organisation, "Email");
                    this.Controls.TextBox(cell.Row.Index, colIndex++, organisation, "Phone");
                    this.Controls.TextBox(cell.Row.Index, colIndex++, organisation, "FinancialContact");

                    this.Controls.Select<Organisation>(cell.Row.Index, colIndex++, options, organisation, nameof(organisation.DefaultPaymentTerm), nameof(PaymentTerm.Name),
                      toDomain: (object key) =>
                      {
                          return this.program.Services.Database.FirstOrDefault<PaymentTerm>(o => string.Equals(o.Name, key));
                      });

                }

                this.Controls.Bind();

                await this.Sheet.Flush().ConfigureAwait(false);
            }
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

        public List<Organisation> Organisations { get; set; } 

        public async Task Refresh()
        {
            this.Organisations = this.program.Services.Database.Get<Organisation>()?.ToList();

            await RefreshSheet().ConfigureAwait(false);
        }

        private async Task RefreshSheet()
        {
            var colIndex = 0;
            //
            this.Controls.Static(0, colIndex++, "Name");
            this.Controls.Static(0, colIndex++, "Street");
            this.Controls.Static(0, colIndex++, "City");
            this.Controls.Static(0, colIndex++, "Country");
            this.Controls.Static(0, colIndex++, "VatNumber");
            this.Controls.Static(0, colIndex++, "Email");
            this.Controls.Static(0, colIndex++, "Phone");
            this.Controls.Static(0, colIndex++, "FinancialContact");
            this.Controls.Static(0, colIndex++, "Payment Term");

            this.Sheet.FreezePanes(new Range(0, -1, 0, 0));

            var rowIndex = 1;
            
            var options = new Range(0, 0, null, null, null, KnownNames.ValidationRangePaymentTerms);

          

            foreach (var organisation in this.Organisations.OrderBy(o => o.Name))
            {
                colIndex = 0;

                this.Controls.TextBox(rowIndex, colIndex++, organisation, "Name");
                this.Controls.TextBox(rowIndex, colIndex++, organisation, "Street");
                this.Controls.TextBox(rowIndex, colIndex++, organisation, "City");
                this.Controls.TextBox(rowIndex, colIndex++, organisation, "Country");
                this.Controls.TextBox(rowIndex, colIndex++, organisation, "VatNumber");
                this.Controls.TextBox(rowIndex, colIndex++, organisation, "Email");
                this.Controls.TextBox(rowIndex, colIndex++, organisation, "Phone");
                this.Controls.TextBox(rowIndex, colIndex++, organisation, "FinancialContact");

                this.Controls.Select<Organisation>(rowIndex, colIndex, options, organisation, nameof(organisation.DefaultPaymentTerm), nameof(PaymentTerm.Description),
               toDomain: (object key) =>
               {
                   return this.program.Services.Database.FirstOrDefault<PaymentTerm>(o => string.Equals(o.Description, key));
               });


                rowIndex++;
            }

            this.Sheet.Workbook.SetNamedRange(KnownNames.ValidationRangeOrganisations, new Range(1, 0, this.Organisations.Count, 1, this.Sheet));

            this.Controls.Bind();

            await this.Sheet.Flush().ConfigureAwait(false);

            await Task.CompletedTask;
        }

        public async Task Load(IWorkbook iWorkbook)
        {
            object tagId = null;

            if( iWorkbook.TryGetCustomProperty(KnownNames.OrganisationTag, ref tagId))
            {
                var xmlDocument = iWorkbook.GetCustomXMLById(Convert.ToString(tagId));
                if(xmlDocument != null)
                {
                    var root = xmlDocument.DocumentElement.Name;
                    XmlSerializer serializer = new XmlSerializer(typeof(Organisation[]), new XmlRootAttribute(root));

                    StringReader stringReader = new StringReader(xmlDocument.OuterXml);

                    var existingOrganisations = (Organisation[])serializer.Deserialize(stringReader);
                    this.program.Services.Database.Store<Organisation>(existingOrganisations);

                    await Refresh().ConfigureAwait(false);
                }
            }          
        }

        internal async Task Save()
        {
            if (this.Organisations != null)
            {
                foreach(var org in this.Organisations)
                {
                    this.program.Services.Database.Save(org);
                }
            }

            await this.Refresh().ConfigureAwait(false);
        }


        public void SaveTo(IWorkbook iWorkbook)
        {
            object tagId = null;

            // Create the XML Document that we will save in the XML parts of the workbook
            string outputXml = null;       
            using (var stringwriter = new System.IO.StringWriter())
            {
                var serializer = new XmlSerializer(typeof(Organisation[]));
                serializer.Serialize(stringwriter, this.Organisations.ToArray());

                outputXml = stringwriter.ToString();

            }

            // Check if there is already an xml part present for invoices
            if (iWorkbook.TryGetCustomProperty(KnownNames.OrganisationTag, ref tagId))
            {
                // Delete the existing xml part
                iWorkbook.TryDeleteCustomXMLById(Convert.ToString(tagId));               
            }

            // Create the new XmlPart
            var xmlDoc = new XmlDocument();
            xmlDoc.LoadXml(outputXml);

            tagId = iWorkbook.SetCustomXML(xmlDoc);

            iWorkbook.TrySetCustomProperty(KnownNames.OrganisationTag, Convert.ToString(tagId));            
        }
    }
}
