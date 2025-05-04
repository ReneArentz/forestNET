namespace ForestNET.Lib.Net.Https.Dynm
{
    /// <summary>
    /// Encapsulation of POST file data with field name, file name, content type and file data.
    /// </summary>
    public class FileData
    {

        /* Fields */

        private byte[]? a_fileData;

        /* Properties */

        public string FieldName { get; set; }
        public string FileName { get; set; }
        public string? ContentType { get; set; }
        public byte[]? Data
        {
            get
            {
                return this.a_fileData;
            }
            set
            {
                if ((value != null) && (value.Length > 0))
                {
                    this.a_fileData = new byte[value.Length];

                    for (int i = 0; i < value.Length; i++)
                    {
                        this.a_fileData[i] = value[i];
                    }
                }
                else
                {
                    this.a_fileData = value;
                }
            }
        }

        /* Methods */

        /// <summary>
        /// FileData constructor, no file data
        /// </summary>
        /// <param name="p_s_fieldName">name of html field element which handled the upload</param>
        /// <param name="p_s_fileName">file name of source file which was uploaded</param>
        public FileData(string p_s_fieldName, string p_s_fileName) :
            this(p_s_fieldName, p_s_fileName, null, null)
        {

        }

        /// <summary>
        /// FileData constructor
        /// </summary>
        /// <param name="p_s_fieldName">name of html field element which handled the upload</param>
        /// <param name="p_s_fileName">file name of source file which was uploaded</param>
        /// <param name="p_s_contentType">content type of source file</param>
        /// <param name="p_a_data">file data</param>
        public FileData(string p_s_fieldName, string p_s_fileName, string? p_s_contentType, byte[]? p_a_data)
        {
            this.FieldName = p_s_fieldName;
            this.FileName = p_s_fileName;
            this.ContentType = p_s_contentType;
            this.Data = p_a_data;
        }
    }
}
