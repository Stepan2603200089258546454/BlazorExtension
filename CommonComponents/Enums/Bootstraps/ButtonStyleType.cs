using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    public enum ButtonStyleType
    {
        [StyleValue]
        Empty = -2,
        [StyleValue("btn")]
        Btn = -1,
        [StyleValue("btn btn-primary")]
        Primary = 0,
        [StyleValue("btn btn-outline-primary")]
        OutlinePrimary = 1,
        [StyleValue("btn btn-secondary")]
        Secondary = 2,
        [StyleValue("btn btn-outline-secondary")]
        OutlineSecondary = 3,
        [StyleValue("btn btn-success")]
        Success = 4,
        [StyleValue("btn btn-outline-success")]
        OutlineSuccess = 5,
        [StyleValue("btn btn-danger")]
        Danger = 6,
        [StyleValue("btn btn-outline-danger")]
        OutlineDanger = 7,
        [StyleValue("btn btn-warning")]
        Warning = 8,
        [StyleValue("btn btn-outline-warning")]
        OutlineWarning = 9,
        [StyleValue("btn btn-info")]
        Info = 10,
        [StyleValue("btn btn-outline-info")]
        OutlineInfo = 11,
        [StyleValue("btn btn-light")]
        Light = 12,
        [StyleValue("btn btn-outline-light")]
        OutlineLight = 13,
        [StyleValue("btn btn-dark")]
        Dark = 14,
        [StyleValue("btn btn-outline-dark")]
        OutlineDark = 15,
        [StyleValue("btn btn-link")]
        Link = 16,
    }
    public enum ButtonType
    {
        [StringValue("button")]
        Button = 0,
        [StringValue("submit")]
        Submit = 1,
        [StringValue("reset")]
        Reset = 2,
    }
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
