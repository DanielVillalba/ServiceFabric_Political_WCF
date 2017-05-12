using Contracts;
using Microsoft.ServiceFabric.Services.Communication.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PressClient
{
    public class PresidentialServiceClient : ServicePartitionClient<WcfCommunicationClient<IPressContract>>
    {
        public PresidentialServiceClient(ICommunicationClientFactory<WcfCommunicationClient<IPressContract>> communicationClientFactoryt, Uri uri)
            : base(communicationClientFactoryt, uri)
        {

        }

        public Task<string> InterviewPresidentName(int id)
        {
            return this.InvokeWithRetryAsync(client => client.Channel.InterviewPresidentName(id));
        }
    }
}
