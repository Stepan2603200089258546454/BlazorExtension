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
        }

        public string Id { get; set; }
        public string IdAccordion { get; set; } = string.Empty;
        public bool IsActive { get; set; } = false;

        public string HeaderClass => $"accordion-button {(IsActive ? string.Empty : "collapsed")}".Trim();
        public string BodyClass => $"accordion-collapse collapse {(IsActive ? "show" : string.Empty)}".Trim();

        public Dictionary<string, object> Attributes { get; private set; } = new Dictionary<string, object>();
    }
}
