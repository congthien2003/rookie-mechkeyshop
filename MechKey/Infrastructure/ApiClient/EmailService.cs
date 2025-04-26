using Application.Interfaces.IServices;
using Shared.ViewModels.Order;
using System.Net;
using System.Net.Mail;

namespace Infrastructure.ApiClient
{
    public class EmailService : IEmailService
    {
        public bool SendEmailConfirm(string email, string token)
        {
            try
            {
                MailMessage mailMessage = new MailMessage();
                var to = email;
                var from = "nhat23891@gmail.com";
                var pass = "zohi sncr hcqk kwwd";
                mailMessage.To.Add(to);
                mailMessage.From = new MailAddress(from);
                mailMessage.Subject = "Welcome to MechKey Shop";

                var confirmUrl = "https://localhost:7256/confirm/" + token; // bạn thay bằng link thật

                var messageBody = @"
                <div style='font-family: Arial, sans-serif; background-color: #f7f7f7; padding: 30px;'>
                  <div style='max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; overflow: hidden; box-shadow: 0 4px 10px rgba(0, 0, 0, 0.1);'>
                    <div style='background-color: #4CAF50; color: white; padding: 20px; text-align: center;'>
                      <h2>Welcome to My Store!</h2>
                    </div>
                    <div style='padding: 30px;'>
                      <p style='font-size: 16px; color: #333;'>Hi there,</p>
                      <p style='font-size: 16px; color: #333;'>
                        Thank you for creating an account with us. To complete your registration, please confirm your email address by clicking the button below.
                      </p>
                      <div style='text-align: center; margin: 30px 0;'>
                        <a href='" + confirmUrl + @"' 
                           style='display: inline-block; background-color: #4CAF50; color: white; padding: 12px 24px; font-size: 16px; border-radius: 6px; text-decoration: none;'>
                          Confirm Email
                        </a>
                      </div>
                      <p style='font-size: 14px; color: #777;'>
                        If you did not create an account, you can safely ignore this email.
                      </p>
                    </div>
                    <div style='background-color: #f1f1f1; color: #555; padding: 15px; text-align: center; font-size: 13px;'>
                      &copy; 2025 My Store. All rights reserved.
                    </div>
                  </div>
                </div>";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body = messageBody;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com");
                smtp.EnableSsl = true;
                smtp.Port = 587;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential(from, pass);

                smtp.Send(mailMessage);
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public bool SendEmailOrder(string email, OrderModel order)
        {
            throw new NotImplementedException();
        }
    }
}
