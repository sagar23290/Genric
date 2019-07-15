using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;

namespace DapperUnitOfWork.Repositories
{
    internal abstract class RepositoryBase<T> where T : class
    {
        protected IDbTransaction Transaction { get; private set; }
        protected IDbConnection Connection { get { return Transaction.Connection; } }

        public RepositoryBase(IDbTransaction transaction)
        {
            Transaction = transaction;
        }

        public IEnumerable<T> GetList(string query,DynamicParameters param = null,CommandType type = CommandType.Text) {

            return  Connection.QueryAsync<T>(query, param, transaction: Transaction,commandType:type).Result.AsList<T>();
        }

        public T FirstOrDefault(string query, DynamicParameters param = null, CommandType type = CommandType.Text) {
            return  Connection.QueryFirstOrDefaultAsync<T>(query, param, Transaction , commandType : CommandType.Text).Result;
        }

        public T SingleOrDefault(string query, DynamicParameters param = null, CommandType type = CommandType.Text) {
            return  Connection.QueryFirstOrDefaultAsync<T>(query, param, Transaction, commandType: CommandType.Text).Result;
        }

        public int spInsert(string spName, DynamicParameters param,string OutParam) {
              Connection.ExecuteAsync(spName, param,Transaction,commandType:CommandType.StoredProcedure);
            return param.Get<int>(OutParam);
        }

        public bool spUpdate(string spName, DynamicParameters param)
        {
           int rowAffected =   Connection.ExecuteAsync(spName, param, Transaction, commandType: CommandType.StoredProcedure).Result;
            return rowAffected > 0;
        }




    }
}
