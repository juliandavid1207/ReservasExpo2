using AllNet.Modules.ReservasExportaciones.Abstractions;
using AllNet.Modules.ReservasExportaciones.Components.Models;
using AllNet.Modules.ReservasExportaciones.Data;
using DataTables;
using DotNetNuke.Services.Mail;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace AllNet.Modules.ReservasExportaciones.Services
{
    public class BookingServices : IBookingsServices
    {
        private readonly IRepository _repository;
        public int mailPriority = (int)MailPriority.Normal;
        public int mailFormat = (int)MailFormat.Text;
        public string smtpServer = ConfigurationManager.AppSettings["SMTPserver"];
        public string emailAuth = ConfigurationManager.AppSettings["SMTPuser"];
        public string passAuth = ConfigurationManager.AppSettings["SMTPpwd"];
        public BookingServices()
        {
            _repository = new Repository();
        }


        public async Task<bool> EnviarEmail(string fromAddress, string toAddress,string ccAddress, string subjectEmail, string message)
        {           
            try
            {               
                await Task.Run(() =>
                    Mail.SendMail(
                        fromAddress,
                        $"{toAddress}",
                        "",
                        ccAddress,
                        (MailPriority)mailPriority,
                        subjectEmail,
                        (MailFormat)mailFormat,
                        Encoding.UTF8,
                        message,
                        String.Empty,
                        smtpServer,
                        "",
                        emailAuth,
                        passAuth,
                        true
                    )
                );
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception("Error al enviar el correo electrónico: " + ex.Message);
            }
        }    

        public async Task<bool> UpdateState(string bl,string codigo, string id)
        {
            try
            {
                Dictionary<string, dynamic> state = new Dictionary<string, dynamic>
                {
                    {"ESTADO", id }
                };

                Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>
                {
                    {"BOOKING", bl},
                    {"CODIGO", codigo }
                };

                _repository.UpdateBookingState(state, condition);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }

        }    

        public async Task<object> GetFilePath(string id)
        {
            try
            {
                var path = _repository.GetFilePath(id);
                return path;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<List<DetCargaBModel>> GetDetCargaB(int id)
        {
            try
            {
                var detcargaB = _repository.GetDetCargaB(id);
                return detcargaB;

            }
            catch (Exception ex)
            {
                return null;
            }
        }

        public async Task<bool> UpdatePesoBultos(decimal peso, decimal bultos,int bloqueo, string id)
        {
            try
            {
                Dictionary<string, dynamic> state = new Dictionary<string, dynamic>
                {
                    {"BULTOS", bultos },
                    {"PESO", peso },
                    {"BLOQUEO_PB", bloqueo }
                };

                Dictionary<string, dynamic> condition = new Dictionary<string, dynamic>
                {
                    {"ID", id}                    
                };

                _repository.UpdateBookingState(state, condition);
                return true;

            }
            catch (Exception ex)
            {
                return false;
            }


        }

        public async Task<int> GetLastIdDetCargaB()
        {
            try
            {
                var id = _repository.GetLastId();
                return id;
            }
            catch
            {
                return 0;
            }
        }
    }
}