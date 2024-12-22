using System.Text;

namespace ForestNETLib.IO
{
    /// <summary>
    /// File class for creating, manipulating or deleting files in various ways.
    /// Multiple static methods to check file or directory existence, move or copy files/directories, modify file permissions(only windows supported).
    /// Static methods for creating random file content or replacing whole file content.
    /// </summary>
    public class File
    {

        /* Constants */

        public const string ENCODING = "UTF-8";
        public static readonly char DIR = System.IO.Path.DirectorySeparatorChar;
        public static readonly string NEWLINE = Environment.NewLine;

        /* Fields */

        private string s_filename;
        private long l_fileLength;
        private readonly bool b_ready;
        private List<string> a_fileContent;
        private readonly Encoding o_encoding;
        private string s_pathToFile;
        private readonly string s_lineBreak;

        /* Properties */

        public int FileLines
        {
            get { return this.a_fileContent.Count; }
        }

        public string FileContent
        {
            get
            {
                if (!this.b_ready)
                {
                    return "";
                }

                /* nothing to read in file */
                if ((this.l_fileLength == 0) || (!System.IO.File.Exists(this.s_pathToFile)))
                {
                    return "";
                }

                StringBuilder o_stringBuilder = new();

                for (int i = 0; i < this.a_fileContent.Count; i++)
                {
                    o_stringBuilder.Append(this.a_fileContent[i] + this.s_lineBreak);
                }

                return o_stringBuilder.ToString();
            }
        }

        public List<string>? FileContentAsList
        {
            get
            {
                List<string> a_content = [];

                if (!this.b_ready)
                {
                    return null;
                }

                /* nothing to read in file */
                if ((this.l_fileLength == 0) || (!System.IO.File.Exists(this.s_pathToFile)))
                {
                    return null;
                }

                for (int i = 0; i < this.a_fileContent.Count; i++)
                {
                    a_content.Add(this.a_fileContent[i]);
                }

                return a_content;
            }
        }

        public DateTime LastModifiedValue
        {
            get { return File.LastModified(this.s_pathToFile); }
        }

        public List<string> FileContentFromList
        {
            set
            {
                if ((value != null) && (value.Count > 0))
                {
                    StringBuilder o_stringBuilder = new();

                    foreach (string s_line in value)
                    {
                        o_stringBuilder.Append(s_line + this.s_lineBreak);
                    }

                    /* write file content */
                    try
                    {
                        System.IO.File.WriteAllText(this.s_pathToFile, o_stringBuilder.ToString(), this.o_encoding);
                    }
                    catch (Exception o_exc)
                    {
                        throw new System.IO.IOException("File[" + this.s_pathToFile + "] write access not possible: " + o_exc.Message);
                    }
                }
            }
        }

        public string FileName
        {
            get { return this.s_filename; }
        }

        /* Methods */

        /// <summary>
        /// Constructor to create file instance, opening an existing file.
        /// default charset: UTF-8.
        /// </summary>
        /// <param name="p_s_pathToFile">full path + filename to the file</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public File(string p_s_pathToFile) : this(p_s_pathToFile, Encoding.GetEncoding(ENCODING), NEWLINE)
        {

        }

        /// <summary>
        /// Constructor to create file instance, opening an existing file.
        /// default charset: UTF-8.
        /// </summary>
        /// <param name="p_s_pathToFile">full path + filename to the file</param>
        /// <param name="p_s_lineBreak">use alternative line break '\n', '\r\n', or '\r'</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public File(string p_s_pathToFile, string p_s_lineBreak) : this(p_s_pathToFile, Encoding.GetEncoding(ENCODING), false, p_s_lineBreak)
        {

        }

        /// <summary>
        /// Constructor to create file instance.
        /// default charset: UTF-8.
        /// </summary>
        /// <param name="p_s_pathToFile">full path + filename to the file</param>
        /// <param name="p_b_new">flag to control if file should be created new or is already exisiting</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public File(string p_s_pathToFile, bool p_b_new) : this(p_s_pathToFile, Encoding.GetEncoding(ENCODING), p_b_new, NEWLINE)
        {

        }

        /// <summary>
        /// Constructor to create file instance.
        /// default charset: UTF-8.
        /// </summary>
        /// <param name="p_s_pathToFile">full path + filename to the file</param>
        /// <param name="p_b_new">flag to control if file should be created new or is already exisiting</param>
        /// <param name="p_s_lineBreak">use alternative line break '\n', '\r\n', or '\r'</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public File(string p_s_pathToFile, bool p_b_new, string p_s_lineBreak) : this(p_s_pathToFile, Encoding.GetEncoding(ENCODING), p_b_new, p_s_lineBreak)
        {

        }

        /// <summary>
        /// Constructor to create file instance
        /// </summary>
        /// <param name="p_s_pathToFile">full path + filename to the file</param>
        /// <param name="p_b_new">flag to control if file should be created new or is already exisiting</param>
        /// <param name="p_o_encoding">which encoding will be used accessing/modifying the file content</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public File(string p_s_pathToFile, bool p_b_new, Encoding p_o_encoding) : this(p_s_pathToFile, p_o_encoding, p_b_new, NEWLINE)
        {

        }

        /// <summary>
        /// Constructor to create file instance
        /// </summary>
        /// <param name="p_s_pathToFile">full path + filename to the file</param>
        /// <param name="p_b_new">flag to control if file should be created new or is already exisiting</param>
        /// <param name="p_o_encoding">which encoding will be used accessing/modifying the file content</param>
        /// <param name="p_s_lineBreak">use alternative line break '\n', '\r\n', or '\r'</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public File(string p_s_pathToFile, bool p_b_new, Encoding p_o_encoding, string p_s_lineBreak) : this(p_s_pathToFile, p_o_encoding, p_b_new, p_s_lineBreak)
        {

        }

        /// <summary>
        /// Constructor to create file instance, opening an existing file
        /// </summary>
        /// <param name="p_s_pathToFile">full path + filename to the file</param>
        /// <param name="p_o_encoding">which encoding will be used accessing/modifying the file content</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public File(string p_s_pathToFile, Encoding p_o_encoding) : this(p_s_pathToFile, p_o_encoding, false, NEWLINE)
        {

        }

        /// <summary>
        /// Constructor to create file instance, opening an existing file
        /// </summary>
        /// <param name="p_s_pathToFile">full path + filename to the file</param>
        /// <param name="p_o_encoding">which encoding will be used accessing/modifying the file content</param>
        /// <param name="p_s_lineBreak">use alternative line break '\n', '\r\n', or '\r'</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public File(string p_s_pathToFile, Encoding p_o_encoding, string p_s_lineBreak) : this(p_s_pathToFile, p_o_encoding, false, p_s_lineBreak)
        {

        }

