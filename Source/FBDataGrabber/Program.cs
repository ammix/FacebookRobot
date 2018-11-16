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
			using (var writer = new StreamWriter(File.Open("emails_and_phones_UTF8.csv", FileMode.Append, FileAccess.Write), Encoding.UTF8))
			{
				var facebookGrabber = new FacebookGrabber("https://www.facebook.com/groups/256686171663733/requests/");

				facebookGrabber.FacebookLogin("lu.brendell@gmail.com", "{123456}");

				//var n = facebookGrabber.GetRequestsNumber();

				Thread.Sleep(2000);
				//for (var i = 1; i <= n; i++)
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
							Console.WriteLine("New member requests do not found. Tring to refresh.");
							facebookGrabber.RefreshPage();
							Thread.Sleep(60000);
						}
					}
					var data = facebookGrabber.GetEmailPhoneTextFromRequest(hyperlink.uid);

					//if (data == null)
					//{
					//    //workIndex++;
					//    continue;
					//}

					//writer.Write($"{data.Item1},{data.Item2},{data.Item3}");

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

					var text = $"{hyperlink.name},{hyperlink.hyperlink},{email},{phone},{data}";

					writer.Write(text);
					writer.WriteLine();
					writer.Flush();

					facebookGrabber.ConfirmRequest(hyperlink.uid);

					Console.WriteLine(text);
					//Console.ReadLine();
					Thread.Sleep(500);
				}
			}

            Console.WriteLine("Congrats!!!");

		    /*
            var requestNumberElement = RequestNumberElement(driver);
            var n = requestNumberElement.Text;



            //element.Click();
            //Console.ReadLine();

            Console.ReadLine();
            driver.Close();
            */
		}

		private static string FindEmail(string data)
		{
			var emailRegex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
			return emailRegex.Match(data).Value;
		}

		private static string FindPhone(string data)
		{
			var emailRegex = new Regex(@"\(?0\d{2}\)?(-? *\d){7}"); // @"\(?0\d{2}\)?-? *\d{3}-? *\d{2}-? *\d{2}";
			var phone = emailRegex.Match(data).Value;

			if (string.IsNullOrEmpty(phone))
				return null;

			var sb = new StringBuilder("38" + phone);
			sb = sb.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
			return sb.ToString();
		}
	}
}
