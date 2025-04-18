using ForestNET.Lib.SQL;
using ForestNET.Lib.SQL.MariaDB;
using ForestNET.Lib.SQL.MSSQL;
using ForestNET.Lib.SQL.NOSQLMDB;
using ForestNET.Lib.SQL.Oracle;
using ForestNET.Lib.SQL.PGSQL;
using ForestNET.Lib.SQL.SQLite;

namespace ForestNET.Tests.SQL
{
    public class BaseCredentials
    {
        //private readonly string s_ip = "172.28.234.162";
        private readonly string s_ip = "172.28.226.144";
        public BaseGateway e_baseGatewayBaseThread;
        public string s_hostBaseThread = string.Empty;
        public string s_datasourceBaseThread = string.Empty;
        public string s_userBaseThread = string.Empty;
        public string s_passwordBaseThread = string.Empty;

        /* sqlite is the only unit test we can try on any platform, please feel free to use other database gateways or test it in a separate sandbox */
        public static Dictionary<string, int> BaseGateways
        {
            get
            {
                return new()
                {
                    //[BaseGateway.MARIADB.ToString()] = 0,
                    [BaseGateway.SQLITE.ToString()] = 1,
                    //[BaseGateway.MSSQL.ToString()] = 2,
                    //[BaseGateway.PGSQL.ToString()] = 3,
                    //[BaseGateway.ORACLE.ToString()] = 4,
                    //[BaseGateway.NOSQLMDB.ToString()] = 5
                };
            }
        }

        public BaseCredentials(string p_s_testDirectory)
        {
            ForestNET.Lib.Global o_glob = ForestNET.Lib.Global.Instance;

            this.e_baseGatewayBaseThread = o_glob.BaseGateway;

            if (o_glob.BaseGateway == BaseGateway.MARIADB)
            {
                this.s_hostBaseThread = s_ip + ":3306";
                this.s_datasourceBaseThread = "test";
                this.s_userBaseThread = "root";
                this.s_passwordBaseThread = "root";
                o_glob.Base = new BaseMariaDB(this.s_hostBaseThread, this.s_datasourceBaseThread, this.s_userBaseThread, this.s_passwordBaseThread);
            }
            else if (o_glob.BaseGateway == BaseGateway.SQLITE)
            {
                this.s_hostBaseThread = p_s_testDirectory + "testBase.db";
                o_glob.Base = new BaseSQLite(this.s_hostBaseThread);
            }
            else if (o_glob.BaseGateway == BaseGateway.PGSQL)
            {
                this.s_hostBaseThread = s_ip + ":5432";
                this.s_datasourceBaseThread = "test";
                this.s_userBaseThread = "postgres";
                this.s_passwordBaseThread = "root";
                o_glob.Base = new BasePGSQL(this.s_hostBaseThread, this.s_datasourceBaseThread, this.s_userBaseThread, this.s_passwordBaseThread);
            }
            else if (o_glob.BaseGateway == BaseGateway.MSSQL)
            {
                //this.s_hostBaseThread = s_ip + ":1433\\SQLEXPRESS|TrustServerCertificate|EncryptStrict";
                this.s_hostBaseThread = s_ip + ":1433|TrustServerCertificate|EncryptNo";
                this.s_datasourceBaseThread = "test";
                this.s_userBaseThread = "sa";
                this.s_passwordBaseThread = "sa";
                o_glob.Base = new BaseMSSQL(this.s_hostBaseThread, this.s_datasourceBaseThread, this.s_userBaseThread, this.s_passwordBaseThread);
            }
            else if (o_glob.BaseGateway == BaseGateway.ORACLE)
            {
                this.s_hostBaseThread = s_ip + ":1521";
                this.s_datasourceBaseThread = ":xe";
                //this.s_datasourceBaseThread = ":free";
                this.s_userBaseThread = "system";
                this.s_passwordBaseThread = "root";
                o_glob.Base = new BaseOracle(this.s_hostBaseThread, this.s_datasourceBaseThread, this.s_userBaseThread, this.s_passwordBaseThread);
            }
            else if (o_glob.BaseGateway == BaseGateway.NOSQLMDB)
            {
                this.s_hostBaseThread = s_ip + ":27017|DisableAutoCommit";
                this.s_datasourceBaseThread = "test";
                o_glob.Base = new BaseNOSQLMDB(this.s_hostBaseThread, this.s_datasourceBaseThread);
            }
        }
    }
}
