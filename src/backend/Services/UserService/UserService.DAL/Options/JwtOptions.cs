﻿namespace UserService.DAL.Options
{
    public class JwtOptions
    {
        public string Key { get; init; } = string.Empty;
        public string Issuer { get; init; } = string.Empty;
        public string Audience { get; init; } = string.Empty;
        public int AccessTokenExpirationMins { get; init; }
        public int RefreshTokenExpirationDays { get; init; }
    }
}
