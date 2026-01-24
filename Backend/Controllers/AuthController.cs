using Microsoft.AspNetCore.Mvc;
using MyApp.Contract;
using MyApp.Domain;
using MyApp.DTOs;
using MyApp.Infrastructure;
using MyApp.Repositories;

namespace MyApp.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _profileRepository;

    public AuthController(
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserRepository userRepository,
        IUserProfileRepository profileRepository)
    {
        _unitOfWorkFactory = unitOfWorkFactory;
        _userRepository = userRepository;
        _profileRepository = profileRepository;
    }

    [HttpPost("register")]
    public IActionResult Register(RegisterUserRequest request)
    {
        using var uow = _unitOfWorkFactory.Create();
        uow.BeginTransaction();

        try
        {
            // Username uniqueness
            if (_userRepository.UserNameExists(request.UserName))
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Username already exists",
                    Data = null
                });

            }
            // Password rule (length only for now)
            if (request.Password.Length < 6)
            {
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Password must be at least 6 characters",
                    Data = null
                });
            }

            var user = new UsersCredentials
            {
                UserName = request.UserName,
                Password = request.Password, // hashing later
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            _userRepository.CreateUser(user);

            uow.Commit();
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Data = null,
                Message = "User registered successfully"
            });

        }
        catch(Exception ex)
        {
            uow.Rollback();
            return StatusCode(500, new ApiResponse<object>
            {
                Data = null,
                Success = false,
                Message = ex.Message
            });

        }
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(request.UserName) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest(new ApiResponse<object>
            {
                Success = false,
                Message = "Username and password are required",
                Data = null
            });
        }

        // Fetch user by username
        var user = _userRepository.GetByUserName(request.UserName);

        if (user == null)
            return BadRequest(new ApiResponse<object>{
                Success = false,
                Message = "Invalid username",
                Data = null
        });

        // Validate password (plain text for now)
        if (user.Password != request.Password)
            return BadRequest(new ApiResponse<object> { Success = false, Message = "Invalid password", Data = null });

        // Check profile existence
        bool hasProfile = _profileRepository.ProfileExists(user.UserId);

        // Return decision to frontend
        var response = new LoginResponse
        {
            UserId = user.UserId,
            HasProfile = hasProfile
        };

        return Ok(new ApiResponse<LoginResponse>
        {
            Message = "Login successful",
            Data = response,
            Success = true
        });
    }

    [HttpPut("update-credentials")]
    public IActionResult UpdateCredentials(UpdateCredentialsRequest request)
    {
        var userId = (int?)HttpContext.Items["UserId"];
        if (userId == null)
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "User not authenticated",
                Data = null
            });

        using var uow = _unitOfWorkFactory.Create();
        uow.BeginTransaction();

        try
        {
            var user = _userRepository.GetById(userId.Value);
            if (user == null)
                return Ok(new { data = (object?)null });

            // Username update
            if (!string.IsNullOrWhiteSpace(request.UserName))
            {
                if (_userRepository.UserNameExists(request.UserName))
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Username already exists",
                        Data = null
                    });

                user.UserName = request.UserName;
            }

            // Password update (plain text for now)
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                if (request.Password.Length < 6)
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Password must be at least 6 characters",
                        Data = null
                    });

                user.Password = request.Password;
            }
            user.UpdatedDate = DateTime.Now;
            _userRepository.UpdateCredentials(user);
            uow.Commit();

            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "Credentials updated successfully",
                Data = null
            });

        }
        catch(Exception ex)
        {
            uow.Rollback();
            return StatusCode(500, new ApiResponse<object>
            {
                Data = null,
                Success = false,
                Message = ex.Message
            });
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // DEV / PRE-JWT:
        // Nothing to invalidate on server
        // Frontend will clear local storage / memory

        return Ok(new ApiResponse<object>
        {
            Success = true,
            Data = null,
            Message = "Logged out successfully"
        });
    }

    [HttpGet("get-credentials")]
    public IActionResult GetCredentials()
    {
        var userId = (int?)HttpContext.Items["UserId"];
        if (userId == null || userId < 0 )
            return Unauthorized(new ApiResponse<object> { Success = false, Message = "User not authenticated", Data = null });
        var user = _userRepository.GetById(userId.Value);
        if (user == null)
            return Ok(new ApiResponse<object>
            {
                Success = true,
                Message = "No user found",
                Data = null
            });

        LoginRequest response = new LoginRequest
        {
            UserName = user.UserName,
            Password = user.Password
        };

        return Ok(new ApiResponse<LoginRequest>
        {
            Success = true,
            Message = "User credentials fetched successfully",
            Data = response
        });
    }

}