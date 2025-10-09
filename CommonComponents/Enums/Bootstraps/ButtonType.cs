using CommonComponents.Enums.Exstension;

namespace CommonComponents.Enums.Bootstraps
{
    public enum ButtonType
    {
        [AttributeValue("type", "button")]
        Button = 0,
        [AttributeValue("type", "submit")]
        Submit = 1,
        [AttributeValue("type", "reset")]
        Reset = 2,
    }
}
