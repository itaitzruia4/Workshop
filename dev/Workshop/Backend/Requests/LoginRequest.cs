﻿namespace API.Requests
{
    public class LoginRequest
    {
        public string Membername { get; set; }
        public string Password { get; set; }
        public int UserId { get; set; }
    }
}
