using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using user_service.application.dto;
using user_service.domain.logics;
using Token = NuGet.Common.Token;

namespace user_service.application.rest
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(AuthLogic _authLogic) : ControllerBase
    {

        [HttpPost("login")]
        public async  Task<ActionResult<Token>> Login(AuthData authData)
        {
            var res = await _authLogic.Authenticate(authData);
            if (res.IsFailure)
                return BadRequest(res.Error);
            
            return Ok(res.Value);
        }

        [HttpPost("register")]
        public async Task<ActionResult<Token>> GetRegisterData(RegisterData registerData)
        {
            var res = await _authLogic.GetRegisterData(registerData);
            if (res.IsFailure)
                return BadRequest(res.Error);
            
            return Ok();
        }

        [HttpPost("confirm_email")]
        public async Task<ActionResult> ConfirmEmailAndFinishRegister(string email, string code)
        {
            var res = await _authLogic.ConfrimEmailAndRegister(
                new ConfirmEmailData
                {
                    Email = email, 
                    Code = code
                });
            
            if (res.IsFailure)
                return BadRequest(res.Error);
            
            return Ok();
        }
        
        
    }
}
