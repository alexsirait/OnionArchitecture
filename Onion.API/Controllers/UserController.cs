using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Onion.Application.Resource;
using Onion.Application.Service.Contract;

namespace Onion.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<UserResource>> Get(int id, CancellationToken cancellationToken)
        {
            var response = await _userService.Get(id, cancellationToken);
            return Ok(response);
        }

        [HttpGet]
        public async Task<ActionResult<UserResource>> GetAll(CancellationToken cancellationToken)
        {
            var response = await _userService.GetAll(cancellationToken);
            return Ok(response);
        }

        [HttpPost]
        public async Task<ActionResult<UserResource>> Create([FromBody]UserResource userResource, CancellationToken cancellationToken)
        {
            var response = await _userService.Create(userResource, cancellationToken);
            return CreatedAtAction(nameof(Get), new {response.Id}, response);
        }

        [HttpPut]
        public async Task<ActionResult<UserResource>> Update([FromBody]UserResource userResource, CancellationToken cancellationToken)
        {
            var response = await _userService.Update(userResource, cancellationToken);
            return Ok(response);
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
        {
            await _userService.Delete(id, cancellationToken);
            return NoContent();
        }
    }
}
