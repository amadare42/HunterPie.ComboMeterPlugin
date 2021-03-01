using System.Collections.Generic;
using System.Linq;
using HunterPie.GUI;

namespace ComboMeter.TestApp
{
    public class OverlaySimulator
    {
        private List<Widget> widgets = new List<Widget>();

        public bool IsDesignMode
        {
            get
            {
                return widgets.FirstOrDefault(w => w.InDesignMode)?.InDesignMode ?? false;
            }
            set
            {
                widgets.ForEach(widget => widget.InDesignMode = value);
            }
        }

        public void Add(Widget widget)
        {
            typeof(Widget).GetProperty(nameof(Widget.OverlayActive)).SetValue(widget, true, null);
            typeof(Widget).GetProperty(nameof(Widget.WidgetActive)).SetValue(widget, true, null);
            typeof(Widget).GetProperty(nameof(Widget.OverlayFocusActive)).SetValue(widget, false, null);
            widget.ChangeVisibility();
            widget.ApplySettings();
            widgets.Add(widget);
        }
    }
}