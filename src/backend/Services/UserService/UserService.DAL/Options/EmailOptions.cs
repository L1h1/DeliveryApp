﻿namespace UserService.DAL.Options
{
    public class EmailOptions
    {
        public string Host { get; init; } = string.Empty;
        public int Port { get; init; }
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public string BaseUrl { get; init; } = string.Empty;
    }
}
