namespace ForestNET.Lib.SQL
{
    /// <summary>
    /// SQL class to generate a alter sql query based on fields and properties, called by ToString method
    /// </summary>
    public class Alter : QueryAbstract
    {

        /* Fields */

        /* Properties */

        public List<ColumnStructure> Columns = [];
        public List<Constraint> Constraints = [];

        public List<Dictionary<string, string>> SQLiteColumnsDefinition = [];
        public List<Dictionary<string, string>> SQLiteIndexes = [];

        /* Methods */

        /// <summary>
        /// constructor will be called with Query object, because all necessary database gateway, sql type and table information are part of Query class
        /// </summary>
        /// <param name="p_o_query">query object with table and sql type information</param>
        /// <exception cref="ArgumentException">invalid database gateway value from query parameter object</exception>
        public Alter(IQuery? p_o_query) : base(p_o_query)
        {

        }

        /// <summary>
        /// create alter sql string query
        /// </summary>
        public override string ToString()
        {
            string s_foo = "";

            try
            {
                bool b_exc = false;

                switch (this.BaseGateway)
                {
                    case BaseGateway.MARIADB:
                    case BaseGateway.NOSQLMDB:
                        /* start alter query */
                        s_foo = "ALTER TABLE " + "`" + this.Table + "`" + " ";

                        if ((this.Columns.Count <= 0) && (this.Constraints.Count <= 0))
                        {
                            throw new Exception("Columns and Constraints object lists are empty");
                        }
                        else
                        {
                            if (this.Columns.Count > 0)
                            {
                                /* list all alter columns within query */
                                foreach (ColumnStructure o_column in this.Columns)
                                {
                                    s_foo += o_column.ToString() + ", ";
                                }
                            }
                            else if (this.Constraints.Count > 0)
                            {
                                /* list all alter constraints within query */
                                foreach (Constraint o_constraint in this.Constraints)
                                {
                                    s_foo += o_constraint.ToString() + ", ";
                                }
                            }

                            /* remove last ', ' separator */
                            s_foo = s_foo.Substring(0, s_foo.Length - 2);
                        }
                        break;
                    case BaseGateway.SQLITE:
                        if (!((this.Columns.Count > 0) ^ (this.Constraints.Count > 0)))
                        {
                            throw new Exception("Columns and Constraints object lists are both empty or both set");
                        }
                        else
                        {
                            /* handle column changes for sqlite */
                            if (this.Columns.Count >= 1)
                            {
                                int i = 1;
                                ColumnStructure? o_changeColumn = null;
                                List<string> a_deleteColumns = [];

                                /* iterate each column */
                                foreach (ColumnStructure o_column in this.Columns)
                                {
                                    /* start alter query */
                                    s_foo += "ALTER TABLE " + "`" + this.Table + "`" + " ";

                                    /* change a column, only one possible for one alter query */
                                    if (o_column.AlterOperation.Equals("CHANGE"))
                                    {
                                        if (this.Columns.Count > 1)
                                        {
                                            throw new Exception("Columns object lists must contain only one item for CHANGE operation");
                                        }

                                        o_changeColumn = o_column;
                                        break;
                                    }

                                    /* drop a column, notice column which should be deleted for not creating it new with new temp table */
                                    if (o_column.AlterOperation.Equals("DROP"))
                                    {
                                        a_deleteColumns.Add(o_column.Name);
                                    }

                                    if (o_column.AlterOperation.Equals("ADD"))
                                    {
                                        s_foo += "ADD ";
                                    }

                                    if (this.Columns.Count == i++)
                                    {
                                        s_foo += o_column.ToString();
                                    }
                                    else
                                    {
                                        s_foo += o_column.ToString() + this.QuerySeparator;
                                    }
                                }

                                /* if we want to change a column or dropping multiple columns */
                                if ((o_changeColumn != null) || (a_deleteColumns.Count > 0))
                                {
                                    s_foo = "";

                                    /* get all columns and indices of current table */
                                    if (this.SQLiteColumnsDefinition.Count < 1)
                                    {
                                        throw new Exception("No sqlite columns information loaded");
                                    }

                                    List<string> a_columnsNew = [];
                                    List<string> a_columnsOld = [];

                                    /* generate a random prefix for temp table name, not starting with a digit */
                                    string? s_randomPrefix = null;

                                    do
                                    {
                                        s_randomPrefix = ForestNET.Lib.Helper.GenerateRandomString(16) + "_";
                                    } while (char.IsDigit(s_randomPrefix[0]));

                                    /* CREATE TABLE random prefix + "table" (all columns with column new name) */
                                    Query<Create> o_queryNew = new(this.BaseGateway, SqlType.CREATE, s_randomPrefix + this.Table);

                                    foreach (Dictionary<string, string> o_columnDefinition in this.SQLiteColumnsDefinition)
                                    {
                                        ColumnStructure o_column = new(o_queryNew);
                                        o_column.ColumnTypeAllocation(o_columnDefinition["columnType"]);
                                        o_column.Name = o_columnDefinition["name"];
                                        o_column.AlterOperation = "ADD";

                                        if (a_deleteColumns.Contains(o_columnDefinition["name"].ToString()))
                                        {
                                            /* skip columns which should be deleted */
                                            continue;
                                        }
                                        else if ((o_changeColumn != null) && (o_column.Name.Equals(o_changeColumn.Name)))
                                        {
                                            /* handle change column */
                                            o_column.Name = o_changeColumn.NewName;
                                            o_column.AssumeColumnTypeSqlite(o_changeColumn);
                                            a_columnsNew.Add(o_changeColumn.NewName);
                                            a_columnsOld.Add(o_changeColumn.Name);
                                        }
                                        else
                                        {
                                            /* assume all other columns */
                                            a_columnsNew.Add(o_columnDefinition["name"].ToString());
                                            a_columnsOld.Add(o_columnDefinition["name"].ToString());
                                        }

                                        /* add all stored constraint settings for column */
                                        if (o_columnDefinition.TryGetValue("constraints", out string? s_bar))
                                        {
                                            string[] a_constraints = s_bar.Split(";");

                                            for (int j = 0; j < a_constraints.Length; j++)
                                            {
                                                o_column.AddConstraint(o_queryNew.ConstraintTypeAllocation(a_constraints[j]));

                                                if ((a_constraints[j].CompareTo("DEFAULT") == 0) && (o_columnDefinition.TryGetValue("constraintDefaultValue", out string? s_baz)))
                                                {
                                                    o_column.ConstraintDefaultValue = (Object)s_baz;
                                                }
                                            }
                                        }

                                        o_queryNew.GetQuery<Create>()?.Columns.Add(o_column);
                                    }

                                    /* Create table does not return a value */
                                    s_foo += o_queryNew.ToString() + this.QuerySeparator;

                                    /* INSERT INTO random prefix + "table" (all columns with column new name) SELECT (all columns with column old name) FROM "table" */
                                    Query<Insert> o_queryInsert = new(this.BaseGateway, SqlType.INSERT, s_randomPrefix + this.Table);
                                    o_queryInsert.SetQuery("INSERT INTO `" + s_randomPrefix + this.Table + "` (`" + string.Join("`,`", a_columnsNew) + "`) SELECT `" + string.Join("`,`", a_columnsOld) + "` FROM `" + this.Table + "`");
                                    s_foo += o_queryInsert.ToString() + this.QuerySeparator;

                                    /* DROP "table" */
                                    Query<Drop> o_queryDrop = new(this.BaseGateway, SqlType.DROP, this.Table ?? "");
                                    s_foo += o_queryDrop.ToString() + this.QuerySeparator;

                                    /* ALTER TABLE random prefix + "table" RENAME TO "table" */
                                    Query<Alter> o_queryAlter = new(this.BaseGateway, SqlType.ALTER, s_randomPrefix + this.Table);
                                    o_queryAlter.SetQuery("ALTER TABLE `" + s_randomPrefix + this.Table + "` RENAME TO `" + this.Table + "`");
                                    s_foo += o_queryAlter.ToString() + this.QuerySeparator;

                                    /* add old indices to table */
                                    if (this.SQLiteIndexes.Count > 0)
                                    {
                                        foreach (Dictionary<string, string> o_index in this.SQLiteIndexes)
                                        {
                                            if (ForestNET.Lib.Helper.IsStringEmpty(o_index["columns"]))
                                            {
                                                throw new Exception("Sqlite[" + o_index["name"] + "] index has no columns");
                                            }

                                            string Name = o_index["name"];
                                            bool b_unique = o_index["unique"].Equals("1");
                                            string[] a_indexColumns = o_index["columns"].Split(";");

                                            bool b_skip = false;

                                            /* check for columns to be deleted not going as indices to table */
                                            foreach (string s_indexColumn in a_indexColumns)
                                            {
                                                if (a_deleteColumns.Contains(s_indexColumn))
                                                {
                                                    b_skip = true;
                                                }
                                            }

                                            /* skip deleted columns */
                                            if (b_skip)
                                            {
                                                continue;
                                            }

                                            /* if we want to change a column, we have to change it's name for indices as well */
                                            if ((o_changeColumn != null) && (Name.Equals(o_changeColumn.Name)))
                                            {
                                                Name = Name.Replace(o_changeColumn.Name, o_changeColumn.NewName);

                                                for (int j = 0; j < a_indexColumns.Length; j++)
                                                {
                                                    if (a_indexColumns[j].Equals(o_changeColumn.Name))
                                                    {
                                                        a_indexColumns[j] = o_changeColumn.NewName;
                                                    }
                                                }
                                            }

                                            /* create index for table */
                                            string s_constraintType = "INDEX";

                                            if (b_unique)
                                            {
                                                s_constraintType = "UNIQUE";
                                            }

                                            Query<Alter> o_queryAlterSqlite = new(this.BaseGateway, SqlType.ALTER, this.Table ?? "");
                                            Constraint o_constraint = new(o_queryAlterSqlite, s_constraintType, o_index["name"], "", "ADD");

                                            foreach (string s_indexColumn in a_indexColumns)
                                            {
                                                o_constraint.Columns.Add(s_indexColumn);
                                            }

                                            o_queryAlterSqlite.GetQuery<Alter>()?.Constraints.Add(o_constraint);

                                            s_foo += o_queryAlterSqlite.ToString() + this.QuerySeparator;
                                        }
                                    }
                                }
                            }
                            else if (this.Constraints.Count == 1)
                            {
                                /* list all alter constraints within query */
                                foreach (Constraint o_constraint in this.Constraints)
                                {
                                    s_foo += o_constraint.ToString() + ", ";
                                }

                                /* remove last ', ' separator */
                                s_foo = s_foo.Substring(0, s_foo.Length - 2);
                            }
                            else
                            {
                                throw new Exception("Constraints object lists must contain only one item");
                            }
                        }
                        break;
                    case BaseGateway.MSSQL:
                        /* start alter query */
                        s_foo = "ALTER TABLE " + "[" + this.Table + "]" + " ";

                        if ((this.Columns.Count <= 0) && (this.Constraints.Count <= 0))
                        {
                            throw new Exception("Columns and Constraints object lists are empty");
                        }
                        else
                        {
                            /* list all alter columns within query */
                            if (this.Columns.Count > 0)
                            {
                                bool b_once = false;
                                int i = 1;

                                /* iterate each column */
                                foreach (ColumnStructure o_column in this.Columns)
                                {
                                    if (o_column.AlterOperation.Equals("ADD"))
                                    {
                                        /* just add 'ADD ' once within alter query */
                                        if (!b_once)
                                        {
                                            s_foo += "ADD ";
                                            b_once = true;
                                        }
                                    }
                                    else if (o_column.AlterOperation.Equals("CHANGE"))
                                    {
                                        s_foo = "";

                                        /* use mssql execute command for renaming a column, and set it as first query command */
                                        if (!ForestNET.Lib.Helper.IsStringEmpty(o_column.NewName))
                                        {
                                            s_foo += "EXEC sp_rename \"[" + this.Table + "].[" + o_column.Name + "]\", \"" + o_column.NewName + "\", \"COLUMN\"" + this.QuerySeparator;
                                        }

                                        /* renew start alter query */
                                        s_foo += "ALTER TABLE " + "[" + this.Table + "]" + " ALTER ";
                                    }
                                    else if (o_column.AlterOperation.Equals("DROP"))
                                    {
                                        /* just add 'DROP ' once within alter query */
                                        if (!b_once)
                                        {
                                            s_foo += "DROP ";
                                            b_once = true;
                                        }
                                    }

                                    /* add each column to alter sql query */
                                    if (this.Columns.Count == i++)
                                    {
                                        s_foo += o_column.ToString();
                                    }
                                    else
                                    {
                                        /* if we change a column, we use query separator, because of mssql execute command */
                                        if (o_column.AlterOperation.Equals("CHANGE"))
                                        {
                                            s_foo += o_column.ToString() + this.QuerySeparator;
                                        }
                                        else
                                        {
                                            s_foo += o_column.ToString() + ", ";
                                        }
                                    }
                                }
                            }
                            else if (this.Constraints.Count > 0)
                            { /* list all alter constraints within query */
                                s_foo = "";

                                foreach (Constraint o_constraint in this.Constraints)
                                {
                                    s_foo += o_constraint.ToString() + this.QuerySeparator;
                                }

                                /* remove last ', ' separator */
                                s_foo = s_foo.Substring(0, s_foo.Length - this.QuerySeparator.Length);
                            }
                        }
                        break;
                    case BaseGateway.ORACLE:
                        /* start alter query */
                        s_foo = "ALTER TABLE " + "\"" + this.Table + "\"" + " ";

                        if ((this.Columns.Count <= 0) && (this.Constraints.Count <= 0))
                        {
                            throw new Exception("Columns and Constraints object lists are empty");
                        }
                        else
                        {
                            if (this.Columns.Count > 0)
                            { /* list all alter columns within query */
                                bool b_closeAdd = false;
                                bool b_closeModify = false;
                                bool b_closeDrop = false;

                                /* add 'ADD ' command bracket and notice that we need to close it */
                                if (this.Columns[0].AlterOperation.Equals("ADD"))
                                {
                                    s_foo += "ADD (";
                                    b_closeAdd = true;
                                }

                                /* add 'MODIFY ' command bracket and notice that we need to close it */
                                if (this.Columns[0].AlterOperation.Equals("CHANGE"))
                                {
                                    s_foo += "MODIFY (";
                                    b_closeModify = true;

                                    /* use additional alter query for renaming a column, and set it as first query command separated with query separator */
                                    if (!ForestNET.Lib.Helper.IsStringEmpty(this.Columns[0].NewName))
                                    {
                                        s_foo = "ALTER TABLE " + "\"" + this.Table + "\"" + " RENAME COLUMN \"" + this.Columns[0].Name + "\" TO \"" + this.Columns[0].NewName + "\"" + this.QuerySeparator + s_foo;
                                    }
                                }

                                /* add 'DROP ' command bracket and notice that we need to close it */
                                if (this.Columns[0].AlterOperation.Equals("DROP"))
                                {
                                    s_foo += "DROP (";
                                    b_closeDrop = true;
                                }

                                /* add each column to alter query */
                                foreach (ColumnStructure o_column in this.Columns)
                                {
                                    s_foo += o_column.ToString() + ", ";
                                }

                                /* remove last ', ' separator */
                                s_foo = s_foo.Substring(0, s_foo.Length - 2);

                                /* close command bracket */
                                if ((b_closeAdd) || (b_closeModify) || (b_closeDrop))
                                {
                                    s_foo += ")";
                                }
                            }
                            else if (this.Constraints.Count > 0)
                            { /* list all alter constraints within query */
                                int i = 1;

                                /* iterate each constraint object */
                                foreach (Constraint o_constraint in this.Constraints)
                                {
                                    /* alter index constraint -> empty current alter query */
                                    if (o_constraint.ConstraintStr.Equals("INDEX"))
                                    {
                                        s_foo = "";
                                    }

                                    if (this.Constraints.Count == i++)
                                    { /* add constraint to alter query, without ', ' separator */
                                        s_foo += o_constraint.ToString();
                                    }
                                    else
                                    {
                                        /* handle alter index constraint as separate alter query */
                                        if (o_constraint.ConstraintStr.Equals("INDEX"))
                                        {
                                            s_foo += o_constraint.ToString() + this.QuerySeparator;
                                        }
                                        else
                                        { /* add constraint to alter query */
                                            s_foo += o_constraint.ToString() + ", ";
                                        }
                                    }
                                }
                            }
                        }
                        break;
                    case BaseGateway.PGSQL:
                        /* start alter query */
                        s_foo = "ALTER TABLE " + "\"" + this.Table + "\"" + " ";

                        if ((this.Columns.Count <= 0) && (this.Constraints.Count <= 0))
                        {
                            throw new Exception("Columns and Constraints object lists are empty");
                        }
                        else
                        {
                            if (this.Columns.Count > 0)
                            { /* list all alter columns within query */
                                /* use additional alter query for renaming a column, and set it as first query command separated with query separator */
                                if (this.Columns[0].AlterOperation.Equals("CHANGE"))
                                {
                                    if (!ForestNET.Lib.Helper.IsStringEmpty(this.Columns[0].NewName))
                                    {
                                        s_foo = "ALTER TABLE " + "\"" + this.Table + "\"" + " RENAME COLUMN \"" + this.Columns[0].Name + "\" TO \"" + this.Columns[0].NewName + "\"" + this.QuerySeparator + s_foo;
                                    }
                                }

                                /* add each column to alter query */
                                foreach (ColumnStructure o_column in this.Columns)
                                {
                                    s_foo += o_column.ToString() + ", ";
                                }

                                /* remove last ', ' separator */
                                s_foo = s_foo.Substring(0, s_foo.Length - 2);
                            }
                            else if (this.Constraints.Count > 0)
                            { /* list all alter constraints within query */
                                int i = 1;

                                /* iterate each constraint object */
                                foreach (Constraint o_constraint in this.Constraints)
                                {
                                    /* alter index constraint -> empty current alter query */
                                    if (o_constraint.ConstraintStr.Equals("INDEX"))
                                    {
                                        s_foo = "";
                                    }

                                    if (this.Constraints.Count == i++)
                                    { /* add constraint to alter query, without ', ' separator */
                                        s_foo += o_constraint.ToString();
                                    }
                                    else
                                    {
                                        /* handle alter index constraint as separate alter query */
                                        if (o_constraint.ConstraintStr.Equals("INDEX"))
                                        {
                                            s_foo += o_constraint.ToString() + this.QuerySeparator;
                                        }
                                        else
                                        { /* add constraint to alter query */
                                            s_foo += o_constraint.ToString() + ", ";
                                        }
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
                s_foo = " >>>>> Alter class Exception: [" + o_exc + "] <<<<< ";
            }

            return s_foo;
        }
    }
}