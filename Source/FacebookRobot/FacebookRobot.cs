using System.Threading;
using FacebookRobot.Writers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace FacebookRobot
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

			Thread.Sleep(4000); //TODO: adjust
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

		private void CloseMessanger()
		{
			var webElement = chromeDriver.FindElementByCssSelector("a[aria-label='Закрити вкладку'][href='#']");
			webElement.Click();
		}

		public void Close()
		{
			chromeDriver.Close();
		}

		public void ProcessNewMemberRequests(IWriter[] writers, int delay)
		{
			while (true)
			{
				Hyperlink facebookName = null;
				while (true)
				{
					facebookName = GetNewMemberName();
					if (facebookName != null)
					{
						break;
					}
					else
					{
						Thread.Sleep(300000); //5 min
						RefreshPage();
						Thread.Sleep(4000);  //TODO: adjust
					}
				}

				var memberContacts = GetNewMemberContacts(facebookName.Uid);
				SaveNewMemberContacts(writers, facebookName, memberContacts);

				while (true)
				{
					try
					{
						ConfirmRequest(facebookName.Uid);
						break;
					}
					catch (WebDriverException)
					{
						CloseMessanger();
					}
				}

				Thread.Sleep(delay);
			}
		}

		private static void SaveNewMemberContacts(IWriter[] writers, Hyperlink hyperlink, string contacts)
		{
			var email = ContactsParser.FindEmail(contacts);
			var phone = ContactsParser.FindPhone(contacts);
			var adjustedData = ContactsParser.AdjustContactsString(email, phone, contacts);

			foreach (var writer in writers)
				writer.Write(hyperlink.Name, hyperlink.Link, email, phone, adjustedData);
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
