using Allors.Excel;
using Allors.Excel.Interop;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;
using System;
using System.Runtime.InteropServices;
using System.Xml;

namespace ProductManager
{
    internal class OfficeDecorator : IOffice
    {
        private ThisAddIn thisAddIn;

        public OfficeDecorator(ThisAddIn thisAddIn)
        {
            this.thisAddIn = thisAddIn;
        }

        public object MsoPropertyTypeString => MsoDocProperties.msoPropertyTypeString;

        public object MsoPropertyTypeBoolean => MsoDocProperties.msoPropertyTypeBoolean;

        public object MsoPropertyTypeDate => MsoDocProperties.msoPropertyTypeDate;

        public object MsoPropertyTypeFloat => MsoDocProperties.msoPropertyTypeFloat;

        public object MsoPropertyTypeNumber => MsoDocProperties.msoPropertyTypeNumber;

        public void AddPicture(Microsoft.Office.Interop.Excel.Worksheet interopWorksheet, string filename, System.Drawing.Rectangle rectangle)
        {
            interopWorksheet.Shapes.AddPicture(filename, MsoTriState.msoFalse, MsoTriState.msoTrue, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }

        public XmlDocument GetCustomXMLById(Microsoft.Office.Interop.Excel.Workbook interopWorkbook, string id)
        {
            var xmlDocument = new XmlDocument();
            var customXMLPart = interopWorkbook.CustomXMLParts.SelectByID(id);

            if (customXMLPart != null)
            {
                xmlDocument.LoadXml(customXMLPart.XML);

                return xmlDocument;
            }

            return null;
        }

        public string SetCustomXmlPart(Microsoft.Office.Interop.Excel.Workbook interopWorkbook, XmlDocument xmlDocument)
        {
            return interopWorkbook.CustomXMLParts.Add(xmlDocument.OuterXml, Type.Missing).Id;
        }

        public bool TryDeleteCustomXMLById(Microsoft.Office.Interop.Excel.Workbook interopWorkbook, string id)
        {
            try
            {
                var customXMLPart = interopWorkbook.CustomXMLParts.SelectByID(id);
                customXMLPart.Delete();
                return true;
            }
            catch (COMException)
            {
                return false;
            }
        }
    }
}