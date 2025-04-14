using AllNet.Modules.ReservasExportaciones.Abstractions;
using AllNet.Modules.ReservasExportaciones.Components.Models;
using AllNet.Modules.ReservasExportaciones.Services;
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
        private readonly IBookingsServices _bookingsServices;
        public BookingsController()
        {
            _bookingsServices = new BookingServices();
        }
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
    }
}