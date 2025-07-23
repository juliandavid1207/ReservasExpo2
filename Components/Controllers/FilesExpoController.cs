using AllNet.Modules.ReservasExportaciones.Abstractions;
using AllNet.Modules.ReservasExportaciones.Components.Models;
using AllNet.Modules.ReservasExportaciones.Services;
using DataTables;
using DotNetNuke.Common.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;
using System.Threading.Tasks;

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
                var PESO = request.Form.GetValues(3).FirstOrDefault().Replace(".",",");
                var BULTOS = request.Form.GetValues(4).FirstOrDefault().Replace(".", ","); ;

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
                        .DbType(System.Data.DbType.Decimal)    
                        .SetValue(PESO)
                    )
                    .Field(new Field("BULTOS")                      
                        .DbType(System.Data.DbType.Decimal)       
                        .SetValue(BULTOS)
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
                    .Field(new Field("BLOQUEO_PB")
                        .Validator(Validation.NotEmpty())
                        .DbType(System.Data.DbType.Int32)
                        .SetValue(0)
                    )
                    .MJoin(new MJoin("ARCHIVOS_SELECTIVIDAD")
                        .Link("BOOKINGS_EXPO_DOCS.ID", "USERS_FILES_SL.BOOKINGS_ID")
                        .Link("ARCHIVOS_SELECTIVIDAD.Id", "USERS_FILES_SL.SELECTIVIDAD_ID")
                        .Model<ARCHIVOS_SELECTIVIDAD>()                        
                        .Field(new Field("web_path"))
                        .Field(new Field("system_path"))
                        .Field(new Field("filename"))
                        .Field(new Field("Id")
                        .Upload(new Upload(path + @"__NAME_____ID____EXTN__")
                            .Db("ARCHIVOS_SELECTIVIDAD", "Id", new Dictionary<string, object>
                            {
                                {"web_path", DataTables.Upload.DbType.WebPath},
                                {"system_path", DataTables.Upload.DbType.SystemPath},
                                {"filename", DataTables.Upload.DbType.FileName}
                            })
                            .Validator(Validation.FileSize(5000000, "Tamaño máximo 5MB"))
                            .Validator(Validation.FileExtensions(new[] { "jpg", "png", "pdf" }, "Formato inválido"))
                        )
    )
                    )
                    .MJoin(new MJoin("PLANILLA_TRASLADO")
                        .Link("BOOKINGS_EXPO_DOCS.ID", "USERS_FILES_PT.BOOKINGS_ID")
                        .Link("PLANILLA_TRASLADO.Id", "USERS_FILES_PT.PLANILLA_ID")
                        .Model<ARCHIVOS_SELECTIVIDAD>()
                        .Field(new Field("web_path"))
                        .Field(new Field("system_path"))
                        .Field(new Field("filename"))
                        .Field(new Field("Id")
                        .Upload(new Upload(path + @"__NAME_____ID____EXTN__")
                            .Db("PLANILLA_TRASLADO", "Id", new Dictionary<string, object>
                            {
                                {"web_path", DataTables.Upload.DbType.WebPath},
                                {"system_path", DataTables.Upload.DbType.SystemPath},
                                {"filename", DataTables.Upload.DbType.FileName}
                            })
                            .Validator(Validation.FileSize(5000000, "Tamaño máximo 5MB"))
                            .Validator(Validation.FileExtensions(new[] { "jpg", "png", "pdf" }, "Formato inválido"))
                        )
    )
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

        [HttpGet]
        [ActionName("GetPathArchivo")]
        public HttpResponseMessage Get(string id)
        {
            try
            {
                dynamic rutaFisica = _bookingsServices.GetFilePath(id).Result;
                

                if (rutaFisica == null)
                {
                    return Request.CreateResponse(HttpStatusCode.NotFound, "Archivo no encontrado");
                }

                string path=rutaFisica.path;
                string name = rutaFisica.name;

                var stream = new FileStream(path, FileMode.Open, FileAccess.Read);
                var result = new HttpResponseMessage(HttpStatusCode.OK)
                {
                    Content = new StreamContent(stream)
                };

                result.Content.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue("application/octet-stream");
                result.Content.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("attachment")
                {
                    FileName = name ?? Path.GetFileName(path)
                };

                return result;
            }
            catch (Exception ex)
            {
                return Request.CreateResponse(HttpStatusCode.InternalServerError, "Error: " + ex.Message);
            }
        }




    }
}