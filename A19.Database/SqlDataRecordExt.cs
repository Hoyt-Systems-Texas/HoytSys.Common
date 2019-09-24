using Microsoft.SqlServer.Server;

namespace A19.Database
{
    public static class SqlDataRecordExt
    {
        public static SqlDataRecord SetStringNull(this SqlDataRecord r, int pos, string v)
        {
            if (string.IsNullOrWhiteSpace(v))
            {
                r.SetDBNull(pos);
            }
            else
            {
                r.SetString(pos, v);
            }
            return r;
        }

        public static SqlDataRecord SetInt16(this SqlDataRecord r, int pos, short? v)
        {
            if (!v.HasValue)
            {
                r.SetDBNull(pos);
            }
            else
            {
                r.SetInt16(pos, v.Value);
            }
            return r;
        }

        public static SqlDataRecord SetInt32(this SqlDataRecord r, int pos, int? v)
        {
            if (!v.HasValue)
            {
                r.SetDBNull(pos);
            }
            else
            {
                r.SetInt32(pos, v.Value);
            }
            return r;
        }

        public static SqlDataRecord SetInt64(this SqlDataRecord r, int pos, long? v)
        {
            if (!v.HasValue)
            {
                r.SetDBNull(pos);
            }
            else
            {
                r.SetInt64(pos, v.Value);
            }
            return r;
        }
        
    }
}