


using Backend.Extensions;
using Backend.Models;
using Backend.Models.Cloudinary;
using Backend.Models.DTOs;
using Backend.Models.EmailSettings;
using Backend.Models.GoogleSettings;
using Backend.Models.Jwt;
using Backend.Models.Options;
using Backend.Services.Interfaces;
using Google.Apis.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.RegularExpressions;

namespace Backend.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly UserManager<User> _userManager;
        private readonly IEmailService _emailService;
        private readonly ITokenCreationService _jwtService;
        private readonly IUserService _userService;
        private readonly ILogger<UsersController> _logger;
        private readonly ISubscribtionService _subService;
        private readonly GoogleCredentials _googleSettings;

        public UsersController(
            UserManager<User> userManager,
            IEmailService emailService,
            ITokenCreationService jwtService,
            IUserService userService,
            ILogger<UsersController> logger,
            ISubscribtionService subService,
            IOptions<GoogleCredentials> googleSettings)
        {
            _userManager = userManager;
            _emailService = emailService;
            _jwtService = jwtService;
            _userService = userService;
            _logger = logger;
            _subService = subService;
            _googleSettings = googleSettings.Value;


        }

        [HttpGet("ConfirmEmail")]
        public async Task<IActionResult> ConfirmEmail(string userId, string token)
        {
            bool success = await _userService.ConfirmEmailAsync(userId, token);

            //var user = await _userManager.FindByIdAsync(userId);
            //if (user == null) return NotFound();

            //var result = await _userManager.ConfirmEmailAsync(user, token);

            if (success) //result.Succeeded
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }

        [HttpPost("resend")]
        public async Task<ActionResult> ResendVerificationEmailAsync(EmailDto email)
        {
            User user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email.Email);
            if (user != null)
            {
                var emailConfirmationToken = await _userService.GenerateConfirmationLinkAsync(user);

                var confirmationLink = Url.Action(nameof(ConfirmEmail), "Users", new { token = emailConfirmationToken, userId = user.Id }, Request.Scheme);

                await _emailService.SendConfirmationEmail(email.Email, confirmationLink!);
                return Ok();
            }
            return BadRequest("email not match");
        }

        [HttpPost("CreateUser")]
        public async Task<ActionResult<CreateUserDto>> CreateUser(CreateUserDto createUser)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var (newUser, errorMessage) = await _userService.CreateUserAsync(createUser);

            if (newUser == null)
            {
                return BadRequest(errorMessage);
            }

            // Generálunk egy e-mail megerősítési tokent
            var emailConfirmationToken = await _userService.GenerateConfirmationLinkAsync(newUser);

            // Generálunk egy megerősítő linket
            var confirmationLink = Url.Action(nameof(ConfirmEmail), "Users", new { token = emailConfirmationToken, userId = newUser.Id }, Request.Scheme);

            // Küldjünk e-mailt a felhasználónak a confirmationLink-el
            await _emailService.SendConfirmationEmail(newUser.Email, confirmationLink);

            if (createUser.IsSubscribed)
            {
                await _subService.SubscribeUserToNewsletterAsync(newUser);
            }

            return Ok();
        }

        //[HttpPost("forgot-password")]
        //public async Task<IActionResult> ForgotPassword(string email)
        //{
        //    var user = await _userManager.Users.FirstOrDefaultAsync(u => u.Email == email);
        //    //var user = await _userService.GetUserByEmailAsync(email);            
        //    //var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        //    if (user == null)
        //    {
        //        return BadRequest("User not found.");
        //    }

        //    user.PasswordResetToken = CreateRandomToken();
        //    user.ResetTokenExpires = DateTime.Now.AddHours(1);
        //    await _context.SaveChangesAsync();

        //    return Ok("You may now reset your password.");
        //}


        [HttpPost("AddRole")]
        public async Task<ActionResult> AddRoleToUser(string userId, string role)
        {
            var result = await _userService.AddRoleToUser(userId, role);

            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound("User not found");
            }
        }

        [HttpDelete("RemoveRole")]
        public async Task<ActionResult> RemoveRoleFromUser(string userId, string role)
        {
            var result = await _userService.RemoveRoleFromUser(userId, role);

            if (result)
            {
                return Ok();
            }
            else
            {
                return NotFound("User not found");
            }
        }

        [HttpPost("Login")]
        public async Task<ActionResult<AuthenticationResponse>> Login(AuthenticationRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Bad email or username!");
            }

            var user = await _userManager.FindByNameAsync(request.UserName);

            if (user == null)
            {
                return BadRequest("Do not have this user!");
            }

            var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);

            if (!isPasswordValid)
            {
                return BadRequest("Invalid Passwors!");
            }

            if (!user.EmailConfirmed)
            {
                return BadRequest("Email address did not verified!!!");
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);

            AuthenticationResponse token = await _jwtService.CreateTokensAsync(user);

            return Ok(token);
        }

        [HttpPost(nameof(Refresh))]
        public async Task<ActionResult<JwtToken>> Refresh(RefreshRequest refreshRequest)
        {
            try
            {
                AuthenticationResponse authenticationResponse = await _jwtService.RenewTokensAsync(refreshRequest.RefreshToken);
                return Ok(authenticationResponse);
            }
            catch (JwtException)
            {
                return Forbid();
            }
        }

        [HttpPost(nameof(Logout))]
        public ActionResult Logout(LogoutRequest logoutRequest)
        {
            _jwtService.ClearRefreshToken(logoutRequest.RefreshToken);
            return Ok();
        }

        [HttpGet(nameof(Me)), Authorize]
        public async Task<ActionResult<UserInfoDto>> Me()
        {
            string currentUserId = User.GetCurrentUserId();

            string profilePictureUrl = await _userService.GetProfilePictureOfUserById(currentUserId);

            return Ok(new UserInfoDto
            {
                UserId = User.Claims.First(claim => claim.Type == ClaimTypes.NameIdentifier).Value,
                UserName = User.Claims.First(claim => claim.Type == ClaimTypes.Name).Value,
                Email = User.Claims.First(claim => claim.Type == ClaimTypes.Email).Value,
                Roles = User.Claims.Where(claim => claim.Type == ClaimTypes.Role).Select(c => c.Value),
                ProfilePictureUrl = profilePictureUrl
            });
        }

        [HttpGet(nameof(GetAllUsers))]
        public async Task<ActionResult<IEnumerable<UserDetailsDto>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsersAsync();
            return Ok(users);
        }

        [HttpPut("ChangeProfilePicture")]
        [Authorize(Roles = "Admin, Journalist, User")]
        public async Task<ActionResult> ChangeProfilePictureAsync([FromForm] AddProfilePicture profilePicture)
        {
            var currentUserId = User.GetCurrentUserId();
            await _userService.ChangeProfilePictureAsync(profilePicture, currentUserId);
            return Ok();

        }
        //[HttpPost(nameof(LoginWithGoogle))]
        //public async Task<ActionResult> LoginWithGoogle(LoginWithCredential loginCredential)
        //{
        //    var result = await _userService.LoginWithGoogle(loginCredential);

        //    if (result.IsSuccess)
        //    {
        //        return Ok(result.Data);
        //    }
        //    else
        //    {
        //        return BadRequest(result.ErrorMessage);
        //    }
        //}

        [HttpPost(nameof(LoginWithGoogle))]
        public async Task<ActionResult> LoginWithGoogle(LoginWithCredential loginCredential)
        {
            var settings = new GoogleJsonWebSignature.ValidationSettings()
            {
                Audience = new List<string> { this._googleSettings.GoogleClientId }
            };

            var payload = await GoogleJsonWebSignature.ValidateAsync(loginCredential.Credential, settings);

            var userToFind = await _userManager.FindByEmailAsync(payload.Email);

            if (userToFind != null)
            {
                AuthenticationResponse authenticationResponse = await _jwtService.CreateTokensAsync(userToFind);

                return Ok(authenticationResponse);
            }
            else
            {
                var defaultRole = "User";
                var usname = GenerateUsername(payload.Email);
                User user = new User
                {
                    UserName = usname,
                    Email = payload.Email,
                    ProfilePictureUrl = "https://res.cloudinary.com/dzprbzfjm/image/upload/v1684929800/Trawell/d7a9621b-20b9-4af2-8985-a4a5b8b59a26.png",
                    EmailConfirmed = true
                };



                var result = await _userManager.CreateAsync(user);
                var addToRoleResult = await _userManager.AddToRoleAsync(user, defaultRole);
                return Ok(result);
            }

        }
        static string GenerateUsername(string email)
        {
            // Remove special characters using regular expressions
            var username = Regex.Replace(email, "@.*", "");

            return username;
        }
    }

    public record LoginWithCredential
    {
        public string Credential { get; set; }
    }
}

