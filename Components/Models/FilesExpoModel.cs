using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllNet.Modules.ReservasExportaciones.Components.Models
{
    public class FilesExpoModel
    {
        public int ID { get; set; }
        public string BOOKING { get; set; }
        public string CODIGO { get; set; }
        public string CATEGORIA { get; set; }
        public decimal PESO { get; set; }
        public decimal BULTOS { get; set; }
        public string COMENTARIO { get; set; }
        public int ADJUNTO { get; set; }
        public int ESTADO { get; set; }

    }
}