using CommonComponents.Enums.Bootstraps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Models.Bootstraps
{
    public class TabListSettings : BaseTabSettings
    {
        /// <summary>
        /// id контейнера содержимого вкладок
        /// </summary>
        public string IdContent => $"{Id}-content";
        private string TypeStyleValue => $"nav-{TypeValue}s";
        /// <summary>
        /// Расположение вкладок
        /// </summary>
        public TabListOrientation Orientation { get; set; } = TabListOrientation.Horizontal;
        /// <summary>
        /// Расположение вкладок строковое значение атрибута
        /// </summary>
        public string OrientationValue => Orientation switch
        {
            TabListOrientation.Horizontal => "horizontal",
            TabListOrientation.Vertical => "vertical",
            _ => "horizontal"
        };
        private string OrientationStyleValue => Orientation switch
        {
            TabListOrientation.Horizontal => string.Empty,
            TabListOrientation.Vertical => "flex-column me-3",
            _ => string.Empty
        };
        /// <summary>
        /// Заполнение ширины объекта вкладками
        /// </summary>
        public TabListFill Fill { get; set; }
        private string FillStyleValue => Fill switch
        {
            TabListFill.Default => string.Empty,
            TabListFill.Fill => "nav-fill",
            TabListFill.Justified => "nav-justified",
            _ => string.Empty,
        };
        /// <summary>
        /// Итоговый класс вкладки
        /// </summary>
        public string Class => $"nav {TypeStyleValue} {FillStyleValue} {OrientationStyleValue}".Trim();

        /// <summary>
        /// Получить настройки вкладок
        /// </summary>
        public TabListItemSettings GetItemsSettings(string id, bool isActive = false)
        {
            return new TabListItemSettings()
            {
                Id = id,
                IsActive = isActive,
                Type = this.Type,
            };
        }
    }
}
