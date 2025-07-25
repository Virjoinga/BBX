namespace Constants
{
	public static class User
	{
		public enum DataKeys
		{
			CurrentLoadout = 0,
			isAdmin = 1,
			emailOptIn = 2,
			mailchimpId = 3
		}

		public const string VALID_DISPLAY_NAME_REGEX = "^(?=.{3,20}$)[a-zA-Z]+(?:[a-zA-Z0-9]+)?$";

		public const string INVALID_CHARACTER_REGEX = "[^a-zA-Z0-9]+";

		public const string DefaultLoadout = "{\"weaponId\": \"cannon\"}";

		public const string VALID_EMAIL_REGEX = "^[A-Z0-9._%+-]+@[A-Z0-9.-]+\\.[A-Z]{2,}$";
	}
}
