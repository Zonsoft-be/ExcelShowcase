using Allors.Excel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Sheets
{
    public class ProductSheet : ISheet
    {
        private Program program;
        private IWorksheet activeWorksheet;

        public ProductSheet(Program program, IWorksheet activeWorksheet)
        {
            this.program = program;
            this.activeWorksheet = activeWorksheet;
        }

        public async Task Refresh()
        {
            //
        }
    }
}
