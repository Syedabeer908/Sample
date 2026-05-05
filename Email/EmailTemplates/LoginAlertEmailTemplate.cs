using WebApplication1.Common.Parsers;
using WebApplication1.Entities;

namespace WebApplication1.Email.EmailTemplates
{
    public class LoginAlertEmailTemplate
    {
        public EmailRequest Template(User user, UserInfo info)
        {
            return new EmailRequest
            {
                ToEmail = user.Email,
                ToName = user.Username,
                Subject = "New Login Detected",
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
            <td style='background:#ff9800; padding:20px; text-align:center; color:#ffffff;'>
                <h1 style='margin:0;'>YourApp</h1>
                <p style='margin:0;'>Security Alert</p>
            </td>
        </tr>

        <!-- Body -->
        <tr>
            <td style='padding:30px; color:#333333;'>
                <h2 style='margin-top:0;'>Hello {user.Username},</h2>

                <p>We noticed a new login to your account.</p>

                <table width='100%' style='margin:20px 0; border-collapse:collapse;'>
                    <tr>
                        <td><strong>Time:</strong></td>
                        <td>{info.LoginTime}</td>
                    </tr>
                    <tr>
                        <td><strong>IP Address:</strong></td>
                        <td>{info.IpAddress}</td>
                    </tr>
                    <tr>
                        <td><strong>Device:</strong></td>
                        <td>{info.DeviceInfo}</td>
                    </tr>
                </table>

                <p>If this was you, you can safely ignore this email.</p>

                <p style='color:red;'><strong>If this was NOT you:</strong></p>
                <ul>
                    <li>Change your password immediately</li>
                    <li>Enable 2FA if available</li>
                    <li>Contact support</li>
                </ul>

                <div style='text-align:center; margin:30px 0;'>
                    <a href='#' style='background:#ff9800; color:#ffffff; padding:12px 20px; text-decoration:none; border-radius:5px; display:inline-block;'>
                        Secure My Account
                    </a>
                </div>

                <p>Stay safe,<br><strong>YourApp Security Team</strong></p>
            </td>
        </tr>

        <!-- Footer -->
        <tr>
            <td style='background:#f1f1f1; padding:15px; text-align:center; font-size:12px; color:#777777;'>
                <p style='margin:0;'>© {DateTime.UtcNow.Year} YourApp. All rights reserved.</p>
                <p style='margin:0;'>This is an automated security notification.</p>
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
