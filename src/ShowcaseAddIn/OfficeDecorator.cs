using Allors.Excel;
using Microsoft.Office.Core;
using Microsoft.Office.Interop.Excel;

namespace ProductManager
{
    internal class OfficeDecorator : IOffice
    {
        private ThisAddIn thisAddIn;

        public OfficeDecorator(ThisAddIn thisAddIn)
        {
            this.thisAddIn = thisAddIn;
        }

        public void AddPicture(Worksheet interopWorksheet, string filename, System.Drawing.Rectangle rectangle)
        {
            interopWorksheet.Shapes.AddPicture(filename, MsoTriState.msoFalse, MsoTriState.msoTrue, rectangle.X, rectangle.Y, rectangle.Width, rectangle.Height);
        }
    }
}