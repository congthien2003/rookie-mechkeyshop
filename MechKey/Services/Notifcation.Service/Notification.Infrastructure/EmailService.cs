using Notification.Application;
using Shared.ViewModels.Order;
using System.Net;
using System.Net.Mail;

namespace Notifcation.Infrastructure
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
            try
            {
                MailMessage mailMessage = new MailMessage();
                var to = email;
                var from = "nhat23891@gmail.com";
                var pass = "zohi sncr hcqk kwwd";
                mailMessage.To.Add(to);
                mailMessage.From = new MailAddress(from);
                mailMessage.Subject = "MechkeyShop - THANK YOU FOR YOUR ORDER";
                string messageBody = $@"
                <html>
                <head>
                    <style>
                        body {{
                            font-family: Arial, sans-serif;
                            background-color: #f9f9f9;
                            margin: 0;
                            padding: 20px;
                        }}
                        .container {{
                            background-color: #ffffff;
                            padding: 20px;
                            border-radius: 8px;
                            max-width: 600px;
                            margin: auto;
                            box-shadow: 0 2px 8px rgba(0,0,0,0.1);
                        }}
                        h2 {{
                            color: #333333;
                        }}
                        h3 {{
                            color: #555555;
                            border-bottom: 1px solid #eeeeee;
                            padding-bottom: 5px;
                        }}
                        ul {{
                            list-style: none;
                            padding: 0;
                        }}
                        li {{
                            padding: 8px 0;
                            border-bottom: 1px solid #f1f1f1;
                        }}
                        li strong {{
                            display: inline-block;
                            width: 150px;
                        }}
                        p {{
                            color: #666666;
                            line-height: 1.5;
                        }}
                    </style>
                </head>
                <body>
                    <div class='container'>
                        <h2>Hi, {order.Name}</h2>
                        <p>We have received your order and are preparing it for shipment.</p>
                        <h3>Order infomation:</h3>
                        <ul>
                            <li><strong>ID:</strong> {order.Id.ToString().Substring(0, 8)}</li>
                            <li><strong>Total:</strong> {order.TotalAmount:C}</li>
                            <li><strong>Phone:</strong> {order.Phone}</li>
                            <li><strong>Shipping Address:</strong> {order.Address}</li>
                        </ul>
                        <p>Peaceful,<br/>MechKeyShop</p>
                    </div>
                </body>
                </html>";

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
    }
}
