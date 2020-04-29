using CoreUtilitiesPack;
using System.Collections.Generic;
using System.Linq;

namespace RemoteAccessUtility
{
    public static class DbAccessor
    {
        private static SqliteAccessor db = new SqliteAccessor();

        public static void InitializeSetting()
        {
            if (db.HasTable("setting"))
                return;

            var record = new Setting
            {
                RdpOption = new RdpOption(),
            };
            var sql = SqliteAccessor.GetCreateTableSQL("setting", record);
            db.ExecuteNonQuery(sql);
            db.ToDictionary(record, out var dic);
            db.Upsert("setting", dic);
        }

        public static void InitializeAccount()
        {
            if (db.HasTable("account"))
                return;

            var record = new Account
            {
                Name = @"administrator",
                Password = @"password",
            };
            var sql = SqliteAccessor.GetCreateTableSQL("account", record);
            db.ExecuteNonQuery(sql);
            db.ToDictionary(record, out var dic);
            db.Upsert("account", dic);
        }

        public static void InitializeEnvironment()
        {
            if (db.HasTable("environment"))
                return;

            var accounts = (IEnumerable<Account>)SelectAccounts();
            var record = new Environment
            {
                HostName = @"localhost",
                ConnectionAddress = @"127.0.0.1",
                OsType = OperatingSystemType.Windows,
                AccountGuid = accounts.First().Guid,
            };
            var sql = SqliteAccessor.GetCreateTableSQL("environment", record);
            db.ExecuteNonQuery(sql);
            db.ToDictionary(record, out var dic);
            db.Upsert("environment", dic);
        }

        public static List<Account> SelectAccounts()
        {
            var sql = "select * from account";
            var records = new List<Account>();
            db.ToRecords(db.ExecuteQuery(sql), out records);
            return records;
        }

        public static List<Environment> SelectEnvironments()
        {
            var sql = "select * from environment";
            var records = new List<Environment>();
            db.ToRecords(db.ExecuteQuery(sql), out records);
            return records;
        }

        public static Setting SelectSetting()
        {
            var sql = "select * from setting";
            var records = new List<Setting>();
            db.ToRecords(db.ExecuteQuery(sql), out records);
            return records.First();
        }

        public static void UpdatetAccounts(List<Account> accounts)
        {
            var target = SelectAccounts();
            foreach (var account in accounts)
            {
                if (target.Contains(account))
                {
                    target.Remove(account);
                }
            }
            db.ToDictionaries(target, out var deletes);
            db.Delete("account", deletes);

            db.ToDictionaries(accounts, out var upserts);
            db.Upserts("account", upserts);
        }

        public static void UpsertEnvironment(Environment environment)
        {
            db.ToDictionary(environment, out var upsert);
            db.Upsert("environment", upsert);
        }

        public static void DeleteEnvironment(Environment environment)
        {
            var environments = new List<Environment>()
            {
                environment,
            };
            db.ToDictionaries(environments, out var deletes);
            db.Delete("environment", deletes);
        }

        public static Environment Copy(Environment env)
        {
            db.ToDictionary(env, out var item);
            db.ToRecord(item, out Environment result);
            return result;
        }
    }
}