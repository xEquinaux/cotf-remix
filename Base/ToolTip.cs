using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using cotf.Base;
using cotf.World;

namespace cotf.Base
{
    public class ToolTip
    {
        internal readonly string name;
        internal readonly string text;
        internal readonly Color textColor;
        public ToolTip() { }
        public ToolTip(string name, string text, Color color)
        {
            this.name = name;
            this.text = text;
            this.textColor = color;
        }
        private static ToolTip SetToolTip(string Name, string Tooltip, Color color)
        {
            return new ToolTip(Name, Tooltip, color);
        }
        public override string ToString()
        {
            return $"{name}, {text}";
        }
    }
}
