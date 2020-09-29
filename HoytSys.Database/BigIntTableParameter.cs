using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using Dapper;
using Microsoft.SqlServer.Server;

namespace A19.Database
{
    public class BigIntTableParameter : SqlMapper.IDynamicParameters
    {

        private string _paramName;
        private readonly IEnumerable<long> _values;
        private readonly List<(string, object)> _additionalParameters = new List<(string, object)>(1);

        public BigIntTableParameter(
            string paramName,
            IEnumerable<long> values)
        {
            _paramName = paramName;
            _values = values;
        }
        
        public void AddParameters(IDbCommand command, SqlMapper.Identity identity)
        {
            var sqlCommand = (SqlCommand) command;

            var sqlMetaData = new SqlMetaData[]
            {
                new SqlMetaData("value", SqlDbType.BigInt), 
            };

            var p = sqlCommand.Parameters.Add(_paramName, SqlDbType.Structured);
            p.Value = _values.Select(v =>
            {
                var record = new SqlDataRecord(sqlMetaData);
                record.SetInt64(0, v);
                return record;
            });

            foreach (var parameter in _additionalParameters)
            {
                sqlCommand.Parameters.AddWithValue(parameter.Item1, parameter.Item2);
            }
        }

        public BigIntTableParameter Add(string parameter, object value)
        {
            _additionalParameters.Add((parameter, value));
            return this;
        }
    }
}