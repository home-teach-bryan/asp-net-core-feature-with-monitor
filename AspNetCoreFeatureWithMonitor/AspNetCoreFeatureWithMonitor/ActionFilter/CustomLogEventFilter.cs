using Serilog.Core;
using Serilog.Events;

namespace AspNetCoreFeatureWithMonitor.ActionFilter;

public class CustomLogEventFilter : ILogEventFilter
{
    private readonly List<string> _excludedKeywords;

    public CustomLogEventFilter(List<string> excludedKeywords)
    {
        _excludedKeywords = excludedKeywords;
    }
    
    public bool IsEnabled(LogEvent logEvent)
    {
        if (logEvent.Properties.TryGetValue("RequestPath", out var requestPathValue))
        {
            var requestPath = requestPathValue.ToString();
            if (_excludedKeywords.Any(keyword => requestPath.Contains(keyword, StringComparison.OrdinalIgnoreCase)))
            {
                return false;
            }
        }
        return true;
    }
}