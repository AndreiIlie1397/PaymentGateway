using Microsoft.AspNetCore.Mvc;
using PaymentGateway.Application.WriteOperations;
using PaymentGateway.Data;
using PaymentGateway.PublishedLanguage.WriteSide;

namespace PaymentGateway.WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly CreateAccountOperation _createAccountCommandHandler;
        public AccountController(CreateAccountOperation createAccountCommandHandler)
        {
            _createAccountCommandHandler = createAccountCommandHandler;
        }
        [HttpPost]
        [Route("Create")]
        public string CreateAccount(CreateAccountCommand command)
        {
            Database db = new Database();
            _createAccountCommandHandler.PerformOperation(command);
            return "ok";
        }
    }
}
