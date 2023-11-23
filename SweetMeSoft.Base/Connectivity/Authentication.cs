namespace SweetMeSoft.Base.Connectivity
{
    public class Authentication
    {
        public AuthenticationType Type { get; set; }

        public string Key { get; set; }

        public string Value { get; set; }
    }

    public enum AuthenticationType
    {
        Bearer,
        ApiKey,
        Cookie,
        Basic
    }
}
