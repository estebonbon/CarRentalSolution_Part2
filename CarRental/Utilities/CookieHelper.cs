namespace CarRental.Utilities
{
    public static class CookieHelper
    {
        /// <summary>
        /// set the cookie
        /// </summary>
        /// <param name="_context">the HttpContext</param>
        /// <param name="key">key (unique identifier)</param>
        /// <param name="value">value to store in cookie object</param>
        /// <param name="expireTime">expiration time</param>
        public static void CookieSet(
            HttpContext _context,
            string key,
            string value,
            int? expireTime)
        {
            CookieOptions option = new CookieOptions();

            if (expireTime.HasValue)
                option.Expires = DateTime.Now.AddMinutes(expireTime.Value);
            else
                option.Expires = DateTime.Now.AddMilliseconds(10);

            _context.Response.Cookies.Append(key, value, option);
        }
    }
}

/// <summary>
/// Saves information in the user's browser so the
/// website can remember their preferences.
/// </summary>

/// <param name="_context">
/// HttpContext = Connects to the user's browser request and response.
/// </param>

/// <param name="key">
/// key = The name of the cookie, e.g., "PageSize".
/// </param>

/// <param name="value">
/// value = The information being saved, e.g., "10".
/// </param>

/// <param name="expireTime">
/// expireTime = How many minutes before the cookie expires.
/// </param>