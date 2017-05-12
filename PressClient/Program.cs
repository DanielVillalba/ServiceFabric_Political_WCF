using Contracts;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Communication.Wcf.Client;
using System;
using System.Collections.Generic;
using System.Fabric;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WCFBindingConfig;

namespace PressClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // binding data
            Binding binding = new BindingConfig().GetBindingWSHttp();

            ServicePartitionResolver spr = new ServicePartitionResolver(() => new FabricClient());
            WcfCommunicationClientFactory<IPressContract> communicationClientFactory = new WcfCommunicationClientFactory<IPressContract>(binding, servicePartitionResolver: spr);
            Uri uri = new Uri("fabric:/Political/PressService");

            PresidentialServiceClient pressClient = new PresidentialServiceClient(communicationClientFactory, uri);

            int Id = 15;
            Console.WriteLine(pressClient.InterviewPresidentName(Id).Result);

            Console.ReadKey();

            // test the deployment
            //int number = 0;
            //while (true)
            //{
            //    number++;
            //    Console.WriteLine(pressClient.InterviewPresidentName(number).Result);
            //    Thread.Sleep(1000);
            //}
        }
    }
}
