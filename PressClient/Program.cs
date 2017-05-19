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

            ListEndpoints();    // DANIEL: List the total available endpoints in the cluster

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

        private static void ListEndpoints()
        {
            var resolver = ServicePartitionResolver.GetDefault();
            var fabricClient = new FabricClient();
            var apps = fabricClient.QueryManager.GetApplicationListAsync().Result;
            foreach (var app in apps)
            {
                Console.WriteLine($"Discovered application:'{app.ApplicationName}");

                var services = fabricClient.QueryManager.GetServiceListAsync(app.ApplicationName).Result;
                foreach (var service in services)
                {
                    Console.WriteLine($"Discovered Service:'{service.ServiceName}");

                    var partitions = fabricClient.QueryManager.GetPartitionListAsync(service.ServiceName).Result;
                    foreach (var partition in partitions)
                    {
                        Console.WriteLine($"Discovered Service Partition:'{partition.PartitionInformation.Kind} {partition.PartitionInformation.Id}");


                        ServicePartitionKey key;
                        switch (partition.PartitionInformation.Kind)
                        {
                            case ServicePartitionKind.Singleton:
                                key = ServicePartitionKey.Singleton;
                                break;
                            case ServicePartitionKind.Int64Range:
                                var longKey = (Int64RangePartitionInformation)partition.PartitionInformation;
                                key = new ServicePartitionKey(longKey.LowKey);
                                break;
                            case ServicePartitionKind.Named:
                                var namedKey = (NamedPartitionInformation)partition.PartitionInformation;
                                key = new ServicePartitionKey(namedKey.Name);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException("partition.PartitionInformation.Kind");
                        }
                        var resolved = resolver.ResolveAsync(service.ServiceName, key, CancellationToken.None).Result;
                        foreach (var endpoint in resolved.Endpoints)
                        {
                            Console.WriteLine($"Discovered Service Endpoint:'{endpoint.Address}");
                        }
                    }
                }
            }
        }
    }
}
