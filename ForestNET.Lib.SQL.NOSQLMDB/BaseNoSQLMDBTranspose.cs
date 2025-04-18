using MongoDB.Bson;

namespace ForestNET.Lib.SQL.NOSQLMDB
{
    /// <summary>
    /// Transpose a ForestNET.Lib.SQL.Query object to bson command list for nosqlmdb use.
    /// </summary>
    public class BaseNoSQLMDBTranspose
    {

        /* Fields */

        /* Properties */

        /* Methods */

        /// <summary>
        /// transpose method which will handle all the work by using ForestNET.Lib.SQL.Query object as parameter
        /// automatic identification of query type and peculiarities of queries
        /// </summary>
        /// <param name="p_o_sqlQuery">ForestNET.Lib.SQL.Query object</param>
        /// <returns>bson command list for nosqlmdb use</returns>
        /// <exception cref="ArgumentException">illegal constellation within sql query where we cannot transpose do bson command list for nosqlmdb, e.g. XOR filter operator</exception>
        public static List<BsonDocument>? Transpose(IQuery p_o_sqlQuery)
        {
            List<BsonDocument>? a_return = null;

            switch (p_o_sqlQuery.GetQuery<QueryAbstract>()?.SqlType)
            {
                case SqlType.CREATE:
                    a_return = TransposeCreateQuery(p_o_sqlQuery);
                    break;
                case SqlType.ALTER:
                    a_return = TransposeAlterQuery(p_o_sqlQuery);
                    break;
                case SqlType.INSERT:
                    a_return = TransposeInsertQuery(p_o_sqlQuery);
                    break;
                case SqlType.UPDATE:
                    a_return = TransposeUpdateQuery(p_o_sqlQuery);
                    break;
                case SqlType.DELETE:
                    a_return = TransposeDeleteQuery(p_o_sqlQuery);
                    break;
                case SqlType.TRUNCATE:
                    a_return = TransposeTruncateQuery(p_o_sqlQuery);
                    break;
                case SqlType.DROP:
                    a_return = TransposeDropQuery(p_o_sqlQuery);
                    break;
                case SqlType.SELECT:
                    a_return = TransposeSelectQuery(p_o_sqlQuery);
                    break;
            }

            return a_return;
        }

        private static List<BsonDocument> TransposeCreateQuery(IQuery p_o_sqlQuery)
        {
            Create o_createQuery = p_o_sqlQuery.GetQuery<Create>() ?? throw new NullReferenceException("Create query is null");
            string s_collection = p_o_sqlQuery.Table ?? throw new NullReferenceException("Table within create query is null");
            List<BsonDocument> a_return =
            [
                /* create table as nosqlmdb collection */
                new BsonDocument("create", s_collection)
            ];

            /* list of primary keys or unique keys */
            List<BsonDocument> a_puks = [];

            /* iterate columns structures of create query */
            foreach (ColumnStructure ColumnStructure in o_createQuery.Columns)
            {
                if (ColumnStructure.AlterOperation == "ADD")
                {
                    /* column name */
                    string s_name = ColumnStructure.Name;

                    /* add constraints to current columns structure */
                    if (ColumnStructure.ConstraintList.Count > 0)
                    {
                        foreach (string s_constraint in ColumnStructure.ConstraintList)
                        {
                            if ((s_constraint.Equals("PRIMARY KEY")) || (s_constraint.Equals("UNIQUE")))
                            {
                                a_puks.Add(
                                    new BsonDocument("key",
                                        new BsonDocument(s_name, 1)
                                    )
                                    .Add("name", s_collection + "_" + s_name + "_puk")
                                    .Add("unique", 1)
                                );
                            }
                        }
                    }
                }
            }

            /* check if we need to create some indexes for primary key or unique columns */
            if (a_puks.Count > 0)
            {
                a_return.Add(
                    new BsonDocument()
                        .Add("createIndexes", s_collection)
                        .Add("indexes", new BsonArray(a_puks))
                );
            }

            return a_return;
        }

        private static List<BsonDocument> TransposeAlterQuery(IQuery p_o_sqlQuery)
        {
            Alter o_alterQuery = p_o_sqlQuery.GetQuery<Alter>() ?? throw new NullReferenceException("Alter query is null");
            string s_collection = p_o_sqlQuery.Table ?? throw new NullReferenceException("Table within alter query is null");
            List<BsonDocument> a_return = [];

            /* can only handle adding/changing/deleting columns or constraints but not both at the same time */
            if (!((o_alterQuery.Columns.Count > 0) ^ (o_alterQuery.Constraints.Count > 0)))
            {
                throw new ArgumentException("Columns and Constraints object lists are both empty or both set");
            }
            else
            {
                if (o_alterQuery.Columns.Count > 0)
                {
                    /* list of primary keys or unique keys */
                    List<BsonDocument> a_puks = [];

                    /* variables to gather column alter operations */
                    BsonDocument? o_addColumns = null;
                    BsonDocument? o_changeColumns = null;
                    BsonDocument? o_deleteColumns = null;

                    /* handle all alter columns within query */
                    foreach (ColumnStructure ColumnStructure in o_alterQuery.Columns)
                    {
                        if (ColumnStructure.AlterOperation.Equals("ADD"))
                        { /* add a column */
                            /* column name */
                            string s_name = ColumnStructure.Name;

                            /* use new name */
                            if (ColumnStructure.NewName.Length > 0)
                            {
                                s_name = ColumnStructure.NewName;
                            }

                            if (o_addColumns == null)
                            {
                                o_addColumns = new BsonDocument(s_name, BsonNull.Value);
                            }
                            else
                            {
                                o_addColumns.Add(s_name, BsonNull.Value);
                            }

                            /* add constraints to current columns structure */
                            if (ColumnStructure.ConstraintList.Count > 0)
                            {
                                foreach (string s_constraint in ColumnStructure.ConstraintList)
                                {
                                    if ((s_constraint.Equals("PRIMARY KEY")) || (s_constraint.Equals("UNIQUE")))
                                    {
                                        a_puks.Add(
                                            new BsonDocument("key",
                                                new BsonDocument(s_name, 1)
                                            )
                                            .Add("name", s_collection + "_" + s_name + "_puk")
                                            .Add("unique", 1)
                                        );
                                    }
                                }
                            }
                        }
                        else if (ColumnStructure.AlterOperation.Equals("CHANGE"))
                        { /* change a column */
                            /* change column name */
                            if (ColumnStructure.NewName.Length > 0)
                            {
                                if (o_changeColumns == null)
                                {
                                    o_changeColumns = new BsonDocument(ColumnStructure.Name, ColumnStructure.NewName);
                                }
                                else
                                {
                                    o_changeColumns.Add(ColumnStructure.Name, ColumnStructure.NewName);
                                }
                            }
                        }
                        else if (ColumnStructure.AlterOperation.Equals("DROP"))
                        { /* drop a column */
                            if (o_deleteColumns == null)
                            {
                                o_deleteColumns = new BsonDocument(ColumnStructure.Name, 0);
                            }
                            else
                            {
                                o_deleteColumns.Add(ColumnStructure.Name, 0);
                            }
                        }
                    }

                    /* only create commands if we have new columns in query */
                    if (o_addColumns != null)
                    {
                        a_return.Add(
                            new BsonDocument()
                                .Add("update", s_collection)
                                .Add("updates", new BsonArray(
                                    new List<BsonDocument>() {
                                        new BsonDocument("q", new BsonDocument())
                                        .Add("u", new BsonDocument(
                                            "$set",
                                                o_addColumns
                                        ))
                                        .Add("multi", true)
                                    }
                                ))
                        );

                        /* check if we need to create some indexes for primary key or unique columns */
                        if (a_puks.Count > 0)
                        {
                            a_return.Add(
                                new BsonDocument()
                                    .Add("createIndexes", s_collection)
                                    .Add("indexes", new BsonArray(a_puks)
                                )
                            );
                        }
                    }

                    /* only create commands if we change any columns name in query */
                    if (o_changeColumns != null)
                    {
                        a_return.Add(
                            new BsonDocument()
                                .Add("update", s_collection)
                                .Add("updates", new BsonArray(
                                    new List<BsonDocument>() {
                                        new BsonDocument("q", new BsonDocument())
                                        .Add("u", new BsonDocument(
                                            "$rename",
                                                o_changeColumns
                                        ))
                                        .Add("multi", true)
                                    }
                                ))
                        );
                    }

                    /* only create commands if we delete some columns in query */
                    if (o_deleteColumns != null)
                    {
                        a_return.Add(
                            new BsonDocument()
                                .Add("update", s_collection)
                                .Add("updates", new BsonArray(
                                    new List<BsonDocument>() {
                                        new BsonDocument("q", new BsonDocument())
                                        .Add("u", new BsonDocument(
                                            "$unset",
                                                o_deleteColumns
                                        ))
                                        .Add("multi", true)
                                    }
                                ))
                        );
                    }
                }
                else if (o_alterQuery.Constraints.Count > 0)
                {
                    /* list to gather constraint alter operations */
                    List<string> a_deleteConstraintsBecauseOfChange = [];
                    List<BsonDocument> a_constraints = [];
                    List<string> a_deleteConstraints = [];

                    /* handle all alter constraints within query */
                    foreach (Constraint o_constraint in o_alterQuery.Constraints)
                    {
                        if (o_constraint.AlterOperation.Equals("ADD"))
                        { /* add new constraint */
                            /* gather all columns for unique key constraint */
                            BsonDocument? a_columns = null;

                            /* add all columns of constraint to document variable */
                            foreach (string s_column in o_constraint.Columns)
                            {
                                if (a_columns == null)
                                {
                                    a_columns = new BsonDocument(s_column, 1);
                                }
                                else
                                {
                                    a_columns.Add(s_column, 1);
                                }
                            }

                            /* add unique key for createIndexes command */
                            a_constraints.Add(
                                new BsonDocument("key",
                                    a_columns
                                )
                                .Add("name", s_collection + "_" + o_constraint.Name + ((o_constraint.ConstraintStr.Equals("UNIQUE")) ? "_puk" : "_ik"))
                                .Add("unique", ((o_constraint.ConstraintStr.Equals("UNIQUE")) ? 1 : 0))
                            );
                        }
                        else if (o_constraint.AlterOperation.Equals("CHANGE"))
                        { /* change existing constraint */
                            a_deleteConstraintsBecauseOfChange.Add(s_collection + "_" + o_constraint.Name + ((o_constraint.ConstraintStr.Equals("UNIQUE")) ? "_puk" : "_ik"));

                            /* gather all columns for unique key constraint */
                            BsonDocument? a_columns = null;

                            /* add all columns of constraint to document variable */
                            foreach (string s_column in o_constraint.Columns)
                            {
                                if (a_columns == null)
                                {
                                    a_columns = new BsonDocument(s_column, 1);
                                }
                                else
                                {
                                    a_columns.Add(s_column, 1);
                                }
                            }

                            /* add unique key for createIndexes command */
                            a_constraints.Add(
                                new BsonDocument("key",
                                    a_columns
                                )
                                .Add("name", s_collection + "_" + o_constraint.NewName + ((o_constraint.ConstraintStr.Equals("UNIQUE")) ? "_puk" : "_ik"))
                                .Add("unique", ((o_constraint.ConstraintStr.Equals("UNIQUE")) ? 1 : 0))
                            );
                        }
                        else if (o_constraint.AlterOperation.Equals("DROP"))
                        { /* drop constraint */
                            a_deleteConstraints.Add(s_collection + "_" + o_constraint.Name + ((o_constraint.ConstraintStr.Equals("UNIQUE")) ? "_puk" : "_ik"));
                        }
                    }

                    /* only create commands if we delete some indexes in query because of changing a constraint */
                    if (a_deleteConstraintsBecauseOfChange.Count > 0)
                    {
                        a_return.Add(
                            new BsonDocument()
                                .Add("dropIndexes", s_collection)
                                .Add("index", new BsonArray(a_deleteConstraintsBecauseOfChange)
                            )
                        );
                    }

                    /* check if we need to create some indexes for unique columns */
                    if (a_constraints.Count > 0)
                    {
                        a_return.Add(
                            new BsonDocument()
                                .Add("createIndexes", s_collection)
                                .Add("indexes", new BsonArray(a_constraints)
                            )
                        );
                    }

                    /* only create commands if we delete some indexes in query */
                    if (a_deleteConstraints.Count > 0)
                    {
                        a_return.Add(
                            new BsonDocument()
                                .Add("dropIndexes", s_collection)
                                .Add("index", new BsonArray(a_deleteConstraints)
                            )
                        );
                    }
                }
            }

            return a_return;
        }