        /// <summary>
        /// Constructor to create file instance
        /// </summary>
        /// <param name="p_s_pathToFile">full path + filename to the file</param>
        /// <param name="p_o_encoding">which encoding will be used accessing/modifying the file content</param>
        /// <param name="p_b_new">flag to control if file should be created new or is already exisiting</param>
        /// <param name="p_s_lineBreak">use alternative line break '\n', '\r\n', or '\r'</param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="System.IO.FileNotFoundException"></exception>
        /// <exception cref="System.IO.IOException"></exception>
        public File(string p_s_pathToFile, Encoding p_o_encoding, bool p_b_new, string p_s_lineBreak)
        {
            if ((!p_s_lineBreak.Equals("\n")) && (!p_s_lineBreak.Equals("\r\n")) && (!p_s_lineBreak.Equals("\r")))
            {
                throw new ArgumentException("Line break parameter must be '\n', '\r\n' or '\r'");
            }

            this.s_pathToFile = p_s_pathToFile;
            this.s_filename = "";
            this.l_fileLength = 0;
            this.b_ready = false;
            this.a_fileContent = [];
            this.o_encoding = p_o_encoding;
            this.s_lineBreak = p_s_lineBreak;

            /* separate filepath from filename */
            this.s_pathToFile = this.s_pathToFile.Replace('\\', File.DIR);
            this.s_pathToFile = this.s_pathToFile.Replace('/', File.DIR);

            /* separate filepath from filename */
            this.s_filename = System.IO.Path.GetFileName(this.s_pathToFile);

            if (p_b_new)
            {
                try
                {
                    /* check if file exists */
                    if (System.IO.File.Exists(this.s_pathToFile))
                    {
                        throw new System.IO.IOException("File[" + this.s_pathToFile + "] does already exist");
                    }

                    /* check if file can be created */
                    System.IO.File.Create(this.s_pathToFile).Dispose();

                    /* check if file creation was successful */
                    if (!System.IO.File.Exists(this.s_pathToFile))
                    {
                        throw new System.IO.FileNotFoundException("File[" + this.s_pathToFile + "] cannot be created");
                    }

                    /* calculate filelength */
                    this.l_fileLength = new System.IO.FileInfo(this.s_pathToFile).Length;
                }
                catch (Exception o_exc)
                {
                    throw new System.IO.IOException("File[" + this.s_pathToFile + "] cannot be created: " + o_exc.Message);
                }

                /* set ready flag that you can use all other methods on the file */
                this.b_ready = true;
            }
            else
            {
                /* check if file exists */
                if (!System.IO.File.Exists(this.s_pathToFile))
                {
                    throw new System.IO.FileNotFoundException("File[" + this.s_pathToFile + "] does not exist");
                }

                /* check if file can be read */
                try
                {
                    /* calculate filelength */
                    this.l_fileLength = new System.IO.FileInfo(this.s_pathToFile).Length;

                    /* if filelength > 0, read content of the file */
                    if (this.l_fileLength > 0)
                    {
                        this.a_fileContent = [.. System.IO.File.ReadAllLines(this.s_pathToFile, this.o_encoding)];
                    }
                }
                catch (Exception o_exc) when (o_exc is UnauthorizedAccessException || o_exc is System.Security.SecurityException)
                {
                    throw new System.IO.IOException("Cannot read file[" + this.s_pathToFile + "] with encoding [" + this.o_encoding + "]: " + o_exc.Message);
                }
                catch (Exception o_exc)
                {
                    throw new System.IO.IOException("File[" + this.s_pathToFile + "] read access not possible: " + o_exc.Message);
                }

                /* if last element is empty, delete it */
                if ((this.l_fileLength > 0) && (Core.Helper.IsStringEmpty(this.a_fileContent[this.a_fileContent.Count - 1])))
                {
                    this.a_fileContent.RemoveAt(this.a_fileContent.Count - 1);
                }

                /* set ready flag that you can use all other methods on the file */
                this.b_ready = true;
            }
        }

        /// <summary>
        /// Read one line of open file instance
        /// </summary>
        /// <param name="p_i_line">line number</param>
        /// <returns>line as String</returns>
        /// <exception cref="InvalidOperationException">if file is not open or readable</exception>
        /// <exception cref="ArgumentException">invalid line number parameter</exception>
        public string ReadLine(int p_i_line)
        {
            if (!this.b_ready)
            {
                throw new InvalidOperationException("File[" + this.s_filename + "] cannot be used, not ready for reading");
            }

            /* nothing to read in file */
            if ((this.l_fileLength == 0) || (!System.IO.File.Exists(this.s_pathToFile)))
            {
                return "";
            }

            /* line number must be positive */
            if (!(p_i_line - 1 >= 0))
            {
                throw new ArgumentException("Line number[" + p_i_line + "] must be greater than 0");
            }

            /* if line does not exist */
            if (p_i_line - 1 >= this.a_fileContent.Count)
            {
                throw new ArgumentException("Invalid line[" + p_i_line + "]; line does not exist");
            }

            /* return line in file content */
            return this.a_fileContent[p_i_line - 1];
        }

        /// <summary>
        /// Insert new line of open file instance at the end of file
        /// </summary>
        /// <param name="p_s_value">String line which will be appended</param>
        /// <exception cref="InvalidOperationException">if file is not open or writable</exception>
        /// <exception cref="System.IO.IOException">error with write access</exception>
        public void AppendLine(string p_s_value)
        {
            if (!this.b_ready)
            {
                throw new InvalidOperationException("File[" + this.s_filename + "] cannot be used, not ready for writing");
            }

            /* insert new line at end of file */
            this.a_fileContent.Add(p_s_value);

            p_s_value += this.s_lineBreak;

            /* write file content */
            try
            {
                System.IO.File.AppendAllText(this.s_pathToFile, p_s_value, this.o_encoding);
            }
            catch (Exception o_exc)
            {
                throw new System.IO.IOException("File[" + this.s_filename + "] write access not possible: " + o_exc.Message);
            }
        }

        /// <summary>
        /// Insert new line at line position of open file instance
        /// </summary>
        /// <param name="p_s_value">String line which will be written at line position</param>
        /// <param name="p_i_line">line number where the new line will be written</param>
        /// <exception cref="InvalidOperationException">if file is not open or writable</exception>
        /// <exception cref="ArgumentException">invalid line number parameter</exception>
        /// <exception cref="System.IO.FileNotFoundException">file does not exist</exception>
        /// <exception cref="System.IO.IOException">error with read or/and write access to file instance</exception>
        public void WriteLine(string p_s_value, int p_i_line)
        {
            if (!this.b_ready)
            {
                throw new InvalidOperationException("File[" + this.s_filename + "] cannot be used, not ready for writing");
            }

            if ((p_i_line - 1) == 0)
            {
                this.a_fileContent.Insert((p_i_line - 1), p_s_value);
            }
            else
            {
                /* line number must be positive */
                if (!(p_i_line - 1 >= 0))
                {
                    throw new ArgumentException("Line number[" + p_i_line + "] must be greater 0");
                }

                /* if line does not exist */
                if (p_i_line > this.a_fileContent.Count)
                {
                    throw new ArgumentException("Line in file[" + this.s_filename + "] does not exist");
                }
                else
                {
                    /* insert new line at line position */
                    this.a_fileContent.Insert((p_i_line - 1), p_s_value);
                }
            }

            /* update file content */
            this.SetFileContent();
        }

        /// <summary>
        /// Replace line at line position of open file instance
        /// </summary>
        /// <param name="p_s_value">String line which will replace line at line position</param>
        /// <param name="p_i_line">line number where line will be replaced</param>
        /// <exception cref="InvalidOperationException">if file is not open or writable</exception>
        /// <exception cref="ArgumentException">invalid line number parameter</exception>
        /// <exception cref="System.IO.FileNotFoundException">file does not exist</exception>
        /// <exception cref="System.IO.IOException">error with read or/and write access to file instance</exception>
        public void ReplaceLine(string p_s_value, int p_i_line)
        {
            if (!this.b_ready)
            {
                throw new InvalidOperationException("File[" + this.s_filename + "] cannot be used, not ready for writing");
            }

            /* line number must be positive */
            if (!(p_i_line - 1 >= 0))
            {
                throw new ArgumentException("Line number[" + p_i_line + "] must be greater 0");
            }

            /* if line does not exist */
            if (p_i_line > this.a_fileContent.Count)
            {
                throw new ArgumentException("Invalid line[" + p_i_line + "]");
            }

            /* replace line */
            this.a_fileContent[p_i_line - 1] = p_s_value;

            /* update file content */
            this.SetFileContent();
        }

        /// <summary>
        /// Delete line at line position of open file instance
        /// </summary>
        /// <param name="p_i_line">line number where the line will be deleted</param>
        /// <exception cref="InvalidOperationException">if file is not open or writable</exception>
        /// <exception cref="ArgumentException">invalid line number parameter</exception>
        /// <exception cref="System.IO.IOException">error with read or/and write access to file instance</exception>
        public void DeleteLine(int p_i_line)
        {
            if (!this.b_ready)
            {
                throw new InvalidOperationException("File[" + this.s_filename + "] cannot be used, not ready for writing");
            }

            /* line number must be positive */
            if (!(p_i_line - 1 >= 0))
            {
                throw new ArgumentException("Line number[" + p_i_line + "] must be greater 0");
            }

            /* if line does not exist */
            if (p_i_line > this.a_fileContent.Count)
            {
                throw new ArgumentException("Invalid line[" + p_i_line + "]");
            }

            /* delete line */
            this.a_fileContent.RemoveAt(p_i_line - 1);

            /* update file content */
            if (this.a_fileContent.Count <= 0)
            {
                this.TruncateContent();
            }
            else
            {
                this.SetFileContent();
            }
        }

