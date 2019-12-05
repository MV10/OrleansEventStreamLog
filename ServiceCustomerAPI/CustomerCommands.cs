using DomainModel;
using DomainModel.DomainEvents;
using Microsoft.Extensions.Logging;
using Orleans;
using Orleans.Concurrency;
using ServiceCustomerManager;
using System;
using System.Threading.Tasks;

namespace ServiceCustomerAPI
{
    [Reentrant]
    [StatelessWorker]
    public class CustomerCommands 
        : Grain, ICustomerCommands
    {
        private readonly IClusterClient OrleansClient;
        private readonly ILogger<CustomerCommands> Log;

        public CustomerCommands(IClusterClient clusterClient, ILogger<CustomerCommands> log)
        {
            OrleansClient = clusterClient;
            Log = log;
        }

        public async Task<APIResult<Customer>> AddAccount(string customerId, Account account)
        {
            try
            {
                var mgr = OrleansClient.GetGrain<ICustomerManager>(customerId);
                await mgr.RaiseEvent(new AccountAdded { 
                    Account = account 
                });
                await mgr.ConfirmEvents();
                return new APIResult<Customer>(await mgr.GetManagedState());
            }
            catch (Exception ex)
            {
                return new APIResult<Customer>(ex);
            }
        }

        public async Task<APIResult<Customer>> NewCustomer(string customerId, Person primaryAccountHolder, Address mailingAddress)
        {
            Log.LogInformation("NewCustomer: start");
            try
            {
                Log.LogInformation("NewCustomer: get manager");
                var mgr = OrleansClient.GetGrain<ICustomerManager>(customerId);
                Log.LogInformation($"NewCustomer: manager is null? {mgr is null}");
                Log.LogInformation("NewCustomer: raising CustomerCreated event");
                await mgr.RaiseEvent(new CustomerCreated { 
                    PrimaryAccountHolder = primaryAccountHolder, 
                    MailingAddress = mailingAddress 
                });
                Log.LogInformation("NewCustomer: confirming events");
                await mgr.ConfirmEvents();
                Log.LogInformation("NewCustomer: returning GetManagedState");
                return new APIResult<Customer>(await mgr.GetManagedState());
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "NewCustomer: exception");
                return new APIResult<Customer>(ex);
            }
        }

        public async Task<APIResult<Customer>> RemoveAccount(string customerId, string accountNumber)
        {
            try
            {
                var mgr = OrleansClient.GetGrain<ICustomerManager>(customerId);
                await mgr.RaiseEvent(new AccountRemoved { 
                    AccountNumber = accountNumber 
                });
                await mgr.ConfirmEvents();
                return new APIResult<Customer>(await mgr.GetManagedState());
            }
            catch (Exception ex)
            {
                return new APIResult<Customer>(ex);
            }
        }

        public async Task<APIResult<Customer>> UpdateMailingAddress(string customerId, Address address)
        {
            try
            {
                var mgr = OrleansClient.GetGrain<ICustomerManager>(customerId);
                await mgr.RaiseEvent(new MailingAddressChanged { 
                    Address = address
                });
                await mgr.ConfirmEvents();
                return new APIResult<Customer>(await mgr.GetManagedState());
            }
            catch (Exception ex)
            {
                return new APIResult<Customer>(ex);
            }
        }

        public async Task<APIResult<Customer>> UpdatePrimaryResidence(string customerId, Address address)
        {
            try
            {
                var mgr = OrleansClient.GetGrain<ICustomerManager>(customerId);
                await mgr.RaiseEvent(new ResidencePrimaryChanged { 
                    Address = address
                });
                await mgr.ConfirmEvents();
                return new APIResult<Customer>(await mgr.GetManagedState());
            }
            catch (Exception ex)
            {
                return new APIResult<Customer>(ex);
            }
        }

        public async Task<APIResult<Customer>> UpdateSpouseResidence(string customerId, Address address)
        {
            try
            {
                var mgr = OrleansClient.GetGrain<ICustomerManager>(customerId);
                await mgr.RaiseEvent(new ResidenceSpouseChanged { 
                    Address = address
                });
                await mgr.ConfirmEvents();
                return new APIResult<Customer>(await mgr.GetManagedState());
            }
            catch (Exception ex)
            {
                return new APIResult<Customer>(ex);
            }
        }

        public async Task<APIResult<Customer>> UpdateSpouse(string customerId, Person spouse)
        {
            try
            {
                var mgr = OrleansClient.GetGrain<ICustomerManager>(customerId);
                if (spouse != null)
                {
                    await mgr.RaiseEvent(new SpouseChanged
                    {
                        Spouse = spouse
                    });
                }
                else
                {
                    await mgr.RaiseEvent(new SpouseRemoved());
                }
                await mgr.ConfirmEvents();
                return new APIResult<Customer>(await mgr.GetManagedState());
            }
            catch (Exception ex)
            {
                return new APIResult<Customer>(ex);
            }
        }

        public async Task<APIResult<Customer>> PostAccountTransaction(string customerId, string accountNumber, decimal amount)
        {
            try
            {
                var mgr = OrleansClient.GetGrain<ICustomerManager>(customerId);

                var acct = (await mgr.GetManagedState()).Accounts.Find(a => a.AccountNumber.Equals(accountNumber));
                if (acct == null) return new APIResult<Customer>("Account not found");

                var oldBalance = acct.Balance;
                var newBalance = oldBalance + amount;
                if (oldBalance >= 0 && newBalance < 0) return new APIResult<Customer>("Insufficient funds");

                await mgr.RaiseEvent(new TransactionPosted
                {
                    AccountNumber = accountNumber,
                    Amount = amount,
                    OldBalance = oldBalance,
                    NewBalance = newBalance
                });
                await mgr.ConfirmEvents();
                return new APIResult<Customer>(await mgr.GetManagedState());
            }
            catch (Exception ex)
            {
                return new APIResult<Customer>(ex);
            }
        }

    }
}