        private static List<BsonDocument> TransposeInsertQuery(IQuery p_o_sqlQuery)
        {
            Insert o_insertQuery = p_o_sqlQuery.GetQuery<Insert>() ?? throw new NullReferenceException("Insert query is null");
            string s_collection = p_o_sqlQuery.Table ?? throw new NullReferenceException("Table within create query is null");
            List<BsonDocument> a_return = [];

            /* add autoincrement command  */
            if (o_insertQuery.NoSQLMDBColumnAutoIncrement != null)
            {
                a_return.Add(new BsonDocument("autoincrement_collection", s_collection).Add("autoincrement_column", o_insertQuery.NoSQLMDBColumnAutoIncrement.ColumnStr));
            }

            /* store values we retrieve from insert query */
            List<KeyValuePair<string, Object>> a_values = [];

            _ = Query<Column>.ConvertToPreparedStatementQuery(BaseGateway.NOSQLMDB, o_insertQuery.ToString(), a_values, false);

            LogValuesFromQuery(a_values);

            /* check if amount of values for query statement and column values of insert query are equal */
            if (a_values.Count != o_insertQuery.ColumnValues.Count)
            {
                throw new ArgumentException("Amount of values does not match between query statement and query column values [" + a_values.Count + " != " + o_insertQuery.ColumnValues.Count + "]");
            }

            /* document variable for all column value pairs in insert query  */
            BsonDocument? o_insertColumnValuePairs = null;
            /* gather all column value pairs as a dictionary */
            Dictionary<string, Object?> o_columnValuePairs = [];

            /* handle all column value pairs of insert query */
            int i = 0;

            foreach (ColumnValue ColumnValue in o_insertQuery.ColumnValues)
            {
                if (o_insertColumnValuePairs == null)
                {
                    o_insertColumnValuePairs = new BsonDocument("_id", ForestNET.Lib.Helper.GenerateUUID().Replace("-", "").Substring(0, 24));

                    if (o_insertQuery.NoSQLMDBColumnAutoIncrement != null)
                    {
                        o_insertColumnValuePairs.Add(o_insertQuery.NoSQLMDBColumnAutoIncrement.ColumnStr, "FORESTJ_REPLACE_AUTOINCREMENT_VALUE");
                    }
                }

                /* next column value pair */
                KeyValuePair<string, Object> o_entry = a_values[i++];
                /* add column value pair to dictionary */
                o_columnValuePairs.Add(ColumnValue.Column?.ColumnStr ?? "no key value", TransposeValueFromQuery(o_entry));
            }

            /* add all column value pairs to insert bson document */
            o_insertColumnValuePairs?.AddRange(o_columnValuePairs);

            if (o_insertColumnValuePairs != null)
            {
                a_return.Add(
                    new BsonDocument()
                        .Add("insert", s_collection)
                        .Add("documents",
                            new BsonArray { o_insertColumnValuePairs }
                        )
                        .Add("ordered", true)
                );
            }

            return a_return;
        }

