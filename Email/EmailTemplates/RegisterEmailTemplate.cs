using WebApplication1.Entities;

namespace WebApplication1.Email.EmailTemplates
{
    public class RegisterEmailTemplate
    {
        public EmailRequest Template(User user)
        {
            return new EmailRequest
            {
                ToEmail = user.Email,
                ToName = user.Username,
                Subject = "Welcome to Our Platform",
                Body = $@"
<!DOCTYPE html>
<html>
<head>
<meta charset='UTF-8'>
<meta name='viewport' content='width=device-width, initial-scale=1.0'>
</head>

<body style='margin:0; padding:0; background-color:#f4f4f4; font-family:Arial, sans-serif;'>

<table width='100%' bgcolor='#f4f4f4' cellpadding='0' cellspacing='0'>
<tr>
<td align='center'>

    <table width='600' cellpadding='0' cellspacing='0' style='background:#ffffff; margin:20px 0; border-radius:8px; overflow:hidden;'>

        <!-- Header -->
        <tr>
            <td style='background:#4CAF50; padding:20px; text-align:center; color:#ffffff;'>
                <h1 style='margin:0;'>YourApp</h1>
                <p style='margin:0;'>Welcome Aboard 🚀</p>
            </td>
        </tr>

        <!-- Body -->
        <tr>
            <td style='padding:30px; color:#333333;'>
                <h2 style='margin-top:0;'>Hello {user.Username},</h2>

                <p>We're excited to have you on board! Your account has been successfully created.</p>

                <p>You can now start exploring all the features we offer.</p>

                <div style='text-align:center; margin:30px 0;'>
                    <a href='#' style='background:#4CAF50; color:#ffffff; padding:12px 20px; text-decoration:none; border-radius:5px; display:inline-block;'>
                        Get Started
                    </a>
                </div>

                <p>If you did not create this account, please contact our support immediately.</p>

                <p>Best regards,<br><strong>YourApp Team</strong></p>
            </td>
        </tr>

        <!-- Footer -->
        <tr>
            <td style='background:#f1f1f1; padding:15px; text-align:center; font-size:12px; color:#777777;'>
                <p style='margin:0;'>© {DateTime.UtcNow.Year} YourApp. All rights reserved.</p>
                <p style='margin:0;'>This is an automated message, please do not reply.</p>
            </td>
        </tr>

    </table>

</td>
</tr>
</table>

</body>
</html>"
            };
        }
    }
}
