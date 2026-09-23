namespace Support.Auth.Id.Constans;

public static class Message
{
    public static class Status
    {
        public const string Pending = "Pending";
        public const string Processing = "Processing";
        public const string Completed = "Completed";
        public const string Failed = "Failed";
    }

    public static class Type
    {
        public const string ForgotPassword = "FogotPasswordEmail";
        public const string VerifyAccount = "VerifyAccountEmail";
        public const string Reminder = "ReminderEmail";
    }
}
