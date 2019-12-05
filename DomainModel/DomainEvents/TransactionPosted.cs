
namespace DomainModel.DomainEvents
{
    public class TransactionPosted : DomainEventBase
    {
        public string AccountNumber;
        public decimal Amount;
        public decimal OldBalance;
        public decimal NewBalance;
    }
}
