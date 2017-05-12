using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;

namespace WCFBindingConfig
{
    public class BindingConfig
    {
        public Binding GetBinding()
        {
            return binding;
        }

        public Binding GetBindingHttp()
        {
            return bindingHttp;
        }

        public Binding GetBindingWSHttp()
        {
            return bindingWSHttp;
        }

        const int bufferSize = 512000; // 500KB

        // this configuration provides a TCP binding
        private NetTcpBinding binding = new NetTcpBinding(SecurityMode.None)
        {
            SendTimeout = TimeSpan.FromSeconds(30),
            ReceiveTimeout = TimeSpan.FromSeconds(30),
            CloseTimeout = TimeSpan.FromSeconds(30),
            MaxConnections = 1000,
            MaxReceivedMessageSize = bufferSize,
            MaxBufferSize = bufferSize,
            MaxBufferPoolSize = bufferSize * Environment.ProcessorCount
        };

        // this configuration provides a HTTP binding
        private Binding bindingHttp = new BasicHttpBinding(BasicHttpSecurityMode.None)
        {
            SendTimeout = TimeSpan.FromSeconds(30),
            ReceiveTimeout = TimeSpan.FromSeconds(30),
            CloseTimeout = TimeSpan.FromSeconds(30),
            //MaxConnections = 1000,
            MaxReceivedMessageSize = bufferSize,
            MaxBufferSize = bufferSize,
            MaxBufferPoolSize = bufferSize * Environment.ProcessorCount
        };


        // this configuration provides a WS-HTTP binding
        private Binding bindingWSHttp = new WSHttpBinding(SecurityMode.None)
        {
            SendTimeout = TimeSpan.FromSeconds(30),
            ReceiveTimeout = TimeSpan.FromSeconds(30),
            CloseTimeout = TimeSpan.FromSeconds(30),
            //MaxConnections = 1000,
            MaxReceivedMessageSize = bufferSize,
            //MaxBufferSize = bufferSize,
            MaxBufferPoolSize = bufferSize * Environment.ProcessorCount
        };
    }
}
