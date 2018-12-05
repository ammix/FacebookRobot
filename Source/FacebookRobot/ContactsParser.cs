using System.Text;
using System.Text.RegularExpressions;

namespace FacebookRobot
{
	static class ContactsParser
	{
		public static string FindEmail(string data)
		{
			if (data == null) return null; //TODO: check if this need or it works without this

			var emailRegex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
			return emailRegex.Match(data).Value;
		}

		public static string FindPhone(string data)
		{
			if (data == null) return null;

			var emailRegex = new Regex(@"\(?0(-? *\d){2}\)?(-? *\d){7}");
			var phone = emailRegex.Match(data).Value;

			if (string.IsNullOrEmpty(phone))
				return null;

			var sb = new StringBuilder("38" + phone);
			sb = sb.Replace(" ", "").Replace("-", "").Replace("(", "").Replace(")", "");
			return sb.ToString();
		}

		public static string AdjustContactsString(string email, string phone, string contactsString)
		{
			if (email != null && phone != null)
				return null;
			if (email == contactsString || phone == contactsString)
				return null;

			return contactsString;
		}
	}
}
