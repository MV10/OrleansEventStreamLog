using DomainModel;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using ServiceCustomerManager;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace ServiceCustomerAPI
{
    [Reentrant]
    [StatelessWorker]
    public class CustomerQueries 
        : Grain, ICustomerQueries
    {
        private static string ConnectionString = @"Server=(localdb)\MSSQLLocalDB;Integrated Security=true;Database=OrleansESL";

        private readonly IClusterClient OrleansClient;
        private readonly ILogger<CustomerQueries> Log;

        public CustomerQueries(IClusterClient clusterClient, ILogger<CustomerQueries> log)
        {
            OrleansClient = clusterClient;
            Log = log;
        }

        public async Task<APIResult<Customer>> FindCustomer(string customerId)
        {
            try
            {
                var mgr = OrleansClient.GetGrain<ICustomerManager>(customerId);
                var customer = await mgr.GetManagedState();
                return new APIResult<Customer>(customer);
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "FindCustomer exception");
                return new APIResult<Customer>(ex);
            }
        }

        public async Task<APIResult<List<string>>> FindAllCustomerIds()
        {
            try
            {
                List<string> results = new List<string>();
                using var conn = new SqlConnection(ConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand("SELECT DISTINCT CustomerId FROM CustomerEventStream;", conn);
                using var reader = await cmd.ExecuteReaderAsync();
                if(reader.HasRows)
                {
                    while(await reader.ReadAsync())
                        results.Add(reader.GetString(0));
                }
                await reader.CloseAsync();
                await conn.CloseAsync();
                return new APIResult<List<string>>(results);
            }
            catch(Exception ex)
            {
                Log.LogError(ex, "FindAllCustomerIds exception");
                return new APIResult<List<string>>(ex);
            }
        }

        public async Task<APIResult<bool>> CustomerExists(string customerId)
        {
            try
            {
                using var conn = new SqlConnection(ConnectionString);
                await conn.OpenAsync();
                using var cmd = new SqlCommand("SELECT COUNT(*) AS ScalarVal FROM CustomerEventStream WHERE CustomerId = @customerId;", conn);
                cmd.Parameters.AddWithValue("@customerId", customerId);
                int count = (int)await cmd.ExecuteScalarAsync();
                await conn.CloseAsync();
                return new APIResult<bool>(count > 0);
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "CustomerExists exception");
                return new APIResult<bool>(ex);
            }
        }
    }
}
