using DomainModel;
using Microsoft.Extensions.Logging;
using Orleans;
using System;
using System.Threading.Tasks;

// for grain references
using ServiceCustomerAPI;
using ServiceCustomerManager;
using System.Diagnostics;

namespace DemoClient
{
    class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Connecting client.");
            var clusterClient = await GetOrleansClusterClient();

            Console.WriteLine("Retrieving CQRS grains.");
            var cmd = clusterClient.GetGrain<ICustomerCommands>(0);
            var query = clusterClient.GetGrain<ICustomerQueries>(0);

            string id = "12345678";

            var exists = await query.CustomerExists(id);
            Console.WriteLine($"Customer exists? {exists.Output}");

            APIResult<Customer> snapshot;

            if(!exists.Output)
            {
                var residence = new Address
                {
                    Street = "10 Main St.",
                    City = "Anytown",
                    StateOrProvince = "TX",
                    PostalCode = "90210",
                    Country = "USA"
                };

                var person = new Person
                {
                    FullName = "John Doe",
                    FirstName = "John",
                    LastName = "Doe",
                    Residence = residence,
                    TaxId = "555-55-1234",
                    DateOfBirth = DateTimeOffset.Parse("05/01/1960")
                };

                Console.WriteLine("Creating new customer.");
                snapshot = await cmd.NewCustomer(id, person, residence);
            }
            else
            {
                Console.WriteLine("Retrieving customer.");
                snapshot = await query.FindCustomer(id);
            }

            if(snapshot.Success)
            {
                Console.WriteLine($"Customer name: {snapshot.Output.PrimaryAccountHolder.FullName}");
            }
            else
            {
                Console.WriteLine($"Exception:\n{snapshot.Message}");
            }

            await clusterClient.Close();
            clusterClient.Dispose();

            if(!Debugger.IsAttached)
            {
                Console.WriteLine("\n\nPress any key to exit...");
                Console.ReadKey(true);
            }
        }

        static async Task<IClusterClient> GetOrleansClusterClient()
        {
            var client = new ClientBuilder()
                .ConfigureLogging(logging => {
                    logging
                    .AddFilter("Microsoft", LogLevel.Warning)
                    .AddFilter("Orleans", LogLevel.Warning)
                    .AddFilter("Runtime", LogLevel.Warning)
                    .AddConsole();
                })
                .UseLocalhostClustering() // cluster and service IDs default to "dev"
                .ConfigureApplicationParts(parts =>
                {
                    parts.AddApplicationPart(typeof(CustomerCommands).Assembly).WithReferences();
                    parts.AddApplicationPart(typeof(CustomerQueries).Assembly).WithReferences();
                    parts.AddApplicationPart(typeof(CustomerManager).Assembly).WithReferences();
                })
                .Build();

            await client.Connect();

            return client;
        }
    }
}
