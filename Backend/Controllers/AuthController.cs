using Microsoft.AspNetCore.Mvc;
using MyApp.Domain;
using MyApp.DTOs;
using MyApp.Infrastructure;
using MyApp.Repositories;
using System.Text.RegularExpressions;

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
            return Ok("User registered successfully");
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

    [HttpPost("profile")]
    public IActionResult CreateProfile(CreateProfileRequest request)
    {
        // TEMPORARY SOURCE (until JWT)
        int userId = HttpContext.Items["UserId"] as int? ?? 0;

        if (userId <= 0)
            return Unauthorized("Invalid user session");


        if (string.IsNullOrWhiteSpace(request.FirstName) ||
            string.IsNullOrWhiteSpace(request.LastName))
            return BadRequest("First name and last name are required");

        // Phone validation (digits + length)
        if (!Regex.IsMatch(request.PhoneNumber, @"^\d{10}$"))
            return BadRequest("Invalid phone number format");

        // Ensure profile does not already exist
        if (_profileRepository.ProfileExists(userId))
            return BadRequest("Profile already exists");

        using var uow = _unitOfWorkFactory.Create();
        uow.BeginTransaction();

        try
        {

            var profile = new UserProfile
            {
                UserId = userId,
                FirstName = request.FirstName,
                LastName = request.LastName,
                Address = request.Address,
                PhoneNumber = request.PhoneNumber,
                CreatedDate = DateTime.UtcNow,
                UpdatedDate = DateTime.UtcNow
            };

            _profileRepository.CreateProfile(profile);

            uow.Commit();
            return Ok("Profile created successfully");
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }



}