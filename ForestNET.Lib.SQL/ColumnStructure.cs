namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// Column structure class with column, name, new name, constraints, column type and alter operations properties.
    /// </summary>
    public class ColumnStructure : QueryAbstract
    {

        /* Fields */

        private string s_columnType = "";
        private string s_alterOperation = "";
        private Object? o_constraintDefaultValue = null;

        /* Properties */

        public string Name = "";

        public string NewName = "";

        public int ColumnTypeLength = 0;

        public int ColumnTypeDecimalLength = 0;

        public List<string> ConstraintList { get; private set; } = [];

        private string ColumnType
        {
            get
            {
                return this.s_columnType;

            }
            set
            {
                bool b_accept = false;

                /* check if value is a valid sql column type */
                for (int i = 0; i < this.SqlColumnTypes.Length; i++)
                {
                    if (this.SqlColumnTypes[i] == value)
                    {
                        b_accept = true;
                    }
                }

                if (b_accept)
                {
                    this.s_columnType = value;
                }
                else
                {
                    throw new ArgumentException("Value[" + value + "] is not in defined list[" + string.Join(", ", this.SqlColumnTypes) + "]");
                }
            }
        }

        public string AlterOperation
        {
            get
            {
                return this.s_alterOperation;
            }
            set
            {
                bool b_accept = false;

                /* check if value is a valid sql alter operation */
                for (int i = 0; i < this.AlterOperations.Length; i++)
                {
                    if (this.AlterOperations[i] == value)
                    {
                        b_accept = true;
                    }
                }

                if (b_accept)
                {
                    this.s_alterOperation = value;
                }
                else
                {
                    throw new ArgumentException("Value[" + value + "] is not in defined list[" + string.Join(", ", this.AlterOperations) + "]");
                }
            }
        }

        public Object? ConstraintDefaultValue
        {
            get
            {
                return this.o_constraintDefaultValue;
            }
            set
            {
                this.o_constraintDefaultValue = this.ParseValue(value);
            }
        }

        /* Methods */

        /// <summary>
        /// Column structure constructor, need at least query object as parameter for table information
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object or could not parse DateTime to string</exception>
        public ColumnStructure(IQuery? p_o_query) :
            this(p_o_query, "", "", null, "", 0, 0, "")
        {

        }

        /// <summary>
        /// Column structure constructor, need at least query object as parameter for table information
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <param name="p_s_name">define column's name</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object or could not parse DateTime to string</exception>
        public ColumnStructure(IQuery? p_o_query, string p_s_name) :
            this(p_o_query, p_s_name, "", null, "", 0, 0, "")
        {

        }

        /// <summary>
        /// Column structure constructor, need at least query object as parameter for table information
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <param name="p_s_name">define column's name</param>
        /// <param name="p_s_newName">define new name for column</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object or could not parse DateTime to string</exception>
        public ColumnStructure(IQuery? p_o_query, string p_s_name, string p_s_newName) :
            this(p_o_query, p_s_name, p_s_newName, null, "", 0, 0, "")
        {

        }

        /// <summary>
        /// Column structure constructor, need at least query object as parameter for table information
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <param name="p_s_name">define column's name</param>
        /// <param name="p_s_newName">define new name for column</param>
        /// <param name="p_o_constraintDefaultValue">define constraint default value for column</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object or could not parse DateTime to string</exception>
        public ColumnStructure(IQuery? p_o_query, string p_s_name, string p_s_newName, Object? p_o_constraintDefaultValue) :
            this(p_o_query, p_s_name, p_s_newName, p_o_constraintDefaultValue, "", 0, 0, "")
        {

        }

        /// <summary>
        /// Column structure constructor, need at least query object as parameter for table information
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <param name="p_s_name">define column's name</param>
        /// <param name="p_s_newName">define new name for column</param>
        /// <param name="p_o_constraintDefaultValue">define constraint default value for column</param>
        /// <param name="p_s_columnType">define column type</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object or could not parse DateTime to string</exception>
        public ColumnStructure(IQuery? p_o_query, string p_s_name, string p_s_newName, Object? p_o_constraintDefaultValue, string p_s_columnType) :
            this(p_o_query, p_s_name, p_s_newName, p_o_constraintDefaultValue, p_s_columnType, 0, 0, "")
        {

        }

        /// <summary>
        /// Column structure constructor, need at least query object as parameter for table information
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <param name="p_s_name">define column's name</param>
        /// <param name="p_s_newName">define new name for column</param>
        /// <param name="p_o_constraintDefaultValue">define constraint default value for column</param>
        /// <param name="p_s_columnType">define column type</param>
        /// <param name="p_i_columnTypeLength">define column type length for column</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object or could not parse DateTime to string</exception>
        public ColumnStructure(IQuery? p_o_query, string p_s_name, string p_s_newName, Object? p_o_constraintDefaultValue, string p_s_columnType, int p_i_columnTypeLength) :
            this(p_o_query, p_s_name, p_s_newName, p_o_constraintDefaultValue, p_s_columnType, p_i_columnTypeLength, 0, "")
        {

        }

        /// <summary>
        /// Column structure constructor, need at least query object as parameter for table information
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <param name="p_s_name">define column's name</param>
        /// <param name="p_s_newName">define new name for column</param>
        /// <param name="p_o_constraintDefaultValue">define constraint default value for column</param>
        /// <param name="p_s_columnType">define column type</param>
        /// <param name="p_i_columnTypeLength">define column type length for column</param>
        /// <param name="p_i_columnTypeDecimalLength">define column type decimal length for column</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object or could not parse DateTime to string</exception>
        public ColumnStructure(IQuery? p_o_query, string p_s_name, string p_s_newName, Object? p_o_constraintDefaultValue, string p_s_columnType, int p_i_columnTypeLength, int p_i_columnTypeDecimalLength) :
            this(p_o_query, p_s_name, p_s_newName, p_o_constraintDefaultValue, p_s_columnType, p_i_columnTypeLength, p_i_columnTypeDecimalLength, "")
        {

        }

        /// <summary>
        /// Column structure constructor, need at least query object as parameter for table information
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <param name="p_s_name">define column's name</param>
        /// <param name="p_s_newName">define new name for column</param>
        /// <param name="p_o_constraintDefaultValue">define constraint default value for column</param>
        /// <param name="p_s_columnType">define column type</param>
        /// <param name="p_i_columnTypeLength">define column type length for column</param>
        /// <param name="p_i_columnTypeDecimalLength">define column type decimal length for column</param>
        /// <param name="p_s_alterOperation">define alter operation for column</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object or could not parse DateTime to string</exception>
        public ColumnStructure(IQuery? p_o_query, string p_s_name, string p_s_newName, Object? p_o_constraintDefaultValue, string p_s_columnType, int p_i_columnTypeLength, int p_i_columnTypeDecimalLength, string p_s_alterOperation) :
            base(p_o_query)
        {
            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_name))
            {
                this.Name = p_s_name;
            }

            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_newName))
            {
                this.NewName = p_s_newName;
            }

            if (p_o_constraintDefaultValue != null)
            {
                this.ConstraintDefaultValue = p_o_constraintDefaultValue;
            }

            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_columnType))
            {
                this.ColumnType = p_s_columnType;
            }

            if (p_i_columnTypeLength > 0)
            {
                this.ColumnTypeLength = p_i_columnTypeLength;
            }

            if (p_i_columnTypeDecimalLength > 0)
            {
                this.ColumnTypeDecimalLength = p_i_columnTypeDecimalLength;
            }

            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_alterOperation))
            {
                this.AlterOperation = p_s_alterOperation;
            }
        }

        /// <summary>
        /// Add constraint value to column structure
        /// </summary>
        /// <param name="p_s_value">constraint value</param>
        /// <exception cref="ArgumentException">invalid constraint value as parameter</exception>
        public void AddConstraint(string p_s_value)
        {
            bool b_accept = false;

            /* check if p_s_value is a valid sql constraint */
            for (int i = 0; i < this.SqlConstraints.Length; i++)
            {
                if (this.SqlConstraints[i] == p_s_value)
                {
                    b_accept = true;
                }
            }

            if (b_accept)
            {
                this.ConstraintList.Add(p_s_value);
            }
            else
            {
                throw new ArgumentException("Constraint[" + p_s_value + "] is not in defined list[" + string.Join(", ", this.SqlConstraints) + "]");
            }
        }

        /// <summary>
        /// create column structure part of a sql string query
        /// </summary>
        public override string ToString()
        {
            string s_foo = "";

            try
            {
                /* check for implemented sql type */
                if ((this.SqlType != SqlType.CREATE) && (this.SqlType != SqlType.ALTER) && (this.SqlType != SqlType.DROP))
                {
                    throw new Exception("SqlType[" + this.SqlType + "] not implemented");
                }

                bool b_exc = false;

                switch (this.BaseGateway)
                {
                    case BaseGateway.MARIADB:
                    case BaseGateway.NOSQLMDB:
                        if (this.s_alterOperation == "DROP")
                        { /* add 'DROP' columns structure to query */
                            s_foo += "DROP `" + this.Name + "`";
                        }
                        else
                        {
                            if (this.s_alterOperation == "ADD")
                            {
                                /* new name is also name with 'ADD' columns structure */
                                this.NewName = this.Name;

                                if (this.SqlType == SqlType.ALTER)
                                { /* add 'ADD' columns structure to query */
                                    s_foo += "ADD ";
                                }
                            }
                            else if (this.s_alterOperation == "CHANGE")
                            { /* add 'CHANGE' columns structure to query with old column name */
                                s_foo += "CHANGE `" + this.Name + "` ";
                            }

                            /* add column name or new column name with 'CHANGE' alter operation */
                            s_foo += "`" + this.NewName + "`";

                            /* add column type */
                            if (ForestNET.Lib.Helper.IsStringEmpty(this.ColumnType))
                            {
                                throw new Exception("ColumnType not set for sql column");
                            }

                            s_foo += " " + this.ColumnType;

                            /* add column type length */
                            if (this.ColumnTypeLength > 0)
                            {
                                s_foo += "(" + this.ColumnTypeLength;

                                /* additionally add column type decimal length */
                                if (this.ColumnTypeDecimalLength > 0)
                                {
                                    s_foo += "," + this.ColumnTypeDecimalLength;
                                }

                                s_foo += ")";
                            }

                            /* add constraints to current columns structure */
                            if (this.ConstraintList.Count > 0)
                            {
                                foreach (string s_constraint in this.ConstraintList)
                                {
                                    s_foo += " " + s_constraint;

                                    /* check if we want to add default constraint value */
                                    if (s_constraint == "DEFAULT")
                                    {
                                        if (this.ConstraintDefaultValue == null)
                                        {
                                            throw new Exception("No value for constraint DEFAULT");
                                        }

                                        /* allow CURRENT_TIMESTAMP as default value */
                                        if ((this.ConstraintDefaultValue.ToString() ?? "").Equals("[" + this.QueryValueTag + "]CURRENT_TIMESTAMP[/" + this.QueryValueTag + "]"))
                                        {
                                            this.o_constraintDefaultValue = (Object)"CURRENT_TIMESTAMP";
                                        }

                                        /* allow NULL as default value */
                                        if ((this.ConstraintDefaultValue.ToString() ?? "").Equals("[" + this.QueryValueTag + "]NULL[/" + this.QueryValueTag + "]"))
                                        {
                                            this.o_constraintDefaultValue = (Object)"NULL";
                                        }

                                        /* add default constraint value */
                                        s_foo += " " + (this.ConstraintDefaultValue.ToString() ?? "");
                                    }
                                }
                            }
                        }
                        break;
                    case BaseGateway.SQLITE:
                        /* new name is also name */
                        this.NewName = this.Name;

                        /* add column name */
                        s_foo += "`" + this.NewName + "`";

                        /* add column type */
                        if (ForestNET.Lib.Helper.IsStringEmpty(this.ColumnType))
                        {
                            throw new Exception("ColumnType not set for sql column");
                        }

                        s_foo += " " + this.ColumnType;

                        /* add column type length */
                        if (this.ColumnTypeLength > 0)
                        {
                            s_foo += "(" + this.ColumnTypeLength;

                            /* additionally add column type decimal length */
                            if (this.ColumnTypeDecimalLength > 0)
                            {
                                s_foo += "," + this.ColumnTypeDecimalLength;
                            }

                            s_foo += ")";
                        }

                        /* add constraints to current columns structure */
                        if (this.ConstraintList.Count > 0)
                        {
                            foreach (string s_constraint in this.ConstraintList)
                            {
                                s_foo += " " + s_constraint;

                                /* check if we want to add default constraint value */
                                if (s_constraint == "DEFAULT")
                                {
                                    if (this.ConstraintDefaultValue == null)
                                    {
                                        throw new Exception("No value for constraint DEFAULT");
                                    }

                                    /* allow CURRENT_TIMESTAMP as default value */
                                    if ((this.ConstraintDefaultValue.ToString() ?? "").Equals("[" + this.QueryValueTag + "]CURRENT_TIMESTAMP[/" + this.QueryValueTag + "]"))
                                    {
                                        this.o_constraintDefaultValue = (Object)"CURRENT_TIMESTAMP";
                                    }

                                    /* allow NULL as default value */
                                    if ((this.ConstraintDefaultValue.ToString() ?? "").Equals("[" + this.QueryValueTag + "]NULL[/" + this.QueryValueTag + "]"))
                                    {
                                        this.o_constraintDefaultValue = (Object)"NULL";
                                    }

                                    /* remove '" + this.QueryValueTag + "' tags for default constraint value */
                                    if (ForestNET.Lib.Helper.MatchesRegex((this.ConstraintDefaultValue.ToString() ?? ""), "\\[" + this.QueryValueTag + "\\](.*?)\\[/" + this.QueryValueTag + "\\]"))
                                    {
                                        this.o_constraintDefaultValue = (Object)((this.ConstraintDefaultValue.ToString() ?? "").Replace("[" + this.QueryValueTag + "]", "'").Replace("[/" + this.QueryValueTag + "]", "'"));
                                    }

                                    /* add default constraint value */
                                    s_foo += " " + (this.ConstraintDefaultValue.ToString() ?? "");
                                }
                            }
                        }
                        break;
                    case BaseGateway.MSSQL:
                        if (this.s_alterOperation == "DROP")
                        { /* add 'DROP' columns structure to query */
                            s_foo += "COLUMN [" + this.Name + "]";
                        }
                        else
                        {
                            if (this.s_alterOperation == "ADD")
                            {
                                /* new name is also name with 'ADD' columns structure */
                                this.NewName = this.Name;
                            }
                            else if (this.s_alterOperation == "CHANGE")
                            {
                                s_foo += "COLUMN ";

                                if (ForestNET.Lib.Helper.IsStringEmpty(this.NewName))
                                {
                                    /* new name is also name if new name is empty */
                                    this.NewName = this.Name;
                                }
                            }

                            /* add column name */
                            s_foo += "[" + this.NewName + "]";

                            /* add column type */
                            if (ForestNET.Lib.Helper.IsStringEmpty(this.ColumnType))
                            {
                                throw new Exception("ColumnType not set for sql column");
                            }

                            s_foo += " " + this.ColumnType;

                            /* add column type length */
                            if (this.ColumnTypeLength > 0)
                            {
                                s_foo += "(" + this.ColumnTypeLength;

                                /* additionally add column type decimal length */
                                if (this.ColumnTypeDecimalLength > 0)
                                {
                                    s_foo += "," + this.ColumnTypeDecimalLength;
                                }

                                s_foo += ")";
                            }

                            /* add constraints to current columns structure */
                            if (this.ConstraintList.Count > 0)
                            {
                                foreach (string s_constraint in this.ConstraintList)
                                {
                                    /* changing default on column is not supported */
                                    if ((this.s_alterOperation == "CHANGE") && (s_constraint == "DEFAULT"))
                                    {
                                        continue;
                                    }

                                    s_foo += " " + s_constraint;

                                    /* check if we want to add default constraint value */
                                    if (s_constraint == "DEFAULT")
                                    {
                                        if (this.ConstraintDefaultValue == null)
                                        {
                                            throw new Exception("No value for constraint DEFAULT");
                                        }

                                        /* allow CURRENT_TIMESTAMP as default value */
                                        if ((this.ConstraintDefaultValue.ToString() ?? "").Equals("[" + this.QueryValueTag + "]CURRENT_TIMESTAMP[/" + this.QueryValueTag + "]"))
                                        {
                                            this.o_constraintDefaultValue = (Object)"CURRENT_TIMESTAMP";
                                        }

                                        /* allow NULL as default value */
                                        if ((this.ConstraintDefaultValue.ToString() ?? "").Equals("[" + this.QueryValueTag + "]NULL[/" + this.QueryValueTag + "]"))
                                        {
                                            this.o_constraintDefaultValue = (Object)"NULL";
                                        }

                                        /* add default constraint value */
                                        s_foo += " " + (this.ConstraintDefaultValue.ToString() ?? "");
                                    }
                                }
                            }
                        }
                        break;
                    case BaseGateway.PGSQL:
                        if (this.s_alterOperation == "DROP")
                        { /* add 'DROP' columns structure to query */
                            s_foo += "DROP \"" + this.Name + "\"";
                        }
                        else
                        {
                            if (this.s_alterOperation == "ADD")
                            {
                                if (this.SqlType == SqlType.ALTER)
                                {
                                    /* add 'ADD' columns structure to query if ALTER query */
                                    s_foo += "ADD ";
                                }
                            }
                            else if (this.s_alterOperation == "CHANGE")
                            {
                                /* disallow changing 'Id' column */
                                if (this.Name == "Id")
                                {
                                    throw new Exception("Cannot change settings for sql column \"Id\"");
                                }

                                /* add 'ALER COLUMN' columns structure to query */
                                s_foo += "ALTER COLUMN ";
                            }

                            string s_fooName = this.Name;

                            /* if alter operation is 'CHANGE' and new name is not empty, set column name as new name */
                            if ((this.s_alterOperation == "CHANGE") && (!ForestNET.Lib.Helper.IsStringEmpty(this.NewName)))
                            {
                                s_fooName = this.NewName;
                            }

                            /* add column name to query */
                            s_foo += "\"" + s_fooName + "\"";

                            /* add column type to query */
                            if (ForestNET.Lib.Helper.IsStringEmpty(this.ColumnType))
                            {
                                throw new Exception("ColumnType not set for sql column");
                            }

                            /* if alter operation is 'CHANGE' add 'TYPE' to query */
                            if (this.s_alterOperation == "CHANGE")
                            {
                                s_foo += " TYPE";
                            }

                            /* add column type to query */
                            s_foo += " " + this.ColumnType;

                            /* add column type length to query */
                            if (this.ColumnTypeLength > 0)
                            {
                                s_foo += "(" + this.ColumnTypeLength;

                                /* additionally add column type decimal length to query */
                                if (this.ColumnTypeDecimalLength > 0)
                                {
                                    s_foo += "," + this.ColumnTypeDecimalLength;
                                }

                                s_foo += ")";
                            }

                            bool b_setNotNull = false;
                            bool b_setDefault = false;

                            /* add constraints to current columns structure */
                            if (this.ConstraintList.Count > 0)
                            {
                                foreach (string s_constraint in this.ConstraintList)
                                {
                                    if (this.s_alterOperation == "CHANGE")
                                    { /* change a column */
                                        if (s_constraint == "NOT NULL")
                                        { /* change column with 'SET NOT NULL' constraint */
                                            s_foo += ", ALTER COLUMN \"" + s_fooName + "\" SET NOT NULL";

                                            b_setNotNull = true;
                                        }
                                        else if (s_constraint == "DEFAULT")
                                        { /* change column with 'SET DEFAULT' constraint */
                                            s_foo += ", ALTER COLUMN \"" + s_fooName + "\" SET DEFAULT ";

                                            if (this.ConstraintDefaultValue == null)
                                            {
                                                throw new Exception("No value for constraint DEFAULT");
                                            }

                                            /* allow CURRENT_TIMESTAMP as default value */
                                            if ((this.ConstraintDefaultValue.ToString() ?? "").Equals("[" + this.QueryValueTag + "]CURRENT_TIMESTAMP[/" + this.QueryValueTag + "]"))
                                            {
                                                this.o_constraintDefaultValue = (Object)"CURRENT_TIMESTAMP";
                                            }

                                            /* allow NULL as default value */
                                            if ((this.ConstraintDefaultValue.ToString() ?? "").Equals("[" + this.QueryValueTag + "]NULL[/" + this.QueryValueTag + "]"))
                                            {
                                                this.o_constraintDefaultValue = (Object)"NULL";
                                            }

                                            /* add constraint default value */
                                            s_foo += " " + (this.ConstraintDefaultValue.ToString() ?? "");

                                            b_setDefault = true;
                                        }
                                    }
                                    else
                                    { /* create a column */
                                        if (s_constraint == "PRIMARY KEY")
                                        {
                                            /* if constraint is 'PRIMARY KEY' add column with serial notation */
                                            s_foo = "";

                                            if (this.s_alterOperation == "ADD")
                                            {
                                                if (this.SqlType == SqlType.ALTER)
                                                {
                                                    s_foo += "ADD ";
                                                }
                                            }

                                            s_foo += "\"" + s_fooName + "\" serial " + s_constraint;
                                            break;
                                        }
                                        else
                                        {
                                            /* add constraint to query */
                                            s_foo += " " + s_constraint;

                                            /* check if we want to use constraint default value */
                                            if (s_constraint == "DEFAULT")
                                            {
                                                if (this.ConstraintDefaultValue == null)
                                                {
                                                    throw new Exception("No value for constraint DEFAULT");
                                                }

                                                /* allow CURRENT_TIMESTAMP as default value */
                                                if ((this.ConstraintDefaultValue.ToString() ?? "").Equals("[" + this.QueryValueTag + "]CURRENT_TIMESTAMP[/" + this.QueryValueTag + "]"))
                                                {
                                                    this.o_constraintDefaultValue = (Object)"CURRENT_TIMESTAMP";
                                                }

                                                /* allow NULL as default value */
                                                if ((this.ConstraintDefaultValue.ToString() ?? "").Equals("[" + this.QueryValueTag + "]NULL[/" + this.QueryValueTag + "]"))
                                                {
                                                    this.o_constraintDefaultValue = (Object)"NULL";
                                                }

                                                /* add constraint default value */
                                                s_foo += " " + (this.ConstraintDefaultValue.ToString() ?? "");
                                            }
                                        }
                                    }
                                }
                            }

                            /* if alter operation is 'CHANGE' and column type is not 'bit' */
                            if ((this.s_alterOperation == "CHANGE") && (this.ColumnType != "bit"))
                            {
                                if (!b_setNotNull)
                                { /* erase NOT NULL from column */
                                    s_foo += ", ALTER COLUMN \"" + s_fooName + "\" DROP NOT NULL";
                                }

                                if (!b_setDefault)
                                { /* erase DEFAULT from column */
                                    s_foo += ", ALTER COLUMN \"" + s_fooName + "\" DROP DEFAULT";
                                }
                            }
                        }
                        break;
                    case BaseGateway.ORACLE:
                        if (this.s_alterOperation == "DROP")
                        { /* add 'DROP' columns structure to query */
                            s_foo += "\"" + this.Name + "\"";
                        }
                        else
                        {
                            string s_fooName = this.Name;

                            /* if alter operation is 'CHANGE' and new name is not empty, set column name as new name */
                            if ((this.s_alterOperation == "CHANGE") && (!ForestNET.Lib.Helper.IsStringEmpty(this.NewName)))
                            {
                                s_fooName = this.NewName;
                            }

                            /* add column name to query */
                            s_foo += "\"" + s_fooName + "\"";

                            /* add column type to query */
                            if (ForestNET.Lib.Helper.IsStringEmpty(this.ColumnType))
                            {
                                throw new Exception("ColumnType not set for sql column");
                            }

                            s_foo += " " + this.ColumnType;

                            /* add column type length to query */
                            if (this.ColumnTypeLength > 0)
                            {
                                s_foo += "(" + this.ColumnTypeLength;

                                /* additionally add column type decimal length to query */
                                if (this.ColumnTypeDecimalLength > 0)
                                {
                                    s_foo += "," + this.ColumnTypeDecimalLength;
                                }

                                s_foo += ")";
                            }

                            /* add constraints to current columns structure */
                            if (this.ConstraintList.Count > 0)
                            {
                                foreach (string s_constraint in this.ConstraintList)
                                {
                                    if (s_constraint == "PRIMARY KEY")
                                    { /* if constraint is 'PRIMARY KEY' write constraint with special notation */
                                        s_foo = "\"" + s_fooName + "\" NUMBER GENERATED by default on null as IDENTITY " + s_constraint;
                                        break;
                                    }
                                    else if (s_constraint == "DEFAULT")
                                    {
                                        /* add constraint 'DEFAULT' to query */
                                        s_foo += " " + s_constraint;

                                        if (this.ConstraintDefaultValue == null)
                                        {
                                            throw new Exception("No value for constraint DEFAULT");
                                        }

                                        if (this.ConstraintDefaultValue is string)
                                        {
                                            /* remove '" + this.QueryValueTag + "' tags from constraint default value if column type is 'TIME' or 'TIMESTAMP', because we must add something manually */
                                            if ((this.ColumnType == "INTERVAL DAY(0) TO SECOND(0)") || (this.ColumnType == "TIMESTAMP"))
                                            {
                                                if (ForestNET.Lib.Helper.MatchesRegex((this.ConstraintDefaultValue.ToString() ?? ""), "\\[" + this.QueryValueTag + "\\](.*?)\\[/" + this.QueryValueTag + "\\]"))
                                                {
                                                    string s_foo2 = (this.ConstraintDefaultValue.ToString() ?? "").Replace("[" + this.QueryValueTag + "]", "").Replace("[/" + this.QueryValueTag + "]", "");

                                                    /* check if time span value has TO_DSINTERVAL conversion */
                                                    if (s_foo2.StartsWith("TO_DSINTERVAL"))
                                                    {
                                                        /* remove TO_DSINTERVAL conversion */
                                                        s_foo2 = s_foo2.Substring(14, s_foo2.Length - 1 - 14);
                                                    }
                                                    else if ((s_foo2.StartsWith("TO_DATE")) && (s_foo2.Contains("yyyy-mm-dd hh24"))) /* datetime */
                                                    {
                                                        /* remove TO_DATE conversion */
                                                        s_foo2 = s_foo2.Substring(8, s_foo2.Length - 26 - 8);
                                                    }
                                                    else if ((s_foo2.StartsWith("TO_DATE")) && (s_foo2.Contains("yyyy-mm-dd"))) /* date */
                                                    {
                                                        /* remove TO_DATE conversion */
                                                        s_foo2 = s_foo2.Substring(8, s_foo2.Length - 15 - 8);
                                                    }

                                                    this.o_constraintDefaultValue = (Object)s_foo2;
                                                }
                                            }

                                            if (this.ColumnType == "INTERVAL DAY(0) TO SECOND(0)")
                                            { /* remove '+0 ' of constraint default value */
                                                if ((this.ConstraintDefaultValue.ToString() ?? "").StartsWith("+0 "))
                                                {
                                                    this.o_constraintDefaultValue = (Object)(this.ConstraintDefaultValue.ToString() ?? "").Substring(3);
                                                }

                                                s_foo += " [" + this.QueryValueTag + "]0 " + (this.ConstraintDefaultValue.ToString() ?? "") + "[/" + this.QueryValueTag + "]";
                                            }
                                            else if (this.ColumnType == "TIMESTAMP")
                                            { /* add 'timestamp ' to constraint default value */
                                                if ((this.ConstraintDefaultValue.ToString() ?? "").Equals("CURRENT_TIMESTAMP"))
                                                {
                                                    s_foo += " " + (this.ConstraintDefaultValue.ToString() ?? "");
                                                }
                                                else
                                                {
                                                    if (ForestNET.Lib.Helper.IsDateTime((this.ConstraintDefaultValue.ToString() ?? "")))
                                                    {
                                                        s_foo += " timestamp [" + this.QueryValueTag + "]" + (this.ConstraintDefaultValue.ToString() ?? "") + "[/" + this.QueryValueTag + "]";
                                                    }
                                                    else if (ForestNET.Lib.Helper.IsDate((this.ConstraintDefaultValue.ToString() ?? "")))
                                                    {
                                                        s_foo += " timestamp [" + this.QueryValueTag + "]" + (this.ConstraintDefaultValue.ToString() ?? "") + " 00:00:00[/" + this.QueryValueTag + "]";
                                                    }
                                                    else
                                                    {
                                                        s_foo += " [" + this.QueryValueTag + "]" + (this.ConstraintDefaultValue.ToString() ?? "") + "[/" + this.QueryValueTag + "]";
                                                    }
                                                }
                                            }
                                            else
                                            {
                                                /* allow NULL as default value */
                                                if ((this.ConstraintDefaultValue.ToString() ?? "").Equals("[" + this.QueryValueTag + "]NULL[/" + this.QueryValueTag + "]"))
                                                {
                                                    this.o_constraintDefaultValue = (Object)"NULL";
                                                }

                                                s_foo += " " + (this.ConstraintDefaultValue.ToString() ?? "");
                                            }

                                            if (this.ConstraintList.Contains("NULL"))
                                            {
                                                /* add 'NULL' constraint to query */
                                                s_foo += " NULL";
                                            }
                                            else if (this.ConstraintList.Contains("NOT NULL"))
                                            {
                                                /* add 'NOT NULL' constraint to query */
                                                s_foo += " NOT NULL";
                                            }
                                        }
                                        else
                                        {
                                            /* add constraint default value to query */
                                            s_foo += " " + (this.ConstraintDefaultValue.ToString() ?? "");
                                        }
                                    }
                                    else
                                    {
                                        /* skip constraints 'NULL' or 'NOT NULL' if constraint 'DEFAULT' is in constraint list */
                                        if (((s_constraint == "NULL") || (s_constraint == "NOT NULL")) && (this.ConstraintList.Contains("DEFAULT")))
                                        {
                                            continue;
                                        }

                                        /* add constraint to query */
                                        s_foo += " " + s_constraint;
                                    }
                                }
                            }
                        }
                        break;
                    default:
                        b_exc = true;
                        break;
                }

                if (b_exc)
                {
                    throw new Exception("BaseGateway[" + this.BaseGateway + "] not implemented");
                }
            }
            catch (Exception o_exc)
            { /* just set exception as query return, so database interface will have an exception as well */
                s_foo = " >>>>> Column structure class Exception: [" + o_exc + "] <<<<< ";
            }

            return s_foo;
        }

        /// <summary>
        /// Determine valid column type, type length and type decimal length, based on sql type parameter and database gateway information with allocation matrix
        /// 										text [36], text [255], text, integer [small], integer [int], integer [big], datetime, time, double, decimal, bool
        /// </summary>
        /// <param name="p_s_sqlType">general sql type, valid values:</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object or invalid sql type</exception>
        public void ColumnTypeAllocation(string p_s_sqlType)
        {
            bool b_exc = false;

            /* check base gateway parameter */
            switch (this.BaseGateway)
            {
                case BaseGateway.MARIADB:
                case BaseGateway.SQLITE:
                case BaseGateway.MSSQL:
                case BaseGateway.PGSQL:
                case BaseGateway.ORACLE:
                case BaseGateway.NOSQLMDB:
                    break;
                default:
                    b_exc = true;
                    break;
            }

            if (b_exc)
            {
                throw new ArgumentException("Invalid BaseGateway[" + this.BaseGateway + "]");
            }

            /* check sql type parameter */
            switch (p_s_sqlType)
            {
                case "text [36]":
                case "text [255]":
                case "text":
                case "integer [small]":
                case "integer [int]":
                case "integer [big]":
                case "datetime":
                case "time":
                case "double":
                case "decimal":
                case "bool":
                    break;
                default:
                    b_exc = true;
                    break;
            }

            if (b_exc)
            {
                throw new ArgumentException("Invalid SqlType[" + p_s_sqlType + "]");
            }

            /* create allocation matrix with helping mappings */
            Dictionary<string, int> a_mapping = new()
            {
                [BaseGateway.MARIADB.ToString()] = 0,
                [BaseGateway.SQLITE.ToString()] = 1,
                [BaseGateway.MSSQL.ToString()] = 2,
                [BaseGateway.PGSQL.ToString()] = 3,
                [BaseGateway.ORACLE.ToString()] = 4,
                [BaseGateway.NOSQLMDB.ToString()] = 5,

                ["text [36]"] = 0,
                ["text [255]"] = 1,
                ["text"] = 2,
                ["integer [small]"] = 3,
                ["integer [int]"] = 4,
                ["integer [big]"] = 5,
                ["datetime"] = 6,
                ["time"] = 7,
                ["double"] = 8,
                ["decimal"] = 9,
                ["bool"] = 10,

                ["columnType"] = 0,
                ["columnLength"] = 1,
                ["decimalLength"] = 2
            };

            string[][][] a_allocation = new string[][][] {
				/* MARIADB */ new string[][] {
                    new string[] {"VARCHAR", "36", "0"},
                    new string[] {"VARCHAR", "255", "0"},
                    new string[] {"TEXT", "0", "0"},
                    new string[] {"SMALLINT", "6", "0"},
                    new string[] {"INT", "10", "0"},
                    new string[] {"BIGINT", "20", "0"},
                    new string[] {"TIMESTAMP", "0", "0"},
                    new string[] {"TIME", "0", "0"},
                    new string[] {"DOUBLE", "0", "0"},
                    new string[] {"DECIMAL", "38", "9"},
                    new string[] {"BIT", "1", "0"}
                },
				/* SQLITE */ new string[][] {
                    new string[] {"varchar", "36", "0"},
                    new string[] {"varchar", "255", "0"},
                    new string[] {"text", "0", "0"},
                    new string[] {"smallint", "0", "0"},
                    new string[] {"integer", "0", "0"},
                    new string[] {"bigint", "0", "0"},
                    new string[] {"datetime", "0", "0"},
                    new string[] {"time", "0", "0"},
                    new string[] {"double", "0", "0"},
                    new string[] {"decimal", "38", "9"},
                    new string[] {"bit", "1", "0"}
                },
				/* MSSQL */ new string[][] {
                    new string[] {"nvarchar", "36", "0"},
                    new string[] {"nvarchar", "255", "0"},
                    new string[] {"text", "0", "0"},
                    new string[] {"smallint", "0", "0"},
                    new string[] {"int", "0", "0"},
                    new string[] {"bigint", "0", "0"},
                    new string[] {"datetime", "0", "0"},
                    new string[] {"time", "0", "0"},
                    new string[] {"float", "0", "0"},
                    new string[] {"decimal", "38", "9"},
                    new string[] {"bit", "0", "0"}
                },
				/* PGSQL */ new string[][] {
                    new string[] {"varchar", "36", "0"},
                    new string[] {"varchar", "255", "0"},
                    new string[] {"text", "0", "0"},
                    new string[] {"smallint", "0", "0"},
                    new string[] {"integer", "0", "0"},
                    new string[] {"bigint", "0", "0"},
                    new string[] {"timestamp", "0", "0"},
                    new string[] {"time", "0", "0"},
                    new string[] {"double precision", "0", "0"},
                    new string[] {"decimal", "38", "9"},
                    new string[] {"boolean", "0", "0"}
                },
				/* ORACLE */ new string[][] {
                    new string[] {"VARCHAR2", "36", "0"},
                    new string[] {"VARCHAR2", "255", "0"},
                    new string[] {"CLOB", "0", "0"},
                    new string[] {"NUMBER", "5", "0"},
                    new string[] {"NUMBER", "10", "0"},
                    new string[] {"LONG", "0", "0"},
                    new string[] {"TIMESTAMP", "0", "0"},
                    new string[] {"INTERVAL DAY(0) TO SECOND(0)", "0", "0"},
                    new string[] {"BINARY_DOUBLE", "0", "0"},
                    new string[] {"NUMBER", "38", "9"},
                    new string[] {"CHAR", "1", "0"}
                },
				/* NoSQLMDB */ new string[][] {
                    new string[] {"VARCHAR", "0", "0"},
                    new string[] {"VARCHAR", "0", "0"},
                    new string[] {"TEXT", "0", "0"},
                    new string[] {"SMALLINT", "0", "0"},
                    new string[] {"INTEGER", "0", "0"},
                    new string[] {"BIGINT", "0", "0"},
                    new string[] {"TIMESTAMP", "0", "0"},
                    new string[] {"TIME", "0", "0"},
                    new string[] {"DOUBLE", "0", "0"},
                    new string[] {"DECIMAL", "0", "0"},
                    new string[] {"BOOL", "0", "0"}
                }
            };

            /* get column properties of allocation matrix */
            string s_columnType = a_allocation[a_mapping[this.BaseGateway.ToString()]][a_mapping[p_s_sqlType]][a_mapping["columnType"]];
            int i_columnLength = int.Parse(a_allocation[a_mapping[this.BaseGateway.ToString()]][a_mapping[p_s_sqlType]][a_mapping["columnLength"]]);
            int i_columnDecimalLength = int.Parse(a_allocation[a_mapping[this.BaseGateway.ToString()]][a_mapping[p_s_sqlType]][a_mapping["decimalLength"]]);

            if (!ForestNET.Lib.Helper.IsStringEmpty(s_columnType))
            {
                this.ColumnType = s_columnType;
            }

            if (i_columnLength > 0)
            {
                this.ColumnTypeLength = i_columnLength;
            }

            if (i_columnDecimalLength > 0)
            {
                this.ColumnTypeDecimalLength = i_columnDecimalLength;
            }
        }

        /// <summary>
        /// Assume column information from column structure parameter to current instance for sqlite
        /// </summary>
        /// <param name="p_o_column">column structure parameter with column type, type length and type decimal length information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public void AssumeColumnTypeSqlite(ColumnStructure p_o_column)
        {
            if (this.BaseGateway == BaseGateway.SQLITE)
            {
                if (!ForestNET.Lib.Helper.IsStringEmpty(p_o_column.ColumnType))
                {
                    this.ColumnType = p_o_column.ColumnType;
                }

                if (p_o_column.ColumnTypeLength > 0)
                {
                    this.ColumnTypeLength = p_o_column.ColumnTypeLength;
                }

                if (p_o_column.ColumnTypeDecimalLength > 0)
                {
                    this.ColumnTypeDecimalLength = p_o_column.ColumnTypeDecimalLength;
                }
            }
        }
    }
}