        /// <summary>
        /// Replace whole file content with string parameter of open file instance
        /// </summary>
        /// <param name="p_s_value">string which will replace file content, can contain line separators</param>
        /// <exception cref="InvalidOperationException">if file is not open or writable</exception>
        /// <exception cref="System.IO.IOException">error with read or/and write access to file instance</exception>
        public void ReplaceContent(string p_s_value)
        {
            if (!this.b_ready)
            {
                throw new InvalidOperationException("File[" + this.s_filename + "] cannot be used, not ready for writing");
            }

            if (!Core.Helper.IsStringEmpty(p_s_value))
            {
                /* generate file content array with the new file content string and CRLF as separate sign */
                this.a_fileContent.Clear();

                foreach (string s_line in p_s_value.Split(this.s_lineBreak))
                {
                    this.a_fileContent.Add(s_line);
                }

                /* update file content */
                this.SetFileContent();
            }
            else
            {
                this.TruncateContent();
            }
        }

        /// <summary>
        /// Replace whole file content with byte array parameter of open file instance, bytes must already have the desired encoding
        /// </summary>
        /// <param name="p_a_value">byte array which will replace file content</param>
        /// <exception cref="InvalidOperationException">if file is not open or writable</exception>
        /// <exception cref="System.IO.FileNotFoundException">file does not exist</exception>
        /// <exception cref="ArgumentException">byte array parameter is null or empty</exception>
        /// <exception cref="System.IO.IOException">error with read or/and write access to file instance</exception>
        public void ReplaceContent(byte[] p_a_value)
        {
            if (!this.b_ready)
            {
                throw new InvalidOperationException("File[" + this.s_filename + "] cannot be used, not ready for writing");
            }

            /* check if file exists */
            if (!System.IO.File.Exists(this.s_pathToFile))
            {
                throw new System.IO.FileNotFoundException("File[" + this.s_filename + "] does not exist");
            }

            /* check byte array parameter */
            if ((p_a_value == null) || (p_a_value.Length < 1))
            {
                throw new ArgumentException("Byte array parameter is null or empty");
            }

            /* write file content */
            try
            {
                System.IO.File.WriteAllBytes(this.s_pathToFile, p_a_value);
            }
            catch (Exception o_exc)
            {
                throw new System.IO.IOException("File[" + this.s_filename + "] write access not possible: " + o_exc.Message);
            }

            /* check if file can be read */
            try
            {
                /* calculate filelength */
                this.l_fileLength = new System.IO.FileInfo(this.s_pathToFile).Length;

                /* if filelength > 0, read content of the file */
                if (this.l_fileLength > 0)
                {
                    this.a_fileContent = [.. System.IO.File.ReadAllLines(this.s_pathToFile, this.o_encoding)];
                }
            }
            catch (Exception o_exc) when (o_exc is UnauthorizedAccessException || o_exc is System.Security.SecurityException)
            {
                throw new System.IO.IOException("Cannot read file[" + this.s_pathToFile + "] with encoding [" + this.o_encoding + "]: " + o_exc.Message);
            }
            catch (System.IO.IOException o_exc)
            {
                throw new System.IO.IOException("File[" + this.s_pathToFile + "] read access not possible: " + o_exc.Message);
            }

            /* if last element is empty, delete it */
            if ((this.l_fileLength > 0) && (Core.Helper.IsStringEmpty(this.a_fileContent[this.a_fileContent.Count - 1])))
            {
                this.a_fileContent.RemoveAt(this.a_fileContent.Count - 1);
            }
        }

        /// <summary>
        /// Truncate file
        /// </summary>
        /// <exception cref="InvalidOperationException">if file is not open or writable</exception>
        /// <exception cref="System.IO.IOException">error with read or/and write access to file instance</exception>
        public void TruncateContent()
        {
            if (!this.b_ready)
            {
                throw new InvalidOperationException("File[" + this.s_filename + "] cannot be used, not ready for writing");
            }

            /* truncate file content */
            try
            {
                using System.IO.FileStream fs = new(this.s_pathToFile, System.IO.FileMode.Truncate);
            }
            catch (Exception o_exc)
            {
                throw new System.IO.IOException("File[" + this.s_filename + "] cannot be truncated: " + o_exc.Message);
            }

            this.a_fileContent.Clear();
            this.l_fileLength = 0;
        }

        /// <summary>
        /// Method to store lines from memory into file
        /// </summary>
        /// <exception cref="InvalidOperationException">if file is not open or writable</exception>
        /// <exception cref="System.IO.FileNotFoundException">file does not exist</exception>
        /// <exception cref="System.IO.IOException">error with read or/and write access to file instance</exception>
        private void SetFileContent()
        {
            if (!this.b_ready)
            {
                throw new InvalidOperationException("File[" + this.s_filename + "] cannot be used, not ready for writing");
            }

            /* check if file exists */
            if (!System.IO.File.Exists(this.s_pathToFile))
            {
                throw new System.IO.FileNotFoundException("File[" + this.s_filename + "] does not exist");
            }

            StringBuilder o_stringBuilder = new();

            /* add our file content to our string builder instance */
            for (int i = 0; i < this.a_fileContent.Count; i++)
            {
                o_stringBuilder.Append(this.a_fileContent[i] + this.s_lineBreak);
            }

            /* write file content */
            try
            {
                System.IO.File.WriteAllText(this.s_pathToFile, o_stringBuilder.ToString(), this.o_encoding);
            }
            catch (Exception o_exc)
            {
                throw new System.IO.IOException("File[" + this.s_pathToFile + "] write access not possible: " + o_exc.Message);
            }

            /* calculate filelength */
            this.l_fileLength = new System.IO.FileInfo(this.s_pathToFile).Length;
        }

        /// <summary>
        /// Rename file with new filename
        /// </summary>
        /// <param name="p_s_newName">new filename</param>
        /// <exception cref="InvalidOperationException">if file is not open or writable</exception>
        /// <exception cref="ArgumentException">invalid new file name</exception>
        /// <exception cref="System.IO.IOException">error with renaming file</exception>
        public void RenameFile(string p_s_newName)
        {
            if (!this.b_ready)
            {
                throw new InvalidOperationException("File[" + this.s_filename + "] cannot be used, not ready for writing");
            }

            /* check new name parameter */
            if (Core.Helper.IsStringEmpty(p_s_newName))
            {
                throw new ArgumentException("New file name parameter is null or empty");
            }

            if ((p_s_newName.Contains('\\')) || (p_s_newName.Contains('/')))
            {
                throw new ArgumentException("New file name parameter contains path separator characters like '\\' or '/'");
            }

            /* rename file */
            try
            {
                string s_dir = new System.IO.FileInfo(this.s_pathToFile).Directory?.FullName ?? throw new ArgumentException("Could not find directory with path to file parameter '" + this.s_pathToFile + "'");
                string s_newPathToFile = System.IO.Path.Combine(s_dir, p_s_newName);

                System.IO.File.Copy(this.s_pathToFile, s_newPathToFile, true);
                System.IO.File.Delete(this.s_pathToFile);

                this.s_pathToFile = s_newPathToFile;
                this.s_filename = p_s_newName;
            }
            catch (Exception o_exc)
            {
                throw new System.IO.IOException("File[" + this.s_filename + "] cannot be renamed: " + o_exc.Message);
            }
        }

        /// <summary>
        /// Create hash string of current file instance
        /// </summary>
        /// <param name="p_s_algorithm">hash-algorithm: 'SHA-256', 'SHA-384', 'SHA-512'</param>
        /// <returns>hash string</returns>
        /// <exception cref="ArgumentException">if hash-algorithm is not 'SHA-256', 'SHA-384' or 'SHA-512'</exception>
        /// <exception cref="NullReferenceException">has method returned null value</exception>
        public string Hash(string p_s_algorithm)
        {
            return Core.Helper.HashByteArray(p_s_algorithm, this.o_encoding.GetBytes(this.FileContent)) ?? throw new NullReferenceException("Method returned 'null' with algorithm '" + p_s_algorithm + "'");
        }

