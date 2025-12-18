public class EmailTemplateService
{
    private const string Template = @"<!DOCTYPE html>
<html lang=""en"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <meta http-equiv=""X-UA-Compatible"" content=""IE=edge"">
    <title>Confirm Your Sign-In</title>
</head>
<body style=""margin: 0; padding: 0; font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif; background-color: #f5f5f5;"">
    <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""background-color: #f5f5f5;"">
        <tr>
            <td style=""padding: 40px 20px;"">
                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""max-width: 600px; margin: 0 auto; background-color: #ffffff; border-radius: 8px; box-shadow: 0 2px 8px rgba(0, 0, 0, 0.05);"">
                    
                    <tr>
                        <td style=""padding: 40px 40px 30px; text-align: center; background-color: #ffffff; border-radius: 8px 8px 0 0;"">
                            <img src=""{{LOGO_URL}}"" alt=""Company Logo"" width=""150"" height=""50"" style=""display: block; margin: 0 auto; border: 0;"">
                        </td>
                    </tr>

                    <tr>
                        <td style=""padding: 0 40px 40px;"">
                            <h1 style=""margin: 0 0 20px; font-size: 28px; font-weight: 600; color: #1a1a1a; line-height: 1.3; text-align: center;"">
                                {{HEADING}}
                            </h1>

                            <p style=""margin: 0 0 30px; font-size: 16px; line-height: 1.6; color: #4a4a4a; text-align: center;"">
                                {{BODY_TEXT}}
                            </p>

                            <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""margin: 0 0 30px;"">
                                <tr>
                                    <td style=""padding: 30px; background-color: #f9f9f9; border: 2px solid #e5e5e5; border-radius: 8px; text-align: center;"">
                                        <div style=""font-size: 36px; font-weight: 700; color: #1a1a1a; letter-spacing: 8px; font-family: 'Courier New', Courier, monospace;"">
                                            {{OTP_CODE}}
                                        </div>
                                    </td>
                                </tr>
                            </table>

                            <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""margin: 0 0 20px;"">
                                <tr>
                                    <td style=""padding: 20px; background-color: #fff8f5; border-left: 4px solid #C86A41; border-radius: 4px;"">
                                        <p style=""margin: 0; font-size: 14px; line-height: 1.5; color: #4a4a4a;"">
                                            <strong style=""color: #1a1a1a;"">Security Note:</strong> This code will expire in 10 minutes. If you didn't request this code, please ignore this email or contact support if you have concerns.
                                        </p>
                                    </td>
                                </tr>
                            </table>

                        </td>
                    </tr>

                    <tr>
                        <td style=""padding: 30px 40px; background-color: #f9f9f9; border-radius: 0 0 8px 8px; border-top: 1px solid #e5e5e5;"">
                            <p style=""margin: 0; font-size: 12px; line-height: 1.5; color: #999999; text-align: center;"">
                                This email was sent to {{USER_EMAIL}}. If you didn't request this verification, no action is needed.
                            </p>
                            <p style=""margin: 15px 0 0; font-size: 12px; line-height: 1.5; color: #999999; text-align: center;"">
                                © 2025 PYRAMIS. All rights reserved.
                            </p>
                        </td>
                    </tr>

                </table>
            </td>
        </tr>
    </table>
</body>
</html>";

    public string GenerateEmailBody(string heading, string bodyText, string otpCode, string userEmail, string logoUrl = "https://res.cloudinary.com/dijyfu0m3/image/upload/v1766090397/zocvo7jehb3elemrwv2x.svg")
    {
        string finalBody = Template
            .Replace("{{HEADING}}", heading)
            .Replace("{{BODY_TEXT}}", bodyText)
            .Replace("{{USER_EMAIL}}", userEmail)
            .Replace("{{LOGO_URL}}", logoUrl);

        // Handle Optional OTP Code
        if (string.IsNullOrEmpty(otpCode))
        {
            // Remove the OTP table block if OTP is empty
            // This is a simple string replacement removal. 
            // Ideally we'd use regex or specific IDs, but for this specific template structure:
             finalBody = finalBody.Replace("{{OTP_CODE}}", "");
             // We can also try to remove the surrounding table if we want to be cleaner, but just clearing the code is safer to avoid breaking HTML structure blindly.
        }
        else
        {
            finalBody = finalBody.Replace("{{OTP_CODE}}", otpCode);
        }
        
        // Remove MAGIC_LINK_URL placeholder if unused (it wasn't in parameters but is in template)
        finalBody = finalBody.Replace("{{MAGIC_LINK_URL}}", "#");

        return finalBody;
    }
}
