using AllNet.Modules.ReservasExportaciones.Abstractions;
using AllNet.Modules.ReservasExportaciones.Components.Models;
using AllNet.Modules.ReservasExportaciones.Services;
using DataTables;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.EnterpriseServices.Internal;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using DotNetNuke.Services.Mail;
using System.Text;

namespace AllNet.Modules.ReservasExportaciones.Components.Controllers
{
    public class CommentsController : MasterController
    {
        private readonly IBookingsServices _bookingsServices;
        public CommentsController()
        {
            _bookingsServices = new BookingServices();
        }
        public override IHttpActionResult Rest(HttpRequest request)
        {
            try
            {
                var draw = request.Form.GetValues("draw")?.FirstOrDefault();
                var bl = request.Form.GetValues("Booking")?.FirstOrDefault();

                using (var db = new Database(DbType, ConnectionString))
                {
                    var editor = new Editor(db, "BOOKINGS_COMMENTS", new[] { "ID_COMMENT" })
                        .Model<CommentsModel>();

                    if (!string.IsNullOrEmpty(bl))
                    {
                        editor.Where("BL_ORIG", bl, "=");
                    }

                    var response = editor.Process(request).Data();

                    var result = new
                    {
                        draw = Convert.ToInt32(draw),
                        recordsTotal = ((dynamic)response).recordsTotal,
                        recordsFiltered = ((dynamic)response).recordsFiltered,
                        data = ((dynamic)response).data
                    };
                
                    return Json(result);
                }
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message });
            }
        }

        public async Task<IHttpActionResult> InsertComment()
        {
            var request = HttpContext.Current.Request;
            var bl_orig = request.Form.GetValues("Booking").FirstOrDefault();
            var comment = request.Form.GetValues("Comment").FirstOrDefault();
            var user1 = request.Form.GetValues("User1").FirstOrDefault();
            var emailController = new EmailController("julian_0207@hotmail.com","jotalora@famcargo.com", "jotalora@famcargo.com","PruebaMailComments",comment);

            int mailPriority = (int)MailPriority.Normal;
            int mailFormat = (int)MailFormat.Text;
            string smtpServer = ConfigurationManager.AppSettings["SMTPserver"];
            string emailAuth = ConfigurationManager.AppSettings["SMTPuser"];
            string passAuth = ConfigurationManager.AppSettings["SMTPpwd"];
            try
            {
                using (var db = new Database(DbType, ConnectionString))
                {
                    Dictionary<string, dynamic> commentData = new Dictionary<string, dynamic>
                    {
                        { "BL_ORIG", bl_orig },
                        { "COMMENT", comment },
                        { "USER", user1 },
                        { "COMMENT_DATE", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
                    };

                    db.Insert("BOOKINGS_COMMENTS", commentData);
                    //await _bookingsServices.EnviarEmail("jotalora@famcargo.com", "julian_0207@hotmail.com","jotalora@famcargo.com", "Prueba envío comentarios", comment);
                    await emailController.SendEmailAsync(mailPriority,mailFormat,smtpServer,emailAuth,passAuth,Encoding.UTF8);
                 
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