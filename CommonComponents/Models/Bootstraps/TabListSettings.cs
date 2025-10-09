using CommonComponents.Enums.Bootstraps;
using CommonComponents.Enums.Exstension;
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
        /// <summary>
        /// Расположение вкладок
        /// </summary>
        public TabListOrientation Orientation { get; set; } = TabListOrientation.Horizontal;
        /// <summary>
        /// Заполнение ширины объекта вкладками
        /// </summary>
        public TabListFill Fill { get; set; } = TabListFill.Default;
        /// <summary>
        /// Итоговый класс вкладки
        /// </summary>
        public string Class => $"nav {Type.GetStyleValue()} {Fill.GetStyleValue()} {Orientation.GetStyleValue()}".Trim();

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
