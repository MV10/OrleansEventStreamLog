using System;

namespace DomainModel
{
    public class Person
    {
        public string FullName;
        public string FirstName;
        public string LastName;
        public Address Residence;
        public string TaxId;
        public DateTimeOffset DateOfBirth;
    }
}
