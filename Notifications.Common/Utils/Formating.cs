namespace Notifications.Common.Utils
{
    public static class Formating
    {
        public static string FormatUserName(this string userName)
        {
            return userName.ToLower().Replace(Constants.DefaultLowerDomainName, "");
        }
    }
}
