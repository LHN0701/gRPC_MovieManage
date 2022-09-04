using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Net.Mime;
using System.Threading.Tasks;

namespace Client_QLRapChieuPhim.Common
{
    public class Utilities
    {
        public static bool SendMail(string subject, string message, string email)
        {
            try
            {
                var mailMsg = new MailMessage();

                mailMsg.To.Add(new MailAddress(email, ""));
                mailMsg.From = new MailAddress("info@csccinema.vn", "CSC Cinema");

                mailMsg.Subject = subject;
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message, null,
                                                                           MediaTypeNames.Text.Plain));
                mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(message, null,
                                                                            MediaTypeNames.Text.Html));

                var smtpClient = new SmtpClient("smtp.gmail.com", 587);
                var credentials = new System.Net.NetworkCredential("info@csccinema.vn", "123456");
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = credentials;
                smtpClient.EnableSsl = true;

                smtpClient.Send(mailMsg);
                return true;
            }
            catch (System.Exception)
            {
                return false;
            }
        }
    }

    public class PropertyCopier<TParent, TChild> where TParent : class
                                            where TChild : class
    {
        public static void Copy(TParent parent, TChild child)
        {
            var parentProperties = parent.GetType().GetProperties();
            var childProperties = child.GetType().GetProperties();

            foreach (var parentProperty in parentProperties)
            {
                if (parentProperty.Name.ToLower() == "id") continue;
                foreach (var childProperty in childProperties)
                {
                    if (parentProperty.Name == childProperty.Name && parentProperty.PropertyType == childProperty.PropertyType && childProperty.SetMethod != null)
                    {
                        if (parentProperty.GetValue(parent) != null)
                            childProperty.SetValue(child, parentProperty.GetValue(parent));
                        break;
                    }
                }
            }
        }
    }
}