using System.Collections.Generic;

namespace DomainModel
{
    public class Customer
    {
        public string CustomerId;
        public Person PrimaryAccountHolder;
        public Person Spouse;
        public Address MailingAddress;
        public List<Account> Accounts = new List<Account>();
    }
}
