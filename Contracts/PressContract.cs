using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;

namespace Contracts
{

    /// <summary>
    /// This will be used to comunicate from outside the service fabric cluster via a WCF client request, in oder to 
    /// </summary>
    [ServiceContract]
    public interface IPressContract
    {
        [OperationContract]
        Task<string> InterviewPresidentName(int id);
    }
}
