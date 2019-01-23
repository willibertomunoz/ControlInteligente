using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ControlInteligente.Negocio
{
    public class CorreoElectronico
    {
        SmtpClient client = new SmtpClient();

        public CorreoElectronico()
        {
            client.Port = 587;
            client.Host = "smtp.gmail.com";
            client.EnableSsl = true;
            client.Timeout = 10000;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;
            client.UseDefaultCredentials = false;
            client.Credentials = new System.Net.NetworkCredential("willibertomunoz@gmail.com", "ipconfig2$");

        }


        public void EnviarCorreo(Usuario usuario)
        {

            MailMessage mm = new MailMessage("donotreply@domain.com", usuario.Correo, "Entrada con RFID",
                "Hola " + usuario.nombre + " " + usuario.apePaterno + "\n Han ocupado tu tarjeta para ingresar");
            mm.BodyEncoding = UTF8Encoding.UTF8;
            mm.DeliveryNotificationOptions = DeliveryNotificationOptions.OnFailure;

            client.Send(mm);
        }
    }
}
