using DomainModel;
using DomainModel.DomainEvents;
using Orleans;
using Orleans.EventSourcing;
using Orleans.EventSourcing.CustomStorage;

namespace ServiceCustomerManager
{
    public interface ICustomerManager 
        : IGrainWithStringKey
        , IEventSourcedGrain<Customer, DomainEventBase>
        , ICustomStorageInterface<CustomerState, DomainEventBase>
    { }
}
