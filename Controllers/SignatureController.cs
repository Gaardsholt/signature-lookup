using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Exchange.WebServices.Data;
using Microsoft.Extensions.Options;
using Prometheus;
using System.ComponentModel.DataAnnotations;

namespace signature_lookup.Controllers
{

    [Route("")]
    [ApiController]
    public class SignatureController : ControllerBase
    {
		private static readonly Counter SignatureRequests = Metrics.CreateCounter("signature_requests", "Number of signature requests.");
		private static readonly Counter SignaturesFound = Metrics.CreateCounter("signature_found", "Number of signatures found.");
		private static readonly Counter SignaturesNotFound = Metrics.CreateCounter("signature_not_found", "Number of signature not found.");
		private static readonly Counter SignaturesMailboxNotFound = Metrics.CreateCounter("signature_mailbox_not_found", "Number of mailboxes not found.");

		private readonly Credentials _credentials;

		public SignatureController(IOptions<Credentials> cred)
		{
			_credentials = cred.Value;
		}


		[HttpGet("{mail}/{type?}")]
		public ContentResult GetSignature([EmailAddress] string mail = "lasse.gaardsholt@bestseller.com", [RegularExpression(@"^(html|text)$", ErrorMessage = "Only 'html' or 'text' is allowed.")] string? type = "html")
		{
			SignatureRequests.Inc();

			var exchangeService = new ExchangeService
			{
				Url = new Uri("https://outlook.office365.com/EWS/Exchange.asmx")
			};

			exchangeService.TraceEnabled = false;
			exchangeService.Credentials = new WebCredentials(_credentials.Username, _credentials.Password);
			exchangeService.ImpersonatedUserId = new ImpersonatedUserId(ConnectingIdType.SmtpAddress, mail);


			Folder folder;

			try
			{
				folder = Folder.Bind(exchangeService, WellKnownFolderName.Root).Result;
			}
			catch (Exception)
			{
				SignaturesMailboxNotFound.Inc();
				return new ContentResult
				{
					Content = $"No mailbox, with the e-mail address '{mail}', could be found.",
					StatusCode = StatusCodes.Status404NotFound
                };
			}

			UserConfiguration userConfiguration = UserConfiguration.Bind(exchangeService, "OWA.UserOptions", folder.ParentFolderId, UserConfigurationProperties.All).Result;

			//string html = userConfiguration.Dictionary.TryGetValue("signaturehtml", out object htmlValue) ? htmlValue.ToString() : "No signature was found.";
			//string text = userConfiguration.Dictionary.TryGetValue("signaturetext", out object textValue) ? textValue.ToString() : "No Signature was found.";

			userConfiguration.Dictionary.TryGetValue($"signature{type.ToLower()}", out object value);

			if (String.IsNullOrEmpty(value.ToString()))
			{
				SignaturesNotFound.Inc();
				return new ContentResult
				{
					Content = $"No signature was found for for e-mail address '{mail}'.",
					StatusCode = StatusCodes.Status404NotFound
				};
			}

			SignaturesFound.Inc();
			
			return new ContentResult
			{
				ContentType = "text/html",
				Content = value.ToString()
			};
        }

	}
}
