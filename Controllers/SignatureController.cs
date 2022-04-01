using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace signature_lookup.Controllers
{

    [Route("")]
    [ApiController]
    public class SignatureController : ControllerBase
    {

		private readonly Credentials _credentials;

		public SignatureController(IOptions<Credentials> cred)
		{
			_credentials = cred.Value;
		}


		[HttpGet("{mail}/{type?}")]
		public ContentResult GetSignature(string mail = "lasse.gaardsholt@bestseller.com", [RegularExpression(@"^(html|text)$", ErrorMessage = "Only 'html' or 'text' is allowed.")] string? type = "html")
		{
			return new ContentResult
			{
				ContentType = "text/html",
				Content = getSignature(mail, type == "html")
			};
        }


		[ApiExplorerSettings(IgnoreApi = true)]
		public string getSignature(string userMail, bool getHTML)
		{
			try
			{
				var exchangeService = new ExchangeService
				{
					Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx")
				};

				exchangeService.TraceEnabled = false;
				exchangeService.Credentials = new WebCredentials(_credentials.Username, _credentials.Password);
				_ = exchangeService.Url;
				exchangeService.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, userMail);


				Folder folder = Folder.Bind(exchangeService, WellKnownFolderName.Root).Result;
				UserConfiguration userConfiguration = UserConfiguration.Bind(exchangeService, "OWA.UserOptions", folder.ParentFolderId, UserConfigurationProperties.All).Result;

                string html = userConfiguration.Dictionary.TryGetValue("signaturehtml", out object htmlValue) ? htmlValue.ToString() : "No signature was found.";
                string text = userConfiguration.Dictionary.TryGetValue("signaturetext", out object textValue) ? textValue.ToString() : "No Signature was found.";

                //string html = userConfiguration.Dictionary.ContainsKey("signaturehtml") ? userConfiguration.Dictionary["signaturehtml"]?.ToString() : "";
                //string text = userConfiguration.Dictionary.ContainsKey("signaturetext") ? userConfiguration.Dictionary["signaturetext"]?.ToString() : "";

                return getHTML ? html : text;
			}
			catch (Exception e)
			{
				return e.Message;
			}
		}

	}
}
