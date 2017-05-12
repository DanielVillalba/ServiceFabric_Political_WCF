using Contracts;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PresidentialClient
{
    public class PresidentialServiceClient: ServicePartitionClient<WcfCommunicationClient<IPresidentialService>>
    {
        public PresidentialServiceClient(ICommunicationClientFactory<WcfCommunicationClient<IPresidentialService>> communicationClientFactoryt, Uri uri)
            : base(communicationClientFactoryt, uri)
        {

        }
        public Task<string> PresidentName(int id)
        {
            return this.InvokeWithRetryAsync(client => client.Channel.PresidentName(id));
        }
        public Task<string> Presidents()
        {
            return this.InvokeWithRetryAsync(client => client.Channel.Presidents());
        }
    }
}
