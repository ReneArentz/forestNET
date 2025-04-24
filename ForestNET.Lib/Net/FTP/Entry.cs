namespace ForestNET.Lib.Net.FTP
{
    /// <summary>
    /// Data holder class to hold all information about a file or directory on a ftp server.
    /// </summary>
    public class Entry
    {

        /* Fields */

        /* Properties */

        public string Name { get; private set; }
        public string Group { get; private set; }
        public string User { get; private set; }
        public string Path { get; private set; }
        public string FullPath { get { return this.Path + this.Name; } }
        public string Access { get; private set; }
        public long Size { get; private set; }
        public DateTime Timestamp { get; private set; }
        public bool IsDirectory { get; private set; }

        /* Methods */

        /// <summary>
        /// Constructor to create a ftp entry object
        /// </summary>
        /// <param name="p_s_name">name of file or directory</param>
        /// <param name="p_s_group">name of assigned group of entry on ftp server</param>
        /// <param name="p_s_user">name of assigned user of entry on ftp server</param>
        /// <param name="p_s_path">path to file or directory</param>
        /// <param name="p_s_access">copy of ftp raw listing of user, group and all access rights (e.g. rw-r-----)</param>
        /// <param name="p_l_size">file size</param>
        /// <param name="p_o_timestamp">timestamp value of file or directory</param>
        /// <param name="p_b_directory">true - entry is directory, false - entry is file</param>
        /// <exception cref="ArgumentException">parameter value is null or empty, or in case of a numeric value is lower than 0</exception>
        public Entry(
            string p_s_name,
            string p_s_group,
            string p_s_user,
            string p_s_path,
            string p_s_access,
            long p_l_size,
            DateTime p_o_timestamp,
            bool p_b_directory
        )
        {
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_name))
            {
                throw new ArgumentException("'Name' is null or empty");
            }

            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_path))
            {
                throw new ArgumentException("'Path' is null or empty");
            }

            if (p_l_size < -1)
            {
                throw new ArgumentException("'Size' is lower than '-1'");
            }

            this.Name = p_s_name;
            this.Group = p_s_group;
            this.User = p_s_user;
            this.Path = p_s_path;
            this.Access = p_s_access;
            this.Size = p_l_size;
            this.Timestamp = p_o_timestamp;
            this.IsDirectory = p_b_directory;
        }

        /// <summary>
        /// Easy way to show all information of ftp entry and all properties in one string line
        /// </summary>
        override public string ToString()
        {
            string s_foo = "";

            foreach (System.Reflection.PropertyInfo o_property in this.GetType().GetProperties())
            {
                string s_value = "";

                try
                {
                    s_value = o_property.GetValue(this)?.ToString() ?? "null";
                }
                catch (Exception)
                {
                    s_value = "null";
                }

                s_foo += o_property.Name + " = " + s_value + "|";
            }

            s_foo = s_foo.Substring(0, s_foo.Length - 1);

            return s_foo;
        }
    }
}