
namespace DomainModel.DomainEvents
{
    public class CustomerCreated : DomainEventBase
    {
        public Person PrimaryAccountHolder;
        public Address MailingAddress;
    }
}
