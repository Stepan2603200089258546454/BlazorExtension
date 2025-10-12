using CommonComponents.Enums.Bootstraps;
using CommonComponents.Enums.Exstension;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Models.Bootstraps
{
    public class SpinnerSettings
    {
        public SpinnerType Type { get; set; } = SpinnerType.Default;
        public TextStyle Style { get; set; } = TextStyle.Default;

        public string Class => $"{Type.GetStyleValue()} {Style.GetStyleValue()}".Trim();
    }
}
