using System;
using System.Threading;
using FacebookRobot.Writers;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace FacebookRobot
{
	public class FacebookRobot
	{
		private readonly ChromeDriver chromeDriver;

		public FacebookRobot(string chromeOptions, string groupId)
		{
			ChromeOptions options = new ChromeOptions();
			options.AddArguments(chromeOptions);
			//options.AddArguments("--disable-notifications --headless --disable-gpu"); // to disable notification
			//options.AddArguments("--disable-notifications"); // to disable notification
			chromeDriver = new ChromeDriver(options) {Url = $"https://www.facebook.com/groups/{groupId}/requests/"};

			//chromeDriver.Manage().Window.Maximize();
		}

		public void FacebookLogin(string email, string pass)
		{
			var emailElement = chromeDriver.FindElement(By.XPath("//*[@id='email']")); //TODO: use shot way
			emailElement.SendKeys(email);

			var passElement = chromeDriver.FindElement(By.XPath("//*[@id='pass']"));
			passElement.SendKeys(pass);

			var loginElement = chromeDriver.FindElement(By.XPath("//*[@id='loginbutton']"));
			loginElement.Submit();
			//TODO: fail if password is invalid

			Thread.Sleep(4000); //TODO: adjust
		}

		public void RefreshPage()
		{
			while (true)
			{
				try
				{
					chromeDriver.Navigate().Refresh();
					return;
				}
				catch (Exception e) //TODO: look in Far
				{
					Console.WriteLine(e.StackTrace);
					Thread.Sleep(2000); //TODO: adjust
				}
			}
		}

		public Hyperlink GetNewMember()
		{
			while (true)
			{
				try
				{
					var nameElement = chromeDriver.FindElementByCssSelector("li[data-testid] a[uid][href]:not(.img)");
					return new Hyperlink(nameElement);
				}
				catch (NoSuchElementException)
				{
					return null;
				}
				catch (WebDriverException)
				{
					RefreshPage();
					Thread.Sleep(4000);  //TODO: Adjust
				}
			}
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
			var webElement = chromeDriver.FindElementByCssSelector("a[aria-label='Закрити вкладку'][href='#']"); //TODO: investigate
			webElement.Click();
		}

		public void Close()
		{
			chromeDriver.Close();
		}

		public void ProcessNewMemberRequests(IWriter[] writers, int betweenClicksDelay, int refreshDelay)
		{
			Hyperlink newMember = null;
			Hyperlink previousMember = new Hyperlink();
			while (true)
			{
				while (true)
				{
					newMember = GetNewMember();
					if (newMember != null)
					{
						if (newMember.Uid == previousMember.Uid)
						{
							RefreshPage();
							Thread.Sleep(4000);  //TODO: adjust
							continue;
						}

						previousMember = newMember;
						break;
					}
					else
					{
						Thread.Sleep(refreshDelay); //TODO: move into separeate function
						RefreshPage();
						Thread.Sleep(4000);  //TODO: adjust
					}
				}

				var memberContacts = GetNewMemberContacts(newMember.Uid);
				SaveNewMemberContacts(writers, newMember, memberContacts);

				while (true)
				{
					try
					{
						PushConfirmButton(newMember.Uid);
						break;
					}
					catch (WebDriverException)
					{
						CloseMessanger();
					}
				}

				Thread.Sleep(betweenClicksDelay);
			}
		}

		private static void SaveNewMemberContacts(IWriter[] writers, Hyperlink hyperlink, string contacts)
		{
			var email = ContactsParser.FindEmail(contacts);
			var phone = ContactsParser.FindPhone(contacts);
			var adjustedData = ContactsParser.AdjustContactsString(email, phone, contacts);

			foreach (var writer in writers)
				writer.Write(hyperlink.Name, hyperlink.Link, email, phone, hyperlink.Uid, adjustedData);
		}

		public void PushConfirmButton(string uid)
		{
			var webElement = chromeDriver.FindElementByCssSelector($"[data-testid='{uid}'] button[name='approve']");
			webElement.Click();
		}
	}

	// TODO: struct vs class ?
	public class Hyperlink
	{
		public string Name;
		public string Link;
		public string Uid;

		public Hyperlink()
		{ }

		public Hyperlink(IWebElement nameElement)
		{
			Name = nameElement.Text;
			Link = nameElement.GetAttribute("href");
			Uid = nameElement.GetAttribute("uid");
		}
	}
}
