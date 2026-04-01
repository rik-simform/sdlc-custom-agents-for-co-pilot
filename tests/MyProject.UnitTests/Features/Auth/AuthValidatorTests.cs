#nullable enable

using FluentAssertions;
using MyProject.Application.Features.Auth.DTOs;
using MyProject.Application.Features.Auth.Validators;

namespace MyProject.UnitTests.Features.Auth;

/// <summary>
/// Unit tests for auth request validators.
/// </summary>
[TestClass]
public class AuthValidatorTests
{
    private readonly LoginRequestValidator _loginValidator = new();
    private readonly RefreshTokenRequestValidator _refreshValidator = new();

    [TestMethod]
    public async Task LoginRequestValidator_WithValidInput_Passes()
    {
        var result = await _loginValidator.ValidateAsync(new LoginRequest("test@example.com", "Password123!"));
        result.IsValid.Should().BeTrue();
    }

    [TestMethod]
    public async Task LoginRequestValidator_WithEmptyEmail_Fails()
    {
        var result = await _loginValidator.ValidateAsync(new LoginRequest("", "Password123!"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [TestMethod]
    public async Task LoginRequestValidator_WithInvalidEmail_Fails()
    {
        var result = await _loginValidator.ValidateAsync(new LoginRequest("not-an-email", "Password123!"));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Email");
    }

    [TestMethod]
    public async Task LoginRequestValidator_WithEmptyPassword_Fails()
    {
        var result = await _loginValidator.ValidateAsync(new LoginRequest("test@example.com", ""));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "Password");
    }

    [TestMethod]
    public async Task RefreshTokenRequestValidator_WithEmptyToken_Fails()
    {
        var result = await _refreshValidator.ValidateAsync(new RefreshTokenRequest(""));
        result.IsValid.Should().BeFalse();
        result.Errors.Should().Contain(e => e.PropertyName == "RefreshToken");
    }

    [TestMethod]
    public async Task RefreshTokenRequestValidator_WithValidToken_Passes()
    {
        var result = await _refreshValidator.ValidateAsync(new RefreshTokenRequest("valid-refresh-token"));
        result.IsValid.Should().BeTrue();
    }
}
