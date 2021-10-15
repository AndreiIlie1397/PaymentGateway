using FluentValidation;
using MediatR;
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
        public class Validator : AbstractValidator<Query>
        {
            public Validator(Database _database)
            {
                RuleFor(q => q).Must(query =>
                {
                    var person = query.PersonId.HasValue ?
                    _database.Persons.FirstOrDefault(x => x.Id == query.PersonId) :
                    _database.Persons.FirstOrDefault(x => x.Cnp == query.Cnp);

                    return person != null;
                }).WithMessage("Customer not found");
            }
        }
        public class Validator2 : AbstractValidator<Query>
        {
            public Validator2(Database database)
            {
                RuleFor(q => q).Must(person =>
                {
                    return person.PersonId.HasValue || !string.IsNullOrEmpty(person.Cnp);
                }).WithMessage("Customer data is invalid - personid");

                RuleFor(q => q.PersonId).Must(personId =>
                {
                    var exists = database.Persons.Any(x => x.Id == personId);
                    return exists;
                }).WithMessage("Customer does not exist");
            }
        }

        public class Query : IRequest<List<Model>>
        {
            public int? PersonId { get; set; }
            public string? Cnp { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, List<Model>>
        {
            private readonly Database _database;

            public QueryHandler(Database database)
            {
                _database = database;
            }

            public object Handle(Query query, object cancellationToken)
            {
                throw new NotImplementedException();
            }

            public Task<List<Model>> Handle(Query request, CancellationToken cancellationToken)
            {


                var person = request.PersonId.HasValue ?
                    _database.Persons.FirstOrDefault(x => x.Id == request.PersonId) :
                    _database.Persons.FirstOrDefault(x => x.Cnp == request.Cnp);

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
