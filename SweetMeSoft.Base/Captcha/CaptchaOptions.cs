namespace SweetMeSoft.Base.Captcha
{
    public class CaptchaOptions
    {
        public CaptchaOptions(CaptchaType captchaType, string twoCaptchaId)
        {
            CaptchaType = captchaType;
            TwoCaptchaId = twoCaptchaId;
        }

        public void InitNormal(string imageBase64, string hintText)
        {
            this.imageBase64 = imageBase64;
            this.hintText = hintText;
        }

        public void InitReCaptcha(string siteKey, string siteUrl)
        {
            this.siteKey = siteKey;
            this.siteUrl = siteUrl;
        }

        public CaptchaType CaptchaType { get; }

        public string TwoCaptchaId { get; }

        public string SiteKey { get { return siteKey; } }

        public string SiteUrl { get { return siteUrl; } }

        public string ImageBase64 { get { return imageBase64; } }

        public string HintText { get { return hintText; } }

        public string EmailNotifications { get; set; }

        private string siteKey;

        private string siteUrl;

        private string imageBase64;

        private string hintText;
    }
}
