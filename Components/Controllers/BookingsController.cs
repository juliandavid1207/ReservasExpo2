using AllNet.Modules.ReservasExportaciones.Abstractions;
using AllNet.Modules.ReservasExportaciones.Components.Models;
using AllNet.Modules.ReservasExportaciones.Services;
using DataTables;
using DotNetNuke.UI.Skins;
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
        public string draw = "";
        public string start = "";
        public string length = "";
        public string sortColumn = "";
        public string sortColumnDir = "";
        public string searchValue = "";
        public int pageSize, skip, recordsTotal;      
        public override IHttpActionResult Rest(HttpRequest request)
        {
            string exc = string.Empty;

            try
            {
                var draw = request.Form.GetValues("draw").FirstOrDefault();
                var start = request.Form.GetValues("start").FirstOrDefault();
                var length = request.Form.GetValues("length").FirstOrDefault();
                var sortColumn = request.Form.GetValues("columns[" + request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
                var sortColumnDir = request.Form.GetValues("order[0][dir]").FirstOrDefault();
                var searchValue = request.Form.GetValues("search[value]").FirstOrDefault();

                pageSize = length != null ? Convert.ToInt32(length) : 0;
                skip = start != null ? Convert.ToInt32(start) : 0;
                recordsTotal = 0;
            }
            catch (Exception ex)
            {
                exc = ex.Message;
            }

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

                    if (searchValue.Equals(string.Empty) && exc.Equals(string.Empty))
                        response_gen.data.Skip(skip).Take(pageSize);

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

                if (searchValue.Equals(string.Empty) && exc.Equals(string.Empty))
                    response_nit.data.Skip(skip).Take(pageSize);

                return Json(response_nit);
            }

        }
    }
}