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
    }
}
