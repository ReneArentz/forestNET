namespace ForestNET.Lib.Net.Mail
{
    /// <summary>
    /// Class to map an email folder with sub folders as child items and a parent item as Folder class.
    /// </summary>
    public class Folder
    {

        /* Constants */

        public const string ROOT = "%__root__%";
        public const string INBOX = "INBOX";
        public const string SENT = "Sent";

        /* Fields */

        private string s_name = string.Empty;
        private List<Folder>? a_children;

        /* Properties */

        public string Name
        {
            get { return this.s_name; }
            set
            {
                if (ForestNET.Lib.Helper.IsStringEmpty(value))
                {
                    throw new ArgumentException("Empty value for name");
                }
                else
                {
                    /* root mail folder has no name value */
                    if (value.Equals(Folder.ROOT))
                    {
                        value = "";
                    }

                    this.s_name = value;
                }
            }
        }

        public Folder? Parent { get; set; }
        public List<Folder>? Children
        {
            get { return this.a_children; }
            set
            {
                if ((value == null) || (value.Count < 1))
                {
                    throw new ArgumentException("Empty children list parameter");
                }
                else
                {
                    this.a_children = value;
                }
            }
        }

        /* Methods */

        /// <summary>
        /// Constructor of mail folder object, no child items and no parent mail folder instance
        /// </summary>
        /// <param name="p_s_name">name of mail folder as string</param>
        /// <exception cref="ArgumentException">invalid name for mail folder</exception>
        public Folder(string p_s_name) :
            this(p_s_name, null, null)
        {

        }

        /// <summary>
        /// Constructor of mail folder object, no child items
        /// </summary>
        /// <param name="p_s_name">name of mail folder as string</param>
        /// <param name="p_o_parent">object instance of parent mail folder</param>
        /// <exception cref="ArgumentException">invalid name for mail folder</exception>
        public Folder(string p_s_name, Folder? p_o_parent) :
            this(p_s_name, p_o_parent, null)
        {

        }

        /// <summary>
        /// Constructor of mail folder object
        /// </summary>
        /// <param name="p_s_name">name of mail folder as string</param>
        /// <param name="p_o_parent">object instance of parent mail folder</param>
        /// <param name="p_a_children">list of children items</param>
        /// <exception cref="ArgumentException">invalid name for mail folder or invalid parameter for children</exception>
        public Folder(string p_s_name, Folder? p_o_parent, List<Folder>? p_a_children)
        {
            this.a_children = [];
            this.Name = p_s_name;
            this.Parent = p_o_parent;

            if (p_a_children != null)
            {
                this.Children = p_a_children;
            }
        }

        /// <summary>
        /// add child item as object to current folder object
        /// </summary>
        /// <param name="p_o_value">children folder object</param>
        /// <exception cref="ArgumentException">children folder object is null</exception>
        public void AddChildren(Folder p_o_value)
        {
            if (p_o_value == null)
            {
                throw new ArgumentException("Empty mail folder parameter");
            }
            else
            {
                if (this.Children == null)
                { /* instance new children list if it is null */
                    this.Children = [];
                }

                /* set parameter object parent as this folder object */
                p_o_value.Parent = this;
                /* add parameter object as child item to list */
                this.Children.Add(p_o_value);
            }
        }

        /// <summary>
        /// add child item as string to current folder object
        /// </summary>
        /// <param name="p_s_value">children folder name</param>
        /// <exception cref="ArgumentException">children folder name is null or empty or child already exists</exception>
        public void AddChildren(string p_s_value)
        {
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_value))
            {
                throw new ArgumentException("Empty mail folder name parameter");
            }
            else
            {
                if (this.Children == null)
                { /* instance new children list if it is null */
                    this.Children = [];
                }

                /* check if children folder does not already exists with that name */
                if (this.GetSubFolder(p_s_value) == null)
                {
                    /* add child as new folder object item to list */
                    this.Children.Add(new Folder(p_s_value, this));
                }
            }
        }

        /// <summary>
        /// Search for folder object in children list
        /// </summary>
        /// <param name="p_s_name">folder name in children list</param>
        /// <returns>Folder						found Folder instance or null</returns>
        /// <exception cref="ArgumentException">parameter name for search is null or empty</exception>
        public Folder? GetSubFolder(string p_s_name)
        {
            Folder? o_return = null;

            /* check parameter value */
            if (ForestNET.Lib.Helper.IsStringEmpty(p_s_name))
            {
                throw new ArgumentException("Please enter a name to get a sub folder of '" + this.Name + "'");
            }

            ForestNET.Lib.Global.ILogFinest("iterate each folder object in children list to find sub folder '" + p_s_name + "'");

            /* iterate each folder object in children list */
            foreach (Folder o_folder in (this.Children ?? []))
            {
                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("check folder object name corresponds to parameter name:\t" + o_folder.Name.ToLower() + " == " + p_s_name.ToLower());

                /* if folder object name corresponds to the name we are looking for */
                if (o_folder.Name.ToLower().Equals(p_s_name.ToLower()))
                {
                    /* set folder object as return value and abort the loop */
                    o_return = o_folder;
                    break;
                }
            }

            return o_return;
        }

        /// <summary>
        /// Returns full path from root folder to current mail folder
        /// </summary>
        /// <returns>string full path</returns>
        public string GetFullPath()
        {
            string s_parentPath = "";

            /* get parent folder instance */
            Folder? o_foo = this.Parent;

            /* iterate all parent instances reverse */
            while (o_foo != null)
            {
                /* add parent instance name to path */
                if (o_foo.Name.Length > 0)
                {
                    s_parentPath = o_foo.Name + "/" + s_parentPath;

                    if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("add parent instance name to path:\t" + s_parentPath);
                }

                /* while we find a parent instance we will continue the loop */
                o_foo = o_foo.Parent;
            }

            if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass("return generated path with name of current folder instance:\t" + s_parentPath + this.Name);

            /* return generated path with name of current folder instance */
            return s_parentPath + this.Name;
        }

        /// <summary>
        /// clear children list
        /// </summary>
        public void ClearChildren()
        {
            this.Children?.Clear();
        }
    }
}