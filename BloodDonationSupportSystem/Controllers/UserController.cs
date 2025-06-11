using BusinessLayer.IService;
using DataAccessLayer.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Threading.Tasks;

namespace BloodDonationSupportSystem.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserServices _userServices;
        public UserController(IUserServices userServices)
        {
            _userServices = userServices;
        }

        // GET: api/user
        [HttpGet]
        public async Task<IActionResult> GetAllUsers()
        {
            var users = await _userServices.GetAllUsersAsync();
            return Ok(users);
        }

        // POST: api/user
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UserDTO userDTO)
        {
            if(userDTO == null)
            {
                Console.WriteLine("UserDTO is null in Post method.");
            }
             await _userServices.AddUserAsync(userDTO);
            return CreatedAtAction(nameof(GetAllUsers), new {}, userDTO);
        }


        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] string value)
        {
            if (id <= 0 || string.IsNullOrEmpty(value))
            {
                return BadRequest("Invalid ID or value.");
            }
            return NoContent();
        }
        // DELETE: api/user/5
        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest("Invalid ID.");
            }
            return NoContent();
        }
    }
}
