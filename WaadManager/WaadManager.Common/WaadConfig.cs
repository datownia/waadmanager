using System.Configuration;

namespace WaadManager.Common
{
    public static class WaadConfig
    {
        public static readonly string DefaultPassword = ConfigurationManager.AppSettings["DefaultPassword"];
        public static readonly string AppKey = ConfigurationManager.AppSettings["AppKey"];
        public static readonly string AppSecret = ConfigurationManager.AppSettings["AppSecret"];

        public static readonly string OAuthEndpoint = ConfigurationManager.AppSettings["OAuthEndpoint"];
        public static readonly string ApiEndPoint = ConfigurationManager.AppSettings["ApiEndPoint"];

        public static readonly string LogInUrl = ConfigurationManager.AppSettings["loginUrl"];

        public static readonly string TenantDomainName = ConfigurationManager.AppSettings["TenantDomainName"];

        public static readonly string Publisher = ConfigurationManager.AppSettings["Publisher"];
        public static readonly string UserApiFullName = ConfigurationManager.AppSettings["ApiUser"];
        public static readonly string GroupApiFullName = ConfigurationManager.AppSettings["ApiGroup"];
        public static readonly string UserGroupApiFullName = ConfigurationManager.AppSettings["ApiUserGroup"];
        public static readonly string ConfScheduleApiFullName = ConfigurationManager.AppSettings["ApiConfSchedule"];
        public static readonly string Environment = ConfigurationManager.AppSettings["Environment"];
        
        //delta data structure
        public static readonly int UserIdFieldIndex = int.Parse(ConfigurationManager.AppSettings["UserIdFieldIndex"]);
        public static readonly int UserDisplayNameFieldIndex = int.Parse(ConfigurationManager.AppSettings["UserDisplayNameFieldIndex"]);
        public static readonly int UserUsernameFieldIndex = int.Parse(ConfigurationManager.AppSettings["UserUsernameFieldIndex"]);

        public static readonly int GroupIdFieldIndex = int.Parse(ConfigurationManager.AppSettings["GroupIdFieldIndex"]);
        public static readonly int GroupNameFieldIndex = int.Parse(ConfigurationManager.AppSettings["GroupNameFieldIndex"]);

        public static readonly int UserGroupUsernameFieldIndex = int.Parse(ConfigurationManager.AppSettings["UserGroupUsernameFieldIndex"]);
        public static readonly int UserGroupGroupFieldIndex = int.Parse(ConfigurationManager.AppSettings["UserGroupGroupFieldIndex"]);
        public static readonly int UserGroupIdFieldIndex = int.Parse(ConfigurationManager.AppSettings["UserGroupIdFieldIndex"]);

        public static int EventIdFieldIndex = int.Parse(ConfigurationManager.AppSettings["EventIdFieldIndex"]);
        public static int EventCodeFieldIndex = int.Parse(ConfigurationManager.AppSettings["EventCodeFieldIndex"]);
        public static int EventLocationFieldIndex = int.Parse(ConfigurationManager.AppSettings["EventLocationFieldIndex"]);
        public static int EventSpeakerFieldIndex = int.Parse(ConfigurationManager.AppSettings["EventSpeakerFieldIndex"]);
        public static int EventTitleFieldIndex = int.Parse(ConfigurationManager.AppSettings["EventTitleFieldIndex"]);
        public static int EventTimeFieldIndex = int.Parse(ConfigurationManager.AppSettings["EventTimeFieldIndex"]);
        public static int EventAreaFieldIndex = int.Parse(ConfigurationManager.AppSettings["EventAreaFieldIndex"]);
        public static int EventDayFieldIndex = int.Parse(ConfigurationManager.AppSettings["EventDayFieldIndex"]);
    }

    public static class WaadConfigEnvironment
    {
        public const string Development = "development";
        public const string Live = "live";
    }
}
