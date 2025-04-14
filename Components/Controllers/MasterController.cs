using AllNet.Modules.ReservasExportaciones.Components.Models;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Web.Api;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Web;
using System.Web.Http;

namespace AllNet.Modules.ReservasExportaciones.Components.Controllers
{
    [DnnAuthorize]
    public class MasterController : DnnApiController
    {
        private string _connectionString;
        private string _dbType;
        private string _secretKey;
        public string SecretKey
        {
            get
            {
                if (string.IsNullOrEmpty(_secretKey))
                {
                    _secretKey = "123";
                }
                return _secretKey;
            }
            set
            {
                _secretKey = value;
            }
        }
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(_connectionString))
                {
                    _connectionString = Config.GetConnectionString("BD_AGP");
                }
                return _connectionString;
            }
            set
            {
                _connectionString = value;
            }
        }
        public string DbType
        {
            get
            {
                if (string.IsNullOrEmpty(_dbType))
                {
                    _dbType = "sqlserver";
                }
                return _dbType;
            }
            set { _dbType = value; }
        }

        public bool IsAgent { get; set; } = false;
        public string NitUser { get; set; } = "";
        public virtual IHttpActionResult Rest(HttpRequest request) { return Json("No implementado"); }
        public virtual IHttpActionResult Rest(HttpRequest request, DataClient d) { return Json("No implementado"); }

        [HttpGet]
        public HttpResponseMessage Hello()
        {
            return Request.CreateResponse(HttpStatusCode.OK, "Hola");
        }

        [HttpPost]
        public virtual IHttpActionResult Get()
         {
            try
            {
                var request = HttpContext.Current.Request;               
                NitUser = request.Form.GetValues("nituser").FirstOrDefault();
                //NitUser = "860000006";
                IsAgent = Convert.ToBoolean(request.Form.GetValues("isagent").FirstOrDefault());
                if (!String.IsNullOrEmpty(request.Form["pkey"]))
                {
                    SecretKey = RC4.Decrypt(SecretKey, request.Form["pkey"].ToString());
                }
                if (!String.IsNullOrEmpty(request.Form["cn"]))
                    ConnectionString = Config.GetConnectionString(RC4.Decrypt(SecretKey, request.Form["cn"].ToString()));
                return Rest(request);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }


        [HttpPost]
        public virtual IHttpActionResult Get2([FromBody] DataClient data)
        {
            try
            {
                var request = HttpContext.Current.Request;
                NitUser = data.nituser;
                IsAgent = Convert.ToBoolean(data.isagent);
                if (!String.IsNullOrEmpty(data.pkey))
                {
                    SecretKey = RC4.Decrypt(SecretKey, data.pkey);
                }
                if (!String.IsNullOrEmpty(data.cn))
                    ConnectionString = Config.GetConnectionString(RC4.Decrypt(SecretKey, data.cn));
                return Rest(request,data);

            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }

        }

        [HttpPost]
        public virtual IHttpActionResult Create()
        {
            try
            {
                var request = HttpContext.Current.Request;
                NitUser = request.Form.GetValues("nituser").FirstOrDefault();
                IsAgent = Convert.ToBoolean(request.Form.GetValues("isagent").FirstOrDefault());
                if (!String.IsNullOrEmpty(request.Form["pkey"]))
                {
                    SecretKey = RC4.Decrypt(SecretKey, request.Form["pkey"].ToString());
                }
                if (!String.IsNullOrEmpty(request.Form["cn"]))
                    ConnectionString = Config.GetConnectionString(RC4.Decrypt(SecretKey, request.Form["cn"].ToString()));
                return Rest(request);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPut]
        public virtual IHttpActionResult Edit()
        {
            try
            {
                var request = HttpContext.Current.Request;
                NitUser = request.Form.GetValues("nituser").FirstOrDefault();
                IsAgent = Convert.ToBoolean(request.Form.GetValues("isagent").FirstOrDefault());
                if (!String.IsNullOrEmpty(request.Form["pkey"]))
                {
                    SecretKey = RC4.Decrypt(SecretKey, request.Form["pkey"].ToString());
                }
                if (!String.IsNullOrEmpty(request.Form["cn"]))
                    ConnectionString = Config.GetConnectionString(RC4.Decrypt(SecretKey, request.Form["cn"].ToString()));
                return Rest(request);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }



        [HttpPost]
        public virtual IHttpActionResult Remove()//[FromBody] FormDataCollection formData
        {
            try
            {
                var request = HttpContext.Current.Request;
                NitUser = request.Form.GetValues("nituser").FirstOrDefault();
                IsAgent = Convert.ToBoolean(request.Form.GetValues("isagent").FirstOrDefault());
                if (!String.IsNullOrEmpty(request.Form["pkey"]))
                {
                    SecretKey = RC4.Decrypt(SecretKey, request.Form["pkey"].ToString());
                }
                if (!String.IsNullOrEmpty(request.Form["cn"]))
                    ConnectionString = Config.GetConnectionString(RC4.Decrypt(SecretKey, request.Form["cn"].ToString()));
                return Rest(request);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }

        [HttpPost]
        public virtual IHttpActionResult Upload()
        {
            try
            {
                var request = HttpContext.Current.Request;
                if (!String.IsNullOrEmpty(request.Form["pkey"]))
                {
                    SecretKey = RC4.Decrypt(SecretKey, request.Form["pkey"].ToString());
                }
                if (!String.IsNullOrEmpty(request.Form["cn"]))
                    ConnectionString = Config.GetConnectionString(RC4.Decrypt(SecretKey, request.Form["cn"].ToString()));
                return Rest(request);
            }
            catch (Exception ex)
            {
                return Json(ex.Message);
            }
        }
    }
}