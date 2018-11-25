using OpenQA.Selenium;
using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace FacebookDataGrabber
{
	class Program
	{
		static void Main(string[] args)
		{
			Console.WriteLine("New version");
			//var spreadSheetsWriter = new GoogleSpreadSheetsWriter("1mBYYx3WFGayXH9BjVKS6CGLSUq9RYsHOeaLQC33qf88");
			var spreadSheetsWriter = new GoogleSpreadSheetsWriter("14OYRfQ2-qdaqUvDOsdz08mwlSEyNZUeoYE0KRV1CAow");

			using (var writer = File.AppendText("emails_and_phones.csv"))
			{
				//var facebookGrabber = new FacebookGrabber("https://www.facebook.com/groups/256686171663733/requests/"); //U.Pak
				var facebookGrabber = new FacebookGrabber("https://www.facebook.com/groups/736242360081825/requests/"); //T.Dugelna

				facebookGrabber.FacebookLogin("lu.brendell@gmail.com", "{123456}");

				Thread.Sleep(2000);

				Hyperlink hyperlink = null;
				while (true)
				{
					bool doit = true;
					while (doit)
					{
						try
						{
							hyperlink = facebookGrabber.GetName();
							doit = false;
						}
						catch (NoSuchElementException)
						{
							Console.WriteLine("New member requests do not found. Trying to refresh.");
							Thread.Sleep(30000);
							facebookGrabber.RefreshPage();
						}
					}
					var data = facebookGrabber.GetEmailPhoneTextFromRequest(hyperlink.uid);

					string email = null;
					string phone = null;

					if (data != null)
					{
						email = FindEmail(data);
						phone = FindPhone(data);

						if (email != null && phone != null)
							data = null;
						if (email == data || phone == data)
							data = null;
					}

					spreadSheetsWriter.Write(hyperlink.name, hyperlink.hyperlink, email, phone, data);
					
					var text = $"{hyperlink.name},{hyperlink.hyperlink},{email},{phone},{data}";

					writer.Write(text);
					writer.WriteLine();
					writer.Flush();

					facebookGrabber.ConfirmRequest(hyperlink.uid);

					Console.WriteLine(text);
					Thread.Sleep(500);
				}
				facebookGrabber.Dispose();
			}

			Console.WriteLine("Congrats!!!");
		}

		private static string FindEmail(string data)
		{
			var emailRegex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
			return emailRegex.Match(data).Value;
		}

		private static string FindPhone(string data)
		{
			var emailRegex = new Regex(@"\(?0(-? *\d){2}\)?(-? *\d){7}");
			var phone = emailRegex.Match(data).Value;

			if (string.IsNullOrEmpty(phone))
				return null;

			var sb = new StringBuilder("38" + phone);
			sb = sb.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
			return sb.ToString();
		}
	}
}
