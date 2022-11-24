using System;
using System.Collections.Generic;
using System.Text;
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
            captcha.SetSiteKey(options.SiteKey); //"6LdTfb4UAAAAAEsThcpWARtYaTPhGjVhZYlX-aC8"
            captcha.SetUrl(options.SiteUrl); //"https://edocnube.com/consulta/wfrmLoginNube.aspx"

            var code = Console.ReadLine();
            try
            {
                var balance = await solver.Balance();
                if (balance < 1)
                {
                    Email.Send(options.EmailNotifications, "Captcha solver Funds", "The balance of your Captcha Solver is under 1 USD. Please recharge your account.");
                }

                var captchaId = await solver.Send(captcha);
                while (code == "" || code == null)
                {
                    Thread.Sleep(2000);
                    return await solver.GetResult(captchaId);
                }
            }
            catch (Exception e)
            {
                Console.Error.WriteLine("Error occurred: " + e.Message);
            }

            return "";
        }
    }
}