        private static List<BsonDocument> TransposeUpdateQuery(IQuery p_o_sqlQuery)
        {
            Update o_updateQuery = p_o_sqlQuery.GetQuery<Update>() ?? throw new NullReferenceException("Update query is null");
            string s_collection = p_o_sqlQuery.Table ?? throw new NullReferenceException("Table within update query is null");
            List<BsonDocument> a_return = [];

            /* store values we retrieve from update query */
            List<KeyValuePair<string, Object>> a_values = [];

            _ = Query<Column>.ConvertToPreparedStatementQuery(BaseGateway.NOSQLMDB, o_updateQuery.ToString(false), a_values, false);

            LogValuesFromQuery(a_values);

            /* check if amount of values for query statement and column values of update query are equal */
            if (a_values.Count != o_updateQuery.ColumnValues.Count)
            {
                throw new ArgumentException("Amount of values does not match between query statement and query column values [" + a_values.Count + " != " + o_updateQuery.ColumnValues.Count + "]");
            }

            /* document variable for all column value pairs in insert query  */
            BsonDocument? o_updateColumnValuePairs = null;

            /* handle all column value pairs of insert query */
            int i = 0;

            foreach (ColumnValue ColumnValue in o_updateQuery.ColumnValues)
            {
                KeyValuePair<string, Object> o_entry = a_values[i++];

                if (o_updateColumnValuePairs == null)
                {
                    o_updateColumnValuePairs = new BsonDocument(new Dictionary<string, Object?>() { { ColumnValue.Column?.ColumnStr ?? "column key is null", TransposeValueFromQuery(o_entry) ?? BsonNull.Value } });
                }
                else
                {
                    o_updateColumnValuePairs.AddRange(new Dictionary<string, Object?>() { { ColumnValue.Column?.ColumnStr ?? "column key is null", TransposeValueFromQuery(o_entry) ?? BsonNull.Value } });
                }
            }

            /* document variable for all where clauses in update query  */
            BsonDocument? o_updateFilter = null;

            /* handle where clauses */
            if (o_updateQuery.Where.Count > 0)
            {
                o_updateFilter = TransposeWhereClause(o_updateQuery.Where);
            }

            if ((o_updateColumnValuePairs != null) && (o_updateFilter == null))
            { /* update query without filter */
                a_return.Add(
                    new BsonDocument()
                        .Add("update", s_collection)
                        .Add("updates", new BsonArray(
                            new List<BsonDocument>() {
                                new BsonDocument("q", new BsonDocument()) /* use no filter */
                                .Add("u", new BsonDocument(
                                    "$set",
                                        o_updateColumnValuePairs
                                ))
                                .Add("multi", true)
                            }
                        ))
                );
            }
            else if ((o_updateColumnValuePairs != null) && (o_updateFilter != null))
            { /* update query with filter */
                a_return.Add(
                    new BsonDocument()
                        .Add("update", s_collection)
                        .Add("updates", new BsonArray(
                            new List<BsonDocument>() {
                                new BsonDocument("q",
                                    o_updateFilter
                                )
                                .Add("u", new BsonDocument(
                                    "$set",
                                        o_updateColumnValuePairs
                                ))
                                .Add("multi", true)
                            }
                        ))
                );
            }

            return a_return;
        }

        private static List<BsonDocument> TransposeDeleteQuery(IQuery p_o_sqlQuery)
        {
            Delete o_deleteQuery = p_o_sqlQuery.GetQuery<Delete>() ?? throw new NullReferenceException("Delete query is null");
            string s_collection = p_o_sqlQuery.Table ?? throw new NullReferenceException("Table within delete query is null");
            List<BsonDocument> a_return = [];

            /* document variable for all where clauses in delete query  */
            BsonDocument? o_deleteFilter = null;

            /* handle where clauses */
            if (o_deleteQuery.Where.Count > 0)
            {
                o_deleteFilter = TransposeWhereClause(o_deleteQuery.Where);
            }

            /* check if we have a valid filter - we do not accept empty filter for security reasons, otherwise you can use Truncate */
            if (o_deleteFilter != null)
            {
                a_return.Add(
                    new BsonDocument()
                        .Add("delete", s_collection)
                        .Add("deletes", new BsonArray(
                            new List<BsonDocument>() {
                                new BsonDocument("q", o_deleteFilter)
                                .Add("limit", 0)
                            }
                        ))
                        .Add("ordered", true)
                );
            }

            return a_return;
        }

        private static List<BsonDocument> TransposeTruncateQuery(IQuery p_o_sqlQuery)
        {
            return
            [
                /* delete all documents in collection */
                new BsonDocument()
                    .Add("delete", p_o_sqlQuery.Table)
                    .Add("deletes", new BsonArray(
                        new List<BsonDocument>() {
                            new BsonDocument("q", new BsonDocument())
                            .Add("limit", 0)
                        }
                    ))
                    .Add("ordered", true)
            ];
        }

        private static List<BsonDocument> TransposeDropQuery(IQuery p_o_sqlQuery)
        {
            List<BsonDocument> a_return =
            [
                /* drop collection */
                new BsonDocument("drop", p_o_sqlQuery.Table)
            ];

            return a_return;
        }

        private static List<BsonDocument> TransposeSelectQuery(IQuery p_o_sqlQuery)
        {
            Select o_selectQuery = p_o_sqlQuery.GetQuery<Select>() ?? throw new NullReferenceException("Select query is null");

            if (o_selectQuery.Columns.Count <= 0)
            {
                throw new ArgumentException("Columns object list is empty for select query");
            }

            bool b_HasColumnsWithAggregationsButOnlyCountAll = true;

            if (o_selectQuery.HasColumnsWithAggregations())
            {
                foreach (Column Column in o_selectQuery.Columns)
                {
                    if ((!ForestNET.Lib.Helper.IsStringEmpty(Column.SqlAggregation)) && (!Column.SqlAggregation.Equals("COUNT")))
                    {
                        b_HasColumnsWithAggregationsButOnlyCountAll = false;
                    }
                }
            }

            if (o_selectQuery.Distinct)
            {
                return TransposeSelectQueryDistinct(p_o_sqlQuery);
            }
            else if ((o_selectQuery.Joins.Count < 1) && (o_selectQuery.GroupBy.Count < 1) && (b_HasColumnsWithAggregationsButOnlyCountAll))
            {
                return TransposeSelectQuerySimple(p_o_sqlQuery);
            }
            else
            {
                return TransposeSelectQueryComplex(p_o_sqlQuery);
            }
        }

        private static List<BsonDocument> TransposeSelectQueryDistinct(IQuery p_o_sqlQuery)
        {
            Select o_selectQuery = p_o_sqlQuery.GetQuery<Select>() ?? throw new NullReferenceException("Select query is null");
            string s_collection = p_o_sqlQuery.Table ?? throw new NullReferenceException("Table within select query is null");
            List<BsonDocument> a_return = [];

            if ((o_selectQuery.Columns.Count != 1) || (o_selectQuery.Columns[0].ColumnStr.Equals("*")))
            {
                throw new ArgumentException("Only one column is allowed for using distinct operator for nosqlmdb, and no '*' as column");
            }

            /* document variable for all where clauses in select query  */
            BsonDocument? o_filter = null;

            /* handle where clauses */
            if (o_selectQuery.Where.Count > 0)
            {
                o_filter = TransposeWhereClause(o_selectQuery.Where);
            }

            /* check if we have a valid filter */
            if (o_filter != null)
            {
                a_return.Add(
                    new BsonDocument()
                        .Add("distinct", s_collection)
                        .Add("key", o_selectQuery.Columns[0].ColumnStr)
                        .Add("query", o_filter)
                );
            }
            else
            {
                a_return.Add(
                    new BsonDocument()
                        .Add("distinct", s_collection)
                        .Add("key", o_selectQuery.Columns[0].ColumnStr)
                );
            }

            return a_return;
        }

        private static List<BsonDocument> TransposeSelectQuerySimple(IQuery p_o_sqlQuery)
        {
            Select o_selectQuery = p_o_sqlQuery.GetQuery<Select>() ?? throw new NullReferenceException("Select query is null");
            string s_collection = p_o_sqlQuery.Table ?? throw new NullReferenceException("Table within select query is null");
            List<BsonDocument> a_return = [];

            /* document variable for find command */
            BsonDocument o_select = new BsonDocument().Add("find", s_collection);

            /* document variable for columns projection */
            BsonDocument? a_columns = TransposeColumns(o_selectQuery.Columns);

            /* add columns from select query to nosqlmdb command */
            if (a_columns != null)
            {
                o_select.Add("projection", a_columns);
            }

            /* document variable for all where clauses in select query  */
            BsonDocument? o_filter = null;

            /* handle where clauses */
            if (o_selectQuery.Where.Count > 0)
            {
                o_filter = TransposeWhereClause(o_selectQuery.Where);
            }

            if (o_filter != null)
            {
                o_select.Add("filter", o_filter);
            }

            /* document variable for order by list */
            BsonDocument? o_orderBy = TransposeOrderBy(o_selectQuery.OrderBy);

            if (o_orderBy != null)
            {
                o_select.Add("sort", o_orderBy);
            }

            /* add limit clauses of query */
            if ((o_selectQuery.Limit != null) && (o_selectQuery.Limit.Interval > 0))
            {
                o_select.Add("skip", o_selectQuery.Limit.Start);
                o_select.Add("limit", o_selectQuery.Limit.Interval);
            }

            a_return.Add(o_select);

            return a_return;
        }

