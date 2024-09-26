using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace AllNet.Modules.ReservasExportaciones.Components.Models
{
    public class DetCargaBModel
    {
        public int ID_DETCARGA { get; set; }
        public string IDEN { get; set; }
        public decimal PESO_ORIG { get; set; }
        public decimal BULTOS { get; set; }
        public int ID_BOOKINGS_EXPO { get; set; }

    }
}