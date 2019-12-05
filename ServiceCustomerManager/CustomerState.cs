using DomainModel;
using DomainModel.DomainEvents;

namespace ServiceCustomerManager
{
    public class CustomerState : Customer
    {
        // The transition methods are assumed to have no side effects other than modifying the state object,
        // and should be deterministic(otherwise, the effects are unpredictable).

        // uses DLR
        // https://github.com/dotnet/orleans/blob/master/src/Orleans.EventSourcing/JournaledGrain.cs#L276

        public void Apply(Initialized e)
        {
            // Do nothing, this represents the default constructor
        }

        public void Apply(AccountAdded e)
        {
            var acct = Accounts.Find(a => a.AccountNumber.Equals(e.Account.AccountNumber));
            if(acct == null) Accounts.Add(e.Account);
        }

        public void Apply(AccountRemoved e)
        {
            Accounts.RemoveAll(a => a.AccountNumber.Equals(e.AccountNumber));
        }

        public void Apply(CustomerCreated e)
        {
            // CustomerId is part of this event but is already assigned.
            // See object creation in CustomerManager ReadStateSnapshot,
            // the id matches the key that was used to access the grain.
            PrimaryAccountHolder = e.PrimaryAccountHolder;
            MailingAddress = e.MailingAddress;
        }

        public void Apply(MailingAddressChanged e)
        {
            MailingAddress = e.Address;
        }

        public void Apply(ResidencePrimaryChanged e)
        {
            PrimaryAccountHolder.Residence = e.Address;
        }

        public void Apply(ResidenceSpouseChanged e)
        {
            Spouse.Residence = e.Address;
        }

        public void Apply(SpouseChanged e)
        {
            Spouse = e.Spouse;
        }

        public void Apply(SpouseRemoved e)
        {
            Spouse = null;
        }

        public void Apply(TransactionPosted e)
        {
            var acct = Accounts.Find(a => a.AccountNumber.Equals(e.AccountNumber));
            if (acct != null) acct.Balance = e.NewBalance;
        }

        public void Apply(DomainEventBase eb)
        {
            switch (eb)
            {
                case Initialized e:
                    Apply(e);
                    break;

                case AccountAdded e:
                    Apply(e);
                    break;

                case AccountRemoved e:
                    Apply(e);
                    break;

                case CustomerCreated e:
                    Apply(e);
                    break;

                case MailingAddressChanged e:
                    Apply(e);
                    break;

                case ResidencePrimaryChanged e:
                    Apply(e);
                    break;

                case ResidenceSpouseChanged e:
                    Apply(e);
                    break;

                case SpouseChanged e:
                    Apply(e);
                    break;

                case SpouseRemoved e:
                    Apply(e);
                    break;

                case TransactionPosted e:
                    Apply(e);
                    break;
            }
        }
    }
}
