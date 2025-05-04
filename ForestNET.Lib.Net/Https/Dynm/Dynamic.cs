namespace ForestNET.Lib.Net.Https.Dynm
{
    /// <summary>
    /// Class which uses ForestSeed implementations to create dynamic content. Methods to handle HTTP POST, PUT or DELETE requests and their body data with boundary etc.
    /// </summary>
    public class Dynamic
    {

        /* Fields */

        private string s_lineBreak = ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;

        /* Properties */

        private ForestNET.Lib.Net.Https.Seed Seed { get; set; }
        private List<string> IncludeHashes { get; set; }

        /* Methods */

        /// <summary>
        /// Dynamic constructor with seed instance object as parameter, so anything within handling dynamic content has access to all request/response/other resources
        /// </summary>
        /// <param name="p_o_value">seed instance object</param>
        public Dynamic(ForestNET.Lib.Net.Https.Seed p_o_seed)
        {
            this.Seed = p_o_seed;
            this.IncludeHashes = [];
        }

        /// <summary>
        /// handling a general POST, PUT or DELETE request with post and file data
        /// </summary>
        /// <returns>HTTP status code as integer value, in case for invalid request</returns>
        public int HandlePostRequest()
        {
            if (
                (this.Seed.RequestHeader.ContentType.Equals(ForestNET.Lib.Net.Request.PostType.HTMLATTACHMENTS.ToString()))
                ||
                (
                    (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.REST)
                    &&
                    (!ForestNET.Lib.Helper.IsStringEmpty(this.Seed.RequestHeader.Boundary))
                )
            )
            {
                /* post type 'multipart/form-data' or mode is REST and boundary value from request header content type is not empty */
                return this.HandlePostFileRequest();
            }
            else if (
                (this.Seed.RequestHeader.ContentType.Equals(ForestNET.Lib.Net.Request.PostType.HTML.ToString()))
                ||
                (
                    (this.Seed.Config.Mode == ForestNET.Lib.Net.Https.Mode.REST)
                    &&
                    (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.RequestHeader.Boundary))
                )
            )
            {
                /* post type 'application/x-www-form-urlencoded' or mode is REST and boundary value from request header content type is empty */

                /* get request body with configuration incoming encoding and split all post values by '&' */
                string[] a_foo = (this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8).GetString(this.Seed.RequestBody ?? []).Split("&");

                /* iterate each post key value pair */
                foreach (string s_foo in a_foo)
                {
                    /* split key and value by '=' */
                    string[] a_foo2 = s_foo.Split("=");

                    if (a_foo2.Length == 2)
                    { /* handle key and value */
                        /* add key and value and decode both with incoming encoding configuration */
                        this.Seed.PostData.Add(
                            System.Web.HttpUtility.UrlDecode(a_foo2[0], this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8),
                            System.Web.HttpUtility.UrlDecode(a_foo2[1], this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8)
                        );
                    }
                    else if (a_foo2.Length == 1)
                    { /* handle key only */
                        /* add key only and decode both with incoming encoding configuration */
                        this.Seed.PostData.Add(
                            System.Web.HttpUtility.UrlDecode(a_foo2[0], this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8),
                            null
                        );
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: HTTP POST key value pair does not contain a '=' -> '" + s_foo + "'");
                        return 400;
                    }
                }
            }
            else
            {
                ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "501 Not Implemented: POST request with content type '" + this.Seed.RequestHeader.ContentType + "' is not implemented");
                return 501;
            }

            /* post data has been processed successfully, returning 200 */
            return 200;
        }

        /// <summary>
        /// handling a general POST, PUT or DELETE request with post and file data and boundary structure
        /// </summary>
        /// <returns>HTTP status code as integer value, in case for invalid request</returns>
        private int HandlePostFileRequest()
        {
            /* check if boundary value in request header is not empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.RequestHeader.Boundary))
            {
                ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: HTTP POST request with content type '" + ForestNET.Lib.Net.Request.PostType.HTMLATTACHMENTS + "' has no boundary for post data separation");
                return 400;
            }

            /* check if request body is not empty */
            if (this.Seed.RequestBody == null)
            {
                ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: HTTP POST request body is empty");
                return 400;
            }

            /* recognize line break from body by reading byte after first boundary, it is "\n" or '\r\n' */
            this.RecognizeLineBreakAfterFirstBoundary(this.Seed.RequestBody, this.Seed.Config.InEncoding, this.Seed.RequestHeader.Boundary);

            int i_pointer = -10;
            int i_lastPointer = -1;

            /* iterate all boundaries within request body data */
            while ((i_pointer = Dynamic.GetNextPostDataBoundary(this.Seed.RequestBody, this.Seed.Config.InEncoding, i_pointer, this.Seed.RequestHeader.Boundary, this.s_lineBreak)) >= 0)
            {
                /* continue only if we have found a new boundary which is not at the beginning of the request body */
                if (i_pointer > 0)
                {
                    /* look for next line break */
                    int i_nextLineBreak = Dynamic.GetNextLineBreak(this.Seed.RequestBody, this.Seed.Config.InEncoding, i_lastPointer, false, this.s_lineBreak);

                    /* check if next line break is not at the current position */
                    if (i_nextLineBreak > i_pointer)
                    {
                        ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: HTTP POST request with file data; next line break is outside of post data");
                        return 400;
                    }

                    /* read next line */
                    string s_expectedContentDisposition = (this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8).GetString(
                        Dynamic.ReadBytePartOfPostData(
                            this.Seed.RequestBody,
                            i_lastPointer,
                            i_nextLineBreak
                        )
                    );

                    /* check if line is expected content disposition line with field name and file name */
                    if (ForestNET.Lib.Helper.MatchesRegex(s_expectedContentDisposition, "Content-Disposition: form-data; name=\".*\"; filename=\".*\""))
                    {
                        /* read field name and file name out of content disposition line */
                        System.Text.RegularExpressions.Regex o_regex = new("(Content-Disposition: form-data; name=\")(.*)(\"; filename=\")(.*)(\")");
                        System.Text.RegularExpressions.Match o_matcher = o_regex.Match(s_expectedContentDisposition);
                        string s_fieldName;
                        string s_fileName;

                        /* store values in variables or return a bad request if they are not found */
                        if ((o_matcher.Success) && (o_matcher.Groups.Count == 6))
                        {
                            s_fieldName = o_matcher.Groups[2].Value;
                            s_fileName = o_matcher.Groups[4].Value;
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: HTTP POST request with file data with invalid content disposition: 'name' and 'filename'");
                            return 400;
                        }

                        /* read next line */
                        string s_expectedContentType = (this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8).GetString(
                            Dynamic.ReadBytePartOfPostData(
                                this.Seed.RequestBody,
                                i_nextLineBreak + this.s_lineBreak.Length,
                                Dynamic.GetNextLineBreak(
                                    this.Seed.RequestBody,
                                    this.Seed.Config.InEncoding,
                                    i_nextLineBreak + this.s_lineBreak.Length,
                                    false,
                                    this.s_lineBreak
                                )
                            )
                        );

                        /* check if line is expected content type line */
                        if (!ForestNET.Lib.Helper.MatchesRegex(s_expectedContentType, "Content-Type:\\s?.*"))
                        {
                            ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: HTTP POST request with file data with invalid content type '" + s_expectedContentType + "'");
                            return 400;
                        }

                        /* pull content type value out of content type line */
                        s_expectedContentType = s_expectedContentType.Substring(13).Trim();

                        /* look for upcoming two line breaks */
                        int i_nextTwoLineBreaks = Dynamic.GetNextLineBreak(this.Seed.RequestBody, this.Seed.Config.InEncoding, i_lastPointer, true, this.s_lineBreak);

                        /* check if two line breaks are not at the current position */
                        if (i_nextLineBreak > i_pointer)
                        {
                            ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: HTTP POST request with file data, could not find two following line breaks");
                            return 400;
                        }

                        /* store all file data from current position to upcoming two line breaks */
                        byte[] a_fileData = Dynamic.ReadBytePartOfPostData(
                            this.Seed.RequestBody,
                            i_nextTwoLineBreaks + this.s_lineBreak.Length + this.s_lineBreak.Length,
                            i_pointer
                        );

                        /* check if content type is not forbidden, if we have read file data from request body and that the file name is not empty */
                        if (
                            (!ForestNET.Lib.Net.Https.Config.KNOWN_EXTENSION_LIST.ContainsValue(s_expectedContentType)) &&
                            (a_fileData.Length >= 0) && (!ForestNET.Lib.Helper.IsStringEmpty(s_fileName))
                        )
                        {
                            ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "403 Forbidden: HTTP POST request with file data has unkown or not allowed content type '" + s_expectedContentType + "'");
                            return 403;
                        }

                        /* add read file data to seed file data list if we have read file data and that the file name is not empty */
                        if ((a_fileData.Length >= 0) && (!ForestNET.Lib.Helper.IsStringEmpty(s_fileName)))
                        {
                            this.Seed.FileData.Add(new FileData(s_fieldName, s_fileName, s_expectedContentType, a_fileData));
                        }
                    }
                    else if (ForestNET.Lib.Helper.MatchesRegex(s_expectedContentDisposition, "Content-Disposition: form-data; name=\".*\""))
                    { /* check if line is expected content disposition line with field name only */
                        /* read field name out of content disposition line */
                        System.Text.RegularExpressions.Regex o_regex = new("(Content-Disposition: form-data; name=\")(.*)(\")");
                        System.Text.RegularExpressions.Match o_matcher = o_regex.Match(s_expectedContentDisposition);
                        string s_fieldName;

                        /* store field name in variable or return a bad request if it is not found or empty */
                        if ((o_matcher.Success) && (!ForestNET.Lib.Helper.IsStringEmpty(o_matcher.Groups[2].Value)))
                        {
                            s_fieldName = o_matcher.Groups[2].Value;
                        }
                        else
                        {
                            ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: HTTP POST request with file data with invalid content disposition: 'name'");
                            return 400;
                        }

                        /* look for upcoming two line breaks */
                        int i_nextTwoLineBreaks = Dynamic.GetNextLineBreak(this.Seed.RequestBody, this.Seed.Config.InEncoding, i_lastPointer, true, this.s_lineBreak);

                        /* check if two line breaks are not at the current position */
                        if (i_nextLineBreak > i_pointer)
                        {
                            ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: HTTP POST request with file data, could not find two following line breaks");
                            return 400;
                        }

                        /* read next line as field value */
                        string s_fieldValue = (this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8).GetString(
                            Dynamic.ReadBytePartOfPostData(
                                this.Seed.RequestBody,
                                i_nextTwoLineBreaks + this.s_lineBreak.Length + this.s_lineBreak.Length,
                                i_pointer)
                        );

                        /* add field data to seed post data list if we have read decoded field name and field value is not empty */
                        this.Seed.PostData.Add(
                            System.Web.HttpUtility.UrlDecode(s_fieldName, this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8),
                            (!ForestNET.Lib.Helper.IsStringEmpty(s_fieldValue)) ? System.Web.HttpUtility.UrlDecode(s_fieldValue, this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8) : null
                        );
                    }
                    else
                    {
                        ForestNET.Lib.Global.ILogWarning(this.Seed.Salt + " " + "400 Bad Request: HTTP POST request with file data, invalid content disposition '" + s_expectedContentDisposition + "'");
                        return 400;
                    }
                }

                /* +2 is the double hyphen */
                if (i_lastPointer < 0)
                {
                    /* set last pointer as current pointer + boundary value length + line break length + double hyphen length */
                    i_lastPointer = i_pointer + this.Seed.RequestHeader.Boundary.Length + this.s_lineBreak.Length + 2;
                }
                else
                {
                    /* set last pointer as current pointer + boundary value length + two line breaks length + double hyphen length */
                    i_lastPointer = i_pointer + this.Seed.RequestHeader.Boundary.Length + this.s_lineBreak.Length + this.s_lineBreak.Length + 2;
                }

                /* increase current pointer with boundary value length + two line breaks length + double hyphen length */
                i_pointer += this.Seed.RequestHeader.Boundary.Length + this.s_lineBreak.Length + this.s_lineBreak.Length + 2;
            }

            /* post data and file data have been processed successfully, returning 200 */
            return 200;
        }

        /// <summary>
        /// recognize line break from body by reading byte after first boundary, it is "\n" or '\r\n'
        /// </summary>
        /// <param name="p_a_bytes">post body data</param>
        /// <param name="p_o_encoding">encoding for reading bytes</param>
        /// <param name="p_s_boundary">looking for boundary value and the next one or two bytes after this</param>
        /// <exception cref="ArgumentNullException">could not find line break after first boundary value or post body data parameter is null or empty</exception>
        /// <exception cref="ArgumentException">invalid pointer value to get line break after first boundary value</exception>
        private void RecognizeLineBreakAfterFirstBoundary(byte[] p_a_bytes, System.Text.Encoding? p_o_encoding, string p_s_boundary)
        {
            string s_foo1;
            string s_foo2;
            string s_hyphen = System.Text.Encoding.UTF8.GetString((p_o_encoding ?? System.Text.Encoding.UTF8).GetBytes("--"));

            /* '--' + boundary + '\r\n' or '\n' */
            int i_lengthFirstBoundary = s_hyphen.Length + p_s_boundary.Length;

            int i_pointer = 0;

            do
            {
                /* read so many bytes until we found '--' + boundary + '\r\n' */
                s_foo1 = (p_o_encoding ?? System.Text.Encoding.UTF8).GetString(Dynamic.ReadBytePartOfPostData(p_a_bytes, i_pointer, i_pointer + i_lengthFirstBoundary + 2));
                /* read so many bytes until we found '--' + boundary + '\n' */
                s_foo2 = (p_o_encoding ?? System.Text.Encoding.UTF8).GetString(Dynamic.ReadBytePartOfPostData(p_a_bytes, i_pointer, i_pointer + i_lengthFirstBoundary + 1));

                if (s_foo1.Equals(s_hyphen + p_s_boundary + (p_o_encoding ?? System.Text.Encoding.UTF8).GetString(System.Text.Encoding.UTF8.GetBytes(ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK))))
                {
                    /* we found that line break after first boundary is '\r\n' */
                    this.s_lineBreak = (p_o_encoding ?? System.Text.Encoding.UTF8).GetString(System.Text.Encoding.UTF8.GetBytes(ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK));
                    break;
                }
                else if (s_foo2.Equals(s_hyphen + p_s_boundary + (p_o_encoding ?? System.Text.Encoding.UTF8).GetString(System.Text.Encoding.UTF8.GetBytes("\n"))))
                {
                    /* we found that line break after first boundary is "\n" */
                    this.s_lineBreak = (p_o_encoding ?? System.Text.Encoding.UTF8).GetString(System.Text.Encoding.UTF8.GetBytes("\n"));
                    break;
                }

                /* increase pointer for reading within post body data bytes */
                i_pointer++;
            } while (i_pointer < p_a_bytes.Length - i_lengthFirstBoundary + 2); /* until the end of post body data bytes - possible length of first boundary and '\r\n' */

            /* check if line break has been found */
            if ((this.s_lineBreak == null) || (this.s_lineBreak.Length < 1))
            {
                throw new ArgumentNullException("Could not find line break after first boundary '" + p_s_boundary + "' in post body data");
            }
        }

        /// <summary>
        /// get position of next boundary value within post body data bytes
        /// </summary>
        /// <param name="p_a_bytes">post body data bytes</param>
        /// <param name="p_o_encoding">encoding for reading bytes</param>
        /// <param name="p_i_pointer">current pointer position within post body data</param>
        /// <param name="p_s_boundary">boundary value</param>
        /// <param name="p_s_lineBreak">line break value</param>
        /// <exception cref="ArgumentNullException">post body data parameter is null or empty</exception>
        /// <exception cref="ArgumentException">invalid pointer value to get next boundary value</exception>
        private static int GetNextPostDataBoundary(byte[] p_a_bytes, System.Text.Encoding? p_o_encoding, int p_i_pointer, string p_s_boundary, string p_s_lineBreak)
        {
            string s_foo;
            string s_hyphen = System.Text.Encoding.UTF8.GetString((p_o_encoding ?? System.Text.Encoding.UTF8).GetBytes("--"));
            string s_lineBreak = System.Text.Encoding.UTF8.GetString((p_o_encoding ?? System.Text.Encoding.UTF8).GetBytes(p_s_lineBreak));

            /* '--' + boundary + '\r\n' */
            int i_lengthFirstBoundary = s_hyphen.Length + p_s_boundary.Length + s_lineBreak.Length;
            /* '\r\n' + '--' + boundary + '\r\n' */
            int i_lengthBoundary = s_lineBreak.Length + s_hyphen.Length + p_s_boundary.Length + s_lineBreak.Length;
            /* '\r\n' + '--' + boundary + '--' */
            int i_lengthLastBoundary = s_lineBreak.Length + s_hyphen.Length + p_s_boundary.Length + s_hyphen.Length;

            /* correct negative pointer value to zero */
            if (p_i_pointer < 0)
            {
                p_i_pointer = 0;
            }

            do
            {
                /* read so many bytes until we found '--' + boundary + '\r\n' or '\n' */
                s_foo = (p_o_encoding ?? System.Text.Encoding.UTF8).GetString(Dynamic.ReadBytePartOfPostData(p_a_bytes, p_i_pointer, p_i_pointer + i_lengthFirstBoundary));

                /* return current pointer if it matches '--' + boundary + '\r\n' or '\n' */
                if (s_foo.Equals(s_hyphen + p_s_boundary + s_lineBreak))
                {
                    return p_i_pointer;
                }

                /* check if we still have enough bytes left to read normal boundary */
                if (p_i_pointer <= p_a_bytes.Length - i_lengthBoundary)
                {
                    /* read so many bytes until we found '\r\n' or '\n' + '--' + boundary + '\r\n' or '\n' */
                    s_foo = (p_o_encoding ?? System.Text.Encoding.UTF8).GetString(Dynamic.ReadBytePartOfPostData(p_a_bytes, p_i_pointer, p_i_pointer + i_lengthBoundary));

                    /* return current pointer if it matches '\r\n' or '\n' + '--' + boundary + '\r\n' or '\n' */
                    if (s_foo.Equals(s_lineBreak + s_hyphen + p_s_boundary + s_lineBreak))
                    {
                        return p_i_pointer;
                    }
                }

                /* check if we still have enough bytes left to read last boundary */
                if (p_i_pointer <= p_a_bytes.Length - i_lengthLastBoundary)
                {
                    /* read so many bytes until we found '\r\n' or '\n' + '--' + boundary + '--' */
                    s_foo = (p_o_encoding ?? System.Text.Encoding.UTF8).GetString(Dynamic.ReadBytePartOfPostData(p_a_bytes, p_i_pointer, p_i_pointer + i_lengthLastBoundary));

                    /* return current pointer if it matches '\r\n' or '\n' + '--' + boundary + '--' */
                    if (s_foo.Equals(s_lineBreak + s_hyphen + p_s_boundary + s_hyphen))
                    {
                        return p_i_pointer;
                    }
                }

                /* increase pointer for reading within post body data bytes */
                p_i_pointer++;
            } while (p_i_pointer <= p_a_bytes.Length - i_lengthLastBoundary); /* until the end of post body data bytes - possible length of last boundary and '\r\n' and two double hyphen */

            /* return -1 if we found nothing at all */
            return -1;
        }

        /// <summary>
        /// get position of next line break value within a byte data array
        /// </summary>
        /// <param name="p_a_bytes">byte data array</param>
        /// <param name="p_o_encoding">encoding for reading bytes</param>
        /// <param name="p_i_pointer">current pointer position within byte data array</param>
        /// <param name="p_b_twoLineBreaks">true - look for two following line breaks, false - look for one line break</param>
        /// <param name="p_s_lineBreak">line break value</param>
        /// <exception cref="ArgumentNullException">byte data array parameter is null or empty</exception>
        /// <exception cref="ArgumentException">invalid pointer value to get next line break value</exception>
        public static int GetNextLineBreak(byte[] p_a_bytes, System.Text.Encoding? p_o_encoding, int p_i_pointer, bool p_b_twoLineBreaks, string p_s_lineBreak)
        {
            string s_foo;
            /* assume line break parameter with encoding parameter */
            string s_lineBreak = System.Text.Encoding.UTF8.GetString((p_o_encoding ?? System.Text.Encoding.UTF8).GetBytes(p_s_lineBreak));

            /* extend variable with additional line break */
            if (p_b_twoLineBreaks)
            {
                s_lineBreak =
                    System.Text.Encoding.UTF8.GetString((p_o_encoding ?? System.Text.Encoding.UTF8).GetBytes(p_s_lineBreak))
                    +
                    System.Text.Encoding.UTF8.GetString((p_o_encoding ?? System.Text.Encoding.UTF8).GetBytes(p_s_lineBreak));
            }

            /* correct negative pointer value to zero */
            if (p_i_pointer < 0)
            {
                p_i_pointer = 0;
            }

            do
            {
                /* read so many bytes until we found '\r\n' or '\n' */
                s_foo = (p_o_encoding ?? System.Text.Encoding.UTF8).GetString(Dynamic.ReadBytePartOfPostData(p_a_bytes, p_i_pointer, p_i_pointer + s_lineBreak.Length));

                /* we found that line break and return pointer position */
                if (s_foo.Equals(s_lineBreak))
                {
                    return p_i_pointer;
                }

                /* increase pointer for reading within byte data array */
                p_i_pointer++;
            } while (p_i_pointer <= p_a_bytes.Length - s_lineBreak.Length); /* until the end of byte data array - possible length line break '\r\n' or '\n' */

            /* return -1 if we found nothing at all */
            return -1;
        }

        /// <summary>
        /// get sub part of a byte array and return it
        /// </summary>
        /// <param name="p_a_bytes">byte data array where we want to cut a sub part</param>
        /// <param name="p_i_start">start position within byte data array</param>
        /// <param name="p_i_end">end position within byte data array</param>
        /// <exception cref="ArgumentNullException">byte data array parameter is null or empty</exception>
        /// <exception cref="ArgumentException">invalid pointer value to get sub part within byte data array</exception>
        private static byte[] ReadBytePartOfPostData(byte[] p_a_bytes, int p_i_start, int p_i_end)
        {
            /* check if byte data array parameter is not null or empty */
            if ((p_a_bytes == null) || (p_a_bytes.Length < 1))
            {
                throw new ArgumentNullException(nameof(p_a_bytes), "Byte content is null or empty");
            }

            /* correct negative pointer value to zero */
            if (p_i_start < 0)
            {
                p_i_start = 0;
            }

            /* correct negative pointer value to zero */
            if (p_i_end < 0)
            {
                p_i_end = 0;
            }

            /* check if start position is not greater than end position */
            if (p_i_start > p_i_end)
            {
                throw new ArgumentException("Start pointer '" + p_i_start + "' is greater to end pointer '" + p_i_end + "'");
            }

            /* create sub part byte array */
            byte[] a_return = new byte[p_i_end - p_i_start];

            /* if end position is greater equal complete length of byte data array, set it to correct end position */
            if (p_i_end >= p_a_bytes.Length)
            {
                p_i_end = p_a_bytes.Length;
            }

            /* copy bytes to sub part byte array */
            for (int i = p_i_start; i < p_i_end; i++)
            {
                a_return[i - p_i_start] = p_a_bytes[i];
            }

            /* return sub part byte array */
            return a_return;
        }

        /// <summary>
        /// core method to render and convert dynamic content within html or htm files
        /// </summary>
        public string RenderDynamic()
        {
            try
            {
                /* check if we have a ForestSeed instance within our tiny server configuration */
                if (this.Seed.Config.ForestSeed == null)
                {
                    throw new ArgumentNullException("Index branch object instance from config is null");
                }

                /* add current response body byte array as hash to our internal list, so we do not get an include loop */
                this.IncludeHashes.Add(ForestNET.Lib.Helper.HashByteArray("SHA-256", this.Seed.ResponseBody) ?? throw new NullReferenceException("Response body cannot be hashed, because it is null"));

                /* fetch content from ForestSeed instance, passing current Seed instance */
                this.Seed.Config.ForestSeed.FetchContent(this.Seed);

                /* render content, by using current response body byte array of html or htm file. our seed buffer will be filled we the correct dynamic content */
                this.RenderContent(this.Seed.ResponseBody ?? throw new NullReferenceException("Response body is null"));

                /* update response body with our seed buffer */
                this.Seed.ResponseBody = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(this.Seed.Buffer.ToString());
                /* update content length in response header */

                this.Seed.ResponseHeader.ContentLength = this.Seed.ResponseBody.Length;
            }
            catch (Exception o_exc)
            {
                if (this.Seed.Config.PrintExceptionStracktrace)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                }

                /* return exception message for return message */
                return o_exc.Message;
            }

            /* return 'OK' for return message */
            return "OK";
        }

        /// <summary>
        /// render content, by using current response body byte array of html or htm file. our seed buffer will be filled we the correct dynamic content
        /// </summary>
        /// <param name="p_a_responseBody">retrieved response body byte array of html or htm file</param>
        /// <exception cref="Exception">any exception which occurred while render content</exception>
        private void RenderContent(byte[] p_a_responseBody)
        {
            string s_start = "<!--#fAny";
            string s_end = "#fAny-->";
            string s_linebreak = this.Seed.Config.ForestSeed?.LineBreak ?? ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            string s_content = System.Text.Encoding.UTF8.GetString(p_a_responseBody);

            int i_startPointer, i_endPointer, i_pointer;
            int i_line = 1;

            /* look for sub string in response body starting with '<!--#fAny' and ending with '#fAny-->' */
            while (((i_startPointer = s_content.IndexOf(s_start)) >= 0) && ((i_endPointer = s_content.IndexOf(s_end)) >= 0))
            {
                /* check if end pointer is not lower than start pointer */
                if (i_endPointer < i_startPointer)
                {
                    throw new Exception("Tags for forestAny dynamic code are in wrong order. '" + s_end + "' read before '" + s_start + "'");
                }

                /* if start pointer is not at the beginning at the response body */
                if (i_startPointer != 0)
                {
                    /* assume all of response body content until start pointer to our buffer */
                    this.Seed.Buffer.Append(s_content.AsSpan(0, i_startPointer));
                    /* update our internal line number within response body, in case we have to throw an exception we have this additional information */
                    i_line += ForestNET.Lib.Helper.CountSubStrings(s_content.Substring(0, i_startPointer), s_linebreak);
                }

                /* now we have found out the exact start and end point of our dynamic content part within response body, next we can handle and parse this part */
                this.HandleForestAnyDynamicCode(s_content.Substring(i_startPointer + s_start.Length, i_endPointer - (i_startPointer + s_start.Length)), i_line, null);

                /* update our internal line number within response body, in case we have to throw an exception we have this additional information */
                i_line += ForestNET.Lib.Helper.CountSubStrings(s_content.Substring(i_startPointer + s_start.Length, i_endPointer - (i_startPointer + s_start.Length)), s_linebreak);

                /* increase our pointer to end of dynamic content of current line */
                i_pointer = i_endPointer + s_end.Length;
                /* update our content variable, so we can continue with all the content after the dynamic part and look with our while loop for other dynamic parts */
                s_content = s_content.Substring(i_pointer);
            }

            /* assume rest of response body content to our buffer */
            this.Seed.Buffer.Append(s_content);
        }

        /// <summary>
        /// handle and substitute dynamic forestAny content which will be append to our internal buffer
        /// </summary>
        /// <param name="p_s_forestAny">dynamic content command</param>
        /// <param name="p_i_line">which line we are within response body</param>
        /// <param name="p_o_foreachElement">handling dynamic content with a Collection or Map object</param>
        /// <exception cref="Exception">any exception which occurred while handling dynamic content</exception>
        private void HandleForestAnyDynamicCode(string p_s_forestAny, int p_i_line, Object? p_o_foreachElement)
        {
            string s_linebreak = this.Seed.Config.ForestSeed?.LineBreak ?? ForestNET.Lib.Net.Https.Config.HTTP_LINEBREAK;
            string s_forestAny = p_s_forestAny;

            /* remove all line breaks within our forestAny command */
            s_forestAny = s_forestAny.Replace(s_linebreak, "");

            bool b_withinQuotes = false;
            string s_withinQuotesTemp = "";
            bool b_withinKeyword = false;
            string s_withinKeywordTemp = "";

            /* iterate each character within forestAny command */
            for (int i = 0; i < s_forestAny.Length; i++)
            {
                /* '"' indicates that we have static content here */
                if (s_forestAny[i] == '"')
                {
                    if (b_withinQuotes)
                    { /* we already are within double quotes (end), so we can add the static content to our internal buffer */
                        this.Seed.Buffer.Append(s_withinQuotesTemp);
                        /* reset variable for static content */
                        s_withinQuotesTemp = "";
                        /* set double quotes flag to false */
                        b_withinQuotes = false;
                    }
                    else
                    { /* we are not within double quotes (start) */
                        /* set double quotes flag to true */
                        b_withinQuotes = true;
                    }

                    /* continue with next character */
                    continue;
                }

                if (b_withinQuotes)
                { /* we are within static content, so we can add character to our variable for static content */
                    s_withinQuotesTemp += s_forestAny[i];
                }
                else
                { /* handle forestAny command */
                    /* skip whitespace or tab */
                    if ((s_forestAny[i] == ' ') || (s_forestAny[i] == '\t'))
                    {
                        continue;
                    }

                    /* '%' indicates that we are handling a keyword here */
                    if (s_forestAny[i] == '%')
                    {
                        if (b_withinKeyword)
                        { /* we already are within keyword (end), so we can handle they forestAny keyword with our method */
                            /* handle forestAny keyword and add the result to our internal buffer */
                            Object? o_foo = this.HandleRecognizedKeyword(s_withinKeywordTemp, false, p_i_line);
                            this.Seed.Buffer.Append(o_foo?.ToString());

                            /* reset variable for keyword */
                            s_withinKeywordTemp = "";
                            /* set keyword flag to false */
                            b_withinKeyword = false;
                        }
                        else
                        { /* we are not within keyword (start) */
                            /* set keyword flag to true */
                            b_withinKeyword = true;
                        }

                        /* continue with next character */
                        continue;
                    }

                    if (b_withinKeyword)
                    { /* we are within a keyword, so we can add character to our variable for keyword */
                        s_withinKeywordTemp += s_forestAny[i];
                    }
                    else
                    { /* handle keyword */
                        /* check if keyword starts with '?' so we have an if construct, INCLUDE or FOREACH */
                        if (
                            (s_forestAny[i] == '?') ||
                            (
                                ((i + 7) < s_forestAny.Length) &&
                                (
                                    (s_forestAny.Substring(i, 7).Equals("INCLUDE")) ||
                                    (s_forestAny.Substring(i, 7).Equals("FOREACH"))
                                )
                            )
                        )
                        {
                            string s_forestDynamicCode = "";
                            bool b_foundSemicolon = false;
                            int i_withinCurlyBrackets = 0;
                            int j;

                            /* iterate each character within keyword, to get our dynamic code */
                            for (j = i; j < s_forestAny.Length; j++)
                            {
                                /* check for semicolon at current position, if we are not within curly brackets */
                                if ((s_forestAny[j] == ';') && (i_withinCurlyBrackets == 0))
                                {
                                    b_foundSemicolon = true;
                                    /* add last found semicolon to dynamic code */
                                    s_forestDynamicCode += ";";

                                    /* handle dynamic code */
                                    this.HandleForestDynamicCodeCommand(s_forestDynamicCode, p_i_line);

                                    /* abort for loop */
                                    break;
                                }
                                else
                                {
                                    if (s_forestAny[j] == '{')
                                    { /* increase curly brackets counter */
                                        i_withinCurlyBrackets++;
                                    }
                                    else if (s_forestAny[j] == '}')
                                    { /* decrease curly brackets counter */
                                        i_withinCurlyBrackets--;
                                    }

                                    /* add character to dynamic code variable */
                                    s_forestDynamicCode += s_forestAny[j];
                                }
                            }

                            /* jump after dynamic code */
                            i = j;

                            /* check if semicolon has been used at the end of dynamic code */
                            if ((i == s_forestAny.Length) && (!b_foundSemicolon))
                            {
                                throw new Exception("Error: Missing ';' within forestAny dynamic code in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
                            }
                        }
                        else if ((i > 0) && (s_forestAny[i - 1] == '[') && (s_forestAny[i] == ']'))
                        { /* check if we have empty brackets '[]' */
                            /* foreach element must not be null */
                            if (p_o_foreachElement == null)
                            {
                                throw new Exception("Error: Missing array element within forestAny dynamic code in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
                            }

                            /* foreach element must NOT be a list or a dictionary */
                            if (
                                (p_o_foreachElement.GetType().IsGenericType) &&
                                ((p_o_foreachElement.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IList))) ||
                                (p_o_foreachElement.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IDictionary))))
                            )
                            {
                                throw new Exception("Error: Array element is an iterable object within forestAny dynamic code in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
                            }

                            /* append foreach element as string value to our internal buffer */
                            this.Seed.Buffer.Append(p_o_foreachElement.ToString());
                        }
                        else if ((i < s_forestAny.Length - 1) && (s_forestAny[i] == '[') && (s_forestAny[i + 1] != ']'))
                        { /* check if we have brackets with an index '[42]' */
                            /* foreach element must not be null */
                            if (p_o_foreachElement == null)
                            {
                                throw new Exception("Error: Missing array element within forestAny dynamic code in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
                            }

                            /* foreach element MUST BE a list or a dictionary */
                            if (!(
                                (p_o_foreachElement.GetType().IsGenericType) &&
                                ((p_o_foreachElement.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IList))) ||
                                (p_o_foreachElement.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IDictionary))))
                            ))
                            {
                                throw new Exception("Error: Array element is not an iterable object within forestAny dynamic code in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
                            }

                            string s_index = "";
                            bool b_foundClosingBrackets = false;
                            int j;

                            /* iterate each character within index value */
                            for (j = i + 1; j < s_forestAny.Length; j++)
                            {
                                if (s_forestAny[j] == ']')
                                { /* we found our closing bracket, so we can handle the index value now */
                                    b_foundClosingBrackets = true;

                                    System.Text.RegularExpressions.Regex o_regex = new("^([a-zA-Z0-9]*)$");
                                    System.Text.RegularExpressions.Match o_matcher = o_regex.Match(s_index);

                                    /* index value must be an alphanumeric value */
                                    if (!o_matcher.Success)
                                    {
                                        throw new Exception("Error: Invalid index '" + s_index + "' found in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
                                    }
                                    else
                                    {
                                        /* our general temporary list is of type Map<string, object> so we cast our foreach element the same way */
                                        System.Collections.IDictionary a_tempFoo = (System.Collections.IDictionary)p_o_foreachElement;
                                        Dictionary<string, Object?> a_foo = [];

                                        foreach (System.Collections.DictionaryEntry o_keyAndValue in a_tempFoo)
                                        {
                                            a_foo.Add(o_keyAndValue.Key.ToString() ?? throw new NullReferenceException("Key value in dictionary cannot be converted to a non-nullable string"), o_keyAndValue.Value);
                                        }

                                        /* check if our index is within temporary list */
                                        if (!a_foo.TryGetValue(s_index, out object? o_foo))
                                        {
                                            throw new Exception("Error: Index '[" + s_index + "]' not found in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
                                        }

                                        /* value behind index must NOT be a list or a dictionary */
                                        if (
                                            (o_foo != null) && (o_foo.GetType().IsGenericType) &&
                                            ((o_foo.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IList))) ||
                                            (o_foo.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IDictionary))))
                                        )
                                        {
                                            throw new Exception("Error: Cannot render iterable object behind index '[" + s_index + "]' within foreach loop in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
                                        }

                                        /* append value behind index as string value to our internal buffer */
                                        this.Seed.Buffer.Append(o_foo?.ToString());
                                    }

                                    break;
                                }
                                else
                                {
                                    /* add character to index variable */
                                    s_index += s_forestAny[j];
                                }
                            }

                            /* jump after index value */
                            i = j;

                            /* check if closing bracket has been used at the end of index value */
                            if ((i == s_forestAny.Length) && (!b_foundClosingBrackets))
                            {
                                throw new Exception("Error: Missing ']' to read index for an interable object in a foreach loop within forestAny dynamic code in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// handle and substitute dynamic forestAny command which will be append to our internal buffer
        /// </summary>
        /// <param name="p_s_forestAnyDynamicCommand">dynamic content command</param>
        /// <param name="p_i_line">which line we are within response body</param>
        /// <exception cref="Exception">any exception which occurred while handling dynamic content</exception>
        private void HandleForestDynamicCodeCommand(string p_s_forestAnyDynamicCommand, int p_i_line)
        {
            System.Text.RegularExpressions.Regex o_regex;
            System.Text.RegularExpressions.Match o_matcher;

            if (p_s_forestAnyDynamicCommand.StartsWith("?"))
            { /* check for if construct */
                o_regex = new("^\\?\\s?\\(([a-zA-Z0-9\\[\\]]*)\\)\\s?\\{\\s?([a-zA-Z0-9\\s\\._\\-\\+\\\\/\\[\\]\"\\{\\};:\\(\\)<>\\|%]*)\\s?\\}\\s?:\\s?\\{\\s?(([a-zA-Z0-9\\s\\._\\-\\+\\\\/\\[\\]\"\\{\\};:\\(\\)<>\\|%]*))\\s?\\};$");
                o_matcher = o_regex.Match(p_s_forestAnyDynamicCommand);

                /* check for if and else */
                if (o_matcher.Success)
                {
                    /* handle if else */
                    this.HandleIfConstruct(o_matcher.Groups[1].Value, o_matcher.Groups[2].Value, o_matcher.Groups[3].Value, p_i_line);
                }
                else
                {
                    o_regex = new("^\\?\\s?\\(([a-zA-Z0-9\\[\\]]*)\\)\\s?\\{\\s?([a-zA-Z0-9\\s\\._\\-\\+\\\\/\\[\\]\"\\{\\};:\\(\\)<>\\|%]*)\\s?\\};$");
                    o_matcher = o_regex.Match(p_s_forestAnyDynamicCommand);

                    /* check for if only */
                    if (o_matcher.Success)
                    {
                        /* handle if only */
                        this.HandleIfConstruct(o_matcher.Groups[1].Value, o_matcher.Groups[2].Value, null, p_i_line);
                    }
                    else
                    {
                        throw new Exception("Error: Invalid syntax for if construct '" + p_s_forestAnyDynamicCommand + "' within forestAny dynamic code in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
                    }
                }
            }
            else if (p_s_forestAnyDynamicCommand.StartsWith("FOREACH"))
            { /* check for FOREACH */
                o_regex = new("^FOREACH\\s?\\(([a-zA-Z0-9\\[\\]]*)\\)\\s?\\{\\s?([a-zA-Z0-9\\s\\._\\-\\+\\\\/\\[\\]\"\\{\\};:\\(\\)<>\\|%]*)\\s?\\};$");
                o_matcher = o_regex.Match(p_s_forestAnyDynamicCommand);

                /* check for FOREACH */
                if ((o_matcher.Success) && (!o_matcher.Groups[2].Value.Contains("FOREACH")) && (!o_matcher.Groups[2].Value.Contains("INCLUDE")))
                {
                    /* handle FOREACH */
                    this.HandleForEachLoop(o_matcher.Groups[1].Value, o_matcher.Groups[2].Value, p_i_line);
                }
                else
                {
                    throw new Exception("Error: Invalid syntax for foreach loop '" + p_s_forestAnyDynamicCommand + "' within forestAny dynamic code in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
                }
            }
            else if (p_s_forestAnyDynamicCommand.StartsWith("INCLUDE"))
            { /* check for INCLUDE */
                o_regex = new("^INCLUDE\\s([a-zA-Z0-9\\s\\._\\-\\+\\/]*);$");
                o_matcher = o_regex.Match(p_s_forestAnyDynamicCommand);

                /* check for INCLUDE */
                if (o_matcher.Success)
                {
                    /* handle INCLUDE */
                    this.HandleInclude(o_matcher.Groups[1].Value, p_i_line);
                }
                else
                {
                    throw new Exception("Error: Invalid syntax for include (invalid path) '" + p_s_forestAnyDynamicCommand + "' within forestAny dynamic code in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
                }
            }
        }

        /// <summary>
        /// handle keyword and get it's value from preparing content and return it as object
        /// </summary>
        /// <param name="p_s_keyword">recognized keyword value</param>
        /// <param name="p_b_allowArrayAsReturn">true - allow collection or dictionary as return value</param>
        /// <param name="p_i_line">which line we are within response body</param>
        /// <exception cref="Exception">any exception which occurred while handling dynamic content</exception>
        private Object? HandleRecognizedKeyword(string p_s_keyword, bool p_b_allowArrayAsReturn, int p_i_line)
        {
            string s_keyword;
            string s_firstIndex = string.Empty;
            string s_secondIndex = string.Empty;

            System.Text.RegularExpressions.Regex o_regex;
            System.Text.RegularExpressions.Match o_matcher;

            o_regex = new("^([a-zA-Z0-9]*)(\\[([a-zA-Z0-9]*)\\])(\\[(([a-zA-Z0-9]*))\\])$");
            o_matcher = o_regex.Match(p_s_keyword);

            /* try to recognize keyword, first index and second index */
            if (o_matcher.Success)
            {
                s_keyword = o_matcher.Groups[1].Value;
                s_firstIndex = o_matcher.Groups[3].Value;
                s_secondIndex = o_matcher.Groups[5].Value;
            }
            else
            {
                o_regex = new("^([a-zA-Z0-9]*)(\\[(([a-zA-Z0-9]*))\\])$");
                o_matcher = o_regex.Match(p_s_keyword);

                /* try to recognize keyword and first index */
                if (o_matcher.Success)
                {
                    s_keyword = o_matcher.Groups[1].Value;
                    s_firstIndex = o_matcher.Groups[3].Value;
                }
                else
                {
                    o_regex = new("^([a-zA-Z0-9]*)$");
                    o_matcher = o_regex.Match(p_s_keyword);

                    /* try to recognize keyword only */
                    if (o_matcher.Success)
                    {
                        s_keyword = o_matcher.Groups[0].Value;
                    }
                    else
                    {
                        throw new Exception("Error: Invalid keyword '" + p_s_keyword + "' found in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
                    }
                }
            }

            /* check if key exists with our keyword in out temporary list */
            if (!this.Seed.Temp.TryGetValue(s_keyword, out object? o_currentObject))
            {
                throw new Exception("Error: Keyword '" + s_keyword + "' not found in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
            }

            /* if we have a first index */
            if ((!ForestNET.Lib.Helper.IsStringEmpty(s_firstIndex)) && (o_currentObject != null) && (o_currentObject.GetType().IsGenericType))
            {
                if (o_currentObject.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IDictionary)))
                { /* current object is a dictionary */
                    /* get dictionary object with first index */
                    o_currentObject = this.GetDictionaryObject(o_currentObject, s_firstIndex, p_i_line);

                    /* if we have a second index */
                    if ((!ForestNET.Lib.Helper.IsStringEmpty(s_secondIndex)) && (o_currentObject != null))
                    {
                        if (o_currentObject.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IDictionary)))
                        {
                            /* get dictionary object with second index */
                            o_currentObject = this.GetDictionaryObject(o_currentObject, s_secondIndex, p_i_line);
                        }
                        else if (o_currentObject.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IList)))
                        {
                            /* get collection list object with second index */
                            o_currentObject = this.GetListObject(o_currentObject, s_secondIndex, p_i_line);
                        }
                    }
                }
                else if (o_currentObject.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IList)))
                { /* current object is a collection list */
                    /* get collection list object with first index */
                    o_currentObject = this.GetListObject(o_currentObject, s_firstIndex, p_i_line);

                    /* if we have a second index */
                    if ((!ForestNET.Lib.Helper.IsStringEmpty(s_secondIndex)) && (o_currentObject != null))
                    {
                        if (o_currentObject.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IDictionary)))
                        {
                            /* get dictionary object with second index */
                            o_currentObject = this.GetDictionaryObject(o_currentObject, s_secondIndex, p_i_line);
                        }
                        else if (o_currentObject.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IList)))
                        {
                            /* get collection list object with second index */
                            o_currentObject = this.GetListObject(o_currentObject, s_secondIndex, p_i_line);
                        }
                    }
                }
            }

            /* check if return value is iterable and throw exception if it is, or we want allow iterable as return value */
            if (
                (!p_b_allowArrayAsReturn) && (o_currentObject != null) && (o_currentObject.GetType().IsGenericType) &&
                (
                    (o_currentObject.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IList))) ||
                    (o_currentObject.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IDictionary)))
                )
            )
            {
                throw new Exception("Error: Cannot render iterable object behind keyword '" + s_keyword + "[" + s_firstIndex + "]" + "[" + s_secondIndex + "]' in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
            }

            return o_currentObject;
        }

        /// <summary>
        /// get object out of dictionary object with key value
        /// </summary>
        /// <param name="p_o_object">parameter which contains dictionary object</param>
        /// <param name="p_s_index">key value</param>
        /// <param name="p_i_line">which line we are within response body</param>
        /// <exception cref="Exception">any exception which occurred while handling dynamic content</exception>
        private Object? GetDictionaryObject(Object? p_o_object, string p_s_index, int p_i_line)
        {
            /* return null if parameter is null */
            if (p_o_object == null)
            {
                return null;
            }

            /* cast object to dictionary */
            System.Collections.IDictionary a_tempFoo = (System.Collections.IDictionary)p_o_object;

            foreach (System.Collections.DictionaryEntry o_keyAndValue in a_tempFoo)
            {
                /* check if key exists in our dictionary */
                if (o_keyAndValue.Key.Equals(p_s_index))
                {
                    /* return dictionary object with key index */
                    return o_keyAndValue.Value;
                }
            }

            throw new Exception("Error: Invalid index 'Dictionary[" + p_s_index + "]' not found in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
        }

        /// <summary>
        /// get object out of list collection object with key value
        /// </summary>
        /// <param name="p_o_object">parametr which contains list collection object</param>
        /// <param name="p_s_index">key value</param>
        /// <param name="p_i_line">which line we are within response body</param>
        /// <exception cref="Exception">any exception which occurred while handling dynamic content</exception>
        private Object? GetListObject(Object? p_o_object, string p_s_index, int p_i_line)
        {
            /* return null if parameter is null */
            if (p_o_object == null)
            {
                return null;
            }

            /* cast object to list and parse index */
            System.Collections.IList a_tempList = (System.Collections.IList)p_o_object;
            int i_index = int.Parse(p_s_index);

            /* check if key exists in our list collection */
            if (a_tempList.Count <= i_index)
            {
                throw new Exception("Error: Invalid index 'List[" + p_s_index + "]' not found in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
            }

            /* return list collection object with key index */
            return a_tempList[i_index];
        }

        /// <summary>
        /// determine which dynamic code should be executed by looking at bool values behind keywords
        /// </summary>
        /// <param name="p_s_expression">expression keyword which will by checked as bool value</param>
        /// <param name="p_s_if">execute dynamic code if expression keyword is true</param>
        /// <param name="p_s_else">execute dynamic code if expression keyword is false, and this parameter is not null</param>
        /// <param name="p_i_line">which line we are within response body</param>
        /// <exception cref="Exception">any exception which occurred while handling dynamic content</exception>
        private void HandleIfConstruct(string p_s_expression, string p_s_if, string? p_s_else, int p_i_line)
        {
            /* remove last semicolon from dynamic code for positive expression keyword (if) */
            if (p_s_if.EndsWith(";"))
            {
                p_s_if = p_s_if.Substring(0, p_s_if.Length - 1);
            }

            if ((p_s_else == null) || (ForestNET.Lib.Helper.IsStringEmpty(p_s_else)))
            { /* else expression keyword is null or empty */
                /* check if expression keyword is true */
                if (bool.Parse(this.HandleRecognizedKeyword(p_s_expression, false, p_i_line)?.ToString() ?? "False"))
                {
                    /* execute dynamic code for positive expression keyword */
                    this.HandleForestAnyDynamicCode(p_s_if, p_i_line, null);
                }
            }
            else
            { /* else expression keyword is NOT null or empty */
                /* remove last semicolon from dynamic code for negative expression keyword (else) */
                if (p_s_else.EndsWith(";"))
                {
                    p_s_else = p_s_else.Substring(0, p_s_else.Length - 1);
                }

                /* check if expression keyword is true */
                if (bool.Parse(this.HandleRecognizedKeyword(p_s_expression, false, p_i_line)?.ToString() ?? "False"))
                {
                    /* execute dynamic code for positive expression keyword */
                    this.HandleForestAnyDynamicCode(p_s_if, p_i_line, null);
                }
                else
                {
                    /* execute dynamic code for negative expression keyword */
                    this.HandleForestAnyDynamicCode(p_s_else, p_i_line, null);
                }
            }
        }

        /// <summary>
        /// handle foreach loop as dynamic content and execute dynamic code handling for each element
        /// </summary>
        /// <param name="p_s_array">expression keyword with collection or dictionary</param>
        /// <param name="p_s_loopCommand">dynamic code for loop command</param>
        /// <param name="p_i_line">which line we are within response body</param>
        /// <exception cref="Exception">any exception which occurred while handling dynamic content</exception>
        private void HandleForEachLoop(string p_s_array, string p_s_loopCommand, int p_i_line)
        {
            /* handle expression keyword for collection or dictionary object */
            Object? o_foo = this.HandleRecognizedKeyword(p_s_array, true, p_i_line);

            if ((o_foo != null) && (o_foo.GetType().IsGenericType) && (o_foo.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IList))))
            { /* we have a collection object for iteration */
                /* cast object to collection */
                System.Collections.IList a_foo = (System.Collections.IList)o_foo;

                /* iterate each collection object */
                foreach (Object? o_element in a_foo)
                {
                    /* handle collection object with loop command */
                    this.HandleForestAnyDynamicCode(p_s_loopCommand, p_i_line, o_element);
                }
            }
            else if ((o_foo != null) && (o_foo.GetType().IsGenericType) && (o_foo.GetType().GetGenericTypeDefinition().IsAssignableTo(typeof(System.Collections.IDictionary))))
            { /* we have a dictionary object for iteration */
                /* cast object to dictionary */
                System.Collections.IDictionary a_foo = (System.Collections.IDictionary)o_foo;

                /* iterate each dictionary object */
                foreach (System.Collections.DictionaryEntry o_keyAndValue in a_foo)
                {
                    /* handle dictionary object with loop command */
                    this.HandleForestAnyDynamicCode(p_s_loopCommand, p_i_line, o_keyAndValue.Value);
                }
            }
            else
            {
                throw new Exception("Error: Array element '" + p_s_array + "' is not a List, Set or Map for foreach loop in '" + this.Seed.RequestHeader.File + "' in line " + p_i_line);
            }
        }

        /// <summary>
        /// handle include for dynamic content
        /// </summary>
        /// <param name="p_s_includeFilePath">html or htm file which will be included</param>
        /// <param name="p_i_line">which line we are within response body</param>
        /// <exception cref="Exception">any exception which occurred while handling dynamic content</exception>
        private void HandleInclude(string p_s_includeFilePath, int p_i_line)
        {
            /* '..' within include file path is not allowed */
            if (p_s_includeFilePath.Contains(".."))
            {
                throw new Exception("Error: Invalid including resource path: '" + p_s_includeFilePath + "'; line number '" + p_i_line + "'");
            }

            if (p_s_includeFilePath.StartsWith("./"))
            {
                /* remove './' from beginning of file path */
                p_s_includeFilePath = p_s_includeFilePath.Substring(2);
            }
            else if ((p_s_includeFilePath.StartsWith(".")) || (p_s_includeFilePath.StartsWith("/")))
            {
                /* remove '.' or '/' from beginning of file path */
                p_s_includeFilePath = p_s_includeFilePath.Substring(1);
            }

            /* check if file path is not null or empty */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_includeFilePath))
            {
                throw new Exception("Error: Resource path is null or empty; line number '\" + p_i_line + \"'");
            }

            string s_path = string.Empty;
            string s_file;

            if (p_s_includeFilePath.Contains('/'))
            { /* we have directory characters */
                /* get path value from file path, and replace all directory characters with system-dependent default name-separator character */
                s_path = p_s_includeFilePath.Substring(0, p_s_includeFilePath.LastIndexOf("/")).Replace('/', ForestNET.Lib.IO.File.DIR);
                /* get file value from file path */
                s_file = p_s_includeFilePath.Substring(p_s_includeFilePath.LastIndexOf("/") + 1);
            }
            else
            { /* we just have a file name */
                s_file = p_s_includeFilePath;
            }

            /* file value must end with '.html' or '.htm' */
            if ((!s_file.EndsWith(".html")) && (!s_file.EndsWith(".htm")))
            {
                throw new Exception("Error: Including resource '" + p_s_includeFilePath + "' file extension is not supported; line number '\" + p_i_line + \"'");
            }

            /* create absolute file path on server */
            string s_includeFileAbsolutePath = this.Seed.Config.RootDirectory + s_path + ForestNET.Lib.IO.File.DIR + s_file;

            /* check if file really exists */
            if (ForestNET.Lib.IO.File.Exists(s_includeFileAbsolutePath))
            {
                /* check if included file does not exceed max. payload */
                if (ForestNET.Lib.IO.File.FileLength(s_includeFileAbsolutePath) > this.Seed.Config.MaxPayload * 1024 * 1024)
                {
                    throw new Exception("Error: Payload Too Large - File length for include is to long, max. payload is '" + ForestNET.Lib.Helper.FormatBytes(this.Seed.Config.MaxPayload * 1024 * 1024) + "'; line number '\" + p_i_line + \"'");
                }

                /* read including file as byte array */
                byte[]? a_includeContent = ForestNET.Lib.IO.File.ReadAllBytes(s_includeFileAbsolutePath, this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8, this.Seed.Config.InEncoding ?? System.Text.Encoding.UTF8);
                /* create hash of byte array */
                string s_hash = ForestNET.Lib.Helper.HashByteArray("SHA-256", a_includeContent) ?? throw new NullReferenceException("hash value of including file[" + s_includeFileAbsolutePath + "] is null; line number '\" + p_i_line + \"'");

                /* check if hash does not exist in current dynamic instance to prevent include loops */
                if (this.IncludeHashes.Contains(s_hash))
                {
                    throw new Exception("Error: Resource '" + p_s_includeFilePath + "' has already been included by current request, recursive includes are not supported; line number '\" + p_i_line + \"'");
                }
                else
                {
                    /* add hash to list for following checks */
                    this.IncludeHashes.Add(s_hash);
                }

                /* save old file name */
                string s_oldFile = this.Seed.RequestHeader.File;
                /* set new file name from including file */
                this.Seed.RequestHeader.File = s_file;

                /* render including file content */
                this.RenderContent(a_includeContent ?? throw new NullReferenceException("including file content[" + s_includeFileAbsolutePath + "] is null or empty; line number '" + p_i_line + "'"));

                /* restore old file name */
                this.Seed.RequestHeader.File = s_oldFile;
            }
            else
            { /* file does not exist */
                throw new Exception("Error: Including resource '" + p_s_includeFilePath + "' not found; line number '" + p_i_line + "'");
            }
        }

        /// <summary>
        /// core method to render and execute REST instance with implemented methods
        /// </summary>
        public string RenderREST()
        {
            try
            {
                /* check if we have a ForestREST instance within our tiny server configuration */
                if (this.Seed.Config.ForestREST == null)
                {
                    throw new ArgumentNullException("REST object instance from config is null");
                }

                /* execute REST implementation from ForestREST instance, passing current Seed instance and expecting REST response as string value */
                string s_restResponse = this.Seed.Config.ForestREST.HandleREST(this.Seed);

                /* update response body with returning REST string value */
                this.Seed.ResponseBody = (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).GetBytes(s_restResponse);
                /* update content length in response header */
                this.Seed.ResponseHeader.ContentLength = this.Seed.ResponseBody.Length;

                /* if no response content type has been defined */
                if ((this.Seed.Config.ForestREST.ResponseContentType == null) || (ForestNET.Lib.Helper.IsStringEmpty(this.Seed.Config.ForestREST.ResponseContentType)))
                {
                    /* we will just use .txt as response content type + outgoing encoding encoding value */
                    this.Seed.ResponseHeader.ContentType = ForestNET.Lib.Net.Https.Config.KNOWN_EXTENSION_LIST[".txt"] + "; charset=" + (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).WebName;
                }
                else
                {
                    /* if we respond with text|json|xml data we are using our outgoing encoding setting */
                    if (this.Seed.Config.ForestREST.ResponseContentType.StartsWith("text") || this.Seed.Config.ForestREST.ResponseContentType.Contains("json") || this.Seed.Config.ForestREST.ResponseContentType.Contains("xml"))
                    {
                        this.Seed.ResponseHeader.ContentType = this.Seed.Config.ForestREST.ResponseContentType + "; charset=" + (this.Seed.Config.OutEncoding ?? System.Text.Encoding.UTF8).WebName;
                    }
                    else
                    { /* otherwise we just use found content type */
                        this.Seed.ResponseHeader.ContentType = this.Seed.Config.ForestREST.ResponseContentType;
                    }
                }

                /* reset content type for next request */
                this.Seed.Config.ForestREST.SetResponseContentTypeByFileExtension(null);

                if (s_restResponse.StartsWith("400;"))
                {
                    return s_restResponse;
                }
            }
            catch (Exception o_exc)
            {
                if (this.Seed.Config.PrintExceptionStracktrace)
                {
                    ForestNET.Lib.Global.LogException(o_exc);
                }

                /* return exception message for return message */
                return o_exc.Message;
            }

            /* return 'OK' for return message */
            return "OK";
        }
    }
}