﻿namespace TaskManagementSystem.Infrastructure.Identity;

public class TokenRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}