        private static List<BsonDocument> TransposeSelectQueryComplex(IQuery p_o_sqlQuery)
        {
            Select o_selectQuery = p_o_sqlQuery.GetQuery<Select>() ?? throw new NullReferenceException("Select query is null");

            if ((o_selectQuery.Joins.Count > 0) && (o_selectQuery.GroupBy.Count < 1) && (!o_selectQuery.HasColumnsWithAggregations()))
            { /* only a join to handle */
                return TransposeSelectQueryComplexOnlyJoin(p_o_sqlQuery);
            }
            else if ((o_selectQuery.Joins.Count < 1) && (o_selectQuery.GroupBy.Count > 0) && (o_selectQuery.HasColumnsWithAggregations()))
            { /* only group by to handle */
                return TransposeSelectQueryComplexOnlyGroupBy(p_o_sqlQuery);
            }
            else
            { /* a join and group by to handle */
                return TransposeSelectQueryComplexJoinAndGroupBy(p_o_sqlQuery);
            }
        }

        private static List<BsonDocument> TransposeSelectQueryComplexOnlyJoin(IQuery p_o_sqlQuery)
        {
            Select o_selectQuery = p_o_sqlQuery.GetQuery<Select>() ?? throw new NullReferenceException("Select query is null");
            string s_collection = p_o_sqlQuery.Table ?? throw new NullReferenceException("Table within select query is null");
            List<BsonDocument> a_return = [];

            if (o_selectQuery.Joins.Count != 1)
            {
                throw new ArgumentException("Nosqlmdb library can only handle one join, not '" + o_selectQuery.Joins.Count + "'");
            }

            /* get join information */
            Join o_join = o_selectQuery.Joins[0];

            if (o_join.Relations.Count != 1)
            {
                throw new ArgumentException("Nosqlmdb library can only handle one relation within a join, not '" + o_join.Relations.Count + "'");
            }

            /* get relation information */
            Relation o_relation = o_join.Relations[0];

            /* we will not handle operator, as we only can use equal, so we only use columnLeft and columnRight info */

            /* create */
            BsonDocument o_joinBsonDocument = new BsonDocument("from", o_join.Table)
                .Add("localField", o_relation.ColumnLeft?.ColumnStr)
                .Add("foreignField", o_relation.ColumnRight?.ColumnStr)
                .Add("as", "join_" + o_join.Table);

            /* document variable for all where clauses in select query  */
            BsonDocument? o_filterBsonDocument = null;

            /* handle where clauses */
            if (o_selectQuery.Where.Count > 0)
            {
                /* iterate all where clauses */
                for (int i = 0; i < o_selectQuery.Where.Count; i++)
                {
                    /* if where clause table is equal to join table, set flag */
                    if ((o_selectQuery.Where[i].Column?.Table ?? "").Equals(o_join.Table))
                    {
                        o_selectQuery.Where[i].IsJoinTable = true;
                    }
                }

                o_filterBsonDocument = TransposeWhereClause(o_selectQuery.Where);
            }

            if (o_selectQuery.Columns.Count > 0)
            {
                /* iterate all columns */
                for (int i = 0; i < o_selectQuery.Columns.Count; i++)
                {
                    /* if column table is equal to join table, set flag */
                    if ((o_selectQuery.Columns[i].Table ?? "").Equals(o_join.Table))
                    {
                        if (!ForestNET.Lib.Helper.IsStringEmpty(o_selectQuery.Columns[i].SqlAggregation))
                        {
                            throw new ArgumentException("Invalid column with aggregation on a join table at the same time. Only aggregation on main table allowed");
                        }

                        o_selectQuery.Columns[i].IsJoinTable = true;
                    }
                }
            }

            /* document variable for columns projection */
            BsonDocument? a_columns = TransposeColumns(o_selectQuery.Columns);

            if (o_selectQuery.OrderBy?.Columns.Count > 0)
            {
                /* iterate all columns of order by clause */
                for (int i = 0; i < o_selectQuery.OrderBy.Columns.Count; i++)
                {
                    /* if column table is equal to join table, set flag */
                    if ((o_selectQuery.OrderBy.Columns[i].Table ?? "").Equals(o_join.Table))
                    {
                        if (!ForestNET.Lib.Helper.IsStringEmpty(o_selectQuery.Columns[i].SqlAggregation))
                        {
                            throw new ArgumentException("Invalid column with aggregation on a join table at the same time. Only aggregation on main table allowed");
                        }

                        o_selectQuery.OrderBy.Columns[i].IsJoinTable = true;
                    }
                }
            }

            /* document variable for order by list */
            BsonDocument? o_orderBy = TransposeOrderBy(o_selectQuery.OrderBy);

            /* create list for aggregate pipeline */
            List<BsonDocument> a_pipeline =
            [
                /* add lookup */
                new BsonDocument("$lookup", o_joinBsonDocument),

                /* add unwind */
                new BsonDocument("$unwind", "$join_" + o_join.Table)
            ];

            /* add filter */
            if (o_filterBsonDocument != null)
            {
                a_pipeline.Add(new BsonDocument("$match", o_filterBsonDocument));
            }

            /* add projection */
            if (a_columns != null)
            {
                a_pipeline.Add(new BsonDocument("$project", a_columns));
            }

            /* add sort */
            if (o_orderBy != null)
            {
                a_pipeline.Add(new BsonDocument("$sort", o_orderBy));
            }

            /* add limit clauses of query */
            if ((o_selectQuery.Limit != null) && (o_selectQuery.Limit.Interval > 0))
            {
                a_pipeline.Add(new BsonDocument("$skip", o_selectQuery.Limit.Start));
                a_pipeline.Add(new BsonDocument("$limit", o_selectQuery.Limit.Interval));
            }

            /* finish aggregate command */
            a_return.Add(
                new BsonDocument()
                    .Add("aggregate", s_collection)
                    .Add("pipeline", new BsonArray(a_pipeline))
                    .Add("cursor", new BsonDocument()) /* needed for aggregate */
            );

            return a_return;
        }

