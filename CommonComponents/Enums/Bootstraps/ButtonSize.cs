using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    public enum ButtonSize
    {
        [StyleValue]
        Default = 0,
        [StyleValue("btn-lg")]
        Large = 1,
        [StyleValue("btn-sm")]
        Small = 2,
    }
}
