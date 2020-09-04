using System;
using System.Collections.Generic;
using System.Text;

namespace Storm.Helpers
{
    static class TypeHelper
    {
        static private Object monitor = new object();
        static private Dictionary<Type, Object> _defValues;

        static public Dictionary<Type, Object> DefValues
        {
            get
            {
                if (_defValues == null)
                {
                    lock (monitor)
                    {
                        if (_defValues == null)
                        {
                            _defValues = new Dictionary<Type, Object>();
                            _defValues[typeof(byte)] = byte.MinValue;
                            _defValues[typeof(sbyte)] = sbyte.MinValue;
                            _defValues[typeof(short)] = (short)0;
                            _defValues[typeof(ushort)] = (ushort)0;
                            _defValues[typeof(int)] = (int)0;
                            _defValues[typeof(uint)] = (uint)0;
                            _defValues[typeof(long)] = (long)0;
                            _defValues[typeof(ulong)] = (ulong)0;
                            _defValues[typeof(float)] = (float)0.0;
                            _defValues[typeof(double)] = (double)0.0;
                            _defValues[typeof(decimal)] = (decimal)0.0;
                            _defValues[typeof(bool)] = false;
                            _defValues[typeof(string)] = null;
                            _defValues[typeof(char)] = char.MinValue;
                            _defValues[typeof(Guid)] = Guid.Empty;
                            _defValues[typeof(DateTime)] = DateTime.MinValue;
                            _defValues[typeof(DateTimeOffset)] = DateTimeOffset.MinValue;
                            _defValues[typeof(byte[])] = null;
                            _defValues[typeof(byte?)] = null;
                            _defValues[typeof(sbyte?)] = null;
                            _defValues[typeof(short?)] = null;
                            _defValues[typeof(ushort?)] = null;
                            _defValues[typeof(int?)] = null;
                            _defValues[typeof(uint?)] = null;
                            _defValues[typeof(long?)] = null;
                            _defValues[typeof(ulong?)] = null;
                            _defValues[typeof(float?)] = null;
                            _defValues[typeof(double?)] = null;
                            _defValues[typeof(decimal?)] = null;
                            _defValues[typeof(bool?)] = null;
                            _defValues[typeof(char?)] = null;
                            _defValues[typeof(Guid?)] = null;
                            _defValues[typeof(DateTime?)] = null;
                            _defValues[typeof(DateTimeOffset?)] = null;
                        }
                    }
                }
                return _defValues;
            }
        }
    }
}