        private static List<BsonDocument> TransposeSelectQueryComplexOnlyGroupBy(IQuery p_o_sqlQuery)
        {
            Select o_selectQuery = p_o_sqlQuery.GetQuery<Select>() ?? throw new NullReferenceException("Select query is null");
            string s_collection = p_o_sqlQuery.Table ?? throw new NullReferenceException("Table within select query is null");
            List<BsonDocument> a_return = [];

            /* document variable for first sort, if we use aggregation like MIN or MAX */
            BsonDocument? o_firstSort = null;

            List<Column> a_aggregations = [];

            foreach (Column o_column in o_selectQuery.Columns)
            {
                if (!ForestNET.Lib.Helper.IsStringEmpty(o_column.SqlAggregation))
                {
                    a_aggregations.Add(o_column);

                    if (o_column.SqlAggregation.Equals("MAX"))
                    {
                        if (o_firstSort == null)
                        {
                            o_firstSort = new BsonDocument(o_column.ColumnStr, -1); /* -1 -> DESC */
                        }
                        else
                        {
                            o_firstSort.Add(o_column.ColumnStr, -1); /* -1 -> DESC */
                        }
                    }
                    else if (o_column.SqlAggregation.Equals("MIN"))
                    {
                        if (o_firstSort == null)
                        {
                            o_firstSort = new BsonDocument(o_column.ColumnStr, 1); /* 1 -> ASC */
                        }
                        else
                        {
                            o_firstSort.Add(o_column.ColumnStr, 1); /* 1 -> ASC */
                        }
                    }
                }
            }

            /* document variable for group by clause */
            BsonDocument? o_groupBy = null;

            foreach (Column o_column in o_selectQuery.GroupBy)
            {
                if (o_groupBy == null)
                {
                    o_groupBy = new BsonDocument(o_column.ColumnStr, "$" + o_column.ColumnStr);
                }
                else
                {
                    o_groupBy.Add(o_column.ColumnStr, "$" + o_column.ColumnStr);
                }
            }

            /* document variable for columns projection */
            BsonDocument? a_columns = TransposeColumns(o_selectQuery.Columns);

            List<Where> Where = [];

            /* handle having object */
            foreach (Where o_having in o_selectQuery.Having)
            {
                Where.Add(o_having);
            }

            /* handle where clauses */
            foreach (Where o_where in o_selectQuery.Where)
            {
                if (ForestNET.Lib.Helper.IsStringEmpty(o_where.FilterOperator))
                {
                    o_where.FilterOperator = "AND";
                }

                Where.Add(o_where);
            }

            /* document variable for all having and where clauses in select query  */
            BsonDocument? o_filterBsonDocument = null;

            if (Where.Count > 0)
            {
                o_filterBsonDocument = TransposeWhereClause(Where);
            }

            /* document variable for order by list */
            BsonDocument? o_orderBy = TransposeOrderBy(o_selectQuery.OrderBy);

            /* create group columns */
            BsonDocument o_groupColumns = new("_id", o_groupBy);

            /* add other aggregations to group columns */
            foreach (Column o_column in a_aggregations)
            {
                string s_column = o_column.ColumnStr;

                if (!ForestNET.Lib.Helper.IsStringEmpty(o_column.SqlAggregation))
                {
                    s_column = o_column.SqlAggregation + "_" + s_column;
                }
                else if (o_column.IsJoinTable)
                {
                    s_column = "join_" + o_column.Table + "." + s_column;
                }

                string? s_aggregateOperator = null;

                if (o_column.SqlAggregation.Equals("AVG"))
                {
                    s_aggregateOperator = "$avg";
                }
                else if (o_column.SqlAggregation.Equals("COUNT"))
                {
                    s_aggregateOperator = "$addToSet";
                }
                else if (o_column.SqlAggregation.Equals("MAX"))
                {
                    s_aggregateOperator = "$max";
                }
                else if (o_column.SqlAggregation.Equals("MIN"))
                {
                    s_aggregateOperator = "$min";
                }
                else if (o_column.SqlAggregation.Equals("SUM"))
                {
                    s_aggregateOperator = "$sum";
                }
                else
                {
                    throw new ArgumentException("Invalid aggregation operator: '" + o_column.SqlAggregation + "'");
                }

                o_groupColumns.Add(s_column, new BsonDocument(s_aggregateOperator ?? "", "$" + o_column.ColumnStr));
            }

            /* put all fields into Record for $replaceRoot later */
            o_groupColumns.Add("Record", new BsonDocument("$first", "$$ROOT"));

            /* aggregate columns for replace root aggregate */
            BsonDocument? o_replaceRootAggregateColumns = null;

            foreach (Column o_column in a_aggregations)
            {
                string s_column = o_column.ColumnStr;

                if (!ForestNET.Lib.Helper.IsStringEmpty(o_column.SqlAggregation))
                {
                    s_column = o_column.SqlAggregation + "_" + s_column;
                }
                else if (o_column.IsJoinTable)
                {
                    s_column = "join_" + o_column.Table + "." + s_column;
                }

                if (o_replaceRootAggregateColumns == null)
                {
                    o_replaceRootAggregateColumns = new BsonDocument(s_column, "$" + s_column);
                }
                else
                {
                    o_replaceRootAggregateColumns.Add(s_column, "$" + s_column);
                }
            }

            /* create replace root aggregate */
            BsonDocument o_replaceRoot = new("$replaceRoot", /* this is how we still hold all fields after $group, but we need to merge with aggregation fields of $group */
                new BsonDocument("newRoot",
                    new BsonDocument("$mergeObjects", new BsonArray() { "$Record", o_replaceRootAggregateColumns })
                )
            );

            /* create list for aggregate pipeline */
            List<BsonDocument> a_pipeline = [];

            /* add first sort */
            if (o_firstSort != null)
            {
                a_pipeline.Add(new BsonDocument("$sort", o_firstSort));
            }

            /* add group */
            a_pipeline.Add(new BsonDocument("$group", o_groupColumns));

            /* add replace root aggregate */
            a_pipeline.Add(o_replaceRoot);

            /* add projection */
            if (a_columns != null)
            {
                a_pipeline.Add(new BsonDocument("$project", a_columns));
            }

            /* add filter */
            if (o_filterBsonDocument != null)
            {
                a_pipeline.Add(new BsonDocument("$match", o_filterBsonDocument));
            }

            /* add sort */
            if (o_orderBy != null)
            {
                a_pipeline.Add(new BsonDocument("$sort", o_orderBy));
            }

            /* add limit clauses of query */
            if ((o_selectQuery.Limit != null) && (o_selectQuery.Limit.Interval > 0))
            {
                a_pipeline.Add(new BsonDocument("$skip", o_selectQuery.Limit.Start));
                a_pipeline.Add(new BsonDocument("$limit", o_selectQuery.Limit.Interval));
            }

            /* finish aggregate command */
            a_return.Add(
                new BsonDocument()
                    .Add("aggregate", s_collection)
                    .Add("pipeline", new BsonArray(a_pipeline))
                    .Add("cursor", new BsonDocument()) /* needed for aggregate */
            );

            return a_return;
        }

