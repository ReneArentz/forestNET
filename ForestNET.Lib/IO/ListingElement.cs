namespace ForestNET.Lib.IO
{
    /// <summary>
    /// Listing element class which is holding general information about a file or a directory element.
    /// </summary>
    public class ListingElement
    {

        /* Fields */

        /* Properties */

        public string? Name { get; private set; }
        public string? FullName { get; private set; }
        public bool IsDirectory { get; private set; }
        public long Size { get; private set; }
        public DateTime? CreationTime { get; private set; }
        public DateTime? LastAccessTime { get; private set; }
        public DateTime? LastModifiedTime { get; private set; }

        /* Methods */

        /// <summary>
        /// Default constructor, all fields have value null, 0 or false
        /// </summary>
        public ListingElement() : this(null, null, false, 0, null, null, null)
        {

        }

        /// <summary>
        /// Default constructor, listing element size is set to 0 and time instances are set to null
        /// </summary>
        /// <param name="p_s_name">name of listing element</param>
        /// <param name="p_s_fullName">full path + name of listing element</param>
        /// <param name="p_b_isDirectory">information if listing element is a directory</param>
        public ListingElement(string? p_s_name, string? p_s_fullName, bool p_b_isDirectory) : this(p_s_name, p_s_fullName, p_b_isDirectory, 0, null, null, null)
        {

        }

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="p_s_name">name of listing element</param>
        /// <param name="p_s_fullName">full path + name of listing element</param>
        /// <param name="p_b_isDirectory">information if listing element is a directory</param>
        /// <param name="p_l_size">file size of listing element</param>
        /// <param name="p_o_creationTime">creation time of listing element</param>
        /// <param name="p_o_lastAccessTime">last access time of listing element</param>
        /// <param name="p_o_lastModifiedTime">last modified time of listing element</param>
        public ListingElement(string? p_s_name, string? p_s_fullName, bool p_b_isDirectory, long p_l_size, DateTime? p_o_creationTime = null, DateTime? p_o_lastAccessTime = null, DateTime? p_o_lastModifiedTime = null)
        {
            this.Name = p_s_name;
            this.FullName = p_s_fullName;
            this.IsDirectory = p_b_isDirectory;
            this.Size = p_l_size;
            this.CreationTime = p_o_creationTime ?? DateTime.MinValue;
            this.LastAccessTime = p_o_lastAccessTime ?? DateTime.MinValue;
            this.LastModifiedTime = p_o_lastModifiedTime ?? DateTime.MinValue;
        }

        /// <summary>
        /// Return all listing element information fields as string line
        /// </summary>
        /// <returns>string line with all listing element information fields, separated by '|'-sign</returns>
        override public string ToString()
        {
            System.Text.StringBuilder s_foo = new();

            /* iterate each property of this class */
            foreach (System.Reflection.PropertyInfo o_propertyInfo in this.GetType().GetProperties())
            {
                /* add property name and value to our output with "|" separator */
                s_foo.Append(o_propertyInfo.Name + " = " + (o_propertyInfo.GetValue(this)?.ToString() ?? "null") + "|");
            }

            /* remove last "|" */
            return s_foo.ToString().Substring(0, s_foo.Length - 1);
        }
    }
}
