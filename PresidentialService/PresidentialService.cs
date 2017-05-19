using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;
using Contracts;
using System.ServiceModel;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using WCFBindingConfig;
using System.ServiceModel.Channels;
using System.Globalization;
using System.ServiceModel.Description;

namespace PresidentialService
{
    /// <summary>
    /// An instance of this class is created for each service instance by the Service Fabric runtime.
    /// </summary>
    internal sealed class PresidentialService : StatelessService, IPresidentialService
    {
        public PresidentialService(StatelessServiceContext context)
            : base(context)
        { }

        // this are the WCF operations that will be executed
        public Task<string> PresidentName(int id)
        {
            Console.WriteLine(string.Format("Node: {0} | {1} |  {2}", this.Context.NodeContext.NodeName, "Calling PresidentName operation with id: " + id.ToString(), DateTime.UtcNow.ToLongTimeString()));    // DANIEL: sent to log file
            return Task.FromResult<string>(string.Format("Node: {0} | {1} |  {2}", this.Context.NodeContext.NodeName, "Calling PresidentName operation with id: " + id.ToString(), DateTime.UtcNow.ToLongTimeString()));
        }

        public Task<string> Presidents()
        {
            ServiceEventSource.Current.ServiceMessage(this.Context, "Calling Presidents operation on {0}", this.Context.NodeContext.NodeName);
            return Task.FromResult<string>(string.Format("Node: {0} | {1} |  {2}", this.Context.NodeContext.NodeName, "Calling Presidents operation !!!", DateTime.UtcNow.ToLongTimeString()));
        }

        /// <summary>
        /// Optional override to create listeners (e.g., TCP, HTTP) for this service replica to handle client or user requests.
        /// </summary>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceInstanceListener> CreateServiceInstanceListeners()
        {
            Binding binding = new BindingConfig().GetBinding();

            string host = this.Context.NodeContext.IPAddressOrFQDN;
            var endpointConfig = this.Context.CodePackageActivationContext.GetEndpoint("ServiceEndpoint");
            int port = endpointConfig.Port;
            string scheme = endpointConfig.Protocol.ToString();
            string nodeName = this.Context.NodeContext.NodeName;
            string servicename = this.Context.ServiceTypeName;
            string uri = string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/{3}/{4}.svc", "net." + scheme, host, port, nodeName, servicename);

            WcfCommunicationListener<IPresidentialService> pressListener = new WcfCommunicationListener<IPresidentialService>(this.Context, this, binding, new EndpointAddress(uri));

            // Check to see if the service host already has a ServiceMetadataBehavior, If not, add one                      // For net.tcp need to find a way to expose metadata
            //if (pressListener.ServiceHost.Description.Behaviors.Find<ServiceMetadataBehavior>() == null)
            //{
            //    ServiceMetadataBehavior behavior = new ServiceMetadataBehavior();
            //    behavior.HttpGetEnabled = true;           // need to figure it out how to bypass this for net.tcp
            //    behavior.HttpGetUrl = new Uri(uri + "mex/");  // need to figure it out how to bypass this for net.tcp
            //    pressListener.ServiceHost.Description.Behaviors.Add(behavior);
            //    pressListener.ServiceHost.AddServiceEndpoint(typeof(IMetadataExchange), MetadataExchangeBindings.CreateMexTcpBinding(), uri + "mex/");
            //}

            ServiceInstanceListener listener = new ServiceInstanceListener(context => pressListener);

            return new[] { listener };
        }

        // DANIEL: This is not required since we are creating a WCF type API

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
