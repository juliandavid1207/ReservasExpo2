﻿using AllNet.Modules.ReservasExportaciones.Abstractions;
using AllNet.Modules.ReservasExportaciones.Components.Models;
using AllNet.Modules.ReservasExportaciones.Services;
using DataTables;
using DotNetNuke.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;

namespace AllNet.Modules.ReservasExportaciones.Components.Controllers
{
    public class FilesExpoController : MasterController
    {
        private readonly IBookingsServices _bookingsServices;
        public FilesExpoController()
        {
            _bookingsServices = new BookingServices();
        }
        public override IHttpActionResult Rest(HttpRequest request)
        {
            using (var db = new Database(DbType, ConnectionString))
            {
                string bl_orig = String.Empty;
                string path = String.Empty;
                var r_query = request.QueryString;
                var BULTOS = request.Form.GetValues(3).FirstOrDefault();
                var PESO = request.Form.GetValues(4).FirstOrDefault();
                if (!String.IsNullOrEmpty(request.Form["BOOKING"]) || !String.IsNullOrEmpty(request.QueryString["BOOKING"]))
                {
                    bl_orig = !String.IsNullOrEmpty(request.Form["BOOKING"]) ? request.Form["BOOKING"].ToString() : String.Empty;
                    bl_orig = String.IsNullOrEmpty(bl_orig) ? request.QueryString["BOOKING"].ToString() : bl_orig;
                }
                if (!String.IsNullOrEmpty(request.Form["CODIGO"]) || !String.IsNullOrEmpty(request.QueryString["CODIGO"]))
                {

                    string temp_codigo = !String.IsNullOrEmpty(request.Form["CODIGO"]) ? request.Form["CODIGO"].ToString() : String.Empty;
                    temp_codigo = String.IsNullOrEmpty(temp_codigo) ? request.QueryString["CODIGO"].ToString() : temp_codigo;
                    path = HttpContext.Current.Server.MapPath($"~/DesktopModules/ReservasExpo/Uploads/{bl_orig}/{temp_codigo}/");
                }
                if (!Directory.Exists(path) && !path.Equals(string.Empty))
                {
                    Directory.CreateDirectory(path);
                }

                var editor = new Editor(db, "BOOKINGS_EXPO_DOCS", "ID")//new[] { "BOOKING", "CODIGO" })
                    .Model<FilesExpoModel>("BOOKINGS_EXPO_DOCS")
                    .Debug(true)
                    .Field(new Field("ID"))
                    .Field(new Field("BOOKING")
                        .Validator(Validation.NotEmpty())
                    )
                    .Field(new Field("CATEGORIA")
                        .Validator(Validation.NotEmpty())
                    )
                    .Field(new Field("PESO")
                        .Validator(Validation.NotEmpty())
                        .DbType(System.Data.DbType.Decimal)
                    )
                    .Field(new Field("BULTOS")
                        .Validator(Validation.NotEmpty())
                        .DbType(System.Data.DbType.Decimal)
                    )
                    .Field(new Field("COMENTARIO")
                        .Validator(Validation.NotEmpty())                      
                    )
                    .Field(new Field("ADJUNTO")
                           .Upload(new Upload(path + @"__NAME_____ID____EXTN__")
                                   .Db("ARCHIVOS_BOOKINGS", "id", new Dictionary<string, object>
                                   {
                                   {"web_path", DataTables.Upload.DbType.WebPath},
                                   {"system_path", DataTables.Upload.DbType.SystemPath},
                                   {"filename", DataTables.Upload.DbType.FileName}
                                       //{"filesize", Upload.DbType.FileSize}                         
                                   })
                                   .DbClean(data =>
                                   {
                                       foreach (var row in data)
                                       {
                                           // Do something;
                                       }
                                       return true;
                                   })
                                   .Validator(Validation.FileSize(5000000, "Tamaño máximo 5000K."))
                                   .Validator(Validation.FileExtensions(new[] { "jpg", "png", "pdf" }, "Archivo no permitido (pdf,jpg,png)"))

                               )
                           )
                    .Field(new Field("CODIGO")
                        .Validator(Validation.NotEmpty())
                    )
                    .Field(new Field("ESTADO")
                        .Validator(Validation.NotEmpty())
                        .DbType(System.Data.DbType.Int32)
                        .SetValue(0)
                    )
                    .MJoin(new MJoin("DETCARGAB")
                       .Model<DetCargaBModel>()                       
                       .Link("DETCARGAB.ID_BOOKINGS_EXPO", "BOOKINGS_EXPO_DOCS.ID")
                    );
                    //.LeftJoin("DETCARGAB", "DETCARGAB.ID_DETCARGA", "=", "BOOKINGS_EXPO_DOCS.ID")    


                if (!String.IsNullOrEmpty(bl_orig))
                {
                    var response = editor
                    .Where(q => q.Where("BOOKINGS_EXPO_DOCS.BOOKING", bl_orig, "="))
                    .Process(request)
                    .Data();
                    return Json(response);
                }
                else
                {
                    var response = editor
                    .Process(request)
                    .Data();
                    return Json(response);
                }



            }

        }

        [HttpPut]
        [ActionName("UpdateState")]
        public IHttpActionResult Put()
        {
            var request = HttpContext.Current.Request;
            var agent = request.Form.GetValues("isagent").FirstOrDefault();
            if (agent == "True")
            {
                var state_id = request.Form.GetValues("state_id").FirstOrDefault();
                var bl = request.Form.GetValues("Booking").FirstOrDefault();
                var codigo = request.Form.GetValues("Codigo").FirstOrDefault();
                var result = _bookingsServices.UpdateState(bl, codigo, state_id);
                return Ok(result);
            }
            return Ok(agent);
        }








    }
}