        /// <summary>
        /// Check if a file exists
        /// </summary>
        /// <param name="p_s_pathToFile">full path to file</param>
        /// <returns>true - file does exist, false - file does not exist</returns>
        public static bool Exists(string p_s_pathToFile)
        {
            return System.IO.File.Exists(p_s_pathToFile);
        }

        /// <summary>
        /// Check if a directory exists, if directory path it must end with directory separator
        /// </summary>
        /// <param name="p_s_pathToDirectory">full path to directory</param>
        /// <returns>true - directory does exist, false - directory does not exist</returns>
        public static bool FolderExists(string p_s_pathToDirectory)
        {
            if (p_s_pathToDirectory.EndsWith(File.DIR))
            {
                p_s_pathToDirectory = p_s_pathToDirectory.Substring(0, p_s_pathToDirectory.Length - 1);
            }

            return File.IsDirectory(p_s_pathToDirectory);
        }

        /// <summary>
        /// Check if a file has an extension
        /// </summary>
        /// <param name="p_s_pathToFile">full path to file</param>
        /// <returns>true - file has an extension, false - file has not an extension</returns>
        public static bool HasFileExtension(string p_s_pathToFile)
        {
            return System.IO.Path.HasExtension(p_s_pathToFile);
        }

        /// <summary>
        /// Check if full path is a file
        /// </summary>
        /// <param name="p_s_pathToFile">full path to file</param>
        /// <returns>true - full path is a file, false - full path is not a file</returns>
        public static bool IsFile(string p_s_pathToFile)
        {
            return System.IO.File.Exists(p_s_pathToFile);
        }

        /// <summary>
        /// Check if full path is a directory
        /// </summary>
        /// <param name="p_s_pathToDirectory">full path to directory</param>
        /// <returns>true - full path is a directory, false - full path is not a directory</returns>
        public static bool IsDirectory(string p_s_pathToDirectory)
        {
            return System.IO.Directory.Exists(p_s_pathToDirectory);
        }

        /// <summary>
        /// Check if current path is a sub directory of other path parameter
        /// </summary>
        /// <param name="p_s_currentPath">current path</param>
        /// <param name="p_s_otherPath">other path</param>
        /// <returns>true - path parameter is a sub directory of other path , false - path parameter is not a sub directory of other path</returns>
        public static bool IsSubDirectory(string p_s_currentPath, string p_s_otherPath)
        {
            return System.IO.Path.GetFullPath(p_s_currentPath).StartsWith(System.IO.Path.GetFullPath(p_s_otherPath));
        }

        /// <summary>
        /// Get all bytes from a file
        /// </summary>
        /// <param name="p_s_pathToFile">full path to file</param>
        /// <returns>byte array or null if path is invalid file path</returns>
        /// <exception cref="System.IO.IOException">could not read all bytes from file</exception>
        public static byte[]? ReadAllBytes(string p_s_pathToFile)
        {
            if (!File.Exists(p_s_pathToFile))
            {
                return null;
            }
            else
            {
                return System.IO.File.ReadAllBytes(p_s_pathToFile);
            }
        }

        /// <summary>
        /// Get all bytes from a file using a decoding encoding
        /// </summary>
        /// <param name="p_s_pathToFile">full path to file</param>
        /// <param name="p_o_returningEncoding">the encoding to use for decoding</param>
        /// <param name="p_o_readingEncoding">the encoding to use for reading bytes from file</param>
        /// <returns>byte array or null if path is invalid file path</returns>
        /// <exception cref="System.IO.IOException">could not read all bytes from file</exception>
        public static byte[]? ReadAllBytes(string p_s_pathToFile, Encoding p_o_returningEncoding, Encoding p_o_readingEncoding)
        {
            if (!File.Exists(p_s_pathToFile))
            {
                return null;
            }
            else
            {
                return p_o_returningEncoding.GetBytes(System.IO.File.ReadAllText(p_s_pathToFile, p_o_readingEncoding));
            }
        }

        /// <summary>
        /// Get file length of full path file as long value
        /// </summary>
        /// <param name="p_s_pathToFile">full path to file</param>
        /// <returns>file length of file or -1 if file could not be read</returns>
        public static long FileLength(string p_s_pathToFile)
        {
            try
            {
                return new System.IO.FileInfo(p_s_pathToFile).Length;
            }
            catch (Exception)
            {
                return -1;
            }
        }

        /// <summary>
        /// Get last modified value of a file
        /// </summary>
        /// <param name="p_s_pathToFile">full path to file</param>
        /// <returns>DateTime or DateTime.MinValue if path is invalid file path</returns>
        public static DateTime LastModified(string p_s_pathToFile)
        {
            try
            {
                return System.IO.File.GetLastWriteTime(p_s_pathToFile);
            }
            catch (Exception)
            {
                return DateTime.MinValue;
            }
        }

