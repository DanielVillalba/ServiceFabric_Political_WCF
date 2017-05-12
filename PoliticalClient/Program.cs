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

namespace PresidentialClient
{
    class Program
    {
        static void Main(string[] args)
        {
            // binding data
            Binding binding = new BindingConfig().GetBinding();

            ServicePartitionResolver spr = new ServicePartitionResolver( () => new FabricClient());
            WcfCommunicationClientFactory<IPresidentialService> communicationClientFactory = new WcfCommunicationClientFactory<IPresidentialService>(binding, servicePartitionResolver: spr);
            Uri uri = new Uri("fabric:/Political/PresidentialService");

            PresidentialServiceClient presidentialServiceClient = new PresidentialServiceClient(communicationClientFactory, uri);

            //int Id = 15;
            //string presidentName = presidentialServiceClient.PresidentName(Id).Result;
            //Console.WriteLine(presidentName);
            //string presidents = presidentialServiceClient.Presidents().Result;
            //Console.WriteLine(presidents);
            //Console.ReadKey();

            // test the deployment
            int number = 0;
            while(true)
            {
                number++;
                Console.WriteLine(presidentialServiceClient.PresidentName(number).Result);
                Console.WriteLine(presidentialServiceClient.Presidents().Result);
                Console.WriteLine(" ");
                Thread.Sleep (1000);
            }
        }
    }
}
