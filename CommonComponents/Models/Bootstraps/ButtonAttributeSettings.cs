namespace CommonComponents.Models.Bootstraps
{
    /// <summary>
    /// Настройки атрибутов кнопки
    /// </summary>
    public class ButtonAttributeSettings
    {
        /// <summary>
        /// Назначенные атрибуты
        /// </summary>
        public Dictionary<string, object> Attributes { get; private set; } = new Dictionary<string, object>();
        /// <summary>
        /// Добавить произвольный атрибут
        /// </summary>
        /// <param name="key">Название атрибута</param>
        /// <param name="value">Значение атрибута</param>
        public ButtonAttributeSettings AddValue(string key, object value)
        {
            if (Attributes.ContainsKey(key))
            {
                Attributes[key] = value;
            }
            else
            {
                Attributes.Add(key, value);
            }
            return this;
        }
        public ButtonAttributeSettings AddAriaLabel(string value)
        {
            return AddValue("aria-label", value);
        }
        /// <summary>
        /// Добавляет атрибуты открытия модального окна
        /// </summary>
        public ButtonAttributeSettings OpenModal(string id)
        {
            return AddValue("data-bs-toggle", "modal").AddValue("data-bs-target", $"#{id}");
        }
        /// <summary>
        /// Добавляет атрибуты закрытия модального окна (в котором расположена кнопка)
        /// </summary>
        public ButtonAttributeSettings CloseModal()
        {
            return AddValue("data-bs-dismiss", "modal");
        }
        /// <summary>
        /// Добавляет атрибуты закрытия уведомления [alert] (в котором расположена кнопка)
        /// </summary>
        public ButtonAttributeSettings CloseAlert()
        {
            return AddValue("data-bs-dismiss", "alert");
        }
        /// <summary>
        /// 
        /// </summary>
        public ButtonAttributeSettings AccordionItemHeader(string id, bool isActive = false)
        {
            return AddValue("data-bs-toggle", "collapse")
                .AddValue("data-bs-target", $"#{id}")
                .AddValue("aria-expanded", isActive.ToString())
                .AddValue("aria-controls", id);
        }
    }
}
