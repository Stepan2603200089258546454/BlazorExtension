using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    public enum ButtonGroupOrientation
    {
        [StyleValue("btn-group")]
        Horizontal = 0,
        [StyleValue("btn-group-vertical")]
        Vertical = 1,
    }
    public enum ButtonGroupSize
    {
        [StyleValue]
        Default = 0,
        [StyleValue("btn-group-lg")]
        Large = 1,
        [StyleValue("btn-group-sm")]
        Small = 2,
    }
}
