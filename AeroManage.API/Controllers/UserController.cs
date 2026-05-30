using AeroMange.UserManagement.Application.Commands.Users;
using AeroMange.UserManagement.Application.DTOs;
using AeroMange.UserManagement.Application.Queries.Users;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AeroManage.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;
        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpDelete("deleteUser")]
        public async Task<IActionResult> DeleteUser(int userId)
        {
            try
            {
                var data = await _mediator.Send(new DeleteUserCommand(userId));
                if (data == null)
                    return BadRequest("User not found for this id");
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut("updateUser")]
        public async Task<IActionResult> UpdateUser(UpdateUserDto dto)
        {
            try
            {
                var data = await _mediator.Send(new UpdateUserCommand(dto));
                if (data == null)
                    return BadRequest("Unable to update user");
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpPut("updateuserProfile")]
        public async Task<IActionResult> UpdateUserProfile(UpdateUserProfileDto dto)
        {
            try
            {
                var data = await _mediator.Send(new UpdateUserProfileCommand(dto));
                if (data == null)
                    return BadRequest("Unable to update User Profile");
                return Ok(data);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        [HttpGet("getAllUser")]
        public async Task<IActionResult> GetAllUser(GetUserDto dto)
        {
            try
            {
                var data = await _mediator.Send(new GetAllUsersQuery(dto));
                if (data == null)
                    return NotFound("Unable to find user");
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

        [HttpGet("getUser/{id}")]
        public async Task<IActionResult> GetUserById(int userId)
        {
            try
            {
                var data = await _mediator.Send(new GetUserByIdQuery(userId));
                if (data == null)
                    return NotFound("Unable to find User");
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }
        [HttpGet("getAllrole")]
        public async Task<IActionResult> GetAllRoles()
        {
            try
            {
                var data = await _mediator.Send(new GetAllRolesQuery());
                if (data == null)
                    return NotFound("Unable to find All role");
                return Ok(data);
            }
            catch(Exception ex)
            {
                throw;
            }
        }

    }
}
