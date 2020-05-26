using System;
using System.Collections.Generic;
using System.Text;

namespace Application
{
    public interface ISheet
    {
        System.Threading.Tasks.Task Refresh();
    }
}
