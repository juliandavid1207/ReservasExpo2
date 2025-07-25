﻿using AllNet.Modules.ReservasExportaciones.Abstractions;
using AllNet.Modules.ReservasExportaciones.Components.Models;
using AllNet.Modules.ReservasExportaciones.Services;
using DataTables;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace AllNet.Modules.ReservasExportaciones.Components.Controllers
{
    public class CntrController : MasterController
    {
        private readonly IBookingsServices _bookingsServices;
        public CntrController()
        {
            _bookingsServices = new BookingServices();
        }

        public override IHttpActionResult Rest(HttpRequest request)
        {
            using (var db = new Database(DbType, ConnectionString))
            {
                var ID_BOOKINGS_EXPO = request.Form.GetValues("ID").FirstOrDefault();
                var BULTOS = request.Form.GetValues(1).FirstOrDefault();
                var PESO = request.Form.GetValues(2).FirstOrDefault();
                string bl_orig = String.Empty;
                string path = String.Empty;
                var codigo = request.Form["CODIGO"].ToString();
                var r_query = request.QueryString;
                if (!String.IsNullOrEmpty(request.Form["BOOKING"]) || !String.IsNullOrEmpty(request.QueryString["BOOKING"]))
                {
                    bl_orig = !String.IsNullOrEmpty(request.Form["BOOKING"]) ? request.Form["BOOKING"].ToString() : String.Empty;
                    bl_orig = String.IsNullOrEmpty(bl_orig) ? request.QueryString["BOOKING"].ToString() : bl_orig;
                }
                //if (!String.IsNullOrEmpty(request.Form["CODIGO"]) || !String.IsNullOrEmpty(request.QueryString["CODIGO"]))
                //{

                //    string temp_codigo = !String.IsNullOrEmpty(request.Form["CODIGO"]) ? request.Form["CODIGO"].ToString() : String.Empty;
                //    temp_codigo = String.IsNullOrEmpty(temp_codigo) ? request.QueryString["CODIGO"].ToString() : temp_codigo;
                //    path = HttpContext.Current.Server.MapPath($"~/DesktopModules/ReservasExpo/Uploads/{bl_orig}/{temp_codigo}/");
                //}
                if (!Directory.Exists(path) && !path.Equals(string.Empty))
                {
                    Directory.CreateDirectory(path);
                }

                var editorCntr = new Editor(db, "DETCARGAB", "ID_DETCARGA")//new[] { "BOOKING", "CODIGO" })
                    .Model<DetCargaBModel>("DETCARGAB")
                    .Debug(true)
                    .Field(new Field("DETCARGAB.ID_DETCARGA"))
                    .Field(new Field("DETCARGAB.IDEN")
                       .Validator(Validation.NotEmpty())
                    )
                    .Field(new Field("DETCARGAB.PESO_ORIG")                  
                       .Validator(Validation.NotEmpty())
                       .SetValue(PESO)

                    )
                    .Field(new Field("DETCARGAB.BULTOS")                
                       .Validator(Validation.NotEmpty())
                       .SetValue(BULTOS)
                    )                
                    .Field(new Field("DETCARGAB.ID_BOOKINGS_EXPO")                       
                        .SetValue(Convert.ToInt32(ID_BOOKINGS_EXPO))
                    )
                    .LeftJoin("BOOKINGS_EXPO_DOCS", "BOOKINGS_EXPO_DOCS.ID", "=", "DETCARGAB.ID_BOOKINGS_EXPO");
                //.LeftJoin("DETCARGAB", "DETCARGAB.ID_DETCARGA", "=", "BOOKINGS_EXPO_DOCS.ID")               

                

                if (!String.IsNullOrEmpty(ID_BOOKINGS_EXPO))
                {                   
                    var response = editorCntr
                    .Where(q => q.Where("DETCARGAB.ID_BOOKINGS_EXPO", ID_BOOKINGS_EXPO, "="))
                    .Process(request)
                    .Data();

                    var sum = GetSum(Convert.ToInt32(ID_BOOKINGS_EXPO)).Result;
                    var bloqueo = sum[2] == 0 ? 0 : 1;
                    _bookingsServices.UpdatePesoBultos(sum[0], sum[1], bloqueo, ID_BOOKINGS_EXPO);

                    return Json(response);                  

                }
                else
                {
                    var response = editorCntr
                    .Process(request)
                    .Data();
                    return Json(response);
                }



            }

        }

        private async Task<List<decimal>> GetSum(int ID_BOOKINGS_EXPO)
        {
            decimal sumBultos = 0;
            decimal sumPeso = 0;
            decimal enable = 0;
            var dc = await _bookingsServices.GetDetCargaB(ID_BOOKINGS_EXPO);

            if (dc != null)
            {
                if (dc.Count > 0)
                {
                    foreach (var item in dc)
                    {
                        sumPeso += item.PESO_ORIG;
                        sumBultos += item.BULTOS;
                    }
                    enable = 1;
                }              
            }           

            return new List<decimal> { sumPeso, sumBultos, enable };
         
        }

       

        public override IHttpActionResult Rest(HttpRequest request, DataClient data)
        {
            using (var db = new Database(DbType, ConnectionString))
            {
                var ID_BOOKINGS_EXPO = data.ID;
                string bl_orig = String.Empty;
                string path = String.Empty;
            
                var r_query = request.QueryString;
                if (!String.IsNullOrEmpty(data.Booking) || !String.IsNullOrEmpty(request.QueryString["BOOKING"]))
                {
                    bl_orig = !String.IsNullOrEmpty(data.Booking) ? data.Booking.ToString() : String.Empty;
                    bl_orig = String.IsNullOrEmpty(bl_orig) ? data.Booking.ToString() : bl_orig;
                }
          
                if (!Directory.Exists(path) && !path.Equals(string.Empty))
                {
                    Directory.CreateDirectory(path);
                }

                var editorCntr = new Editor(db, "DETCARGAB", "ID_DETCARGA")//new[] { "BOOKING", "CODIGO" })
                    .Model<DetCargaBModel>("DETCARGAB")
                    .Debug(true)
                    .Field(new Field("DETCARGAB.ID_DETCARGA"))
                    .Field(new Field("DETCARGAB.IDEN")

                    )
                    .Field(new Field("DETCARGAB.PESO_ORIG")
                       .DbType(System.Data.DbType.Decimal)

                    )
                    .Field(new Field("DETCARGAB.BULTOS")
                       .DbType(System.Data.DbType.Decimal)

                    )                 
                    .Field(new Field("DETCARGAB.ID_BOOKINGS_EXPO")
                        .SetValue(Convert.ToInt32(ID_BOOKINGS_EXPO))
                    )
                    .LeftJoin("BOOKINGS_EXPO_DOCS", "BOOKINGS_EXPO_DOCS.ID", "=", "DETCARGAB.ID_BOOKINGS_EXPO");         


                if (!String.IsNullOrEmpty(ID_BOOKINGS_EXPO))
                {
                    var response = editorCntr
                    .Where(q => q.Where("DETCARGAB.ID_BOOKINGS_EXPO", ID_BOOKINGS_EXPO, "="))
                    .Process(request)
                    .Data();
                    return Json(response);
                }
                else
                {
                    var response = editorCntr
                    .Process(request)
                    .Data();
                    return Json(response);
                }



            }

        }

    }
}