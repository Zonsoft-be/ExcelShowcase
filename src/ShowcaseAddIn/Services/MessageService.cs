using Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ProductManager.Services
{
    public class MessageService : IMessageService
    {
        public bool Confirm(string message)
        {
            var dialogResult = MessageBox.Show(message, "Confirmation", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            return dialogResult == DialogResult.Yes;
        }

        public void Show(string message)
        {
            MessageBox.Show(message);
        }
    }
}
