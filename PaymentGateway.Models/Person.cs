using System.Collections.Generic;

namespace PaymentGateway.Models
{
    public class Person
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Cnp { get; set; }
        public PersonType Type { get; set; }

        public List<Account> Accounts { get; set; } = new List<Account>(); 
    }
}