        /// <summary>
        /// Creates a hash string from file content, using standard system encoding for reading file content
        /// </summary>
        /// <param name="p_s_pathTofile">full path to file</param>
        /// <param name="p_s_algorithm">hash-algorithm: 'SHA-256', 'SHA-384', 'SHA-512'</param>
        /// <returns>hash string or null if file does not exist or is not a file</returns>
        /// <exception cref="ArgumentException">if hash-algorithm is not 'SHA-256', 'SHA-384' or 'SHA-512'</exception>
        /// <exception cref="System.IO.IOException">could not read all bytes from file</exception>
        public static string? HashFile(string p_s_pathTofile, string p_s_algorithm)
        {
            if (File.Exists(p_s_pathTofile))
            {
                return Core.Helper.HashByteArray(p_s_algorithm, File.ReadAllBytes(p_s_pathTofile)) ?? null;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Creates a hash string from directory file contents, not including sub directories, using standard system encoding for reading file content
        /// </summary>
        /// <param name="p_s_pathToDirectory">full path to directory</param>
        /// <param name="p_s_algorithm">hash-algorithm: 'SHA-256', 'SHA-384', 'SHA-512'</param>
        /// <returns>hash string of directory file contents</returns>
        /// <exception cref="ArgumentException">if hash-algorithm is not 'SHA-256', 'SHA-384' or 'SHA-512'</exception>
        /// <exception cref="System.IO.IOException">could not read all bytes from file</exception>
        public static string? HashDirectory(string p_s_pathToDirectory, string p_s_algorithm)
        {
            return File.HashDirectory(p_s_pathToDirectory, p_s_algorithm, false);
        }

        /// <summary>
        /// Creates a hash string from directory and its sub directories(optional) file contents, using standard system encoding for reading file content
        /// </summary>
        /// <param name="p_s_pathToDirectory">full path to directory</param>
        /// <param name="p_s_algorithm">hash-algorithm: 'SHA-256', 'SHA-384', 'SHA-512'</param>
        /// <param name="p_b_recursive">include sub directories</param>
        /// <returns>hash string of directory and its sub directories(optional) file contents</returns>
        /// <exception cref="ArgumentException">if hash-algorithm is not 'SHA-256', 'SHA-384' or 'SHA-512'</exception>
        /// <exception cref="System.IO.IOException">could not read all bytes from file</exception>
        public static string? HashDirectory(string p_s_pathToDirectory, string p_s_algorithm, bool p_b_recursive)
        {
            /* check if full path leads to a directory */
            if (!File.IsDirectory(p_s_pathToDirectory))
            {
                throw new ArgumentException("Path '" + p_s_pathToDirectory + "' is not a directory");
            }

            /* check for a valid algorithm parameter, before we read all file contents */
            if (!(new string[] { "SHA-256", "SHA-384", "SHA-512" }).Contains(p_s_algorithm))
            {
                throw new ArgumentException("Invalid algorithm '" + p_s_algorithm + "', please use a valid algorithm['" + string.Join("', '", new string[] { "SHA-256", "SHA-384", "SHA-512" }) + "']");
            }

            /* get list of all file elements, sub directories are optional with p_b_recursive */
            List<ListingElement> a_files = File.ListDirectory(p_s_pathToDirectory, p_b_recursive);
            List<string> a_filePaths = [];

            long l_size = 0;

            /* iterate each file element */
            foreach (ListingElement o_file in a_files)
            {
                /* skip directory elements */
                if (o_file.IsDirectory)
                {
                    ForestNETLib.Core.Global.ILogFinest("skip directory '" + o_file.FullName + "'");
                    continue;
                }

                /* skip empty file name */
                if (o_file.FullName == null)
                {
                    ForestNETLib.Core.Global.ILogFinest("skip emtpy file name");
                    continue;
                }

                /* sum up all file sizes and add file element full paths */
                l_size += o_file.Size;
                a_filePaths.Add(o_file.FullName);

                ForestNETLib.Core.Global.ILogFinest("added file '" + o_file.FullName + "'");
            }

            /* overall byte array for all file contents */
            byte[] by_array = new byte[l_size];
            int i = 0;

            /* sort file element full paths, so it is always the correct order when hashing byte array */
            ForestNETLib.Core.Sorts.QuickSort(a_filePaths as List<string?>);

            /* iterate each file element full path */
            foreach (string s_file in a_filePaths)
            {
                /* read all bytes of file element */
                byte[]? a_temp = File.ReadAllBytes(s_file);

                /* add bytes to overall byte array */
                if (a_temp != null)
                {
                    foreach (byte by_value in a_temp)
                    {
                        by_array[i++] = by_value;
                    }
                }
            }

            /* returned hashed overall byte array */
            return Core.Helper.HashByteArray(p_s_algorithm, by_array);
        }

        /// <summary>
        /// Get list of windows file permissions as several acl entries
        /// </summary>
        /// <param name="p_s_path">full path to file/directory</param>
        /// <returns>list of windows file permissions as several authorization rules or null if they cannot be retrieved</returns>
        /// <exception cref="System.IO.IOException">file/path does not exist</exception>
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public static System.Security.AccessControl.AuthorizationRuleCollection? GetFilePermission(string p_s_path)
        {
            if ((!File.IsDirectory(p_s_path)) && (!File.IsFile(p_s_path)))
            {
                throw new System.IO.IOException("Path parameter[" + p_s_path + "] is not valid file or directory path");
            }

            try
            {
                System.Security.AccessControl.FileSecurity o_fileSecurity = new(
                    p_s_path,
                    System.Security.AccessControl.AccessControlSections.Owner |
                    System.Security.AccessControl.AccessControlSections.Group |
                    System.Security.AccessControl.AccessControlSections.Access
                );

                return o_fileSecurity.GetAccessRules(true, true, typeof(System.Security.Principal.NTAccount));
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Set windows file permission with FileSecurity instance you must prepare before
        /// </summary>
        /// <param name="p_s_path">full path to file/directory</param>
        /// <param name="p_a_fileSecurityPermissions">FileSecurity instance parameter</param>
        /// <returns>true - permission could be set, false - error while setting permission</returns>
        /// <exception cref="System.IO.IOException">path does not exist</exception>
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public static bool SetFilePermission(string p_s_path, System.Security.AccessControl.FileSystemSecurity p_a_fileSecurityPermissions)
        {
            if ((!File.IsDirectory(p_s_path)) && (File.IsFile(p_s_path)))
            {
                throw new System.IO.IOException("Path parameter[" + p_s_path + "] is not valid file or directory path");
            }

            try
            {
                if (File.IsDirectory(p_s_path))
                {
                    new System.IO.DirectoryInfo(p_s_path).SetAccessControl((System.Security.AccessControl.DirectorySecurity)p_a_fileSecurityPermissions);
                }
                else if (File.IsFile(p_s_path))
                {
                    new System.IO.FileInfo(p_s_path).SetAccessControl((System.Security.AccessControl.FileSecurity)p_a_fileSecurityPermissions);
                }
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get owner of a file/directory
        /// </summary>
        /// <param name="p_s_path">full path to file/directory</param>
        /// <returns>owner as string value or null if this information could not be retrieved</returns>
        /// <exception cref="System.IO.IOException">path does not exist</exception>
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public static string? GetOwner(string p_s_path)
        {
            if ((!File.IsDirectory(p_s_path)) && (File.IsFile(p_s_path)))
            {
                throw new System.IO.IOException("Path parameter[" + p_s_path + "] is not valid file or directory path");
            }

            try
            {
                System.Security.AccessControl.FileSecurity o_fileSecurity = new(
                    p_s_path,
                    System.Security.AccessControl.AccessControlSections.Owner |
                    System.Security.AccessControl.AccessControlSections.Group |
                    System.Security.AccessControl.AccessControlSections.Access
                );

                return o_fileSecurity.GetOwner(typeof(System.Security.Principal.SecurityIdentifier))?.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Set owner of a file/directory
        /// </summary>
        /// <param name="p_s_path">full path to file/directory</param>
        /// <param name="p_s_newOwner">new owner as string value</param>
        /// <returns>true - owner changed, false - owner could not be changed</returns>
        /// <exception cref="System.IO.IOException">path does not exist</exception>
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public static bool SetOwner(string p_s_path, string p_s_newOwnerDomain, string p_s_newOwnerName)
        {
            if ((!File.IsDirectory(p_s_path)) && (File.IsFile(p_s_path)))
            {
                throw new System.IO.IOException("Path parameter[" + p_s_path + "] is not valid file or directory path");
            }

            try
            {
                System.Security.AccessControl.FileSecurity o_fileSecurity = new(
                    p_s_path,
                    System.Security.AccessControl.AccessControlSections.Owner |
                    System.Security.AccessControl.AccessControlSections.Group |
                    System.Security.AccessControl.AccessControlSections.Access
                );

                o_fileSecurity.SetOwner(new System.Security.Principal.NTAccount(p_s_newOwnerDomain, p_s_newOwnerName));
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Get group of a file/directory
        /// </summary>
        /// <param name="p_s_path">full path to file/directory</param>
        /// <returns>group as string value or null if this information could not be retrieved</returns>
        /// <exception cref="System.IO.IOException">path does not exist</exception>
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public static string? GetGroup(string p_s_path)
        {
            if ((!File.IsDirectory(p_s_path)) && (File.IsFile(p_s_path)))
            {
                throw new System.IO.IOException("Path parameter[" + p_s_path + "] is not valid file or directory path");
            }

            try
            {
                System.Security.AccessControl.FileSecurity o_fileSecurity = new(
                    p_s_path,
                    System.Security.AccessControl.AccessControlSections.Owner |
                    System.Security.AccessControl.AccessControlSections.Group |
                    System.Security.AccessControl.AccessControlSections.Access
                );

                return o_fileSecurity.GetGroup(typeof(System.Security.Principal.SecurityIdentifier))?.ToString();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Set group of a file/directory
        /// </summary>
        /// <param name="p_s_path">full path to file/directory</param>
        /// <param name="p_s_newGroupDomain">new group domain as string value</param>
        /// <param name="p_s_newGroupName">new group domain as string value</param>
        /// <returns>true - group changed, false - group could not be changed</returns>
        /// <exception cref="System.IO.IOException">path does not exist</exception>
        [System.Runtime.Versioning.SupportedOSPlatform("windows")]
        public static bool SetGroup(string p_s_path, string p_s_newGroupDomain, string p_s_newGroupName)
        {
            if ((!File.IsDirectory(p_s_path)) && (File.IsFile(p_s_path)))
            {
                throw new System.IO.IOException("Path parameter[" + p_s_path + "] is not valid file or directory path");
            }

            try
            {
                System.Security.AccessControl.FileSecurity o_fileSecurity = new(
                    p_s_path,
                    System.Security.AccessControl.AccessControlSections.Owner |
                    System.Security.AccessControl.AccessControlSections.Group |
                    System.Security.AccessControl.AccessControlSections.Access
                );

                o_fileSecurity.SetGroup(new System.Security.Principal.NTAccount(p_s_newGroupDomain, p_s_newGroupName));
            }
            catch (Exception)
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Copy file from source location to destination location incl. filename, destination will be overwritten if it already exists
        /// </summary>
        /// <param name="p_s_source">full path of source file</param>
        /// <param name="p_s_destination">full path of destination file</param>
        /// <exception cref="System.IO.FileNotFoundException">file does not exist</exception>
        /// <exception cref="System.IO.IOException">copy process could not be completed successfully</exception>
        public static void CopyFile(string p_s_source, string p_s_destination)
        {
            /* check if source file exists */
            if (!File.Exists(p_s_source))
            {
                throw new System.IO.FileNotFoundException("File[" + p_s_source + "] does not exist");
            }

            /* copy file */
            try
            {
                System.IO.File.Copy(p_s_source, p_s_destination, true);
            }
            catch (Exception o_exc)
            {
                throw new System.IO.IOException("File[" + p_s_source + "] cannot be copied: " + o_exc.Message);
            }
        }

        /// <summary>
        /// Move file from source location to destination location incl. filename
        /// </summary>
        /// <param name="p_s_source">full path of source file</param>
        /// <param name="p_s_destination">full path of destination file</param>
        /// <exception cref="System.IO.FileNotFoundException">file does not exist</exception>
        /// <exception cref="System.IO.IOException">move process could not be completed successfully</exception>
        public static void MoveFile(string p_s_source, string p_s_destination)
        {
            /* check if source file exists */
            if (!File.Exists(p_s_source))
            {
                throw new System.IO.FileNotFoundException("File[" + p_s_source + "] does not exist");
            }

            /* move file */
            try
            {
                System.IO.File.Move(p_s_source, p_s_destination);
            }
            catch (Exception o_exc)
            {
                throw new System.IO.IOException("File[" + p_s_source + "] cannot be moved: " + o_exc.Message);
            }
        }

        /// <summary>
        /// Delete file
        /// </summary>
        /// <param name="p_s_source">full path of source file</param>
        /// <exception cref="System.IO.FileNotFoundException">file does not exist</exception>
        /// <exception cref="System.IO.IOException">delete process could not be completed successfully</exception>
        public static void DeleteFile(string p_s_source)
        {
            /* check if source file exists */
            if (!File.Exists(p_s_source))
            {
                throw new System.IO.FileNotFoundException("File[" + p_s_source + "] does not exist");
            }

            /* delete file */
            try
            {
                System.IO.File.Delete(p_s_source);
            }
            catch (Exception o_exc)
            {
                throw new System.IO.IOException("File[" + p_s_source + "] could not be deleted: " + o_exc.Message);
            }
        }

        /// <summary>
        /// List directory elements
        /// </summary>
        /// <param name="p_s_path">full path to file/directory</param>
        /// <returns>list of ListingElement objects</returns>
        /// <exception cref="ArgumentException">path does not exist</exception>
        /// <exception cref="System.IO.IOException">directory could not be listed or issue with reading file/directory attributes</exception>
        public static List<ListingElement> ListDirectory(string p_s_path)
        {
            return File.ListDirectory(p_s_path, false);
        }

        /// <summary>
        /// List directory elements
        /// </summary>
        /// <param name="p_s_path">full path to file/directory</param>
        /// <param name="p_b_recursive">include all sub directories</param>
        /// <returns>list of ListingElement objects</returns>
        /// <exception cref="ArgumentException">path does not exist</exception>
        /// <exception cref="System.IO.IOException">directory could not be listed or issue with reading file/directory attributes</exception>
        public static List<ListingElement> ListDirectory(string p_s_path, bool p_b_recursive)
        {
            List<ListingElement> a_listing = [];

            /* check if parameter is not a directory */
            if (File.IsFile(p_s_path))
            {
                /* get file info */
                System.IO.FileInfo o_basicFileAttributes = new(p_s_path);

                /* add element as a new listing object to our listing */
                a_listing.Add(
                    new ListingElement(
                        o_basicFileAttributes.Name,
                        o_basicFileAttributes.FullName,
                        false,
                        o_basicFileAttributes.Length,
                        o_basicFileAttributes.CreationTime,
                        o_basicFileAttributes.LastAccessTime,
                        o_basicFileAttributes.LastWriteTime
                    )
                );
            }
            else if (File.IsDirectory(p_s_path)) /* parameter is a directory */
            {
                /* get directory info */
                System.IO.DirectoryInfo o_directoryInfo = new(p_s_path);

                /* iterate all directories */
                foreach (System.IO.DirectoryInfo o_directory in o_directoryInfo.GetDirectories())
                {
                    /* not iterate a directory element with the same name as parameter, preventing endless loop in recursion */
                    if (!o_directory.FullName.Equals(p_s_path))
                    {
                        /* add element as a new listing object to our listing */
                        a_listing.Add(
                            new ListingElement(
                                o_directory.Name,
                                o_directory.FullName,
                                true,
                                0,
                                o_directory.CreationTime,
                                o_directory.LastAccessTime,
                                o_directory.LastWriteTime
                            )
                        );

                        /* if element is a directory and we want to scan the directory recursive, continue listing with recursion */
                        if (p_b_recursive)
                        {
                            try
                            {
                                /* iterate all elements in recursion to our listing */
                                foreach (ListingElement o_listingObject in File.ListDirectory(o_directory.FullName, p_b_recursive))
                                {
                                    a_listing.Add(o_listingObject);
                                }
                            }
                            catch (Exception)
                            {
                                /* nothing to do */
                            }
                        }
                    }
                }

                /* iterate all files */
                foreach (System.IO.FileInfo o_file in o_directoryInfo.GetFiles())
                {
                    /* not iterate a file element with the same name as parameter, preventing endless loop in recursion */
                    if (!o_file.FullName.Equals(p_s_path))
                    {
                        /* add element as a new listing object to our listing */
                        a_listing.Add(
                            new ListingElement(
                                o_file.Name,
                                o_file.FullName,
                                false,
                                o_file.Length,
                                o_file.CreationTime,
                                o_file.LastAccessTime,
                                o_file.LastWriteTime
                            )
                        );
                    }
                }
            }
            else
            {
                throw new ArgumentException("Path parameter[" + p_s_path + "] is not valid file or directory path");
            }

            return a_listing;
        }

        /// <summary>
        /// Create a directory
        /// </summary>
        /// <param name="p_s_source">full path to target directory</param>
        /// <exception cref="IllegalArgumentException">invalid directory path or directory already exists</exception>
        /// <exception cref="System.IO.IOException">directory could not be created</exception>
        public static void CreateDirectory(string p_s_source)
        {
            File.CreateDirectory(p_s_source, false);
        }

        /// <summary>
        /// Create a directory
        /// </summary>
        /// <param name="p_s_source">full path to target directory</param>
        /// <param name="p_b_autoCreate">true - create path to target directory if directories to this target are missing</param>
        /// <exception cref="ArgumentException">invalid directory path or directory already exists</exception>
        /// <exception cref="System.IO.IOException">directory could not be created</exception>
        public static void CreateDirectory(string p_s_source, bool p_b_autoCreate)
        {
            /* check if we do not have a filename as parameter */
            if (p_s_source.Substring(p_s_source.LastIndexOf(File.DIR)).Contains('.'))
            {
                throw new ArgumentException("Directory parameter contains a filename with '.' -> [" + p_s_source + "]");
            }

            /* check if directory exists */
            if (File.IsDirectory(p_s_source))
            {
                throw new ArgumentException("Directory[" + p_s_source + "] does already exist");
            }

            /* create path to target directory if directories to this target are missing */
            if (p_b_autoCreate)
            {
                /* prepare directories iteration */
                string s_foo = "";
                string[] a_directories = p_s_source.Split(File.DIR);

                /* iterate each directory until target directory has been reached */
                for (int i = 0; i < (a_directories.Length - 1); i++)
                {
                    /* add next directory to target path */
                    s_foo += a_directories[i] + File.DIR;

                    /* if directory path does not exist */
                    if (!File.IsDirectory(s_foo))
                    {
                        try
                        {
                            /* create directory */
                            System.IO.Directory.CreateDirectory(s_foo);

                            ForestNETLib.Core.Global.ILogFinest("auto create directory '" + s_foo + "'");
                        }
                        catch (Exception o_exc)
                        {
                            throw new System.IO.IOException("Directory[" + s_foo + "] could not be created: " + o_exc.Message);
                        }
                    }
                }
            }

            try
            {
                /* create directory */
                System.IO.Directory.CreateDirectory(p_s_source);
            }
            catch (Exception o_exc)
            {
                throw new System.IO.IOException("Directory[" + p_s_source + "] could not be created: " + o_exc.Message);
            }
        }

        /// <summary>
        /// Copy directory to a destination directory path, incl. sub directories with file contents
        /// </summary>
        /// <param name="p_s_source">full path to source directory</param>
        /// <param name="p_s_destination">full path to destination directory</param>
        /// <exception cref="ArgumentException">directory does not exist</exception>
        /// <exception cref="System.IO.IOException">file or directory could not be copied or destination directory could not be created</exception>
        public static void CopyDirectory(string p_s_source, string p_s_destination)
        {
            /* check if directory exists */
            if (!File.IsDirectory(p_s_source))
            {
                throw new ArgumentException("Directory" + p_s_source + "] does not exist");
            }

            /* create destination directory */
            File.CreateDirectory(p_s_destination);

            /* copy content of directory recursive */
            try
            {
                /* get all information about source directory */
                System.IO.DirectoryInfo o_sourceDirectoryInfo = new(p_s_source);

                /* iterate each directory */
                foreach (System.IO.DirectoryInfo o_directoryInfo in o_sourceDirectoryInfo.GetDirectories())
                {
                    /* not iterate a directory element with the same name as parameter, preventing endless loop in recursion */
                    if (!o_directoryInfo.FullName.Equals(p_s_source))
                    {
                        /* copy directory recursively */
                        File.CopyDirectory(o_directoryInfo.FullName, p_s_destination + File.DIR + o_directoryInfo.Name);

                        ForestNETLib.Core.Global.ILogFinest("copied directory '" + o_directoryInfo.Name + "'");
                    }
                }

                /* iterate each file */
                foreach (System.IO.FileInfo o_fileInfo in o_sourceDirectoryInfo.GetFiles())
                {
                    /* not iterate a file element with the same name as parameter, preventing endless loop in recursion */
                    if (!o_fileInfo.FullName.Equals(p_s_source))
                    {
                        /* copy file */
                        File.CopyFile(o_fileInfo.FullName, p_s_destination + File.DIR + o_fileInfo.Name);

                        ForestNETLib.Core.Global.ILogFinest("copied file '" + o_fileInfo.FullName + "'");
                    }
                }
            }
            catch (Exception o_exc)
            {
                throw new System.IO.IOException("CopyDirectory exception: " + o_exc.Message);
            }
        }

        /// <summary>
        /// Move directory to a destination directory path, incl. sub directories with file contents
        /// </summary>
        /// <param name="p_s_source">full path to source directory</param>
        /// <param name="p_s_destination">full path to destination directory</param>
        /// <exception cref="ArgumentException">source or destination directory does not exist</exception>
        /// <exception cref="System.IO.IOException">file or directory could not be moved or source directory could not be deleted</exception>
        public static void MoveDirectory(string p_s_source, string p_s_destination)
        {
            /* check if source directory exists */
            if (!File.IsDirectory(p_s_source))
            {
                throw new ArgumentException("Source directory[" + p_s_source + "] does not exist");
            }

            /* check if destination directory exists */
            if (File.IsDirectory(p_s_destination))
            {
                throw new ArgumentException("Destination directory[" + p_s_destination + "] does already exist");
            }

            /* move directory */
            try
            {
                File.CopyDirectory(p_s_source, p_s_destination);
                ForestNETLib.Core.Global.ILogFinest("copied directory '" + p_s_source + "'");
                File.DeleteDirectory(p_s_source);
                ForestNETLib.Core.Global.ILogFinest("deleted directory '" + p_s_source + "'");
            }
            catch (Exception o_exc)
            {
                throw new System.IO.IOException("Directory[" + p_s_source + "] cannot be moved: " + o_exc.Message);
            }
        }

        /// <summary>
        /// Rename a directory
        /// </summary>
        /// <param name="p_s_source">full path to source directory</param>
        /// <param name="p_s_destination">full path to destination directory</param>
        /// <exception cref="ArgumentException">source or destination directory does not exist</exception>
        /// <exception cref="System.IO.IOException">directory could not be renamed</exception>
        public static void RenameDirectory(string p_s_source, string p_s_destination)
        {
            File.MoveDirectory(p_s_source, p_s_destination);
        }

        /// <summary>
        /// Delete a directory, incl. all files and sub directories
        /// </summary>
        /// <param name="p_s_source">full path to directory</param>
        /// <exception cref="ArgumentException">directory does not exist</exception>
        /// <exception cref="System.IO.IOException">directory could not be deleted</exception>
        public static void DeleteDirectory(string p_s_source)
        {
            /* check if directory exists */
            if (!File.IsDirectory(p_s_source))
            {
                throw new ArgumentException("Directory" + p_s_source + "] does not exist");
            }

            /* delete content of directory recursive */
            try
            {
                /* get all information about source directory */
                System.IO.DirectoryInfo o_sourceDirectoryInfo = new(p_s_source);

                /* iterate each directory */
                foreach (System.IO.DirectoryInfo o_directoryInfo in o_sourceDirectoryInfo.GetDirectories())
                {
                    /* not iterate a directory element with the same name as parameter, preventing endless loop in recursion */
                    if (!o_directoryInfo.FullName.Equals(p_s_source))
                    {
                        /* copy directory recursively */
                        File.DeleteDirectory(o_directoryInfo.FullName);
                        ForestNETLib.Core.Global.ILogFinest("deleted directory '" + o_directoryInfo.Name + "'");
                    }
                }

                /* iterate each file */
                foreach (System.IO.FileInfo o_fileInfo in o_sourceDirectoryInfo.GetFiles())
                {
                    /* not iterate a file element with the same name as parameter, preventing endless loop in recursion */
                    if (!o_fileInfo.FullName.Equals(p_s_source))
                    {
                        /* copy file */
                        File.DeleteFile(o_fileInfo.FullName);
                        ForestNETLib.Core.Global.ILogFinest("deleted file '" + o_fileInfo.Name + "'");
                    }
                }
            }
            catch (Exception o_exc)
            {
                throw new System.IO.IOException("DeleteDirectory exception: " + o_exc.Message);
            }

            /* delete directory */
            try
            {
                System.IO.Directory.Delete(p_s_source, false);
            }
            catch (Exception o_exc)
            {
                throw new System.IO.IOException("Directory[" + p_s_source + "] could not be deleted: " + o_exc.Message);
            }
        }

        /// <summary>
        /// Create hex file folder structure.
        /// 2-character-length [00-FF].
        /// with leading zero.
        /// </summary>
        /// <param name="p_s_source">parent directory full path of the new file folder structure</param>
        /// <exception cref="ArgumentException">directory does not exist</exception>
        /// <exception cref="System.IO.IOException">a directory could not be created</exception>
        public static void CreateHexFileFolderStructure(string p_s_source)
        {
            /* check if directory exists */
            if (File.IsDirectory(p_s_source))
            {
                throw new ArgumentException("Directory" + p_s_source + "] does already exist");
            }

            /* create 256 directories */
            for (int i = 0; i < 256; i++)
            {
                /* hex folder name with capital letters */
                string s_hex = i.ToString("X").ToUpper();

                /* with leading zero */
                if (s_hex.Length == 1)
                {
                    s_hex = "0" + s_hex;
                }

                /* create new hex directory */
                File.CreateDirectory(p_s_source + File.DIR + s_hex);

                if (ForestNETLib.Core.Global.IsILevel((byte)ForestNETLib.Log.Level.MASS)) ForestNETLib.Core.Global.ILogMass("created sub directory '" + s_hex + "'");
            }
        }

        /// <summary>
        /// Generate random file content, using default ASCII encoding and default line length of 256.
        /// automatic length of 1 KB.
        /// </summary>
        /// <returns>StringBuilder instance</returns>
        public static StringBuilder GenerateRandomFileContent_1KB()
        {
            return File.GenerateRandomFileContent(256, 1024L);
        }

        /// <summary>
        /// Generate random file content, using default ASCII encoding and default line length of 256.
        /// automatic length of 1 MB.
        /// </summary>
        /// <returns>StringBuilder instance</returns>
        public static StringBuilder GenerateRandomFileContent_1MB()
        {
            return File.GenerateRandomFileContent(256, 1024L * 1024L);
        }

        /// <summary>
        /// Generate random file content, using default ASCII encoding and default line length of 256.
        /// automatic length of 10 MB.
        /// </summary>
        /// <returns>StringBuilder instance</returns>
        public static StringBuilder GenerateRandomFileContent_10MB()
        {
            return File.GenerateRandomFileContent(256, 10L * 1024L * 1024L);
        }

        /// <summary>
        /// Generate random file content, using default ASCII encoding and default line length of 256.
        /// automatic length of 50 MB.
        /// </summary>
        /// <returns>StringBuilder instance</returns>
        public static StringBuilder GenerateRandomFileContent_50MB()
        {
            return File.GenerateRandomFileContent(256, 50L * 1024L * 1024L);
        }

        /// <summary>
        /// Generate random file content, using default ASCII encoding and default line length of 256.
        /// automatic length of 100 MB.
        /// </summary>
        /// <returns>StringBuilder instance</returns>
        public static StringBuilder GenerateRandomFileContent_100MB()
        {
            return File.GenerateRandomFileContent(256, 100L * 1024L * 1024L);
        }

        /// <summary>
        /// Generate random file content, using default ASCII encoding and default line length of 256.
        /// automatic length of 250 MB.
        /// </summary>
        /// <returns>StringBuilder instance</returns>
        public static StringBuilder GenerateRandomFileContent_250MB()
        {
            return File.GenerateRandomFileContent(256, 250L * 1024L * 1024L);
        }

        /// <summary>
        /// Generate random file content, using default ASCII encoding and default line length of 256.
        /// automatic length of 500 MB.
        /// </summary>
        /// <returns>StringBuilder instance</returns>
        public static StringBuilder GenerateRandomFileContent_500MB()
        {
            return File.GenerateRandomFileContent(256, 500L * 1024L * 1024L);
        }

        /// <summary>
        /// Generate random file content, using default ASCII encoding and default line length of 256.
        /// automatic length of 1 GB.
        /// </summary>
        /// <returns>StringBuilder instance</returns>
        public static StringBuilder GenerateRandomFileContent_1GB()
        {
            return File.GenerateRandomFileContent(256, 1024L * 1024L * 1024L);
        }

        /// <summary>
        /// Generate random file content, using default ASCII encoding and default line length of 256
        /// </summary>
        /// <param name="p_l_length">length of random content in bytes</param>
        /// <returns>StringBuilder instance</returns>
        public static StringBuilder GenerateRandomFileContent(long p_l_length)
        {
            return File.GenerateRandomFileContent(256, p_l_length);
        }

        /// <summary>
        /// Generate random file content, using default ASCII encoding
        /// </summary>
        /// <param name="p_i_lineLength">line length within random file content</param>
        /// <param name="p_l_length">length of random content in bytes</param>
        /// <returns>StringBuilder instance</returns>
        public static StringBuilder GenerateRandomFileContent(int p_i_lineLength, long p_l_length)
        {
            return File.GenerateRandomFileContent(Encoding.ASCII, p_i_lineLength, p_l_length);
        }

        /// <summary>
        /// Generate random file content
        /// </summary>
        /// <param name="p_o_encodingWrite">the encoding will be used when creating random content characters</param>
        /// <param name="p_i_lineLength">line length within random file content</param>
        /// <param name="p_l_length">length of random content in bytes</param>
        /// <returns>StringBuilder instance</returns>
        public static StringBuilder GenerateRandomFileContent(Encoding p_o_encodingWrite, int p_i_lineLength, long p_l_length)
        {
            /* check charset write parameter */
            if (p_o_encodingWrite == null)
            {
                throw new ArgumentException("Charset write parameter is empty");
            }

            /* check line length parameter */
            if (p_i_lineLength < 1)
            {
                throw new ArgumentException("Line length parameter must be positive");
            }

            /* check length parameter */
            if (p_l_length < 1)
            {
                throw new ArgumentException("Length parameter must be positive");
            }

            /* check length parameter not overflow */
            if (p_l_length > long.MaxValue)
            {
                throw new ArgumentException("Max. value of length parameter is " + long.MaxValue);
            }

            StringBuilder o_stringBuilder = new();
            int i_lineLength = 0;

            /* create each byte for random content */
            for (long i = 0; i < p_l_length; i++)
            {
                /* if we reached end of line */
                if ((i_lineLength != 0) && ((i_lineLength % p_i_lineLength) == 0))
                {
                    /* generate random bytes */
                    byte[] a_bytes = new byte[i_lineLength];
                    new Random().NextBytes(a_bytes);

                    /* add bytes to random content, using encoding parameter */
                    o_stringBuilder.Append(p_o_encodingWrite.GetString(a_bytes));

                    /* append line separator */
                    o_stringBuilder.Append(p_o_encodingWrite.GetString(p_o_encodingWrite.GetBytes(File.NEWLINE.ToCharArray())));
                    /* increase counter with length of line separator */
                    i += File.NEWLINE.Length;

                    /* reset line length counter */
                    i_lineLength = 0;
                }
                else
                {
                    /* increment line length counter */
                    i_lineLength++;
                }
            }

            /* create remaining random bytes for last line */
            if ((p_l_length - o_stringBuilder.Length) > 0)
            {
                /* generate random bytes */
                byte[] a_bytes = new byte[(p_l_length - o_stringBuilder.Length)];
                new Random().NextBytes(a_bytes);

                /* add bytes to random content, using encoding parameter */
                o_stringBuilder.Append(p_o_encodingWrite.GetString(a_bytes));
            }

            /* return random content */
            return o_stringBuilder;
        }

        /// <summary>
        /// Replace file content with content of string builder, default ENCODING constant from File class is used
        /// </summary>
        /// <param name="p_s_pathToFile">full file path to change its content</param>
        /// <param name="p_o_stringBuilder">new content gathered in string builder instance</param>
        /// <exception cref="ArgumentException">invalid file path, empty string builder or empty encoding parameter</exception>
        /// <exception cref="System.IO.FileNotFoundException">invalid file path</exception>
        /// <exception cref="System.IO.IOException">write access to file not possible</exception>
        public static void ReplaceFileContent(string p_s_pathToFile, StringBuilder p_o_stringBuilder)
        {
            ReplaceFileContent(p_s_pathToFile, p_o_stringBuilder, Encoding.GetEncoding(File.ENCODING));
        }

        /// <summary>
        /// Replace file content with content of string builder
        /// </summary>
        /// <param name="p_s_pathToFile">full file path to change its content</param>
        /// <param name="p_o_stringBuilder">new content gathered in string builder instance</param>
        /// <param name="p_o_encoding">encoding instance will be used when writing new content to file</param>
        /// <exception cref="ArgumentException">invalid file path, empty string builder or empty encoding parameter</exception>
        /// <exception cref="System.IO.FileNotFoundException">invalid file path</exception>
        /// <exception cref="System.IO.IOException">write access to file not possible</exception>
        public static void ReplaceFileContent(string p_s_pathToFile, StringBuilder p_o_stringBuilder, Encoding p_o_encoding)
        {
            /* check file path parameter */
            if (Core.Helper.IsStringEmpty(p_s_pathToFile))
            {
                throw new ArgumentException("File path parameter is empty");
            }

            /* check string builder parameter */
            if (p_o_stringBuilder == null)
            {
                throw new ArgumentException("String builder parameter is empty");
            }

            /* check charset parameter */
            if (p_o_encoding == null)
            {
                throw new ArgumentException("Charset parameter is empty");
            }

            /* check if file exists */
            if (!File.Exists(p_s_pathToFile))
            {
                throw new System.IO.FileNotFoundException("File[" + p_s_pathToFile + "] does not exist");
            }

            /* write file content */
            try
            {
                System.IO.File.WriteAllText(p_s_pathToFile, p_o_stringBuilder.ToString(), p_o_encoding);
            }
            catch (Exception o_exc)
            {
                throw new System.IO.IOException("File[" + p_s_pathToFile + "] write access not possible: " + o_exc.Message);
            }
        }
    }
}
