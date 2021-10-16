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
            public Validator(PaymentDbContext _database)
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
            public Validator2(PaymentDbContext database)
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
            public string Cnp { get; set; }
        }

        public class QueryHandler : IRequestHandler<Query, List<Model>>
        {
            private readonly PaymentDbContext _dbContext;

            public QueryHandler(PaymentDbContext dbContext)
            {
                _dbContext = dbContext;
            }

            public object Handle(Query query, object cancellationToken)
            {
                throw new NotImplementedException();
            }

            public Task<List<Model>> Handle(Query request, CancellationToken cancellationToken)
            {


                var person = request.PersonId.HasValue ?
                    _dbContext.Persons.FirstOrDefault(x => x.Id == request.PersonId) :
                    _dbContext.Persons.FirstOrDefault(x => x.Cnp == request.Cnp);

               

                /*
_dbContext.Persons.Where(x => x.Name.Contains("Vasile")); // select * from persons where name like '%Vasile%'
_dbContext.Persons.FirstOrDefault(x => x.Name.Contains("Vasile")); // select top 1 * from persons where name like '%Vasile%'
_database
    .Persons
    .Where(x => x.Name.Contains("Vasile"))
    .Select(x => new { x.Name, x.Cnp })
    .ToList(); //  select name, cnp from persons where name like '%Vasile%'
_database
    .Persons
    .Where(x => x.Name.Contains("Vasile"))
    .Take(5)
    .OrderBy(x=> x.Cnp)
    ; // select top 5 * from persons where name like '%Vasile%' order by cnp
_database
    .Persons
    .Where(x => x.Name.Contains("Vasile"))
    .Skip(10)
    .Take(5)
    //.OrderBy(x => x.Cnp)
    ; // select * from persons where name like '%Vasile%' limit 5, offset 10 order by cnp -- ia randurile de la 11 la 15 ordonate dupa CNP. 
_database
    .Persons
    .Where(x => x.Name.Contains("Vasile"))
    .Skip(10)
    .Take(5)
    ; // ia randurile de la 11 la 15 ordonate dupa CNP. 
*/

                var db = _dbContext.Accounts.Where(x => x.PersonId == person.Id);






                var result = db.Select(x => new Model
                {
                    Balance = x.Balance,
                    Currency = x.Currency,
                    IbanCode = x.Iban,
                    Id = x.Id,
                    Limit = x.Limit,

                }).ToList();
                return Task.FromResult(result);
            }
        }

        public class Model
        {
            public int? Id { get; set; }
            public decimal Balance { get; set; }
            public string Currency { get; set; }
            public string IbanCode { get; set; }
            public AccountType Type { get; set; }
            public AccountStatus Status { get; set; }
            public decimal Limit { get; set; }
        }
    }
}
