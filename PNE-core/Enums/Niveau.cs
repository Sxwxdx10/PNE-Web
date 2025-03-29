using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNE_core.Enums
{
    public enum Niveau
    {
        [Description("vert")]
        Vert = 0,
        [Description("jaune")]
        Jaune = 1,
        [Description("rouge")]
        Rouge = 2
    }

}
