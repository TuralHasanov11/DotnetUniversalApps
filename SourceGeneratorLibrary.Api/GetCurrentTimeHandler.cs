using Microsoft.AspNetCore.Mvc;

namespace SourceGeneratorLibrary.Api;

public class GetCurrentTimeHandler
{
    [HttpGet("time-1")]
    public DateTime GetCurrentTime()
    {
        return DateTime.Now;
    }
}