using System;
using System.Collections.Generic;
using System.Text;

namespace Application.Services
{
    public interface IMessageService
    {
        void Show(string message);

        bool Confirm(string message);
    }
}
