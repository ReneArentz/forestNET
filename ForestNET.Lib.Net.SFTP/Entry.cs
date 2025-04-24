namespace ForestNET.Lib.Net.SFTP
{
    /// <summary>
    /// Data holder class to hold all information about a file or directory on a sftp server, inheriting from ForestNET.Lib.Net.FTP.Entry.
    /// </summary>
    public class Entry : ForestNET.Lib.Net.FTP.Entry
    {

        /* Fields */

        /* Properties */

        /* Methods */

        /// <summary>
        /// Constructor to create a sftp entry object
        /// </summary>
        /// <param name="p_s_name">name of file or directory</param>
        /// <param name="p_s_group">name of assigned group of entry on sftp server</param>
        /// <param name="p_s_user">name of assigned user of entry on sftp server</param>
        /// <param name="p_s_path">path to file or directory</param>
        /// <param name="p_s_access">copy of sftp raw listing of user, group and all access rights</param>
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
        ) : base(p_s_name, p_s_group, p_s_user, p_s_path, p_s_access, p_l_size, p_o_timestamp, p_b_directory)
        {

        }
    }
}
