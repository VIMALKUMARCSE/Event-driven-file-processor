using Amazon;
using Amazon.CognitoIdentityProvider;
using Amazon.CognitoIdentityProvider.Model;
using EventDrivenFileProcessor.Models;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;
using System.Text;

namespace EventDrivenFileProcessor.Services
{
    public class CognitoAuthService
    {
        private readonly CognitoSettings _settings;
        private readonly AmazonCognitoIdentityProviderClient _client;

        public CognitoAuthService(IOptions<CognitoSettings> options)
        {
            _settings = options.Value;

            _client = new AmazonCognitoIdentityProviderClient(
                RegionEndpoint.GetBySystemName(_settings.Region));
        }

        public async Task<InitiateAuthResponse> LoginAsync(string email, string password)
        {
            var authRequest = new InitiateAuthRequest
            {
                ClientId = _settings.ClientId,
                AuthFlow = AuthFlowType.USER_PASSWORD_AUTH,
                AuthParameters = new Dictionary<string, string>
        {
            { "USERNAME", email },
            { "PASSWORD", password },
            { "SECRET_HASH", CalculateSecretHash(email) }
        }
            };

            return await _client.InitiateAuthAsync(authRequest);
        }

        private string CalculateSecretHash(string username)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_settings.ClientSecret);
            var messageBytes = Encoding.UTF8.GetBytes(username + _settings.ClientId);

            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(messageBytes);

            return Convert.ToBase64String(hash);
        }

        public async Task<bool> SetNewPasswordAsync(
    string email,
    string newPassword,
    string session)
        {
            var challengeRequest = new RespondToAuthChallengeRequest
            {
                ClientId = _settings.ClientId,
                ChallengeName = ChallengeNameType.NEW_PASSWORD_REQUIRED,
                Session = session,
                ChallengeResponses = new Dictionary<string, string>
        {
            { "USERNAME", email },
            { "NEW_PASSWORD", newPassword },
            { "SECRET_HASH", CalculateSecretHash(email) }
        }
            };

            var response =
                await _client.RespondToAuthChallengeAsync(challengeRequest);

            return response.AuthenticationResult != null;
        }

        public async Task ForgotPasswordAsync(string email)
        {
            var request = new ForgotPasswordRequest
            {
                ClientId = _settings.ClientId,
                Username = email,
                SecretHash = CalculateSecretHash(email)
            };

            await _client.ForgotPasswordAsync(request);
        }

        public async Task ConfirmForgotPasswordAsync(
    string email,
    string code,
    string newPassword)
        {
            var request =
                new ConfirmForgotPasswordRequest
                {
                    ClientId = _settings.ClientId,
                    Username = email,
                    ConfirmationCode = code,
                    Password = newPassword,
                    SecretHash = CalculateSecretHash(email)
                };

            await _client
                .ConfirmForgotPasswordAsync(request);
        }
    }
}