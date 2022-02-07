using System.IO;
using System.Data.Common;

namespace FMSC.Core.Databases
{
    public static class DbDataReaderExtensions
    {
        public static string GetStringN(this DbDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            else
                return reader.GetString(index);
        }

        public static long? GetInt64N(this DbDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            else
                return reader.GetInt64(index);
        }

        public static int? GetInt32N(this DbDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            else
                return reader.GetInt32(index);
        }

        public static short? GetInt16N(this DbDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            else
                return reader.GetInt16(index);
        }

        public static short? GetShortN(this DbDataReader reader, int index)
        {
            return reader.GetInt16(index);
        }

        public static long? GetLongN(this DbDataReader reader, int index)
        {
            return reader.GetInt64(index);
        }

        public static double? GetDoubleN(this DbDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            else
                return reader.GetDouble(index);
        }

        public static decimal? GetDecimalN(this DbDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            else
                return reader.GetDecimal(index);
        }

        public static float? GetFloatN(this DbDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            else
                return reader.GetFloat(index);
        }

        public static char? GetCharN(this DbDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            else
                return reader.GetChar(index);
        }

        public static byte? GetByteN(this DbDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            else
                return reader.GetByte(index);
        }

        public static bool? GetBoolN(this DbDataReader reader, int index)
        {
            if (reader.IsDBNull(index))
                return null;
            else
                return reader.GetBoolean(index);
        }


        public static byte[] GetBytesEx(this DbDataReader reader, int index, int chunkSize = 2048)
        {
            byte[] buffer = new byte[chunkSize];
            long bytesRead;
            long fieldOffset = 0;
            using (MemoryStream stream = new MemoryStream())
            {
                while ((bytesRead = reader.GetBytes(index, fieldOffset, buffer, 0, buffer.Length)) > 0)
                {
                    stream.Write(buffer, 0, (int)bytesRead);
                    fieldOffset += bytesRead;
                }
                return stream.ToArray();
            }
        }
    }
}
