namespace ForestNET.Lib.Net.Https.Dynm
{
    /// <summary>
    /// Encapsulation of Cookie data with all possible settings.
    /// </summary>
    public class Cookie
    {

        /* Fields */

        /* Properties */

        public string CookieUUID { get; set; }
        public ForestNET.Lib.DateInterval? MaxAge { get; set; }
        public string? Domain { get; set; }
        public string? Path { get; set; }
        public bool Secure { get; set; }
        public bool HttpOnly { get; set; }
        public CookieSameSite? SameSite { get; set; }
        public string? Key { get; set; }
        public string? Value { get; set; }
        public long MaxAgeAsLong { get; set; }
        public string? Expires { get; set; }
        private Dictionary<string, string?> Cookies { get; set; }

        /* Methods */

        /// <summary>
        /// Cookie constructor, auto generating UUID if parameter is null or empty
        /// </summary>
        /// <param name="p_o_maxAge">cookie max age in seconds, stored as date interval</param>
        /// <param name="p_s_domain">cookie domain setting</param>
        public Cookie(ForestNET.Lib.DateInterval? p_o_maxAge, string? p_s_domain)
        {
            this.CookieUUID = ForestNET.Lib.Helper.GenerateUUID();
            this.MaxAge = p_o_maxAge;
            this.Domain = p_s_domain;

            this.SameSite = null;
            this.Key = null;
            this.Value = null;
            this.MaxAgeAsLong = -1;
            this.Expires = null;

            this.Cookies = [];
        }

        /// <summary>
        /// Cookie constructor
        /// </summary>
        /// <param name="p_s_cookieUUID">cookie UUID for clear identification</param>
        /// <param name="p_o_maxAge">cookie max age in seconds, stored as date interval</param>
        /// <param name="p_s_domain">cookie domain setting</param>
        public Cookie(string p_s_cookieUUID, ForestNET.Lib.DateInterval? p_o_maxAge, string? p_s_domain)
        {
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_cookieUUID))
            {
                this.CookieUUID = ForestNET.Lib.Helper.GenerateUUID();
            }
            else
            {
                this.CookieUUID = p_s_cookieUUID;
            }

            this.MaxAge = p_o_maxAge;
            this.Domain = p_s_domain;

            this.SameSite = null;
            this.Key = null;
            this.Value = null;
            this.MaxAgeAsLong = -1;
            this.Expires = null;

            this.Cookies = [];
        }

        /// <summary>
        /// add cookie key value pair
        /// </summary>
        /// <param name="p_s_key">key parameter of a cookie</param>
        /// <param name="p_s_value">value parameter of a cookie</param>
        /// <exception cref="ArgumentNullException">key parameter is null or empty</exception>
        public void AddHTTPCookie(string p_s_key, string? p_s_value)
        {
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_key))
            {
                throw new ArgumentNullException(nameof(p_s_key), "Key parameter is null or empty");
            }

            this.Cookies.Add(p_s_key, p_s_value);
        }

        /// <summary>
        /// create a http valid cookie line for response header
        /// </summary>
        public override string ToString()
        {
            return this.ToString(false);
        }

        /// <summary>
        /// create a http valid cookie line for response header, server side only
        /// </summary>
        /// <param name="p_b_received">print received cookie line of a response header with additional information like Max-Age, Expires and all cookie key-value-pairs</param>
        public string ToString(bool p_b_received)
        {
            string s_foo = "";

            s_foo += "Set-Cookie: forestAny-UUID=" + this.CookieUUID;

            if (this.MaxAge != null)
            {
                s_foo += "; Max-Age=" + this.MaxAge.ToDurationInSeconds();
            }

            if (!ForestNET.Lib.Helper.IsStringEmpty(this.Domain))
            {
                s_foo += "; Domain=" + this.Domain;
            }

            if (!ForestNET.Lib.Helper.IsStringEmpty(this.Path))
            {
                s_foo += "; Path=" + this.Path;
            }
            else
            {
                s_foo += "; Path=/";
            }

            if (this.Secure)
            {
                s_foo += "; Secure";
            }

            if (this.HttpOnly)
            {
                s_foo += "; HttpOnly";
            }

            if (this.SameSite != null)
            {
                if (this.SameSite == CookieSameSite.NONE)
                {
                    s_foo += "; SameSite=None";
                }
                else if (this.SameSite == CookieSameSite.STRICT)
                {
                    s_foo += "; SameSite=Strict";
                }
                else if (this.SameSite == CookieSameSite.LAX)
                {
                    s_foo += "; SameSite=Lax";
                }

                if ((this.SameSite == CookieSameSite.NONE) && (!this.Secure))
                {
                    s_foo += "; Secure";
                }
            }

            if (p_b_received)
            {
                s_foo += "; Max-Age(long)=" + MaxAge;
                s_foo += "; Expires=" + Expires;

                foreach (KeyValuePair<string, string?> o_cookie in this.Cookies)
                {
                    s_foo += "; " + o_cookie.Key + "=" + o_cookie.Value;
                }
            }

            return s_foo;
        }

        /// <summary>
        /// create a http valid cookie line for request header, client side only
        /// </summary>
        public string ClientCookieToString()
        {
            string s_foo = "";

            bool b_first = false;

            foreach (KeyValuePair<string, string?> o_cookie in this.Cookies)
            {
                if (!b_first)
                {
                    s_foo += o_cookie.Key + "=" + o_cookie.Value;
                    b_first = true;
                }
                else
                {
                    s_foo += ";" + o_cookie.Key + "=" + o_cookie.Value;
                }
            }

            if (this.MaxAge != null)
            {
                s_foo += "; Max-Age=" + this.MaxAge.ToDurationInSeconds();
            }

            if (!ForestNET.Lib.Helper.IsStringEmpty(this.Domain))
            {
                s_foo += "; Domain=" + this.Domain;
            }

            if (!ForestNET.Lib.Helper.IsStringEmpty(this.Path))
            {
                s_foo += "; Path=" + this.Path;
            }
            else
            {
                s_foo += "; Path=/";
            }

            if (this.Secure)
            {
                s_foo += "; Secure";
            }

            if (this.HttpOnly)
            {
                s_foo += "; HttpOnly";
            }

            if (this.SameSite != null)
            {
                if (this.SameSite == CookieSameSite.NONE)
                {
                    s_foo += "; SameSite=None";
                }
                else if (this.SameSite == CookieSameSite.STRICT)
                {
                    s_foo += "; SameSite=Strict";
                }
                else if (this.SameSite == CookieSameSite.LAX)
                {
                    s_foo += "; SameSite=Lax";
                }

                if ((this.SameSite == CookieSameSite.NONE) && (!this.Secure))
                {
                    s_foo += "; Secure";
                }
            }

            return s_foo;
        }
    }
}