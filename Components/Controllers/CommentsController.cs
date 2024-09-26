using AllNet.Modules.ReservasExportaciones.Components.Models;
using DataTables;
using System;
using System.Collections.Generic;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Web;
using System.Web.Http;

namespace AllNet.Modules.ReservasExportaciones.Components.Controllers
{
    public class CommentsController : MasterController
    {
        public override IHttpActionResult Rest(HttpRequest request)
        {
            var bl = request.Form.GetValues("Booking").FirstOrDefault();
            using (var db = new Database(DbType, ConnectionString))
            {
                var response = new Editor(db, "BOOKINGS_COMMENTS", new[] { "ID_COMMENT" })
                    .Model<CommentsModel>()                 
                    .Where(bl_orig=> bl_orig.Where("BL_ORIG",bl,"="))
                    .Process(request)
                    .Data();
                return Json(response);
            }

        }

        public IHttpActionResult InsertComment()
        {
            var request = HttpContext.Current.Request;
            var bl_orig = request.Form.GetValues("Booking").FirstOrDefault();
            var comment = request.Form.GetValues("Comment").FirstOrDefault();
            try
            {
                using (var db = new Database(DbType, ConnectionString))
                {
                    Dictionary<string, dynamic> commentData = new Dictionary<string, dynamic>
                    {
                        { "BL_ORIG", bl_orig },
                        { "COMMENT", comment },
                        { "COMMENT_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                    };

                    db.Insert("BOOKINGS_COMMENTS", commentData);             
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