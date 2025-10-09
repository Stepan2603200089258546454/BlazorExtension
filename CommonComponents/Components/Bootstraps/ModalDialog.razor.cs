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
        /// Содержимое заголовка модального окна
        /// </summary>
        [Parameter]
        public RenderFragment? HeaderContent { get; set; }
        /// <summary>
        /// Содержимое модального окна
        /// </summary>
        [Parameter, EditorRequired]
        public RenderFragment BodyContent { get; set; }
        /// <summary>
        /// Содержимое кнопки подтверждения модального окна
        /// </summary>
        [Parameter]
        public RenderFragment? ButtonOkContent { get; set; }
        /// <summary>
        /// Содержимое кнопки отклонения модального окна
        /// </summary>
        [Parameter]
        public RenderFragment? ButtonNoContent { get; set; }
        /// <summary>
        /// Содержимое кнопки закрытия модального окна
        /// </summary>
        [Parameter]
        public RenderFragment? ButtonCancelContent { get; set; }
        /// <summary>
        /// Настройки модального окна
        /// </summary>
        [Parameter, EditorRequired]
        public ModalDialogSettings Settings { get; set; }
        /// <summary>
        /// Нажата кнопка [Подтверждения=true, Отказа=false, Закрытия=null]
        /// </summary>
        [Parameter]
        public EventCallback<bool?> OnResult { get; set; }
        
        private async Task Ok() => await OnResult.InvokeAsync(true);
        private async Task No() => await OnResult.InvokeAsync(false);
        private async Task Cancel() => await OnResult.InvokeAsync(null);
    }
}
