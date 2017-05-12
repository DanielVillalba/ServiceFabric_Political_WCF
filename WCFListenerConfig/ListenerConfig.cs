using System;
using Contracts;
using System.Collections.Generic;
using System.Text;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using System.ServiceModel.Channels;
using System.Fabric;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Runtime;
using System.Globalization;
using System.ServiceModel;
using Microsoft.ServiceFabric.Services.Runtime;
using System.ServiceModel.Description;

namespace WCFListenerConfig
{
    public class ListenerConfig
    {
        private string _endPoint;
        private StatelessServiceContext _context;
        public ListenerConfig(string endPoint, StatelessServiceContext context)
        {
            _endPoint = endPoint;
            _context = context;
        }

        public string GetUri()
        {
            string host = _context.NodeContext.IPAddressOrFQDN;
            var endpointConfig = _context.CodePackageActivationContext.GetEndpoint("ServiceHttpEndPoint");
            int port = endpointConfig.Port;
            string scheme = endpointConfig.Protocol.ToString();
            string nodeName = _context.NodeContext.NodeName;
            string servicename = _context.ServiceTypeName;
            return string.Format(CultureInfo.InvariantCulture, "{0}://{1}:{2}/{3}/{4}/", scheme, host, port, nodeName, servicename);
        }

        public EndpointAddress GetEndPoint()
        {
            return new EndpointAddress(GetUri());
        }
    }
}
