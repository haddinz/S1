// using Microsoft.AspNetCore.Identity;
// using Microsoft.AspNetCore.Mvc;
// using Support.Auth.Id.Models.Dtos.Response;
// using Support.Auth.Id.Models.Entity;
// using Support.Auth.Id.Models.Enum;
// using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

// namespace Support.Auth.Id.Controller;

// [ApiController]
// [Route("api/a/[controller]")]
// public class UserController : ControllerBase
// {
//     private readonly ILogger _logger;
//     private readonly SignInManager<User> _signInManager;
//     private readonly UserManager<User> _userManager;
//     private readonly RoleManager<IdentityRole> _roleManager;

//     public UserController(
//         ILogger<UserController> logger,
//         SignInManager<User> signInManager,
//         UserManager<User> userManager,
//         RoleManager<IdentityRole> roleManager
//     )
//     {
//         _logger = logger;
//         _signInManager = signInManager;
//         _userManager = userManager;
//         _roleManager = roleManager;
//     }

//     [HttpGet]
//     public IActionResult Login()
//     {
//         return Ok("Login Success");
//     }

//     // [HttpPost("login")]
//     // public async Task<ActionResult<ValidationResponse<Login>>> Login(Login model)
//     // {
//     //     _logger.LogInformation("--> Hit Login request");
//     //     if (!ModelState.IsValid)
//     //     {
//     //         return BadRequest(
//     //             ValidationResponse(
//     //                 Message: "Validation Error",
//     //                 StatusCode: StatusCodes.Status400BadRequest,
//     //                 Data: ModelState.ToDictionary(
//     //                     x => x.Key,
//     //                     x => x.Value!.Errors.Select(e => e.ErrorMessage)
//     //                 )
//     //             )
//     //         );
//     //     }

//     //     SignInResult result = await _signInManager.PasswordSignInAsync(
//     //         model.Email,
//     //         model.Password,
//     //         model.RememberMe,
//     //         lockoutOnFailure: false
//     //     );

//     //     if (result.IsNotAllowed)
//     //     {
//     //         _logger.LogInformation("--> Login failed email not confirmed");
//     //         return Unauthorized(
//     //             ValidationResponse<object>(
//     //                 Message: "Login failed email not confirmed",
//     //                 StatusCode: StatusCodes.Status401Unauthorized,
//     //                 Data: null!
//     //             )
//     //         );
//     //     }

//     //     if (!result.Succeeded)
//     //     {
//     //         _logger.LogError("--> Invalid Login Attemp");
//     //         return Unauthorized(
//     //             ValidationResponse<object>(
//     //                 Message: "Invalid username or password",
//     //                 StatusCode: StatusCodes.Status401Unauthorized,
//     //                 Data: null!
//     //             )
//     //         );
//     //     }

//     //     _logger.LogInformation("--> Login request success");
//     //     return Ok(
//     //         ValidationResponse(
//     //             Message: "Login Successfully",
//     //             StatusCode: StatusCodes.Status200OK,
//     //             Data: model
//     //         )
//     //     );
//     // }

//     [HttpPost("register")]
//     public async Task<IActionResult> Register(Register model)
//     {
//         _logger.LogInformation("--> Hit Regiter request");
//         if (!ModelState.IsValid)
//         {
//             return BadRequest(ModelState);
//         }

//         // User user = new()
//         // {
//         //     FullName = model.Name,
//         //     UserName = model.Email,
//         //     Email = model.Email,
//         //     // NormalizedUserName = model.Email.ToUpper(),
//         //     // NormalizedEmail = model.Email.ToUpper(),
//         // };

//         IdentityResult result = await _userManager.CreateAsync(user, model.Password);
//         if (!result.Succeeded)
//         {
//             _logger.LogInformation("--> Failed to register {error}", result.Errors);
//             return BadRequest(new { errors = result.Errors.Select(e => e.Description) });
//         }

//         if (!await _roleManager.RoleExistsAsync(RoleEnum.User.ToString()))
//         {
//             IdentityRole role = new(RoleEnum.User.ToString());
//             await _roleManager.CreateAsync(role);
//         }

//         await _userManager.AddToRoleAsync(user, RoleEnum.User.ToString());

//         // SignInAsync digunakan untuk cookie-based authentication (session)
//         // await _signInManager.SignInAsync(user, isPersistent: false);

//         _logger.LogInformation("--> Register user successfully");
//         return StatusCode(201, new { message = "Register Successfully" });
//     }

//     [HttpPost("verify-email")]
//     public async Task<IActionResult> VerifiEmail(VerifiEmail model)
//     {
//         _logger.LogInformation("--> Hit Verifi Email request");
//         if (!ModelState.IsValid)
//         {
//             return BadRequest(ModelState);
//         }

//         User? user = await _userManager.FindByEmailAsync(model.Email);

//         if (user == null)
//         {
//             _logger.LogError("User with email {email} is not found", model.Email);
//             return NotFound(new { message = $"user with {model.Email} is not found" });
//         }

//         return Ok(new { message = "Verify Token Needed" });
//     }

//     [HttpPost("change-password")]
//     public async Task<IActionResult> ChangePassword(ChangePassword model)
//     {
//         _logger.LogInformation("--> Hit Change Password request");
//         if (!ModelState.IsValid)
//         {
//             return BadRequest(ModelState);
//         }

//         User? user = await _userManager.FindByNameAsync(model.Email);
//         if (user == null)
//         {
//             _logger.LogError("--> User with email {email} is not found", model.Email);
//             return NotFound(new { message = $"user with {model.Email} is not found" });
//         }

//         IdentityResult userPassword = await _userManager.RemovePasswordAsync(user);
//         if (!userPassword.Succeeded)
//         {
//             _logger.LogInformation("--> Failed to register {error}", userPassword.Errors);
//             return BadRequest(new { errors = userPassword.Errors.Select(e => e.Description) });
//         }

//         userPassword = await _userManager.AddPasswordAsync(user, model.NewPassword);

//         _logger.LogInformation("--> Changes Password {name} successfully", model.Email);
//         return StatusCode(201, new { message = $"Changes password {model.Email} successfully" });
//     }

//     [HttpPost("logout")]
//     public async Task<IActionResult> Logout()
//     {
//         _logger.LogInformation("--> Hit Logout confirmed");
//         await _signInManager.SignOutAsync();

//         return Ok(new { message = "Logout successfully" });
//     }

//     private ActionResult<ValidationResponse<T>> ValidationResponse<T>(
//         T Data,
//         string Message,
//         int StatusCode,
//         PagingResponse? Pagination = null
//     )
//     {
//         return new ValidationResponse<T>
//         {
//             Message = Message,
//             StatusCode = StatusCode,
//             Data = Data,
//             PagingResponse = Pagination,
//         };
//     }
// }
