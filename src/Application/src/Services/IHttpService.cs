using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Application.Services
{
    public interface IHttpService
    {
        Task<T> Load<T>(string baseAddress, string requestUri);
    }
}
