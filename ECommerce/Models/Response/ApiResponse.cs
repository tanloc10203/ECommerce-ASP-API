﻿namespace ECommerce.Models.Response
{
    public class ApiResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; } = "";
        public object? Data { get; set; }

        public int? StatusCode { get; set; } = StatusCodes.Status200OK;
    }
}
