using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using user_service.application.dto;
using user_service.domain.logics;
using user_service.services.jwt_authentification;
using Token = NuGet.Common.Token;

namespace user_service.application.rest
{
    [Route("api/auth")]
    [ApiController]
    public class AuthController(AuthLogic _authLogic, JwtHelper jwtHelper) : ControllerBase
    {

        [HttpPost("login")]
        public async  Task<ActionResult<AuthorizeDTO>> Login(AuthData authData)
        {
            var res = await _authLogic.Authenticate(authData);
            if (res.IsFailure)
                return BadRequest(res.Error!.Description);
            
            return Ok(res.Value);
        }

        [HttpPost("register")]
        public async Task<ActionResult> GetRegisterData(RegisterData registerData)
        {
            var res = await _authLogic.GetRegisterData(registerData);
            if (res.IsFailure)
                return BadRequest(res.Error!.Description);
            
            return Ok();
        }

        [HttpPost("confirm_email")]
        public async Task<ActionResult<AuthorizeDTO>> ConfirmEmailAndFinishRegister([FromBody]ConfirmEmailData data)
        {
            var res = await _authLogic.ConfrimEmailAndRegister(data);
            
            if (res.IsFailure)
                return BadRequest(res.Error);

            return Ok(res.Value);
        }
        
        [HttpPost("update_token")]
        public async Task<ActionResult<AuthorizeDTO>> UpdateToken([FromBody] UpdateTokenRequest data)
        {
            if (string.IsNullOrWhiteSpace(data.Token))
                return BadRequest("Токен пустой");
                
            var res = await _authLogic.UpdateAccessToken(data.Token);
            if (res.IsFailure)
                return BadRequest(res.Error!.Description);
            
            return Ok(res.Value);
        }
        
    }
}
