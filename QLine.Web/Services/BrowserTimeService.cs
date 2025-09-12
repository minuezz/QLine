using Microsoft.JSInterop;

namespace QLine.Web.Services
{
    public sealed class BrowserTimeService
    {
        private readonly IJSRuntime _js;
        private string? _tz;

        public BrowserTimeService(IJSRuntime js) => _js = js;

        public async Task<string> GetTimeZoneIdAsync()
        {
            if (_tz is not null) return _tz;
            try
            {
                _tz = await _js.InvokeAsync<string?>("qlineTime.getTimeZone") ?? "Europe/Warsaw";
            }
            catch 
            {
                _tz = "Europe/Warsaw";
            }
            return _tz!;
        }

        public async Task<string> FormatUtcAsync(DateTimeOffset utc, string format = "yyyy-MM-dd HH:mm")
        {
            var tzid = await GetTimeZoneIdAsync();
            var tz = TimeZoneInfo.FindSystemTimeZoneById(tzid);
            var local = TimeZoneInfo.ConvertTime(utc, tz);
            return local.ToString(format);
        }
    }
}
