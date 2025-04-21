using ForestNET.Lib;

namespace ForestNET.Lib.Net.Request
{
    /// <summary>
    /// All possible types for http(s) post actions.<br />
    /// HTMLATTACHMENTS - multipart/form-data<br />
    /// HTML - application/x-www-form-urlencoded<br />
    /// JSON - application/json
    /// </summary>
    public enum PostType
    {
        [Value("multipart/form-data")]
        HTMLATTACHMENTS = 0,
        [Value("application/x-www-form-urlencoded")]
        HTML = 1,
        [Value("application/json")]
        JSON = 2
    }
}
