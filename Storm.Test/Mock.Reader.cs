using System;
using System.Data;
using System.Linq;

namespace Storm.Test
{
    public class MockReader : IDataReader
    {
        private readonly object[][] source;
        private readonly string[] names;
        private int row = -1;
        private bool isOpen = true;


        public MockReader(object[][] source, string[] names)
        {
            this.source = source;
            this.names = names;
        }

        public object this[int i] => source[row][i];

        public object this[string name] => source[row][this.GetOrdinal(name)];

        public bool IsClosed => isOpen;

        public int RecordsAffected => source.Length;

        public int FieldCount => names.Length;

        public void Close() { isOpen = false; }

        public void Dispose() { }

        public bool GetBoolean(int i) => (bool)source[row][i];

        public byte GetByte(int i) => (byte)source[row][i];

        public char GetChar(int i) => (char)source[row][i];

        public string GetDataTypeName(int i) => source[row][i].GetType().Name;

        public DateTime GetDateTime(int i) => (DateTime)source[row][i];

        public decimal GetDecimal(int i) => (decimal) source[row][i];

        public double GetDouble(int i) => (double)source[row][i];

        public Type GetFieldType(int i) => source[row][i].GetType();

        public float GetFloat(int i) => (float)source[row][i];

        public Guid GetGuid(int i) => (Guid)source[row][i];

        public short GetInt16(int i) => (short)source[row][i];

        public int GetInt32(int i) => (int) source[row][i];

        public long GetInt64(int i) => (long)source[row][i];

        public string GetName(int i) => names[i];

        public int GetOrdinal(string name) => names.Select((x, i) => (x == name, i)).First(x => x.Item1).i;

        public string GetString(int i) => (string)source[row][i];

        public object GetValue(int i) => source[row][i];

        public bool IsDBNull(int i) => source[row][i] == null;

        public bool Read()
        {
            if (row + 1 < source.Length)
            {
                row++;
                return true;
            }
            else
            {
                return false;
            }        
        }

        public bool NextResult()
        {
            throw new NotImplementedException();
        }

        public int GetValues(object[] values)
        {
            throw new NotImplementedException();
        }

        public DataTable GetSchemaTable()
        {
            throw new NotImplementedException();
        }

        public long GetChars(int i, long fieldoffset, char[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public IDataReader GetData(int i)
        {
            throw new NotImplementedException();
        }

        public long GetBytes(int i, long fieldOffset, byte[] buffer, int bufferoffset, int length)
        {
            throw new NotImplementedException();
        }

        public int Depth => throw new NotImplementedException();

    }
}
