using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllNet.Modules.ReservasExportaciones.Components.Models
{
    public class BookingsModel
    {
        public string BL_ORIG { get; set; }
        public string PRT_RCL { get; set; }
        public string VIAJE { get; set; }
        public string DATE_CUTOFF { get; set; }
        public string CGO_DSC { get; set; }
        public string BLMASTER { get; set; }
        public string TER_FACT { get; set; }
        public string ANEXO { get; set; }
        public int ADJUNTOS { get; set; }
    }
}