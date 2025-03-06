using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PNE_core.Enums
{
    public enum StationPersonnelStatus
    {
        [Description("Aucun employés")]
        Aucun,
        [Description("Il y a des employés")]
        Present,
        [Description("Les employés sont certifiés pour décontaminer")]
        CertifieDecontamination
    }
}
