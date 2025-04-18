namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// Constraint class with constraint, name, new name, alter operation and column list properties.
    /// </summary>
    public class Constraint : QueryAbstract
    {

        /* Fields */

        private string s_constraint = "";
        private string s_alterOperation = "ADD";

        /* Properties */

        public string ConstraintStr
        {
            get
            {
                return this.s_constraint;
            }
            set
            {
                bool b_accept = false;

                /* check if value is a valid sql index constraint */
                for (int i = 0; i < this.SqlIndexConstraints.Length; i++)
                {
                    if (this.SqlIndexConstraints[i] == value)
                    {
                        b_accept = true;
                    }
                }

                if (b_accept)
                {
                    this.s_constraint = value;
                }
                else
                {
                    throw new ArgumentException("Value[" + value + "] is not in defined list[" + string.Join(", ", this.SqlIndexConstraints) + "]");
                }
            }
        }

        public string Name { get; set; } = "";

        public string NewName { get; set; } = "";

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

        public List<string> Columns { get; set; } = [];

        /* Methods */

        /// <summary>
        /// Constraint constructor, need at least query object as parameter for table information
        /// default alter operation is 'ADD'
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Constraint(IQuery? p_o_query) : this(p_o_query, "", "", "", "ADD")
        {

        }

        /// <summary>
        /// Constraint constructor, need at least query object as parameter for table information
        /// default alter operation is 'ADD'
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <param name="p_s_constraint">define constraint</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Constraint(IQuery? p_o_query, string p_s_constraint) : this(p_o_query, p_s_constraint, "", "", "ADD")
        {

        }

        /// <summary>
        /// Constraint constructor, need at least query object as parameter for table information
        /// default alter operation is 'ADD'
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <param name="p_s_constraint">define constraint</param>
        /// <param name="p_s_name">define name for constraint</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Constraint(IQuery? p_o_query, string p_s_constraint, string p_s_name) : this(p_o_query, p_s_constraint, p_s_name, "", "ADD")
        {

        }

        /// <summary>
        /// Constraint constructor, need at least query object as parameter for table information
        /// default alter operation is 'ADD'
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <param name="p_s_constraint">define constraint</param>
        /// <param name="p_s_name">define name for constraint</param>
        /// <param name="p_s_newName">define new name for constraint</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Constraint(IQuery? p_o_query, string p_s_constraint, string p_s_name, string p_s_newName) : this(p_o_query, p_s_constraint, p_s_name, p_s_newName, "ADD")
        {

        }

        /// <summary>
        /// Constraint constructor, need at least query object as parameter for table information
        /// default alter operation is 'ADD'
        /// </summary>
        /// <param name="p_o_query">query object with database gateway and table information</param>
        /// <param name="p_s_constraint">define constraint</param>
        /// <param name="p_s_name">define name for constraint</param>
        /// <param name="p_s_newName">define new name for constraint</param>
        /// <param name="p_s_alterOperation">define alter operation for constraint</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Constraint(IQuery? p_o_query, string p_s_constraint, string p_s_name, string p_s_newName, string p_s_alterOperation) : base(p_o_query)
        {
            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_constraint))
            {
                this.ConstraintStr = p_s_constraint;
            }

            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_name))
            {
                this.Name = p_s_name;
            }

            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_newName))
            {
                this.NewName = p_s_newName;
            }

            if (!ForestNET.Lib.Helper.IsStringEmpty(p_s_alterOperation))
            {
                this.AlterOperation = p_s_alterOperation;
            }
        }

        /// <summary>
        /// create constraint part of a sql string query
        /// </summary>
        public override string ToString()
        {
            string s_foo = "";

            try
            {
                bool b_exc = false;

                /* check for implemented sql type */
                if ((this.SqlType != SqlType.CREATE) && (this.SqlType != SqlType.ALTER) && (this.SqlType != SqlType.DROP))
                {
                    throw new Exception("SqlType[" + this.SqlType + "] not implemented");
                }

                /* if alter operation is 'DROP' we need at least one column object */
                if ((this.s_alterOperation != "DROP") && (this.Columns.Count <= 0))
                {
                    throw new Exception("Columns object list is empty");
                }

                switch (this.BaseGateway)
                {
                    case BaseGateway.MARIADB:
                    case BaseGateway.NOSQLMDB:
                        if (this.s_alterOperation == "ADD")
                        { /* add new constraint */
                            /* add constraint with name to query */
                            s_foo = "ADD " + this.s_constraint + " `" + this.Name + "` (";

                            /* add all columns of constraint to query */
                            for (int i = 0; i < this.Columns.Count; i++)
                            {
                                if (i == this.Columns.Count - 1)
                                {
                                    s_foo += "`" + this.Columns[i] + "`";
                                }
                                else
                                {
                                    s_foo += "`" + this.Columns[i] + "`" + ", ";
                                }
                            }

                            /* close constraint */
                            s_foo += ")";
                        }
                        else if (this.s_alterOperation == "CHANGE")
                        { /* change existing constraint */
                            /* new name must be set for changing existing constraint */
                            if (ForestNET.Lib.Helper.IsStringEmpty(this.NewName))
                            {
                                throw new Exception("No new name for changing constraint");
                            }

                            /* add constraint with new name to query */
                            s_foo = "ADD " + this.s_constraint + " `" + this.NewName + "` (";

                            /* add all columns of constraint to query */
                            for (int i = 0; i < this.Columns.Count; i++)
                            {
                                if (i == this.Columns.Count - 1)
                                {
                                    s_foo += "`" + this.Columns[i] + "`";
                                }
                                else
                                {
                                    s_foo += "`" + this.Columns[i] + "`" + ", ";
                                }
                            }

                            /* close constraint */
                            s_foo += ")";
                            /* drop old constraint */
                            s_foo += ", DROP INDEX `" + this.Name + "`";
                        }
                        else if (this.s_alterOperation == "DROP")
                        { /* drop constraint */
                            s_foo = "DROP INDEX `" + this.Name + "`";
                        }
                        break;
                    case BaseGateway.SQLITE:
                        /* change UNQIUE constraint to INDEX */
                        if (this.s_constraint == "UNIQUE")
                        {
                            this.s_constraint += " INDEX";
                        }

                        if (this.s_alterOperation == "ADD")
                        { /* add new constraint */
                            /* add constraint with name to query */
                            s_foo = "CREATE " + this.s_constraint + " `" + this.Name + "` ON `" + this.Table + "` (";

                            /* add all columns of constraint to query */
                            for (int i = 0; i < this.Columns.Count; i++)
                            {
                                if (i == this.Columns.Count - 1)
                                {
                                    s_foo += "`" + this.Columns[i] + "`";
                                }
                                else
                                {
                                    s_foo += "`" + this.Columns[i] + "`" + ", ";
                                }
                            }

                            /* close constraint */
                            s_foo += ")";
                        }
                        else if (this.s_alterOperation == "CHANGE")
                        { /* change existing constraint */
                            /* drop old constraint with query separator */
                            s_foo = "DROP INDEX `" + this.Name + "`" + this.QuerySeparator;

                            /* new name must be set for changing existing constraint */
                            if (ForestNET.Lib.Helper.IsStringEmpty(this.NewName))
                            {
                                throw new Exception("No new name for changing constraint");
                            }

                            /* add constraint with new name to query */
                            s_foo += "CREATE " + this.s_constraint + " `" + this.NewName + "` ON `" + this.Table + "` (";

                            /* add all columns of constraint to query */
                            for (int i = 0; i < this.Columns.Count; i++)
                            {
                                if (i == this.Columns.Count - 1)
                                {
                                    s_foo += "`" + this.Columns[i] + "`";
                                }
                                else
                                {
                                    s_foo += "`" + this.Columns[i] + "`" + ", ";
                                }
                            }

                            /* close constraint */
                            s_foo += ")";
                        }
                        else if (this.s_alterOperation == "DROP")
                        { /* drop constraint */
                            s_foo = "DROP INDEX `" + this.Name + "`";
                        }
                        break;
                    case BaseGateway.MSSQL:
                        if (this.s_constraint == "UNIQUE")
                        {
                            this.s_constraint += " INDEX";
                        }

                        if (this.s_alterOperation == "ADD")
                        { /* add new constraint */
                            /* add constraint with name to query */
                            s_foo = "CREATE " + this.s_constraint + " [" + this.Name + "] ON [" + this.Table + "] (";

                            /* add all columns of constraint to query */
                            for (int i = 0; i < this.Columns.Count; i++)
                            {
                                if (i == this.Columns.Count - 1)
                                {
                                    s_foo += "[" + this.Columns[i] + "]";
                                }
                                else
                                {
                                    s_foo += "[" + this.Columns[i] + "]" + ", ";
                                }
                            }

                            /* close constraint */
                            s_foo += ")";
                        }
                        else if (this.s_alterOperation == "CHANGE")
                        { /* change existing constraint */
                            /* drop old constraint with query separator */
                            s_foo = "DROP INDEX [" + this.Name + "] ON [" + this.Table + "]" + this.QuerySeparator;

                            /* new name must be set for changing existing constraint */
                            if (ForestNET.Lib.Helper.IsStringEmpty(this.NewName))
                            {
                                throw new Exception("No new name for changing constraint");
                            }

                            /* add constraint with new name to query */
                            s_foo += "CREATE " + this.s_constraint + " [" + this.NewName + "] ON [" + this.Table + "] (";

                            /* add all columns of constraint to query */
                            for (int i = 0; i < this.Columns.Count; i++)
                            {
                                if (i == this.Columns.Count - 1)
                                {
                                    s_foo += "[" + this.Columns[i] + "]";
                                }
                                else
                                {
                                    s_foo += "[" + this.Columns[i] + "]" + ", ";
                                }
                            }

                            /* close constraint */
                            s_foo += ")";
                        }
                        else if (this.s_alterOperation == "DROP")
                        { /* drop constraint */
                            s_foo = "DROP INDEX [" + this.Name + "] ON [" + this.Table + "]";
                        }
                        break;
                    case BaseGateway.PGSQL:
                        if (this.s_alterOperation == "ADD")
                        { /* add new constraint */
                            /* add constraint with name to query as INDEX or as CONSTRAINT */
                            if (this.s_constraint == "INDEX")
                            {
                                s_foo = "CREATE INDEX \"" + this.Name + "\" ON \"" + this.Table + "\" (";
                            }
                            else
                            {
                                s_foo = "ADD CONSTRAINT \"" + this.Name + "\" " + this.s_constraint + " (";
                            }

                            /* add all columns of constraint to query */
                            for (int i = 0; i < this.Columns.Count; i++)
                            {
                                if (i == this.Columns.Count - 1)
                                {
                                    s_foo += "\"" + this.Columns[i] + "\"";
                                }
                                else
                                {
                                    s_foo += "\"" + this.Columns[i] + "\"" + ", ";
                                }
                            }

                            /* close constraint */
                            s_foo += ")";
                        }
                        else if (this.s_alterOperation == "CHANGE")
                        { /* change existing constraint */
                            /* new name must be set for changing existing constraint */
                            if (ForestNET.Lib.Helper.IsStringEmpty(this.NewName))
                            {
                                throw new Exception("No new name for changing constraint");
                            }

                            /* add constraint with new name to query as INDEX or as CONSTRAINT */
                            if (this.s_constraint == "INDEX")
                            {
                                s_foo = "CREATE INDEX \"" + this.NewName + "\" ON \"" + this.Table + "\" (";
                            }
                            else
                            {
                                s_foo = "ADD CONSTRAINT \"" + this.NewName + "\" " + this.s_constraint + " (";
                            }

                            /* add all columns of constraint to query */
                            for (int i = 0; i < this.Columns.Count; i++)
                            {
                                if (i == this.Columns.Count - 1)
                                {
                                    s_foo += "\"" + this.Columns[i] + "\"";
                                }
                                else
                                {
                                    s_foo += "\"" + this.Columns[i] + "\"" + ", ";
                                }
                            }

                            /* close constraint */
                            s_foo += ")";

                            /* drop old constraint with query separator or just additionally if it is not an index */
                            if (this.s_constraint == "INDEX")
                            {
                                s_foo += this.QuerySeparator + "DROP INDEX \"" + this.Name + "\"" + " ON \"" + this.Table + "\"";
                            }
                            else
                            {
                                s_foo += ", DROP CONSTRAINT \"" + this.Name + "\"";
                            }
                        }
                        else if (this.s_alterOperation == "DROP")
                        { /* drop constraint */
                            if (this.s_constraint == "INDEX")
                            { /* drop index constraint */
                                s_foo = "DROP INDEX \"" + this.Name + "\"";
                            }
                            else
                            { /* drop constraint */
                                s_foo = "DROP CONSTRAINT \"" + this.Name + "\"";
                            }
                        }
                        break;
                    case BaseGateway.ORACLE:
                        if (this.s_alterOperation == "ADD")
                        { /* add new constraint */
                            /* add constraint with name to query as INDEX or as CONSTRAINT */
                            if (this.s_constraint == "INDEX")
                            {
                                s_foo = "CREATE INDEX \"" + this.Name + "\" ON \"" + this.Table + "\" (";
                            }
                            else
                            {
                                s_foo = "ADD CONSTRAINT \"" + this.Name + "\" " + this.s_constraint + " (";
                            }

                            /* add all columns of constraint to query */
                            for (int i = 0; i < this.Columns.Count; i++)
                            {
                                if (i == this.Columns.Count - 1)
                                {
                                    s_foo += "\"" + this.Columns[i] + "\"";
                                }
                                else
                                {
                                    s_foo += "\"" + this.Columns[i] + "\"" + ", ";
                                }
                            }

                            /* close constraint */
                            s_foo += ")";
                        }
                        else if (this.s_alterOperation == "CHANGE")
                        { /* change existing constraint */
                            /* new name must be set for changing existing constraint */
                            if (ForestNET.Lib.Helper.IsStringEmpty(this.NewName))
                            {
                                throw new Exception("No new name for changing constraint");
                            }

                            /* drop old constraint with query separator */
                            if (this.s_constraint == "INDEX")
                            {
                                s_foo = "DROP INDEX \"" + this.Name + "\"" + this.QuerySeparator;
                            }
                            else
                            {
                                s_foo = "DROP CONSTRAINT \"" + this.Name + "\"" + this.QuerySeparator + "ALTER TABLE \"" + this.Table + "\" ";
                            }

                            /* add constraint with new name to query as INDEX or as CONSTRAINT */
                            if (this.s_constraint == "INDEX")
                            {
                                s_foo += "CREATE INDEX \"" + this.NewName + "\" ON \"" + this.Table + "\" (";
                            }
                            else
                            {
                                s_foo += "ADD CONSTRAINT \"" + this.NewName + "\" " + this.s_constraint + " (";
                            }

                            /* add all columns of constraint to query */
                            for (int i = 0; i < this.Columns.Count; i++)
                            {
                                if (i == this.Columns.Count - 1)
                                {
                                    s_foo += "\"" + this.Columns[i] + "\"";
                                }
                                else
                                {
                                    s_foo += "\"" + this.Columns[i] + "\"" + ", ";
                                }
                            }

                            /* close constraint */
                            s_foo += ")";
                        }
                        else if (this.s_alterOperation == "DROP")
                        { /* drop constraint */
                            if (this.s_constraint == "INDEX")
                            { /* drop index constraint */
                                s_foo = "DROP INDEX \"" + this.Name + "\"";
                            }
                            else
                            { /* drop constraint */
                                s_foo += "DROP CONSTRAINT \"" + this.Name + "\"";
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
                s_foo = " >>>>> Constraint class Exception: [" + o_exc + "] <<<<< ";
            }

            return s_foo;
        }
    }
}