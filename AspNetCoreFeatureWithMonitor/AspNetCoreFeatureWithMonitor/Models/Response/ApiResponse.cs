﻿using AspNetCoreFeatureWithMonitor.Models.Enum;

namespace AspNetCoreFeatureWithMonitor.Models.Response;

public class ApiResponse<T>
{
    public ApiResponseStatus Status { get; set; }
    
    public string Message { get; set; }
    
    public List<string>? Errors { get; set; }
    public T? Data { get; set; }
}