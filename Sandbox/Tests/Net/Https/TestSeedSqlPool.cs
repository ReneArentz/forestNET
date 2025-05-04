namespace Sandbox.Tests.Net.Https
{
    public class TestSeedSqlPool : ForestNET.Lib.Net.Https.Dynm.ForestSeed
    {
        public override void PrepareContent()
        {
            /* check if Seed instance is availalbe */
            if (this.Seed == null)
            {
                throw new InvalidDataException("Seed instance is not available");
            }

            /* use BasePool as other base source */
            PersonRecord o_record = new()
            {
                OtherBaseSource = this.FetchQueryWithBasePool
            };

            /* get all person records */
            List<PersonRecord> a_rows = o_record.GetRecords();
            /* our record list for dynamic page */
            List<Object> a_list = [];

            /* iterate each row */
            if (a_rows != null)
            {
                foreach (PersonRecord o_row in a_rows)
                {
                    /* transpose record to map */
                    Dictionary<string, Object?> m_record = new()
                    {
                        /* get all columns */
                        { "Id", o_row.ColumnId },
                        { "PersonalIdentificationNumber", o_row.ColumnPersonalIdentificationNumber },
                        { "Name", o_row.ColumnName },
                        { "Age", o_row.ColumnAge },
                        { "City", o_row.ColumnCity },
                        { "Country", o_row.ColumnCountry }
                    };

                    /* add map to list */
                    a_list.Add(m_record);
                }
            }
            else
            {
                ForestNET.Lib.Global.ILogWarning("Could not execute query and retrieve a result");
            }

            /* add list to seed temp */
            this.Seed.Temp.Add("records", a_list);
        }

        private List<Dictionary<string, Object?>> FetchQueryWithBasePool(ForestNET.Lib.SQL.IQuery? p_o_query)
        {
            ForestNET.Lib.Net.SQL.Pool.HttpsConfig? o_config = this.Seed?.Config as ForestNET.Lib.Net.SQL.Pool.HttpsConfig ?? throw new InvalidDataException("Config instance is not available");
            return o_config.BasePool?.FetchQuery(p_o_query) ?? [];
        }
    }
}
