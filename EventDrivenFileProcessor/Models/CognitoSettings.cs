using Amazon.CognitoIdentityProvider.Model;

namespace EventDrivenFileProcessor.Models
{
    public class CognitoSettings
    {
        public string Region { get; set; } = string.Empty;
        public string UserPoolId { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;
    }
}
