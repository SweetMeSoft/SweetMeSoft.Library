# SweetMeSoft.Captcha

Library for automatic resolution of different types of captchas using the 2Captcha service.

## Description

`SweetMeSoft.Captcha` is a library for .NET Standard 2.1 that acts as a wrapper for the [2Captcha](https://2captcha.com/) service, simplifying the process of sending and solving captchas. It is compatible with various types of captcha, including image captchas, ReCaptcha (v2, v3, Invisible, Enterprise) and hCaptcha.

## Features

-   **Resolution of Multiple Captcha Types:**
    -   `Normal`: Image to text captchas.
    -   `ReCaptchaV2`: The classic "I'm not a robot".
    -   `ReCaptchaV2Invisible`: Invisible ReCaptcha that is activated by behavior.
    -   `ReCaptchaV3`: Score-based ReCaptcha.
    -   `ReCaptchaEnterprise`: The enterprise version of ReCaptcha.
-   **Email Notification:** Sends an email notification when the 2Captcha account balance is low, avoiding service interruptions.
-   **Simple Configuration:** Easily configured through the `CaptchaOptions` class.

## Dependencies

-   [SweetMeSoft.Base](https://www.nuget.org/packages/SweetMeSoft.Base/)
-   [SweetMeSoft.Tools](https://www.nuget.org/packages/SweetMeSoft.Tools/)
-   `TwoCaptcha.dll` (Third-party library included in the project).

## Installation

```bash
dotnet add package SweetMeSoft.Captcha
```

## Usage

The library usage is centered on the static `Solver` class. You must create an instance of `CaptchaOptions` and pass it to the `SolveAsync` method.

### `CaptchaOptions`

| Property           | Type          | Description                                                                                              |
| ------------------ | ------------- | -------------------------------------------------------------------------------------------------------- |
| `TwoCaptchaKey`    | `string`      | **Required.** Your 2Captcha service API key.                                                            |
| `CaptchaType`      | `CaptchaType` | **Required.** The type of captcha you want to solve.                                                    |
| `ImageBase64`      | `string`      | Required for `CaptchaType.Normal`. The captcha image encoded in Base64.                                 |
| `HintText`         | `string`      | Optional for `CaptchaType.Normal`. A hint to solve the captcha.                                         |
| `SiteKey`          | `string`      | Required for all ReCaptcha types. The `sitekey` of the web page where the captcha is located.          |
| `SiteUrl`          | `string`      | Required for all ReCaptcha types. The URL of the page where the captcha is displayed.                  |
| `EmailNotifications` | `string`    | Optional. An email address to notify when the 2Captcha balance is low.                                  |

### Usage Example

```csharp
using SweetMeSoft.Base.Captcha;
using SweetMeSoft.Captcha;
using System.Threading.Tasks;

public class CaptchaExample
{
    public async Task SolveImageCaptcha()
    {
        var options = new CaptchaOptions
        {
            TwoCaptchaKey = "YOUR_2CAPTCHA_API_KEY",
            CaptchaType = CaptchaType.Normal,
            ImageBase64 = "iVBORw0KGgoAAAANSUhEUgAA..." // Image content in Base64
        };

        string result = await Solver.SolveAsync(options);
        // `result` will contain the solved captcha text
    }

    public async Task SolveReCaptchaV2()
    {
        var options = new CaptchaOptions
        {
            TwoCaptchaKey = "YOUR_2CAPTCHA_API_KEY",
            CaptchaType = CaptchaType.ReCaptchaV2,
            SiteKey = "THE_PAGE_SITE_KEY",
            SiteUrl = "https://www.google.com/recaptcha/api2/demo"
        };

        string result = await Solver.SolveAsync(options);
        // `result` will contain the ReCaptcha response token
    }
}
```

## License

This project is under the MIT license.