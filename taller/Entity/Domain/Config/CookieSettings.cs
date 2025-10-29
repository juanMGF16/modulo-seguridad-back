﻿using Microsoft.AspNetCore.Http;

namespace Entity.Domain.Config
{
    public class CookieSettings
    {
        public string AccessTokenName { get; set; } = "access_token";
        public string RefreshTokenName { get; set; } = "refresh_token";
        public string CsrfCookieName { get; set; } = "XSRF-TOKEN";
        public string Path { get; set; } = "/";
        public string? Domain { get; set; } = null;
        public bool Secure { get; set; } = true; //true
        public SameSiteMode SameSite { get; set; } = SameSiteMode.None; //none
    }
}
