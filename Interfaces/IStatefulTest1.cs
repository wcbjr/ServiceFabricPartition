using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Remoting;

namespace Interfaces
{
    public interface IStatefulTest1 : IService
    {
        Task<string> TestEntryPoint();
    }
}
