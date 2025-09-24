using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Models
{
    public class ViewportSize
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
    public class DeviceInfo
    {
        public bool IsMobile { get; set; }
        public bool IsTablet { get; set; }
        public bool IsDesktop { get; set; }
        public string UserAgent { get; set; }
        public int ScreenWidth { get; set; }
        public int ScreenHeight { get; set; }
    }
}
