using DomainModel;
using Orleans;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ServiceCustomerAPI
{
    public interface ICustomerQueries 
        : IGrainWithIntegerKey
    {
        Task<APIResult<Customer>> FindCustomer(string customerId);
        Task<APIResult<List<string>>> FindAllCustomerIds();
        Task<APIResult<bool>> CustomerExists(string customerId);
    }
}
