# SweetMeSoft.Captcha

Librería para la resolución automática de diferentes tipos de captchas utilizando el servicio 2Captcha.

## Descripción

`SweetMeSoft.Captcha` es una librería para .NET Standard 2.1 que actúa como un wrapper para el servicio [2Captcha](https://2captcha.com/), simplificando el proceso de envío y resolución de captchas. Es compatible con varios tipos de captcha, incluyendo captchas de imagen, ReCaptcha (v2, v3, Invisible, Enterprise) y hCaptcha.

## Características

-   **Resolución de Múltiples Tipos de Captcha:**
    -   `Normal`: Captchas de imagen a texto.
    -   `ReCaptchaV2`: El clásico "No soy un robot".
    -   `ReCaptchaV2Invisible`: ReCaptcha invisible que se activa por comportamiento.
    -   `ReCaptchaV3`: ReCaptcha basado en puntuación.
    -   `ReCaptchaEnterprise`: La versión empresarial de ReCaptcha.
-   **Notificación por Email:** Envía una notificación por correo electrónico cuando el saldo de la cuenta de 2Captcha es bajo, evitando interrupciones en el servicio.
-   **Configuración Sencilla:** Se configura fácilmente a través de la clase `CaptchaOptions`.

## Dependencias

-   [SweetMeSoft.Base](https://www.nuget.org/packages/SweetMeSoft.Base/)
-   [SweetMeSoft.Tools](https://www.nuget.org/packages/SweetMeSoft.Tools/)
-   `TwoCaptcha.dll` (Librería de terceros incluida en el proyecto).

## Instalación

```bash
dotnet add package SweetMeSoft.Captcha
```

## Uso

El uso de la librería se centra en la clase estática `Solver`. Debes crear una instancia de `CaptchaOptions` y pasarla al método `SolveAsync`.

### `CaptchaOptions`

| Propiedad          | Tipo          | Descripción                                                                                              |
| ------------------ | ------------- | -------------------------------------------------------------------------------------------------------- |
| `TwoCaptchaKey`    | `string`      | **Requerido.** Tu clave API del servicio 2Captcha.                                                       |
| `CaptchaType`      | `CaptchaType` | **Requerido.** El tipo de captcha que quieres resolver.                                                  |
| `ImageBase64`      | `string`      | Requerido para `CaptchaType.Normal`. La imagen del captcha codificada en Base64.                         |
| `HintText`         | `string`      | Opcional para `CaptchaType.Normal`. Una pista para resolver el captcha.                                  |
| `SiteKey`          | `string`      | Requerido para todos los tipos de ReCaptcha. El `sitekey` de la página web donde está el captcha.        |
| `SiteUrl`          | `string`      | Requerido para todos los tipos de ReCaptcha. La URL de la página donde se muestra el captcha.            |
| `EmailNotifications` | `string`    | Opcional. Una dirección de correo para notificar cuando el saldo de 2Captcha es bajo.                    |

### Ejemplo de Uso

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
            TwoCaptchaKey = "TU_API_KEY_DE_2CAPTCHA",
            CaptchaType = CaptchaType.Normal,
            ImageBase64 = "iVBORw0KGgoAAAANSUhEUgAA..." // Contenido de la imagen en Base64
        };

        string result = await Solver.SolveAsync(options);
        // `result` contendrá el texto del captcha resuelto
    }

    public async Task SolveReCaptchaV2()
    {
        var options = new CaptchaOptions
        {
            TwoCaptchaKey = "TU_API_KEY_DE_2CAPTCHA",
            CaptchaType = CaptchaType.ReCaptchaV2,
            SiteKey = "EL_SITE_KEY_DE_LA_PAGINA",
            SiteUrl = "https://www.google.com/recaptcha/api2/demo"
        };

        string result = await Solver.SolveAsync(options);
        // `result` contendrá el token de respuesta de ReCaptcha
    }
}
```

## Licencia

Este proyecto está bajo la licencia MIT. 