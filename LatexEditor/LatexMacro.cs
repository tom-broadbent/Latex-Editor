using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LatexEditor
{
    public partial class LatexMacro
    {
        public string? Name { get; set; } = "";
        public string? Latex { get; set; } = "";
        public string? CommandName { get; set; } = "";
    }
}
