using AllNet.Modules.ReservasExportaciones.Abstractions;
using DataTables;
using DotNetNuke.Common.Utilities;
using System;
using System.Collections.Generic;
using System.EnterpriseServices;
using System.Linq;
using System.Web;

namespace AllNet.Modules.ReservasExportaciones.Data
{
    public class Repository : IRepository
    {
        private readonly string connection_string = Config.GetConnectionString("BD_AGP");

        public void UpdateBookingState(Dictionary<string, dynamic> state, Dictionary<string, dynamic> condition)
        {     
            try
            {
                using (var db = new Database("sqlserver", connection_string))
                {   

                    db.Update("BOOKINGS_EXPO_DOCS", state, condition);
                  
                }
            }
            catch (Exception ex)
            {
               
            }
        }
    }
}