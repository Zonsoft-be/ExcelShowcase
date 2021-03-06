﻿using Application.Data;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;

namespace Application.Services
{
    public interface IServiceLocator
    {
        IMessageService MessageService { get; }

        IHttpService HttpService { get; }

        IConfiguration Configuration { get; }

        IDatabase Database { get; }
    }
}
