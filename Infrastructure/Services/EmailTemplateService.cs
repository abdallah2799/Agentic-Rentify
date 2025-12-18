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

                            {{DYNAMIC_CONTENT}}

                            {{SECURITY_NOTE}}

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

    public string GenerateEmailBody(string heading, string bodyText, string otpCode = "", string actionLink = "", string actionButtonText = "", string userEmail = "", string logoUrl = "https://res.cloudinary.com/dijyfu0m3/image/upload/v1766090397/zocvo7jehb3elemrwv2x.png")
    {
        string finalBody = Template
            .Replace("{{HEADING}}", heading)
            .Replace("{{BODY_TEXT}}", bodyText)
            .Replace("{{USER_EMAIL}}", userEmail)
            .Replace("{{LOGO_URL}}", logoUrl);

        string dynamicContent = "";

        if (!string.IsNullOrEmpty(otpCode))
        {
            dynamicContent = $@"
                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""margin: 0 0 30px;"">
                    <tr>
                        <td style=""padding: 30px; background-color: #f9f9f9; border: 2px solid #e5e5e5; border-radius: 8px; text-align: center;"">
                            <div style=""font-size: 36px; font-weight: 700; color: #1a1a1a; letter-spacing: 8px; font-family: 'Courier New', Courier, monospace;"">
                                {otpCode}
                            </div>
                        </td>
                    </tr>
                </table>";
        }
        if (!string.IsNullOrEmpty(actionLink) && !string.IsNullOrEmpty(actionButtonText))
        {
             dynamicContent = $@"
                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""margin: 0 0 30px;"">
                    <tr>
                        <td align=""center"">
                            <a href=""{actionLink}"" style=""background-color: #C86A41; color: #ffffff; padding: 12px 24px; text-decoration: none; border-radius: 4px; font-weight: bold; display: inline-block; font-size: 16px;"">
                                {actionButtonText}
                            </a>
                        </td>
                    </tr>
                </table>";
        }

        finalBody = finalBody.Replace("{{DYNAMIC_CONTENT}}", dynamicContent);

        // Security Note Logic: Show only if it's an action (OTP or Link)
        string securityNote = "";
        if (!string.IsNullOrEmpty(otpCode) || (!string.IsNullOrEmpty(actionLink) && !string.IsNullOrEmpty(actionButtonText)))
        {
            securityNote = @"
                <table role=""presentation"" cellspacing=""0"" cellpadding=""0"" border=""0"" width=""100%"" style=""margin: 0 0 20px;"">
                    <tr>
                        <td style=""padding: 20px; background-color: #fff8f5; border-left: 4px solid #C86A41; border-radius: 4px;"">
                            <p style=""margin: 0; font-size: 14px; line-height: 1.5; color: #4a4a4a;"">
                                <strong style=""color: #1a1a1a;"">Security Note:</strong> This link/code will expire in 10 minutes. If you didn't request this, please ignore this email or contact support if you have concerns.
                            </p>
                        </td>
                    </tr>
                </table>";
        }

        finalBody = finalBody.Replace("{{SECURITY_NOTE}}", securityNote);
        
        // Cleanup legacy placeholders if any
        finalBody = finalBody.Replace("{{OTP_CODE}}", ""); 
        finalBody = finalBody.Replace("{{MAGIC_LINK_URL}}", "#");

        return finalBody;
    }
}
