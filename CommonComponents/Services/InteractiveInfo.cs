using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Services
{
    /// <summary>
    /// Класс который позволяет узнать где и как выполняется компонент
    /// </summary>
    public class InteractiveInfo
    {
        /// <summary>
        /// Возможные типы текущего и назначаемого режима отрисовки
        /// </summary>
        public enum RenderType
        {
            /// <summary>
            /// Не определен
            /// </summary>
            Unknown,
            /// <summary>
            /// Не назначен
            /// </summary>
            None,
            /// <summary>
            /// Статичный
            /// </summary>
            Static,
            /// <summary>
            /// Сервером
            /// </summary>
            Server,
            /// <summary>
            /// Браузером
            /// </summary>
            WebAssembly,
            /// <summary>
            /// На родном устройстве
            /// </summary>
            WebView,
            /// <summary>
            /// Автоматически
            /// </summary>
            Auto
        }

        public InteractiveInfo(RendererInfo RendererInfo, IComponentRenderMode? AssignedRenderMode)
        {
            _rendererInfo = RendererInfo;
            _assignedRenderMode = AssignedRenderMode;
            // режимы отрисовки вычисляются 1 раз и не меняются динамически, поэтому будем инициализировать тоже 1 раз
            _currentRenderType = new Lazy<RenderType>(() => Enum.TryParse(_rendererInfo.Name, out RenderType result) ? result : RenderType.Unknown);
            _assignedRenderType = new Lazy<RenderType>(() => _assignedRenderMode switch
            {
                InteractiveServerRenderMode => RenderType.Server,
                InteractiveAutoRenderMode => RenderType.Auto,
                InteractiveWebAssemblyRenderMode => RenderType.WebAssembly,
                null => RenderType.None,
                _ => RenderType.Unknown,
            });
        }
        private readonly RendererInfo _rendererInfo;
        private readonly IComponentRenderMode? _assignedRenderMode;
        private readonly Lazy<RenderType> _currentRenderType;
        private readonly Lazy<RenderType> _assignedRenderType;

        /// <summary>
        /// Текущий режим отрисовки
        /// </summary>
        public RenderType CurrentRenderType => _currentRenderType.Value;
        /// <summary>
        /// Назначенный режим отрисовки [разные при предварительном рендеринге и интерактивном выполнении]
        /// </summary>
        public RenderType AssignedRenderType => _assignedRenderType.Value;
        /// <summary>
        /// Выполняется в WASM или PWA приложение [так же можно узнать OperatingSystem.IsBrowser()]
        /// </summary>
        public bool IsWASMRender => CurrentRenderType == RenderType.WebAssembly;
        /// <summary>
        /// Выполняется на сервере
        /// </summary>
        public bool IsServerRender => CurrentRenderType == RenderType.Server;
        /// <summary>
        /// Статичная отрисовка
        /// </summary>
        public bool IsStaticRender => CurrentRenderType == RenderType.Static;
        /// <summary>
        /// Ожидаем интерактивность
        /// </summary>
        public bool IsWaitingInteractive => _assignedRenderMode != null && _rendererInfo.IsInteractive == false;
        /// <summary>
        /// Интерактивный
        /// </summary>
        public bool IsInteractive => _rendererInfo.IsInteractive;
    }
}
