using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public interface ISheet
    {
        bool IsWorksheetUpToDate { get; set; }

        System.Threading.Tasks.Task Refresh();
    }
}
