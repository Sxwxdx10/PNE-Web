using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNE_core.Enums
{
    public enum TypeLavage
    {
        [Description("eau chaude avec pression")]
        EauChaudeAvecPression,

        [Description("eau froide avec pression")]
        EauFroideAvecPression,

        [Description("eau chaude sans pression")]
        EauChaudeSansPression,

        [Description("eau froide sans pression")]
        EauFroideSansPression,

    }
}
