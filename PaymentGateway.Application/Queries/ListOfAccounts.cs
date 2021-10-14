using MediatR;
using PaymentGateway.Abstractions;
using PaymentGateway.Data;
using PaymentGateway.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentGateway.Application.Queries
{
    public class ListOfAccounts
    {
        public class Validator : IValidator<Query>
        {
            private readonly Database _database;
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

        public class Query : IRequest<List<Model>>
        {
            public int? PersonId { get; set; }
            public string cnp { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, List<Model>>
        {
            private readonly Database _database;
            private readonly IValidator<Query> _validator;

            public QueryHandler(Database database, IValidator<Query> validator)
            {
                _database = database;
                _validator = validator;
            }

            public object Handle(Query query, object cancellationToken)
            {
                throw new NotImplementedException();
            }

            public Task<List<Model>> Handle(Query request, CancellationToken cancellationToken)
            {
                var isValid = _validator.Validate(request);
                if (!isValid)
                {
                    throw new Exception("Person not found");
                }

                var person = request.PersonId.HasValue ?
                    _database.Persons.FirstOrDefault(x => x.Id == request.PersonId) :
                    _database.Persons.FirstOrDefault(x => x.Cnp == request.cnp);

                var db = _database.Accounts.Where(x => x.PersonId == request.PersonId);

                var result = db.Select(x => new Model
                {
                    Balance = x.Balance,
                    Currency = x.Currency,
                    IbanCode = x.IbanCode,
                    Id = x.Id,
                    Limit = x.Limit,

                }).ToList();
                return Task.FromResult(result);
            }
        }

        public class Model
        {
            public int? Id { get; set; }
            public double Balance { get; set; }
            public string Currency { get; set; }
            public string IbanCode { get; set; }
            public AccountType Type { get; set; }
            public AccountStatus Status { get; set; }
            public double Limit { get; set; }
        }
    }
}
