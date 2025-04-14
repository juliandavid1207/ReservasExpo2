using DotNetNuke.Services.Mail;
using System;
using System.Text;
using System.Threading.Tasks;

namespace AllNet.Modules.ReservasExportaciones.Components.Controllers
{
    public class EmailController
    {
        // Campos privados para almacenar la información del email
        private string _fromAddress; //Emisor
        private string _to; //Destinatario
        private string _cc; // A quien se copia
        private string _subject; //Asunto
        private string _msg; //Cuerpo del mensaje

        // Constructor que permite inicializar el controlador de email con parámetros opcionales
        public EmailController(string to, string from = null, string cc = null, string subject = null, string msg = null)
        {
            _to = to; // Dirección de correo del destinatario

            // Establece la dirección de origen si se proporciona, de lo contrario usa una por defecto
            _fromAddress = !String.IsNullOrEmpty(from) ? from : "info@famfuturo.com";

            // Establece la dirección de CC si se proporciona, de lo contrario usa una por defecto
            _cc = cc;

            // Establece el asunto del correo si se proporciona, de lo contrario usa un asunto por defecto
            _subject = !String.IsNullOrEmpty(subject) ? subject : $"Información FamFuturo";

            // Establece el cuerpo del mensaje si se proporciona, de lo contrario usa un mensaje por defecto
            _msg = msg;
        }

        // Método para enviar el correo electrónico
        public async Task SendEmailAsync(int mailPriority, int mailFormat, string smtpServer, string emailAuth, string passAuth, Encoding encoding)
        {
            try
            {
                // Ejecuta el método SendMail en un hilo separado usando Task.Run
                await Task.Run(() =>
                    Mail.SendMail(
                        FromAddress,
                        $"{ToAddress}",
                        "",
                        CcAddress,
                        (MailPriority)mailPriority,
                        SubjectEmail,
                        (MailFormat)mailFormat,
                        encoding,
                        ComposeMessage(),
                        String.Empty,
                        smtpServer,
                        "",
                        emailAuth,
                        passAuth,
                        true
                    )
                );
            }
            catch (Exception ex)
            {
                throw new Exception("Error al enviar el correo electrónico: " + ex.Message);
            }
        }

        private string ComposeMessage()
        {
            return _msg.Replace("\\n", "\n");
        }

        public string MessageEmail
        {
            get { return _msg; }
            set { _msg = value; }
        }


        public string SubjectEmail
        {
            get { return $"{_subject}"; }
            set { _subject = value; }
        }

        public string CcAddress
        {
            get { return _cc; }
            set { _cc = value; }
        }


        public string ToAddress
        {
            get { return _to; }
            set { _to = value; }
        }

        public string FromAddress
        {
            get { return _fromAddress; }
            set { _fromAddress = value; }
        }

    }
}