using Allors.Excel;
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
            throw new System.NotImplementedException();
        }
    }
}