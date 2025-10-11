using CommonComponents.Enums.Bootstraps;

namespace CommonComponents.Models.Bootstraps
{
    public class AccordionItemSettings
    {
        public AccordionItemSettings(string id, string idAccordion, bool isActive)
        {
            Id = id;
            IdAccordion = idAccordion ?? string.Empty;
            IsActive = isActive;
            if (string.IsNullOrEmpty(idAccordion) == false)
                Attributes.Add("data-bs-parent", $"#{IdAccordion}");
            buttonSettings = ButtonSettings.GetPresetAccordionItemHeader(HeaderClass, Id, IsActive);
        }
        /// <summary>
        /// Id элемента
        /// </summary>
        public string Id { get; set; }
        /// <summary>
        /// Id области элемента
        /// </summary>
        public string IdAccordion { get; set; } = string.Empty;
        /// <summary>
        /// Активный элемент (обычно первый)
        /// </summary>
        public bool IsActive { get; set; } = false;
        /// <summary>
        /// Стили кнопки
        /// </summary>
        public string HeaderClass => $"accordion-button {(IsActive ? string.Empty : "collapsed")}".Trim();
        /// <summary>
        /// Стили содержимого
        /// </summary>
        public string BodyClass => $"accordion-collapse collapse {(IsActive ? "show" : string.Empty)}".Trim();
        /// <summary>
        /// Динамические атрибуты
        /// </summary>
        public IDictionary<string, object> Attributes { get; private set; } = new Dictionary<string, object>();

        public ButtonSettings buttonSettings { get; private set; }
    }
}
