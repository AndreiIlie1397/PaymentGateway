using Abstractions;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using System;
using System.Collections.Generic;
using System.Linq;

namespace PaymentGateway.Application.ReadOperations
{
    public class ListOfAccounts
    {
        public class Validator : IValidator<Query>
        {
            private readonly Data.Database _database;
            public Validator(Database database)
            {
                _database = database;
            }

            public bool Validate(Query input)
            {
                var person = input.PersonId.HasValue ?
                     _database.Persons.FirstOrDefault(x => x.Id == input.PersonId) :
                     _database.Persons.FirstOrDefault(x => x.Cnp == input.cnp);

                return person != null;
            }
        }

        public class Query
        {
            public int? PersonId { get; set; }
            public string cnp { get; set; }
        }

        public class QueryHandler : IReadOperation<Query, List<Model>>
        {
            private readonly Data.Database _database;
            private readonly IValidator<Query> _validator;

            public QueryHandler(Database database, IValidator<Query> validator)
            {
                _database = database;
                _validator = validator;
            }



            public List<Model> PerformOperation(Query query)
            {
                var isValid = _validator.Validate(query);
                if (!isValid)
                {
                    throw new Exception("Person not found");
                }

                var person = query.PersonId.HasValue ?
                    _database.Persons.FirstOrDefault(x => x.Id == query.PersonId) :
                    _database.Persons.FirstOrDefault(x => x.Cnp == query.cnp);

                var db = _database.Accounts.Where(x => x.PersonId == query.PersonId);
       
                var result = db.Select(x => new Model
                {
                    Balance = x.Balance,
                    Currency = x.Currency,
                    IbanCode = x.IbanCode,
                    Id = x.Id,
                    Limit = x.Limit,

                }).ToList();
                return result;
            }
        }

        public class Model
        {
            public int? Id { get; set; }
            public double Balance { get; set; }
            public string Currency { get; set; }
            public string IbanCode { get; set; }
            // public AccountType Type { get; set; }
            //  public AccountStatus Status { get; set; }
            public double Limit { get; set; }
        }
    }
}