        private static List<BsonDocument> TransposeSelectQueryComplexJoinAndGroupBy(IQuery p_o_sqlQuery)
        {
            Select o_selectQuery = p_o_sqlQuery.GetQuery<Select>() ?? throw new NullReferenceException("Select query is null");
            string s_collection = p_o_sqlQuery.Table ?? throw new NullReferenceException("Table within select query is null");
            List<BsonDocument> a_return = [];

            if (o_selectQuery.Joins.Count != 1)
            {
                throw new ArgumentException("Nosqlmdb library can only handle one join, not '" + o_selectQuery.Joins.Count + "'");
            }

            /* get join information */
            Join o_join = o_selectQuery.Joins[0];

            if (o_join.Relations.Count != 1)
            {
                throw new ArgumentException("Nosqlmdb library can only handle one relation within a join, not '" + o_join.Relations.Count + "'");
            }

            /* get relation information */
            Relation o_relation = o_join.Relations[0];

            /* we will not handle operator, as we only can use equal, so we only use columnLeft and columnRight info */

            /* create */
            BsonDocument o_joinBsonDocument = new BsonDocument("from", o_join.Table)
                .Add("localField", o_relation.ColumnLeft?.ColumnStr)
                .Add("foreignField", o_relation.ColumnRight?.ColumnStr)
                .Add("as", "join_" + o_join.Table);

            /* document variable for first sort, if we use aggregation like MIN or MAX */
            BsonDocument? o_firstSort = null;

            List<Column> a_aggregations = [];

            foreach (Column o_column in o_selectQuery.Columns)
            {
                if (!ForestNET.Lib.Helper.IsStringEmpty(o_column.SqlAggregation))
                {
                    a_aggregations.Add(o_column);

                    if (o_column.SqlAggregation.Equals("MAX"))
                    {
                        if (o_firstSort == null)
                        {
                            o_firstSort = new BsonDocument(o_column.ColumnStr, -1); /* -1 -> DESC */
                        }
                        else
                        {
                            o_firstSort.Add(o_column.ColumnStr, -1); /* -1 -> DESC */
                        }
                    }
                    else if (o_column.SqlAggregation.Equals("MIN"))
                    {
                        if (o_firstSort == null)
                        {
                            o_firstSort = new BsonDocument(o_column.ColumnStr, 1); /* 1 -> ASC */
                        }
                        else
                        {
                            o_firstSort.Add(o_column.ColumnStr, 1); /* 1 -> ASC */
                        }
                    }
                }
            }

            /* document variable for group by clause */
            BsonDocument? o_groupBy = null;

            foreach (Column o_column in o_selectQuery.GroupBy)
            {
                if (o_groupBy == null)
                {
                    o_groupBy = new BsonDocument(o_column.ColumnStr, "$" + o_column.ColumnStr);
                }
                else
                {
                    o_groupBy.Add(o_column.ColumnStr, "$" + o_column.ColumnStr);
                }
            }

            if (o_selectQuery.Columns.Count > 0)
            {
                /* iterate all columns */
                for (int i = 0; i < o_selectQuery.Columns.Count; i++)
                {
                    /* if column table is equal to join table, set flag */
                    if ((o_selectQuery.Columns[i].Table ?? "").Equals(o_join.Table))
                    {
                        if (!ForestNET.Lib.Helper.IsStringEmpty(o_selectQuery.Columns[i].SqlAggregation))
                        {
                            throw new ArgumentException("Invalid column with aggregation on a join table at the same time. Only aggregation on main table allowed");
                        }

                        o_selectQuery.Columns[i].IsJoinTable = true;
                    }
                }
            }

            /* document variable for columns projection */
            BsonDocument? a_columns = TransposeColumns(o_selectQuery.Columns);

            List<Where> Where = [];

            /* handle having object */
            foreach (Where o_having in o_selectQuery.Having)
            {
                /* if having clause table is equal to join table, set flag */
                if ((o_having.Column?.Table ?? "").Equals(o_join.Table))
                {
                    o_having.IsJoinTable = true;
                }

                Where.Add(o_having);
            }

            /* handle where clauses */
            foreach (Where o_where in o_selectQuery.Where)
            {
                /* if where clause table is equal to join table, set flag */
                if ((o_where.Column?.Table ?? "").Equals(o_join.Table))
                {
                    o_where.IsJoinTable = true;
                }

                if (ForestNET.Lib.Helper.IsStringEmpty(o_where.FilterOperator))
                {
                    o_where.FilterOperator = "AND";
                }

                Where.Add(o_where);
            }

            /* document variable for all having and where clauses in select query  */
            BsonDocument? o_filterBsonDocument = null;

            if (Where.Count > 0)
            {
                o_filterBsonDocument = TransposeWhereClause(Where);
            }

            if (o_selectQuery.OrderBy?.Columns.Count > 0)
            {
                /* iterate all columns of order by clause */
                for (int i = 0; i < o_selectQuery.OrderBy.Columns.Count; i++)
                {
                    /* if column table is equal to join table, set flag */
                    if ((o_selectQuery.OrderBy.Columns[i].Table ?? "").Equals(o_join.Table))
                    {
                        if (!ForestNET.Lib.Helper.IsStringEmpty(o_selectQuery.Columns[i].SqlAggregation))
                        {
                            throw new ArgumentException("Invalid column with aggregation on a join table at the same time. Only aggregation on main table allowed");
                        }

                        o_selectQuery.OrderBy.Columns[i].IsJoinTable = true;
                    }
                }
            }

            /* document variable for order by list */
            BsonDocument? o_orderBy = TransposeOrderBy(o_selectQuery.OrderBy);

            /* create group columns */
            BsonDocument o_groupColumns = new("_id", o_groupBy);

            /* add other aggregations to group columns */
            foreach (Column o_column in a_aggregations)
            {
                string s_column = o_column.ColumnStr;

                if (!ForestNET.Lib.Helper.IsStringEmpty(o_column.SqlAggregation))
                {
                    s_column = o_column.SqlAggregation + "_" + s_column;
                }
                else if (o_column.IsJoinTable)
                {
                    s_column = "join_" + o_column.Table + "." + s_column;
                }

                string? s_aggregateOperator = null;

                if (o_column.SqlAggregation.Equals("AVG"))
                {
                    s_aggregateOperator = "$avg";
                }
                else if (o_column.SqlAggregation.Equals("COUNT"))
                {
                    s_aggregateOperator = "$addToSet";
                }
                else if (o_column.SqlAggregation.Equals("MAX"))
                {
                    s_aggregateOperator = "$max";
                }
                else if (o_column.SqlAggregation.Equals("MIN"))
                {
                    s_aggregateOperator = "$min";
                }
                else if (o_column.SqlAggregation.Equals("SUM"))
                {
                    s_aggregateOperator = "$sum";
                }
                else
                {
                    throw new ArgumentException("Invalid aggregation operator: '" + o_column.SqlAggregation + "'");
                }

                o_groupColumns.Add(s_column, new BsonDocument(s_aggregateOperator ?? "", "$" + o_column.ColumnStr));
            }

            /* put all fields into Record for $replaceRoot later */
            o_groupColumns.Add("Record", new BsonDocument("$first", "$$ROOT"));

            /* aggregate columns for replace root aggregate */
            BsonDocument? o_replaceRootAggregateColumns = null;

            foreach (Column o_column in a_aggregations)
            {
                string s_column = o_column.ColumnStr;

                if (!ForestNET.Lib.Helper.IsStringEmpty(o_column.SqlAggregation))
                {
                    s_column = o_column.SqlAggregation + "_" + s_column;
                }
                else if (o_column.IsJoinTable)
                {
                    s_column = "join_" + o_column.Table + "." + s_column;
                }

                if (o_replaceRootAggregateColumns == null)
                {
                    o_replaceRootAggregateColumns = new BsonDocument(s_column, "$" + s_column);
                }
                else
                {
                    o_replaceRootAggregateColumns.Add(s_column, "$" + s_column);
                }
            }

            /* create replace root aggregate */
            BsonDocument o_replaceRoot = new("$replaceRoot", /* this is how we still hold all fields after $group, but we need to merge with aggregation fields of $group */
                new BsonDocument("newRoot",
                    new BsonDocument("$mergeObjects", new BsonArray() { "$Record", o_replaceRootAggregateColumns })
                )
            );

            /* create list for aggregate pipeline */
            List<BsonDocument> a_pipeline = [];

            /* add first sort */
            if (o_firstSort != null)
            {
                a_pipeline.Add(new BsonDocument("$sort", o_firstSort));
            }

            /* add lookup */
            a_pipeline.Add(new BsonDocument("$lookup", o_joinBsonDocument));

            /* add unwind */
            a_pipeline.Add(new BsonDocument("$unwind", "$join_" + o_join.Table));

            /* add group */
            a_pipeline.Add(new BsonDocument("$group", o_groupColumns));

            /* add replace root aggregate */
            a_pipeline.Add(o_replaceRoot);

            /* add projection */
            if (a_columns != null)
            {
                a_pipeline.Add(new BsonDocument("$project", a_columns));
            }

            /* add filter */
            if (o_filterBsonDocument != null)
            {
                a_pipeline.Add(new BsonDocument("$match", o_filterBsonDocument));
            }

            /* add sort */
            if (o_orderBy != null)
            {
                a_pipeline.Add(new BsonDocument("$sort", o_orderBy));
            }

            /* add limit clauses of query */
            if ((o_selectQuery.Limit != null) && (o_selectQuery.Limit.Interval > 0))
            {
                a_pipeline.Add(new BsonDocument("$skip", o_selectQuery.Limit.Start));
                a_pipeline.Add(new BsonDocument("$limit", o_selectQuery.Limit.Interval));
            }

            /* finish aggregate command */
            a_return.Add(
                new BsonDocument()
                    .Add("aggregate", s_collection)
                    .Add("pipeline", new BsonArray(a_pipeline))
                    .Add("cursor", new BsonDocument()) /* needed for aggregate */
            );

            return a_return;
        }

        private static Object? TransposeValueFromQuery(KeyValuePair<string, Object> p_o_entry)
        {
            if (p_o_entry.Key.Equals("object"))
            {
                return p_o_entry.Value;
            }
            else if (p_o_entry.Key.Equals("string"))
            {
                return Convert.ToString(p_o_entry.Value);
            }
            else if (p_o_entry.Key.Equals("bool"))
            {
                return Convert.ToBoolean(p_o_entry.Value);
            }
            else if (p_o_entry.Key.Equals("sbyte"))
            {
                return Convert.ToSByte(p_o_entry.Value);
            }
            else if (p_o_entry.Key.Equals("short"))
            {
                return Convert.ToInt16(p_o_entry.Value);
            }
            else if (p_o_entry.Key.Equals("integer"))
            {
                return Convert.ToInt32(p_o_entry.Value);
            }
            else if (p_o_entry.Key.Equals("long"))
            {
                return Convert.ToInt64(p_o_entry.Value);
            }
            else if (p_o_entry.Key.Equals("byte"))
            {
                return Convert.ToByte(p_o_entry.Value);
            }
            else if (p_o_entry.Key.Equals("ushort"))
            {
                return Convert.ToUInt16(p_o_entry.Value);
            }
            else if (p_o_entry.Key.Equals("uinteger"))
            {
                return Convert.ToUInt32(p_o_entry.Value);
            }
            else if (p_o_entry.Key.Equals("ulong"))
            {
                return Convert.ToUInt64(p_o_entry.Value);
            }
            else if (p_o_entry.Key.Equals("double"))
            {
                return Convert.ToDouble(p_o_entry.Value);
            }
            else if (p_o_entry.Key.Equals("float"))
            {
                return Convert.ToSingle(p_o_entry.Value);
            }
            else if (p_o_entry.Key.Equals("decimal"))
            {
                return Convert.ToDecimal(p_o_entry.Value);
            }
            else if (p_o_entry.Key.Equals("localdatetime"))
            {
                return Convert.ToDateTime(p_o_entry.Value);
            }
            else if (p_o_entry.Key.Equals("localdate"))
            {
                return Convert.ToDateTime(p_o_entry.Value);
            }
            else if (p_o_entry.Key.Equals("localtime"))
            {
                /* handle TimeSpan as string */
                return ((TimeSpan)p_o_entry.Value).ToString("hh\\:mm\\:ss");
            }
            else
            {
                throw new ArgumentException("Illegal type '" + p_o_entry.Key + "'");
            }
        }

