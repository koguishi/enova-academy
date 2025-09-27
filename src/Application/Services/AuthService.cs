using Microsoft.AspNetCore.Identity;

namespace enova_academy.Application.Services;

public class AuthService
{
    private readonly UserManager<IdentityUser> _userManager;
    public AuthService(UserManager<IdentityUser> userManager)
    {
        _userManager = userManager;
    }

    public async Task SelfRegisterUserAsync(string email, string password)
    {
        var user = new IdentityUser
        {
            UserName = email,
            Email = email,
        };
        var result = await _userManager.CreateAsync(user, password);
        await _userManager.AddToRoleAsync(user, "student");
        if (!result.Succeeded)
            throw new Exception("Erro ao criar usuário: " + string.Join(", ", result.Errors.Select(e => e.Description)));
    }

}
