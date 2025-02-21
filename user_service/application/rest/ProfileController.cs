using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using user_service.application.dto;
using user_service.domain.logics;
using user_service.services.jwt_authentification;

namespace user_service.application.rest
{
    [Route("api/profile")]
    [ApiController]
    public class ProfileController(UserLogic userLogic, JwtHelper jwtHelper) : ControllerBase
    {
        [HttpGet]
        public async Task<ActionResult<Profile>> GetMyProfile()
        {
            var resId = jwtHelper.UserId(this.HttpContext);
            if (resId.IsFailure)
                return BadRequest(resId.Error);
            
            var userId = resId.Value;
            
            var res = await userLogic.GetMyProfile(userId);
            if (res.IsFailure)
                return BadRequest(res.Error);
            
            return Ok(res.Value);
        }
    }
}
