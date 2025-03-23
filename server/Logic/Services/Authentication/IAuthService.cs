﻿using beautysalon.Contracts;
using beautysalon.Logic.DTOs.ServerResponse;

public interface IAuthService
{
    Task<ServerResponse> RegisterAsync (RegisterRequest authRequest);
}