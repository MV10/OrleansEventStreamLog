using DomainModel;
using DomainModel.DomainEvents;

namespace ServiceCustomerManager
{
    public class CustomerState : Customer
    {
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
    }
}
