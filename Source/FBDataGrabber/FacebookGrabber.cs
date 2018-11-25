using System;
using System.Threading;

using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace FacebookDataGrabber
{
	public class Hyperlink
	{
		public string name;
		public string hyperlink;
		public string uid;

		public Hyperlink(string name, string hyperlink, string uid)
		{
			this.name = name;
			this.hyperlink = hyperlink;
			this.uid = uid;
		}
	}

    public class FacebookGrabber: IDisposable
	{
        private ChromeDriver chromeDriver;

		public void Dispose()
		{
			chromeDriver.Close();
		}

        public FacebookGrabber(string url)
        {
			/*
			Map<String, Object> prefs = new HashMap<String, Object>();
			prefs.put("profile.default_content_setting_values.notifications", 2);
			ChromeOptions options = new ChromeOptions();
			options.setExperimentalOption("prefs", prefs);
			WebDriver driver = new ChromeDriver(options);
			 */

			//Dictionary<string, object> hash = new Dictionary<string, object>
			//      {
			//       {"profile.default_content_setting_values.notifications", 2}
			//      };
			//      ChromeOptions op = new ChromeOptions();
			//      op.AddAdditionalCapability("prefs", hash);

	        ChromeOptions options = new ChromeOptions();
			options.AddArguments("--disable-notifications"); // to disable notification
			chromeDriver = new ChromeDriver(options) {Url = url};
	        chromeDriver.Manage().Window.Maximize();
		}

        public void FacebookLogin(string email, string pass)
        {
            var emailElement = chromeDriver.FindElement(By.XPath("//*[@id='email']"));
            emailElement.SendKeys(email);

            var passElement = chromeDriver.FindElement(By.XPath("//*[@id='pass']"));
            passElement.SendKeys(pass);

            var loginElement = chromeDriver.FindElement(By.XPath("//*[@id='loginbutton']"));
            loginElement.Submit();
        }

		public void RefreshPage()
		{
			chromeDriver.Navigate().Refresh();
		}

		public Hyperlink GetName()
		{
			//var nameElement = chromeDriver.FindElement(By.ClassName("_z_3"));
			var nameElement = chromeDriver.FindElementByCssSelector("li[data-testid] a[uid][href]:not(.img)");

			var name = nameElement.Text;
			var href = nameElement.GetAttribute("href");
			var uid = nameElement.GetAttribute("uid");

			return new Hyperlink(name, href, uid);

			//var nameElement = RequestNumberElement("//*[@id='u_0_1q']/div/div/div[2]/div[2]/ul/li[1]/div/div[2]/div/div/div[2]");
			//var nameElement = RequestNumberElement("//*[@id='u_0_1q']/div/div/div[2]/div[2]/ul");
		}

		public int GetRequestsNumber()
        {
			Thread.Sleep(3000);                    //*[@id="u_0_1q"]/div/div/div[2]/div[1]/div/div[1]/div/span
            var webElement = RequestNumberElement("//*[@id='u_0_1o']/div/div/div[2]/div[1]/div/div[1]/div/span");
            return int.Parse(webElement.Text.Split(' ')[0]);
        }

        public void ConfirmRequest(string uid)
        {
                                                   //*[@id="u_0_1q"]/div/div/div[2]/div[2]/ul/li[1]/div/div[2]/div/div/div[1]/button[1]
            //var webElement = RequestNumberElement("//*[@id='u_0_1o']/div/div/div[2]/div[2]/ul/li[1]/div/div[2]/div/div/div[1]/button[1]");

			var webElement = chromeDriver.FindElementByCssSelector($"[data-testid='{uid}'] button[name='approve']");
            webElement.Click();

            //var acceptButton = driver.FindElement(By.XPath("//*[@id='u_0_1q']/div/div/div[2]/div[2]/ul/li[1]/div/div[2]/div/div/div[1]/button[1]"));
            //acceptButton.Click();                           //*[@id="u_0_1q"]/div/div/div[2]/div[2]/ul/li[2]/div/div[2]/div/div/div[1]/button[1]
        }

        public /*Tuple<string, string, string>*/string GetEmailPhoneTextFromRequest(string uid)
        {
            //int requestNumber = 1;

            IWebElement webElement;
            try
            {                                       //*[@id="u_0_1q"]/div/div/div[2]/div[2]/ul/li[1]/div/div[2]/div/div/div[4]/ul/li/text
													//webElement = RequestNumberElement($"//*[@id='u_0_1o']/div/div/div[2]/div[2]/ul/li[{requestNumber}]/div/div[2]/div/div/div[4]/ul/li/text");

	            webElement = chromeDriver.FindElement(By.CssSelector($"[data-testid='{uid}'] text[truncate]"));
			}
            catch (NoSuchElementException)
            {
                return null;
            }

            //var text = webElement.Text.ToLower();
            var text = webElement.Text;

            /*
            var fields = new List<string>(text.Split(' '));
            var email = fields.Find(x => x.Contains('@'));
            var phone = fields.Find(x => x.Contains('0'));

            phone = phone?.Replace("-", "").Replace(" ", "").Replace("+", "");

            return new Tuple<string, string, string>(email, phone, text);
            */
            return text;


            //webElement.Text = "Добрый день olga_savch@meta.ua 050-550-99-93"
            //var email = webElement.Text.Split(' ')[0];
            //var phone = webElement.Text.Split(' ')[1];

            //return new Tuple<string, string>(email, phone);

            //var emailAndPhoneElement = driver.FindElement(By.XPath("//*[@id='u_0_1q']/div/div/div[2]/div[2]/ul/li[1]/div/div[2]/div/div/div[4]/ul/li/text"));
            //var emailAndPhone = emailAndPhoneElement.Text;          //*[@id="u_0_1q"]/div/div/div[2]/div[2]/ul/li[2]/div/div[2]/div/div/div[4]/ul/li/text
        }

        IWebElement RequestNumberElement(string elementXpath)
        {
            const int n = 1; //times
            const int waitTime = 1000; //ms

            IWebElement webElement = null;
            var done = false;
            for (var i = 1; i <= n && !done; i++)
            {
                try
                {
                    webElement = chromeDriver.FindElement(By.XPath(elementXpath));
                    done = true;
                }
                catch (NoSuchElementException)
                {
                    if (i == n) throw;

                    Thread.Sleep(waitTime);
                    done = false;
                }
            }

            return webElement;
        }
    }
}
