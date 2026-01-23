using Microsoft.AspNetCore.Mvc;
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
                return BadRequest("Username already exists");
            }
            // Password rule (length only for now)
            if (request.Password.Length < 6)
            {
                return BadRequest("Password must be at least 6 characters");
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
            return Ok(new
            {
                message = "User registered successfully"
            });

        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }

    [HttpPost("login")]
    public IActionResult Login(LoginRequest request)
    {
        // Basic validation
        if (string.IsNullOrWhiteSpace(request.UserName) ||
            string.IsNullOrWhiteSpace(request.Password))
        {
            return BadRequest("Username and password are required");
        }

        // Fetch user by username
        var user = _userRepository.GetByUserName(request.UserName);

        if (user == null)
            return BadRequest("Invalid username");

        // Validate password (plain text for now)
        if (user.Password != request.Password)
            return BadRequest("Invalid password");

        // Check profile existence
        bool hasProfile = _profileRepository.ProfileExists(user.UserId);

        // Return decision to frontend
        var response = new LoginResponse
        {
            UserId = user.UserId,
            HasProfile = hasProfile
        };

        return Ok(response);
    }

    [HttpPut("update-credentials")]
    public IActionResult UpdateCredentials(UpdateCredentialsRequest request)
    {
        var userId = (int?)HttpContext.Items["UserId"];
        if (userId == null)
            return Unauthorized();

        using var uow = _unitOfWorkFactory.Create();
        uow.BeginTransaction();

        try
        {
            var user = _userRepository.GetById(userId.Value);
            if (user == null)
                return NotFound();

            // Username update
            if (!string.IsNullOrWhiteSpace(request.UserName))
            {
                if (_userRepository.UserNameExists(request.UserName))
                    return BadRequest("Username already exists");

                user.UserName = request.UserName;
            }

            // Password update (plain text for now)
            if (!string.IsNullOrWhiteSpace(request.Password))
            {
                if (request.Password.Length < 6)
                    return BadRequest("Password must be at least 6 characters");

                user.Password = request.Password;
            }

            _userRepository.UpdateCredentials(user);
            uow.Commit();

            return Ok(new
            {
                message = "Credentials updated successfully"
            });

        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }

    [HttpPost("logout")]
    public IActionResult Logout()
    {
        // DEV / PRE-JWT:
        // Nothing to invalidate on server
        // Frontend will clear local storage / memory

        return Ok(new
        {
            message = "Logged out successfully"
        });
    }


}