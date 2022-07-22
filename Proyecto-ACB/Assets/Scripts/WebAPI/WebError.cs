using System;
using System.Net;

namespace WebAPI
{
	public class WebError : Exception
	{
		const string MESSAGE_ERROR_400 = "request has invalid child names or invalid/missing/too large data";
		const string MESSAGE_ERROR_401 = "request's authorization has failed";
		const string MESSAGE_ERROR_403 = "request violates Firebase Realtime Database Rules";
		const string MESSAGE_ERROR_404 = "request made over HTTP instead of HTTPS";
		const string MESSAGE_ERROR_417 = "request doesn't specify a Firebase database name";
		const string MESSAGE_ERROR_UNDOCUMENTED = "request's error is not yet documented";

		private readonly HttpStatusCode _mStatus;


		public WebError(HttpStatusCode status, string message) : base(message)
		{
			_mStatus = status;
		}

		public WebError(HttpStatusCode status, string message, Exception inner) : base(message, inner)
		{
			_mStatus = status;
		}

		public WebError(string message) : base(message)
		{
		}

		public WebError(string message, Exception inner) : base(message, inner)
		{
		}

		public static WebError Create(WebException webEx)
		{
			string message;
			HttpStatusCode status = 0;
			var isStatusAvailable = false;

			if (webEx.Status == WebExceptionStatus.ProtocolError)
			{
				if (webEx.Response is HttpWebResponse response) 
				{
					status = response.StatusCode;
					isStatusAvailable = true;
				}
			}

			if (!isStatusAvailable)
				return new WebError(webEx.Message, webEx);

			switch (status) 
			{
				case HttpStatusCode.Unauthorized:
					message = MESSAGE_ERROR_401;
					break;
				case HttpStatusCode.BadRequest:
					message = MESSAGE_ERROR_400;
					break;
				case HttpStatusCode.NotFound:
					message = MESSAGE_ERROR_404;
					break;
				case HttpStatusCode.ExpectationFailed:
					message = MESSAGE_ERROR_417;
					break;
				case HttpStatusCode.Forbidden:
					message = MESSAGE_ERROR_403;
					break;
				default:
					message = webEx.Message;
					break;
			}

			return new WebError(status, message, webEx);
		}

		public static WebError Create(HttpStatusCode status)
		{
			string message;

			switch (status) 
			{
				case HttpStatusCode.Unauthorized:
					message = MESSAGE_ERROR_401;
					break;
				case HttpStatusCode.BadRequest:
					message = MESSAGE_ERROR_400;
					break;
				case HttpStatusCode.NotFound:
					message = MESSAGE_ERROR_404;
					break;
				case HttpStatusCode.ExpectationFailed:
					message = MESSAGE_ERROR_417;
					break;
				case HttpStatusCode.Forbidden:
					message = MESSAGE_ERROR_403;
					break;
				default:
					message = MESSAGE_ERROR_UNDOCUMENTED;
					break;
			}

			return  new WebError (status, message);
		}

		public HttpStatusCode Status => _mStatus;
	}
}

