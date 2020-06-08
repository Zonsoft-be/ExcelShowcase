using Application.Data;
using Application.Services;
using Application.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ProductManager.Services
{
    public class ServiceLocator : IServiceLocator
    {
        public IMessageService MessageService { get; }
        
        public IHttpService HttpService { get; }

        public IConfiguration Configuration { get; }

        public IDatabase Database { get; }

        public ServiceLocator()
        {
            this.MessageService = new MessageService();

            this.HttpService = new HttpService();

            this.Configuration = new ConfigurationBuilder()               
                .AddJsonFile("appSettings.json", false, reloadOnChange: true)
                .Build();

            this.Database = new Database();
        }        
    }
}
