﻿using System.Diagnostics.Metrics;
using System.Security.Claims;
using AspNetCoreFeatureWithMonitor.Jwt;
using AspNetCoreFeatureWithMonitor.Models.Enum;
using AspNetCoreFeatureWithMonitor.Models.Request;
using AspNetCoreFeatureWithMonitor.Models.Response;
using AspNetCoreFeatureWithMonitor.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetCoreFeatureWithMonitor.Controllers;

/// <summary>
/// 取得 Token
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class TokenController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly JwtTokenGenerator _jwtTokenGenerator;
    private readonly IMeterFactory _meterFactory;

    public TokenController(IUserService userService, JwtTokenGenerator jwtTokenGenerator, IMeterFactory meterFactory)
    {
        _userService = userService;
        _jwtTokenGenerator = jwtTokenGenerator;
        _meterFactory = meterFactory;
    }
    
    /// <summary>
    /// 取得Token
    /// </summary>
    /// <param name="request"></param>
    /// <returns>Token</returns>
    [AllowAnonymous]
    [HttpPost]
    [Route("")]
    public IActionResult GetToken([FromBody] GetTokenRequest request)
    {
        var (isValid, user) = _userService.IsValid(request.Name, request.Password);
        if (!isValid)
        {
            return BadRequest(new ApiResponse<object>(ApiResponseStatus.UserNotFound));
        }
        var token = _jwtTokenGenerator.GenerateJwtToken(user.Id, user.Name, user.Roles);

        
        
        return Ok(new ApiResponse<object>(ApiResponseStatus.Success)
        {
            Data = token
        });
    }

    /// <summary>
    /// 取得角色
    /// </summary>
    /// <returns></returns>
    [Authorize]
    [HttpGet]
    [Route("Roles")]
    public IActionResult GetRoles()
    {
        var roleClaim = base.HttpContext.User.Claims.Where(item => item.Type == ClaimTypes.Role);
        if (!roleClaim.Any())
        {
            return BadRequest(new ApiResponse<object>(ApiResponseStatus.Fail)
            {
                Data = null
            });
        }
        var roles = roleClaim.Select(item => item.Value);
        return Ok(new ApiResponse<IEnumerable<string>>(ApiResponseStatus.Success)
        {
            Data = roles
        });
    }
}