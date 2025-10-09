using CommonComponents.Models.Bootstraps;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Components.Bootstraps
{
    public partial class Button : ComponentBase
    {
        /// <summary>
        /// Настойки кнопки
        /// </summary>
        [Parameter, EditorRequired]
        public ButtonSettings Settings { get; set; }
        /// <summary>
        /// Содержимое кнопки
        /// </summary>
        [Parameter, EditorRequired]
        public RenderFragment? ChildContent { get; set; }
        /// <summary>
        /// Нажатие на кнопку
        /// </summary>
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        private async Task Click(MouseEventArgs arg) => await OnClick.InvokeAsync(arg);
    }
}
