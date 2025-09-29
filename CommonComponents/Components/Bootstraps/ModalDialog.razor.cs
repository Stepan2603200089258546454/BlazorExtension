using CommonComponents.Models.Bootstraps;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Components.Bootstraps
{
    public partial class ModalDialog : ComponentBase
    {
        /// <summary>
        /// Содержимое модального окна
        /// </summary>
        [Parameter]
        public RenderFragment? ChildContent { get; set; }
        /// <summary>
        /// Настройки модального окна
        /// </summary>
        [Parameter, EditorRequired]
        public ModalDialogSettings Settings { get; set; }
        /// <summary>
        /// Нажата кнопка [подтверждения=true, отказа=false, закрытия=null]
        /// </summary>
        [Parameter]
        public EventCallback<bool?> OnResult { get; set; }
        
        private async Task Ok() => await OnResult.InvokeAsync(true);
        private async Task No() => await OnResult.InvokeAsync(false);
        private async Task Cancel() => await OnResult.InvokeAsync(null);
    }
}
