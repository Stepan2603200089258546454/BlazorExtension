using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonComponents.Models
{
    public class SelectedModel<T> where T : notnull
    {
        public SelectedModel(T model)
        {
            Model = model;
        }

        public bool IsSelected { get; set; } = false;
        public T Model { get; set; }
    }
}
