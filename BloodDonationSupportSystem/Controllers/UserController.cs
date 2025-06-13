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
        //[HttpPost]
        //public async Task<IActionResult> Post([FromBody] UserDTO userDTO)
        //{
        //    if(userDTO == null)
        //    {
        //        Console.WriteLine("UserDTO is null in Post method.");
        //    }
        //     await _userServices.AddUserAsync(userDTO);
        //    return CreatedAtAction(nameof(GetAllUsers), new {}, userDTO);
        //}


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

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDTO login)
        {
            try
            {
                if (login == null || string.IsNullOrEmpty(login.Email) || string.IsNullOrEmpty(login.Password))
                {
                    return BadRequest(new { status = "failed", message = "Email and password are required" });
                }

                var token = await _userServices.GenerateToken(login);
                if (!string.IsNullOrEmpty(token))
                {
                    return new JsonResult(new
                    {
                        result = token
                    });
                }
                return NotFound(new { status = "failed", message = "Invalid email or password" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Login error: {ex.Message}");
                return new JsonResult(new
                {
                    status = "failed",
                    message = "An error occurred during login"
                });
            }
        }
        [HttpPost("registerDonor")]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerDTO)
        {
            try
            {
                await _userServices.RegisterDonorAsync(registerDTO);
            }
            catch (Exception ex)
            {
                return new BadRequestObjectResult(new
                {
                    status = "failed",
                    msg = ex.Message
                });

            }
            return Ok("Donor registered successfully.");

        }
    }
}
