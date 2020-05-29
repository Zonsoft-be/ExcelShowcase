using Application.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProductManager.Services
{
    public class ServiceLocator : IServiceLocator
    {
        public IMessageService MessageService { get; }
        
        public IHttpService HttpService { get; }
        
        public ServiceLocator()
        {
            this.MessageService = new MessageService();

            this.HttpService = new HttpService();
        }        
    }
}
