using CommonComponents.Enums.Bootstraps;
using CommonComponents.Enums.Exstension;

namespace CommonComponents.Models.Bootstraps
{
    public abstract class BaseCardSettings
    {
        protected abstract string BaseClass { get; }

        public TextBackgroundColor Style { get; set; } = TextBackgroundColor.Default;
        public TextAligned TextAligned { get; set; } = TextAligned.Default;
        public TextStyle TextStyle { get; set; } = TextStyle.Default;
        public BorderColor BorderColor { get; set; } = BorderColor.Default;

        public virtual string Class => $"{BaseClass} {Style.GetStyleValue()} {TextAligned.GetStyleValue()} {TextStyle.GetStyleValue()} {BorderColor.GetStyleValue()}".Trim();
    }
}
