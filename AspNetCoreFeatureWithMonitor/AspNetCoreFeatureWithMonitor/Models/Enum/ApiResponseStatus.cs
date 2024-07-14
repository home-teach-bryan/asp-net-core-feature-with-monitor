using System.ComponentModel;

namespace AspNetCoreFeatureWithMonitor.Models.Enum;

public enum ApiResponseStatus
{
    [Description("失敗")]
    Fail = -1,
    
    [Description("成功")]
    Success = 1
}