        private static string? TransposeOperatorFromQuery(string p_s_operator)
        {
            string? s_nosqlMDBOperator = null;

            switch (p_s_operator)
            {
                case "=":
                case "IS":
                    s_nosqlMDBOperator = "$eq";
                    break;
                case "<>":
                case "IS NOT":
                    s_nosqlMDBOperator = "$ne";
                    break;
                case "<":
                    s_nosqlMDBOperator = "$lt";
                    break;
                case "<=":
                    s_nosqlMDBOperator = "$lte";
                    break;
                case ">":
                    s_nosqlMDBOperator = "$gt";
                    break;
                case ">=":
                    s_nosqlMDBOperator = "$gte";
                    break;
                case "LIKE":
                    // no new bson.BsonDocument, but change value to "/.*" + value + ".*/"
                    break;
                case "NOT LIKE":
                    s_nosqlMDBOperator = "$not";
                    break;
                case "IN":
                    s_nosqlMDBOperator = "$in";
                    break;
                case "NOT IN":
                    s_nosqlMDBOperator = "$nin";
                    break;
                default:
                    s_nosqlMDBOperator = "$eq";
                    break;
            }

            return s_nosqlMDBOperator;
        }

        private static BsonDocument? TransposeColumns(List<Column> p_a_columns)
        {
            /* document variable for columns projection */
            BsonDocument? a_columns = null;

            /* retrieve columns from sql query */
            foreach (Column o_column in p_a_columns)
            {
                /* ignore "*" column */
                if (!o_column.ColumnStr.Equals("*"))
                {
                    if (a_columns == null)
                    {
                        if (!ForestNET.Lib.Helper.IsStringEmpty(o_column.Name))
                        {
                            if (!ForestNET.Lib.Helper.IsStringEmpty(o_column.SqlAggregation))
                            {
                                if (o_column.SqlAggregation.Equals("COUNT"))
                                {
                                    a_columns = new BsonDocument(o_column.Name, new BsonDocument("$size", "$" + o_column.SqlAggregation + "_" + o_column.ColumnStr));
                                }
                                else
                                {
                                    a_columns = new BsonDocument(o_column.Name, "$" + o_column.SqlAggregation + "_" + o_column.ColumnStr);
                                }
                            }
                            else
                            {
                                a_columns = new BsonDocument(o_column.Name, "$" + o_column.ColumnStr);
                            }
                        }
                        else
                        {
                            if (!ForestNET.Lib.Helper.IsStringEmpty(o_column.SqlAggregation))
                            {
                                if (o_column.SqlAggregation.Equals("COUNT"))
                                {
                                    a_columns = new BsonDocument(o_column.SqlAggregation + "_" + o_column.ColumnStr, new BsonDocument("$size", "$" + o_column.SqlAggregation + "_" + o_column.ColumnStr));
                                }
                                else
                                {
                                    a_columns = new BsonDocument(o_column.SqlAggregation + "_" + o_column.ColumnStr, 1);
                                }
                            }
                            else if (o_column.IsJoinTable)
                            {
                                a_columns = new BsonDocument(o_column.ColumnStr, "$join_" + o_column.Table + "." + o_column.ColumnStr);
                            }
                            else
                            {
                                a_columns = new BsonDocument(o_column.ColumnStr, 1);
                            }
                        }
                    }
                    else
                    {
                        if (!ForestNET.Lib.Helper.IsStringEmpty(o_column.Name))
                        {
                            if (!ForestNET.Lib.Helper.IsStringEmpty(o_column.SqlAggregation))
                            {
                                if (o_column.SqlAggregation.Equals("COUNT"))
                                {
                                    a_columns.Add(o_column.Name, new BsonDocument("$size", "$" + o_column.SqlAggregation + "_" + o_column.ColumnStr));
                                }
                                else
                                {
                                    a_columns.Add(o_column.Name, "$" + o_column.SqlAggregation + "_" + o_column.ColumnStr);
                                }
                            }
                            else
                            {
                                a_columns.Add(o_column.Name, "$" + o_column.ColumnStr);
                            }
                        }
                        else
                        {
                            if (!ForestNET.Lib.Helper.IsStringEmpty(o_column.SqlAggregation))
                            {
                                if (o_column.SqlAggregation.Equals("COUNT"))
                                {
                                    a_columns.Add(o_column.SqlAggregation + "_" + o_column.ColumnStr, new BsonDocument("$size", "$" + o_column.SqlAggregation + "_" + o_column.ColumnStr));
                                }
                                else
                                {
                                    a_columns.Add(o_column.SqlAggregation + "_" + o_column.ColumnStr, 1);
                                }
                            }
                            else if (o_column.IsJoinTable)
                            {
                                a_columns.Add("join_" + o_column.Table + "." + o_column.ColumnStr, 1);
                            }
                            else
                            {
                                a_columns.Add(o_column.ColumnStr, 1);
                            }
                        }
                    }
                }
            }

            a_columns?.Add("_id", 0);

            return a_columns;
        }

        private static BsonDocument? TransposeOrderBy(OrderBy? p_o_orderBy)
        {
            /* document variable for order by list */
            BsonDocument? o_orderBy = null;

            /* add order by clause to query */
            if ((p_o_orderBy != null) && (p_o_orderBy.Amount > 0))
            {
                int i = -1;

                /* add each column with direction ASC or DESC */
                foreach (Column o_column in p_o_orderBy.Columns)
                {
                    string s_column = o_column.ColumnStr;

                    if (!ForestNET.Lib.Helper.IsStringEmpty(o_column.SqlAggregation))
                    {
                        s_column = o_column.SqlAggregation + "_" + o_column.ColumnStr;
                    }
                    else if (o_column.IsJoinTable)
                    {
                        s_column = "join_" + o_column.Table + "." + o_column.ColumnStr;
                    }

                    if (p_o_orderBy.Directions[++i])
                    {
                        if (o_orderBy == null)
                        {
                            o_orderBy = new BsonDocument(s_column, 1); /* 1 -> ASC */
                        }
                        else
                        {
                            o_orderBy.Add(s_column, 1); /* 1 -> ASC */
                        }
                    }
                    else
                    {
                        if (o_orderBy == null)
                        {
                            o_orderBy = new BsonDocument(s_column, -1); /* -1 -> DESC */
                        }
                        else
                        {
                            o_orderBy.Add(s_column, -1); /* -1 -> DESC */
                        }
                    }
                }
            }

            return o_orderBy;
        }

        private static BsonDocument? TransposeWhereClause(List<Where> p_WhereClauses)
        {
            return TransposeWhereClause(p_WhereClauses, 0, p_WhereClauses.Count - 1, null);
        }

        private static BsonDocument? TransposeWhereClause(List<Where> p_WhereClauses, int p_i_min, int p_i_max, string? p_s_lastFilterOperator)
        {
            /* check if parameter of where clauses is valid */
            if ((p_WhereClauses == null) || (p_WhereClauses.Count < 1))
            {
                throw new ArgumentException("List of where clauses is null or has no elements.");
            }

            /* return value */
            BsonDocument? o_return = null;

            if (p_i_min == p_i_max)
            { /* check if we only have one where clause */
                KeyValuePair<string, Object> o_valueEntry = WhereClauseGetValue(p_WhereClauses[p_i_min]);
                o_return = WhereClauseToBSONBsonDocument(p_WhereClauses[p_i_min], o_return, o_valueEntry);
            }
            else
            { /* handle multiple where clauses */
                /* check all brackets of where clauses */
                WhereClauseCheckBrackets(p_WhereClauses);

                KeyValuePair<string, Object> o_valueEntry = WhereClauseGetValue(p_WhereClauses[p_i_min]);

                if (p_s_lastFilterOperator == null)
                { /* first element */
                    string s_nextFilterOperator = p_WhereClauses[p_i_min + 1].FilterOperator;

                    /* XOR is not supported for nosqlmdb transpose library */
                    if (s_nextFilterOperator.ToUpper().Equals("XOR"))
                    {
                        throw new ArgumentException("XOR filter operator is not supported for nosqlmdb transpose library");
                    }

                    BsonDocument o_current = WhereClauseToBSONBsonDocument(p_WhereClauses[p_i_min], null, o_valueEntry);

                    /* find other where clauses with current clause with new filter operator */
                    BsonDocument? o_next = TransposeWhereClause(p_WhereClauses, p_i_min + 1, p_i_max, s_nextFilterOperator);

                    if (o_next != null)
                    {
                        /* add new filter operator with sub document list */
                        o_return = o_current.Add(((s_nextFilterOperator.ToUpper().Equals("AND")) ? "$and" : "$or"), new BsonArray(new List<BsonDocument>() { o_next }));
                    }
                    else
                    {
                        /* set current where clause as return object */
                        o_return = o_current;
                    }
                }
                else
                {
                    string s_nextFilterOperator = p_WhereClauses[p_i_min + 1].FilterOperator;

                    /* XOR is not supported for nosqlmdb transpose library */
                    if (s_nextFilterOperator.ToUpper().Equals("XOR"))
                    {
                        throw new ArgumentException("XOR filter operator is not supported for nosqlmdb transpose library");
                    }

                    o_return = WhereClauseToBSONBsonDocument(p_WhereClauses[p_i_min], o_return, o_valueEntry);

                    /* find other where clauses with current clause with new filter operator */
                    BsonDocument? o_next = TransposeWhereClause(p_WhereClauses, p_i_min + 1, p_i_max, s_nextFilterOperator);

                    if (o_next != null)
                    {
                        foreach (BsonElement o_entry in o_next.Elements)
                        {
                            o_return.Add(o_entry.Name, o_entry.Value);
                        }
                    }

                    /* filter operator will change if there is no bracket end */
                    if (!p_s_lastFilterOperator.ToUpper().Equals(s_nextFilterOperator.ToUpper()))
                    {
                        if ((!p_WhereClauses[p_i_min].BracketStart) && (p_WhereClauses[p_i_min].BracketEnd))
                        {
                            BsonDocument? o_before = null;
                            BsonDocument? o_after = null;
                            bool b_one = false;

                            foreach (BsonElement o_entry in o_return.Elements)
                            {
                                if (!b_one)
                                {
                                    o_before = new BsonDocument(o_entry.Name, o_entry.Value);
                                }
                                else
                                {
                                    if (o_after == null)
                                    {
                                        o_after = new BsonDocument(o_entry.Name, o_entry.Value);
                                    }
                                    else
                                    {
                                        o_after.Add(o_entry.Name, o_entry.Value);
                                    }
                                }

                                b_one = true;
                            }

                            o_return = o_before;
                            o_return?.Add(((s_nextFilterOperator.ToUpper().Equals("AND")) ? "$and" : "$or"), new BsonArray(new List<BsonDocument>() { o_after ?? [] }));
                        }
                        else
                        {
                            o_return = new BsonDocument(((s_nextFilterOperator.ToUpper().Equals("AND")) ? "$and" : "$or"), new BsonArray(new List<BsonDocument>() { o_return }));
                        }
                    }
                }
            }

            return o_return;
        }

