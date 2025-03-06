using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PNE_core.Enums
{
    public enum TypePneId
    {
        [Description("serial_embarcation")]
        SerialEmbarcation,

        [Description("serial_lavage")]
        SerialLavage,

        [Description("serial_embarcation_utilisateur")]
        SerialEmbarcationUtilisateur,

        [Description("serial_note")]
        SerialNote,

        [Description("serial_plan_eau")]
        SerialPlanEau,

        [Description("serial_mise_eau")]
        SerialMiseEau,
    }
}
