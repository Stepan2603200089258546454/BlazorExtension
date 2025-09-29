namespace CommonComponents.Models.Bootstraps
{
    public class TabListItemSettings : BaseTabSettings
    {
        /// <summary>
        /// Активная
        /// </summary>
        public bool IsActive { get; set; } = false;
        /// <summary>
        /// Итоговый класс вкладки
        /// </summary>
        public string HeaderClass => $"nav-link {(IsActive ? "active" : string.Empty)}".Trim();
        /// <summary>
        /// id вкладки
        /// </summary>
        public string HeaderId => $"{Id}-tab";
        /// <summary>
        /// Итоговый класс содержимого вкладки
        /// </summary>
        public string BodyClass => $"tab-pane fade {(IsActive ? "show active" : string.Empty)}".Trim();
        /// <summary>
        /// id содержимого вкладки
        /// </summary>
        public string BodyId => $"{Id}-tab-pane";
    }
}
