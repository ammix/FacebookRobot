using System;
using System.Threading;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace FacebookDataGrabber
{
	public class FacebookRobot
	{
		private readonly ChromeDriver chromeDriver;

		public FacebookRobot(string groupId)
		{
			ChromeOptions options = new ChromeOptions();
			options.AddArguments("--disable-notifications"); // to disable notification
			chromeDriver = new ChromeDriver(options) {Url = $"https://www.facebook.com/groups/{groupId}/requests/"};
			chromeDriver.Manage().Window.Maximize();
		}

		public void FacebookLogin(string email, string pass)
		{
			var emailElement = chromeDriver.FindElement(By.XPath("//*[@id='email']")); //TODO: use shot way
			emailElement.SendKeys(email);

			var passElement = chromeDriver.FindElement(By.XPath("//*[@id='pass']"));
			passElement.SendKeys(pass);

			var loginElement = chromeDriver.FindElement(By.XPath("//*[@id='loginbutton']"));
			loginElement.Submit();

			Thread.Sleep(2000); //TODO: adjust
		}

		public void RefreshPage()
		{
			chromeDriver.Navigate().Refresh();
		}

		public Hyperlink GetNewMemberName()
		{
			IWebElement nameElement;
			try
			{
				nameElement = chromeDriver.FindElementByCssSelector("li[data-testid] a[uid][href]:not(.img)");
			}
			catch (NoSuchElementException)
			{
				return null;
			}

			return new Hyperlink(nameElement);
		}

		public string GetNewMemberContacts(string uid)
		{
			IWebElement webElement;
			try
			{
				webElement = chromeDriver.FindElement(By.CssSelector($"[data-testid='{uid}'] text[truncate]"));
			}
			catch (NoSuchElementException)
			{
				return null;
			}

			var text = webElement.Text;
			return text;
		}

		public void Close()
		{
			chromeDriver.Close();
		}

		public void SaveNewMemberContactsAndAddToGroup(IWriter writer, int delay)
		{
			while (true)
			{
				var facebookName = GetNewMemberName();
				if (facebookName == null)
					break;

				var memberContacts = GetNewMemberContacts(facebookName.Uid);
				SaveMemberNameAndContacts(writer, facebookName, memberContacts);

				ConfirmRequest(facebookName.Uid);

				Thread.Sleep(delay);
			}
		}

		private static void SaveMemberNameAndContacts(IWriter writer, Hyperlink hyperlink, string contacts)
		{
			var email = ContactsParser.FindEmail(contacts);
			var phone = ContactsParser.FindPhone(contacts);
			var adjustedData = ContactsParser.AdjustContactsString(email, phone, contacts);

			writer.Write(hyperlink.Name, hyperlink.Link, email, phone, adjustedData);
			var text = $"{hyperlink.Name},{hyperlink.Link},{email},{phone},{adjustedData}";
			Console.WriteLine(text);
		}

		public void ConfirmRequest(string uid)
		{
			var webElement = chromeDriver.FindElementByCssSelector($"[data-testid='{uid}'] button[name='approve']");
			webElement.Click();
		}
	}

	public class Hyperlink
	{
		public string Name;
		public string Link;
		public string Uid;

		public Hyperlink(IWebElement nameElement)
		{
			Name = nameElement.Text;
			Link = nameElement.GetAttribute("href");
			Uid = nameElement.GetAttribute("uid");
		}
	}
}
