using AllNet.Modules.ReservasExportaciones.Components.Models;
using DataTables;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace AllNet.Modules.ReservasExportaciones.Components.Controllers
{
    public class BookingsController : MasterController
    {
        public override IHttpActionResult Rest(HttpRequest request)
        {

            using (var db = new Database(DbType, ConnectionString))
            {
                if (NitUser == "")
                {
                    var response_gen = new Editor(db, "V_Bookings", new[] { "BL_ORIG", "BLMASTER" })
                        .Model<BookingsModel>()
                        .Field(new Field("ADJUNTOS")
                            .Set(false)
                            .GetFormatter((val, row) => db.Sql($"Select ADJUNTO from BOOKINGS_EXPO_DOCS where BOOKING='{row["BL_ORIG"].ToString().Trim()}'").FetchAll().Count())
                            )
                        .Process(request)
                        .Data();
                    return Json(response_gen);
                }

                var response_nit = new Editor(db, "V_Bookings", new[] { "BL_ORIG", "BLMASTER" })
                        .Model<BookingsModel>()
                        .Debug(true)
                        .Field(new Field("ADJUNTOS")
                            .Set(false)
                            .GetFormatter((val, row) => db.Sql($"Select ADJUNTO from BOOKINGS_EXPO_DOCS where BOOKING='{row["BL_ORIG"].ToString().Trim()}'").FetchAll().Count())
                            )
                        .Where(q => q.Where("TER_FACT", NitUser, "="))
                        .Process(request)
                        .Data();
                return Json(response_nit);
            }

        }

        public IHttpActionResult UpdateState()
        {
            var request = HttpContext.Current.Request;
            var state_id = request.Form.GetValues("state_id").FirstOrDefault();
            var bl = request.Form.GetValues("Booking").FirstOrDefault();

            try
            {
                using (var db = new Database(DbType, ConnectionString))
                {
                    Dictionary<string, dynamic> state = new Dictionary<string, dynamic>
                    {
                        {"CGO_DSC", state_id }
                    };

                    Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>
                    {
                        {"BL_ORIG", bl}
                    };
                    db.Update("BOOKINGS", state, condition);
                    return Ok();
                }
            }
            catch (Exception ex)
            {
                return InternalServerError(ex);
            }

        }
    }
}