using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.ReadOperations;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.Data;
using PaymentGateway.PublishedLanguage.WriteSide;
using System.Collections.Generic;

namespace PaymentGateway.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly ListOfAccounts.QueryHandler _queryHandler;
        private readonly CreateAccountOperation _createAccountCommandHandler;
        public AccountController(CreateAccountOperation createAccountCommandHandler, ListOfAccounts.QueryHandler queryHandler)
        {
            _createAccountCommandHandler = createAccountCommandHandler;
            _queryHandler = queryHandler;
        }
        [HttpPost]
        [Route("Create")]
        public string CreateAccount(CreateAccountCommand command)
        {
            Database db = new Database();
            _createAccountCommandHandler.PerformOperation(command);
            return "ok";
        }

        [HttpGet]
        [Route("ListOfAccounts")]
        public List<ListOfAccounts.Model> GetListOfAccounts([FromQuery] ListOfAccounts.Query query)
        {
            var result = _queryHandler.PerformOperation(query);
            return result;
        }
    }
}
