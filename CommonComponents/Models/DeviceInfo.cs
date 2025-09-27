using CommonComponents.Enums;

namespace CommonComponents.Models
{
    public class DeviceInfo
    {
        public DeviceInfo(string userAgent)
        {
            UserAgent = userAgent;
            Browser = DetectBrowser();
            Platform = DetectPlatform();
            IsMobile = IsMobileDevice();
            IsTablet = IsTabletDevice();
            IsDesktop = IsDesktopDevice();
        }

        public string UserAgent { get; set; }
        public DeviceBrowser Browser { get; set; }
        public DevicePlatform Platform { get; set; }
        public bool IsMobile { get; set; }
        public bool IsTablet { get; set; }
        public bool IsDesktop { get; set; }

        private DeviceBrowser DetectBrowser() => DetectBrowser(UserAgent);
        private DeviceBrowser DetectBrowser(string userAgent)
        {
            var ua = userAgent.ToLower();
            if (ua.Contains("chrome")) return DeviceBrowser.Chrome;
            if (ua.Contains("firefox")) return DeviceBrowser.Firefox;
            if (ua.Contains("safari")) return DeviceBrowser.Safari;
            if (ua.Contains("edge")) return DeviceBrowser.Edge;
            if (ua.Contains("opera")) return DeviceBrowser.Opera;
            return DeviceBrowser.Unknown;
        }
        private DevicePlatform DetectPlatform() => DetectPlatform(UserAgent);
        private DevicePlatform DetectPlatform(string userAgent)
        {
            var ua = userAgent.ToLower();
            if (ua.Contains("windows")) return DevicePlatform.Windows;
            if (ua.Contains("mac os")) return DevicePlatform.macOS;
            if (ua.Contains("linux")) return DevicePlatform.Linux;
            if (ua.Contains("android")) return DevicePlatform.Android;
            if (ua.Contains("iphone") || ua.Contains("ipad")) return DevicePlatform.iOS;
            return DevicePlatform.Unknown;
        }
        private bool IsMobileDevice() => IsMobileDevice(UserAgent);
        private bool IsMobileDevice(string userAgent)
        {
            var ua = userAgent.ToLower();
            return ua.Contains("mobile") && ua.Contains("ipad") == false;
        }
        private bool IsTabletDevice() => IsTabletDevice(UserAgent);
        private bool IsTabletDevice(string userAgent)
        {
            var ua = userAgent.ToLower();
            return ua.Contains("ipad") || (ua.Contains("tablet") && ua.Contains("mobile") == false);
        }
        private bool IsDesktopDevice() => IsDesktopDevice(UserAgent);
        private bool IsDesktopDevice(string userAgent)
        {
            return IsMobileDevice(userAgent) == false && IsTabletDevice(userAgent) == false;
        }
    }
}
