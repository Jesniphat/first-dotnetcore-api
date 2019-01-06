using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;

namespace FirstDotNetCore.Models
{
    public class UserQueries
    {
        public readonly AppDb Db;

        public UserQueries(AppDb db)
        {
            Db = db;
        }

        public async Task<Users> FindOneAsync(int id)
        {
            var cmd = Db.Connection.CreateCommand() as MySqlCommand;
            cmd.CommandText = @"SELECT id, username, email, password, name, lastname, DATE_FORMAT(created, '%d-%m-%Y %H:%i:%s') AS created, DATE_FORMAT(updated, '%d-%m-%Y %H:%i:%s') AS updated FROM `users` WHERE `Id` = @id";
            cmd.Parameters.Add(new MySqlParameter
            {
                ParameterName = "@id",
                DbType = DbType.Int32,
                Value = id,
            });
            var result = await ReadAllAsync(await cmd.ExecuteReaderAsync());
            return result.Count > 0 ? result[0] : null;
        }

        public async Task<List<Users>> LatestPostsAsync()
        {
            var cmd = Db.Connection.CreateCommand();
            cmd.CommandText = @"SELECT id, username, email, password, name, lastname, DATE_FORMAT(created, '%d-%m-%Y %H:%i:%s') AS created, DATE_FORMAT(updated, '%d-%m-%Y %H:%i:%s') AS updated FROM `users` ORDER BY `Id` DESC LIMIT 10;";
            return await ReadAllAsync(await cmd.ExecuteReaderAsync());
        }


         public async Task DeleteAllAsync()
         {
            var txn = await Db.Connection.BeginTransactionAsync();
            try
            {
                var cmd = Db.Connection.CreateCommand();
                cmd.CommandText = @"DELETE FROM `users`";
                await cmd.ExecuteNonQueryAsync();
                txn.Commit();
            }
            catch
            {
                txn.Rollback();
                throw;
            }
         }

        private async Task<List<Users>> ReadAllAsync(DbDataReader reader)
        {
            var posts = new List<Users>();
            using (reader)
            {
                while (await reader.ReadAsync())
                {
                    var post = new Users(Db)
                    {
                        Id = await reader.GetFieldValueAsync<int>(0),
                        Username = await reader.GetFieldValueAsync<string>(1),
                        Email = await reader.GetFieldValueAsync<string>(2),
                        Password = await reader.GetFieldValueAsync<string>(3),
                        Name = await reader.GetFieldValueAsync<string>(4),
                        Lastname = await reader.GetFieldValueAsync<string>(5),
                        Created = await reader.GetFieldValueAsync<string>(6),
                        Updated = await reader.GetFieldValueAsync<string>(7),
                    };
                    posts.Add(post);
                }
            }
            return posts;
        }
    }
}
