using System;
using System.Threading.Tasks;
using System.Threading;
using SweetMeSoft.Base.Tools;
using TwoCaptcha.Captcha;

namespace SweetMeSoft.Tools
{
    public class CaptchaSolver
    {
        public static async Task<string> SolveAsync(CaptchaOptions options)
        {
            Console.WriteLine("Resolving Captcha...");
            var solver = new TwoCaptcha.TwoCaptcha(options.TwoCaptchaId)
            {
                DefaultTimeout = 120,
                RecaptchaTimeout = 600,
                PollingInterval = 10
            };

            var captcha = new ReCaptcha();
            captcha.SetSiteKey(options.SiteKey);
            captcha.SetUrl(options.SiteUrl);

            var code = "";
            try
            {
                var balance = (await solver.Balance()) / 100000.0;
                Console.WriteLine("Balance: " + balance.ToString("#.00"));
                if (balance < 1 && !string.IsNullOrEmpty(options.EmailNotifications))
                {
                    Email.Send(new EmailOptions(options.EmailNotifications)
                    {
                        Subject = "Captcha solver Funds",
                        HtmlBody = "The balance of your Captcha Solver is under 1 USD. Please recharge your account."
                    });
                }

                var captchaId = await solver.Send(captcha);
                while (code == "" || code == null)
                {
                    Thread.Sleep(2000);
                    code = await solver.GetResult(captchaId);
                }

                return code;
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error occurred: " + e.Message);
            }

            return "";
        }
    }
}
