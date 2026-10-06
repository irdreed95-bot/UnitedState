using System;

namespace CorruptStateRP.Auth {
    [Serializable]
    public sealed class SessionTokens {
        public string accessToken;
        public string refreshToken;
        public long expiresAtUnix;
        public bool IsUsable => !string.IsNullOrWhiteSpace(refreshToken);
        public bool AccessTokenExpired => expiresAtUnix > 0 && DateTimeOffset.UtcNow.ToUnixTimeSeconds() >= expiresAtUnix;
    }
}
