using DomainModel;
using Orleans;
using System.Threading.Tasks;

namespace ServiceCustomerAPI
{
    public interface ICustomerCommands 
        : IGrainWithIntegerKey
    {
        Task<APIResult<Customer>> NewCustomer(string customerId, Person primaryAccountHolder, Address mailingAddress);
        Task<APIResult<Customer>> UpdateSpouse(string customerId, Person spouse);
        Task<APIResult<Customer>> AddAccount(string customerId, Account account);
        Task<APIResult<Customer>> RemoveAccount(string customerId, string accountNumber);
        Task<APIResult<Customer>> UpdateMailingAddress(string customerId, Address address);
        Task<APIResult<Customer>> UpdatePrimaryResidence(string customerId, Address address);
        Task<APIResult<Customer>> UpdateSpouseResidence(string customerId, Address address);
        Task<APIResult<Customer>> PostAccountTransaction(string customerId, string accountNumber, decimal amount);
    }
}
