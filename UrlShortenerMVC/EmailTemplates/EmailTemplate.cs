namespace UrlShortenerMVC.EmailTemplates
{
    public class EmailTemplate
    {
        public static string GetConfirmEmailBody(string link)
        {
            return "<!DOCTYPE html><html><head></head>" +
                "<body>" +
                "<h1>Thanks for joining Short Them Up!</h1>" +
                "<p> Please confirm your email address by clicking  the link below.</p>" +
                "<p><a href=\"" + link + "\" target=\"_blank\">Confirm Email Address</a></p><hr>" +
                "<a href=\"https://shortthemup.com/\" target=\"_blank\">Short Them Up</a>&nbsp;&nbsp;" +
                "<a href=\"https://shortthemup.com/Home/TermsAndConditions\" target=\"_blank\">Terms &amp; Conditions</a>&nbsp;&nbsp;" +
                "<a href=\"https://shortthemup.com/Home/PrivacyPolicy\" target=\"_blank\">Privacy Policy</a>" +
                "</body></html>";
        }

        public static string GetResetPasswordBody(string link)
        {
            return "<!DOCTYPE html><html><head></head>" +
                "<body>" +
                "<h1>Reset password request.</h1>" +
                "<p> Please click the following link in order to set a new password for your account.</p>" +
                "<p><a href=\"" + link + "\" target=\"_blank\">Reset Password</a></p><hr>" +
                "<a href=\"https://shortthemup.com/\" target=\"_blank\">Short Them Up</a>&nbsp;&nbsp;" +
                "<a href=\"https://shortthemup.com/Home/TermsAndConditions\" target=\"_blank\">Terms &amp; Conditions</a>&nbsp;&nbsp;" +
                "<a href=\"https://shortthemup.com/Home/PrivacyPolicy\" target=\"_blank\">Privacy Policy</a>" +
                "</body></html>";
        }
    }
}