using Microsoft.AspNetCore.Mvc;
using MyApp.Domain;
using MyApp.DTOs;
using MyApp.Infrastructure;
using MyApp.Repositories;
using System.Text.RegularExpressions;


namespace MyApp.Controllers;

[ApiController]
[Route("api/profile")]
public class ProfileController : ControllerBase
{
    private readonly IUserProfileRepository _profileRepository;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;
    private readonly IUserRepository _userRepository;

    public ProfileController(IUserProfileRepository profileRepository, IUnitOfWorkFactory unitOfWorkFactory, IUserRepository userRepository)
    {
        _profileRepository = profileRepository;
        _unitOfWorkFactory = unitOfWorkFactory;
        _userRepository = userRepository;
    }

    [HttpGet]
    public IActionResult GetProfile([FromHeader(Name = "X-User-Id")] int userId)
    {
        if (userId <= 0)
            return Unauthorized("User not authenticated");

        var profile = _profileRepository.GetByUserId(userId);

        if (profile == null)
            return NoContent(); // NEW USER → Page 2

        return Ok(new
        {
            profile.FirstName,
            profile.LastName,
            profile.Address,
            profile.PhoneNumber
        });
    }

    [HttpPost]
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

        using var uow = _unitOfWorkFactory.Create();
        uow.BeginTransaction();

        try
        {

            // Ensure profile does not already exist
            if (_profileRepository.ProfileExists(userId))
                return BadRequest("Profile already exists");

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
            return Ok(new
            {
                message = "Profile created successfully"
            });
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }

    [HttpPut]
    public IActionResult UpdateProfile( [FromHeader(Name = "X-User-Id")] int userId, [FromBody] UpdateProfileRequest request)
    {
        if (userId <= 0)
            return Unauthorized("User not authenticated");

        if (!Regex.IsMatch(request.PhoneNumber, @"^\d{10}$"))
            return BadRequest("Invalid phone number format");

        using var uow = _unitOfWorkFactory.Create();
        uow.BeginTransaction();

        try
        {
            var profile = _profileRepository.GetByUserId(userId);
            if (profile == null)
                return NotFound("Profile does not exist");

            // Update profile fields
            profile.FirstName = request.FirstName;
            profile.LastName = request.LastName;
            profile.Address = request.Address;
            profile.PhoneNumber = request.PhoneNumber;
            profile.UpdatedDate = DateTime.UtcNow;

            _profileRepository.UpdateProfile(profile);

            uow.Commit();
            return Ok(new
            {
                message = "Profile updated successfully"
            });
        }
        catch
        {
            uow.Rollback();
            throw;
        }
    }


    [HttpGet("status")]
    public IActionResult GetProfileStatus([FromHeader(Name = "X-User-Id")] int userId)
    {
        if (userId <= 0)
            return Unauthorized();

        bool exists = _profileRepository.ProfileExists(userId);

        return Ok(new { hasProfile = exists });
    }

}
