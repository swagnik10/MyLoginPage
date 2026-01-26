using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyApp.Contract;
using MyApp.Controllers.Base;
using MyApp.Domain;
using MyApp.DTOs;
using MyApp.Infrastructure;
using MyApp.Repositories;
using System.Text.RegularExpressions;


namespace MyApp.Controllers;

[Authorize]
[ApiController]
[Route("api/profile")]
public class ProfileController : ApiControllerBase
{
    private readonly IUserProfileRepository _profileRepository;
    private readonly IUnitOfWorkFactory _unitOfWorkFactory;

    public ProfileController(IUserProfileRepository profileRepository, IUnitOfWorkFactory unitOfWorkFactory)
    {
        _profileRepository = profileRepository;
        _unitOfWorkFactory = unitOfWorkFactory;
    }

    [HttpGet]
    public IActionResult GetProfile()
    {
        try
        {
            int userId = GetUserId();
            var profile = _profileRepository.GetByUserId(userId);

            if (profile == null)
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Message = "Profile not found",
                    Data = null
                });

            ProfileDto profileData = new()
            {
                FirstName = profile.FirstName,
                LastName = profile.LastName,
                Address = profile.Address,
                PhoneNumber = profile.PhoneNumber
            };

            return Ok(new ApiResponse<ProfileDto>
            {
                Success = true,
                Message = "Profile retrieved successfully",
                Data = profileData
            });
        }
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "User not authenticated",
                Data = null
            });
        }
    }

    [HttpPost]
    public IActionResult CreateProfile(CreateProfileRequest request)
    {
        try
        {
            int userId = GetUserId();
            if (string.IsNullOrWhiteSpace(request.FirstName) ||
                string.IsNullOrWhiteSpace(request.LastName))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "First name and last name are required",
                    Data = null
                });

            // Phone validation (digits + length)
            if (!Regex.IsMatch(request.PhoneNumber, @"^\d{10}$"))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid phone number format",
                    Data = null
                });

            using var uow = _unitOfWorkFactory.Create();
            uow.BeginTransaction();

            try
            {

                // Ensure profile does not already exist
                if (_profileRepository.ProfileExists(userId))
                    return BadRequest(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Profile already exists",
                        Data = null
                    });

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
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = null,
                    Message = "Profile created successfully"
                });
            }
            catch (Exception ex)
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
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "User not authenticated",
                Data = null
            });
        }
    }

    [HttpPut]
    public IActionResult UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        try
        {
            int userId = GetUserId();
            if (!Regex.IsMatch(request.PhoneNumber, @"^\d{10}$"))
                return BadRequest(new ApiResponse<object>
                {
                    Success = false,
                    Message = "Invalid phone number format",
                    Data = null
                });

            using var uow = _unitOfWorkFactory.Create();
            uow.BeginTransaction();

            try
            {
                var profile = _profileRepository.GetByUserId(userId);
                if (profile == null)
                    return Ok(new ApiResponse<object>
                    {
                        Success = false,
                        Message = "Profile not found",
                        Data = null
                    });

                // Update profile fields
                profile.FirstName = request.FirstName;
                profile.LastName = request.LastName;
                profile.Address = request.Address;
                profile.PhoneNumber = request.PhoneNumber;
                profile.UpdatedDate = DateTime.UtcNow;

                _profileRepository.UpdateProfile(profile);

                uow.Commit();
                return Ok(new ApiResponse<object>
                {
                    Success = true,
                    Data = null,
                    Message = "Profile updated successfully"
                });
            }
            catch (Exception ex)
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
        catch (UnauthorizedAccessException)
        {
            return Unauthorized(new ApiResponse<object>
            {
                Success = false,
                Message = "User not authenticated",
                Data = null
            });
        }
    }

}
