using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Contracts;
using System.ServiceModel.Channels;
using WCFBindingConfig;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System.Globalization;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace PressService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class PressService : StatelessService, IPressContract
    {
        public PressService(StatelessServiceContext context)
            : base(context)
        { }

        public Task<string> InterviewPresidentName(int id)
        {
            // invoking the PresidentialService in order to send back a response
            PresidentialServiceClient presindetialService = getPresidentialServiceClient();
            string responseFromPresidentialService = presindetialService.PresidentName(id).Result;

            Console.WriteLine(string.Format("Node: {0} | {1} | {2}", this.Context.NodeContext.NodeName, "Receiving from PresidentialService: \n" + responseFromPresidentialService + "\nSending response Back to PressClient", DateTime.UtcNow.ToLongTimeString()));    // DANIEL: sent to log file
            return Task.FromResult<string>(string.Format("Node: {0} | {1} | {2}", this.Context.NodeContext.NodeName, "Response received from PresidentialService: \n" + responseFromPresidentialService, DateTime.UtcNow.ToLongTimeString()));
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            Binding binding = new BindingConfig().GetBindingWSHttp();

            // set the proper way to expose the URI on cluster manager
            string host = this.Context.NodeContext.IPAddressOrFQDN;
            var endpointConfig = this.Context.CodePackageActivationContext.GetEndpoint("ServiceHttpEndPoint");
            int port = endpointConfig.Port;
            string scheme = endpointConfig.Protocol.ToString();
            string nodeName = this.Context.NodeContext.NodeName;
            string servicename = this.Context.ServiceTypeName;
            string uri = string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/{3}/{4}/", scheme, host, port, nodeName, servicename);

            WcfCommunicationListener<IPressContract> pressListener = new WcfCommunicationListener<IPressContract>(this.Context, this, binding, new EndpointAddress(uri));

            // Check to see if the service host already has a ServiceMetadataBehavior, If not, add one
            if (pressListener.ServiceHost.Description.Behaviors.Find<ServiceMetadataBehavior>() == null)
            {
                ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
                behavior.HttpGetEnabled = true;
                behavior.HttpGetUrl = new Uri(uri + "mex/");
                pressListener.ServiceHost.Description.Behaviors.Add(behavior);
                pressListener.ServiceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexHttpBinding(), uri + "mex/");
            }

            ServiceInstanceListener listener = new ServiceInstanceListener(context => pressListener);
            return new[] { listener };
        }

        private PresidentialServiceClient getPresidentialServiceClient()
        {
            // binding data
            Binding binding = new BindingConfig().GetBinding();

            ServicePartitionResolver spr = new ServicePartitionResolver(() => new FabricClient());
            WcfCommunicationClientFactory<IPresidentialService> communicationClientFactory = new WcfCommunicationClientFactory<IPresidentialService>(binding, servicePartitionResolver: spr);
            Uri uri = new Uri("fabric:/Political/PresidentialService");

            PresidentialServiceClient presidentialServiceClient = new PresidentialServiceClient(communicationClientFactory, uri);
            return presidentialServiceClient;
        }

        // This is not required since this is an WCF API service type

        /// <summary>
        /// This is the main entry point for your service instance.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service instance.</param>
        //protected override async Task RunAsync(CancellationToken cancellationToken)
        //{
        //    // TODO: Replace the following sample code with your own logic 
        //    //       or remove this RunAsync override if it's not needed in your service.

        //    long iterations = 0;

        //    while (true)
        //    {
        //        cancellationToken.ThrowIfCancellationRequested();

        //        ServiceEventSource.Current.ServiceMessage(this.Context, "Working-{0}", ++iterations);

        //        await Task.Delay(TimeSpan.FromSeconds(1), cancellationToken);
        //    }
        //}
    }
}
