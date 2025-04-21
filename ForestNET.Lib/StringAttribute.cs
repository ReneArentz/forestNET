namespace ForestNET.Lib
{
    /// <summary>
    /// Attribute class to represent a string value for a value/field in an enumeration.
    /// </summary>
    [AttributeUsage(AttributeTargets.Field)]
    public class ValueAttribute : Attribute
    {
        /* Fields */

        /* Properties */

        /// <summary>
        /// String value for a value in an enumeration.
        /// </summary>
        public string Value { get; protected set; }

        /* Methods */

        /// <summary>
        /// Constructor for initiation of a string value attribute.
        /// </summary>
        /// <param name="p_s_string">value for string value attribute</param>
        public ValueAttribute(string p_s_string)
        {
            this.Value = p_s_string;
        }
    }

    public static class AttributeExtension
    {
        /// <summary>
        /// Return string value for a given enumeration's value, using enumeration object itself within this method.
        /// </summary>
        /// <param name="p_e_enum">Enumeration class</param>
        /// <returns>string value of an enumeration.</returns>
        public static string? StringValue(this Enum p_e_enum)
        {
            /* get the string value attributes from field info enumeration type */
            ValueAttribute[] a_valueAttributes = (p_e_enum.GetType()?.GetField(p_e_enum.ToString())?.GetCustomAttributes(typeof(ValueAttribute), false) as ValueAttribute[]) ?? [];

            /* return found string value or null */
            return (a_valueAttributes.Length > 0) ? a_valueAttributes[0].Value : null;
        }
    }
}
