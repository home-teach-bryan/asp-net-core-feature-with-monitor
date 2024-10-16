using System.Diagnostics.Metrics;

namespace AspNetCoreFeatureWithMonitor.Services;

public class TokenCounter
{
    private readonly Counter<int> _tokenCounter;

    public TokenCounter(IMeterFactory meterFactory)
    {
        var meter = meterFactory.Create("Token.Counter");
        _tokenCounter = meter.CreateCounter<int>("Token.Generate.Count");
    }

    public void Counter(string userName)
    {
        _tokenCounter.Add(1, new KeyValuePair<string, object?>("Token.User.Name", userName));
    }
}