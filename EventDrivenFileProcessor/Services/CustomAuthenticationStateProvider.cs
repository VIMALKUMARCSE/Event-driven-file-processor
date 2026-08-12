using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace EventDrivenFileProcessor.Services
{
    public class CustomAuthenticationStateProvider
        : AuthenticationStateProvider
    {
        private ClaimsPrincipal _currentUser =
            new(new ClaimsIdentity());

        public override Task<AuthenticationState>
            GetAuthenticationStateAsync()
        {   
            return Task.FromResult(
                new AuthenticationState(_currentUser));
        }

        public void MarkUserAsAuthenticated(string email)
        {
            var identity = new ClaimsIdentity(
                new[]
                {
            new Claim(
                ClaimTypes.Name,
                email)
                },
                "Cognito");

            _currentUser =
                new ClaimsPrincipal(identity);

            NotifyAuthenticationStateChanged(
                Task.FromResult(
                    new AuthenticationState(
                        _currentUser)));
        }

        public void MarkUserAsLoggedOut()
        {
            _currentUser =
                new ClaimsPrincipal(
                    new ClaimsIdentity());

            var authState =
                Task.FromResult(
                    new AuthenticationState(_currentUser));

            NotifyAuthenticationStateChanged(authState);
        }
    }
}