        private static BsonDocument WhereClauseToBSONBsonDocument(Where p_o_where, BsonDocument? p_o_document, KeyValuePair<string, Object> p_o_valueEntry)
        {
            string s_column = p_o_where.Column?.ColumnStr ?? "";

            if (!ForestNET.Lib.Helper.IsStringEmpty(p_o_where.Column?.SqlAggregation))
            {
                s_column = p_o_where.Column?.SqlAggregation + "_" + s_column;
            }
            else if (p_o_where.IsJoinTable)
            {
                s_column = "join_" + p_o_where.Column?.Table + "." + s_column;
            }

            if (p_o_document == null)
            { /* must create bson document object */
                /* handle LIKE and NOT LIKE operator */
                if (p_o_where.Operator.ToUpper().Equals("LIKE"))
                {
                    p_o_document = new BsonDocument(s_column, new BsonDocument("$regex", ".*" + (TransposeValueFromQuery(p_o_valueEntry) ?? BsonNull.Value).ToString()?.Replace("%", "") + ".*"));
                }
                else if (p_o_where.Operator.ToUpper().Equals("NOT LIKE"))
                {
                    p_o_document = new BsonDocument(s_column, new BsonDocument(TransposeOperatorFromQuery(p_o_where.Operator) ?? "operator from query is null", new BsonDocument("$regex", ".*" + (TransposeValueFromQuery(p_o_valueEntry) ?? BsonNull.Value).ToString()?.Replace("%", "") + ".*")));
                }
                else
                {
                    p_o_document = new BsonDocument(s_column, new BsonDocument(new Dictionary<string, Object?>() { { TransposeOperatorFromQuery(p_o_where.Operator) ?? "operator from query is null", TransposeValueFromQuery(p_o_valueEntry) ?? BsonNull.Value } }));
                }
            }
            else
            { /* append to parameter bson document object */
                /* handle LIKE and NOT LIKE operator */
                if (p_o_where.Operator.ToUpper().Equals("LIKE"))
                {
                    p_o_document.Add(s_column, new BsonDocument("$regex", ".*" + (TransposeValueFromQuery(p_o_valueEntry) ?? BsonNull.Value).ToString()?.Replace("%", "") + ".*"));
                }
                else if (p_o_where.Operator.ToUpper().Equals("NOT LIKE"))
                {
                    p_o_document.Add(s_column, new BsonDocument(TransposeOperatorFromQuery(p_o_where.Operator) ?? "operator from query is null", new BsonDocument("$regex", ".*" + (TransposeValueFromQuery(p_o_valueEntry) ?? BsonNull.Value).ToString()?.Replace("%", "") + ".*")));
                }
                else
                {
                    p_o_document.Add(s_column, new BsonDocument(new Dictionary<string, Object?>() { { TransposeOperatorFromQuery(p_o_where.Operator) ?? "operator from query is null", TransposeValueFromQuery(p_o_valueEntry) ?? BsonNull.Value } }));
                }
            }

            return p_o_document;
        }

        private static KeyValuePair<string, Object> WhereClauseGetValue(Where p_o_whereClause)
        {
            /* store value we can retrieve from where clause */
            List<KeyValuePair<string, Object>> a_values = [];

            _ = Query<Column>.ConvertToPreparedStatementQuery(BaseGateway.NOSQLMDB, p_o_whereClause.ToString(), a_values, false);

            if (a_values.Count != 1)
            {
                throw new ArgumentException("Could not retrieve value from where clause");
            }

            LogValuesFromQuery(a_values);

            return a_values[0];
        }

        private static void WhereClauseCheckBrackets(List<Where> p_WhereClauses)
        {
            /* iterate reverse */
            for (int i = 0; i < p_WhereClauses.Count; i++)
            {
                /* check brackets, starting with bracket start */
                if ((p_WhereClauses[i].BracketStart) && (!p_WhereClauses[i].BracketEnd))
                {
                    bool b_bracketClosed = false;
                    string? s_filterOperator = null;

                    /* within brackets all filter operator must be the same */
                    for (int j = i; j < p_WhereClauses.Count; j++)
                    {
                        if ((i != j) && (p_WhereClauses[j].BracketStart) && (!p_WhereClauses[j].BracketEnd))
                        {
                            throw new ArgumentException("Bracket nested within each other in where clause list are not supported for nosqlmdb transpose library");
                        }

                        /* if we have a filter operator and no bracket start */
                        if ((!ForestNET.Lib.Helper.IsStringEmpty(p_WhereClauses[j].FilterOperator)) && (!((p_WhereClauses[j].BracketStart) && (!p_WhereClauses[j].BracketEnd))))
                        {
                            if (s_filterOperator == null)
                            { /* save filter operator for comparison for all clauses within brackets */
                                s_filterOperator = p_WhereClauses[j].FilterOperator.ToUpper();
                            }
                            else if (!s_filterOperator.Equals(p_WhereClauses[j].FilterOperator.ToUpper()))
                            { /* filter operator differs */
                                throw new ArgumentException("All filter operators must be the same within where clause brackets");
                            }
                        }

                        /* bracket end found */
                        if ((!p_WhereClauses[j].BracketStart) && (p_WhereClauses[j].BracketEnd))
                        {
                            b_bracketClosed = true;
                            break;
                        }
                    }

                    /* check if bracket was closed correctly */
                    if (!b_bracketClosed)
                    {
                        throw new ArgumentException("A bracket in where clause was never closed or opened");
                    }
                }
            }
        }

        private static void LogValuesFromQuery(List<KeyValuePair<string, Object>> p_a_values)
        {
            string s_empty = "                              ";

            foreach (KeyValuePair<string, Object> o_entry in p_a_values)
            {
                if (ForestNET.Lib.Global.IsILevel((byte)ForestNET.Lib.Log.Level.MASS)) ForestNET.Lib.Global.ILogMass(
                    "\t" +
                    o_entry.Key + /* key value */
                    s_empty.Substring(0, s_empty.Length - o_entry.Key.Length) + /* white spaces */
                    o_entry.Value.GetType().FullName + /* type value */
                    s_empty.Substring(0, s_empty.Length - o_entry.Value.GetType().FullName?.Length ?? 0) + /* white spaces */
                    (ForestNET.Lib.Global.Instance.LogCompleteSqlQuery ? o_entry.Value.ToString() : "") /* value */
                );
            }
        }
    }
}