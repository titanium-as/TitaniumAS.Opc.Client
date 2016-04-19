using System;
using System.Runtime.InteropServices;
using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Interop.Helpers
{
    /// <summary>
    /// Represents helper class for conversion between VarEnum and .NET Type.
    /// </summary>
    internal class TypeConverter
    {
        /// <summary>
        /// Converts a VarEnum to .NET type.
        /// </summary>
        /// <param name="varEnum">The VarEnum.</param>
        /// <returns>
        /// The .NET type.
        /// </returns>
        public static Type FromVarEnum(VarEnum varEnum)
        {
            switch (varEnum)
            {
                case VarEnum.VT_EMPTY:
                    return null;
                case VarEnum.VT_I1:
                    return typeof (sbyte);
                case VarEnum.VT_UI1:
                    return typeof (byte);
                case VarEnum.VT_I2:
                    return typeof (short);
                case VarEnum.VT_UI2:
                    return typeof (ushort);
                case VarEnum.VT_I4:
                    return typeof (int);
                case VarEnum.VT_UI4:
                    return typeof (uint);
                case VarEnum.VT_I8:
                    return typeof (long);
                case VarEnum.VT_UI8:
                    return typeof (ulong);
                case VarEnum.VT_R4:
                    return typeof (float);
                case VarEnum.VT_R8:
                    return typeof (double);
                case VarEnum.VT_CY:
                    return typeof (decimal);
                case VarEnum.VT_BOOL:
                    return typeof (bool);
                case VarEnum.VT_DATE:
                    return typeof (DateTime);
                case VarEnum.VT_BSTR:
                    return typeof (string);
                case VarEnum.VT_ARRAY | VarEnum.VT_I1:
                    return typeof (sbyte[]);
                case VarEnum.VT_ARRAY | VarEnum.VT_UI1:
                    return typeof (byte[]);
                case VarEnum.VT_ARRAY | VarEnum.VT_I2:
                    return typeof (short[]);
                case VarEnum.VT_ARRAY | VarEnum.VT_UI2:
                    return typeof (ushort[]);
                case VarEnum.VT_ARRAY | VarEnum.VT_I4:
                    return typeof (int[]);
                case VarEnum.VT_ARRAY | VarEnum.VT_UI4:
                    return typeof (uint[]);
                case VarEnum.VT_ARRAY | VarEnum.VT_I8:
                    return typeof (long[]);
                case VarEnum.VT_ARRAY | VarEnum.VT_UI8:
                    return typeof (ulong[]);
                case VarEnum.VT_ARRAY | VarEnum.VT_R4:
                    return typeof (float[]);
                case VarEnum.VT_ARRAY | VarEnum.VT_R8:
                    return typeof (double[]);
                case VarEnum.VT_ARRAY | VarEnum.VT_CY:
                    return typeof (decimal[]);
                case VarEnum.VT_ARRAY | VarEnum.VT_BOOL:
                    return typeof (bool[]);
                case VarEnum.VT_ARRAY | VarEnum.VT_DATE:
                    return typeof (DateTime[]);
                case VarEnum.VT_ARRAY | VarEnum.VT_BSTR:
                    return typeof (string[]);
                case VarEnum.VT_ARRAY | VarEnum.VT_VARIANT:
                    return typeof (object[]);
                default:
                    return typeof (IllegalType);
            }
        }

        /// <summary>
        /// Converts .NET type to a VarEnum.
        /// </summary>
        /// <param name="type">The .NET type.</param>
        /// <returns>
        /// The VarEnum.
        /// </returns>
        public static VarEnum ToVarEnum(Type type)
        {
            if (type == null)
                return VarEnum.VT_EMPTY;

            if (type == typeof (sbyte))
                return VarEnum.VT_I1;

            if (type == typeof (byte))
                return VarEnum.VT_UI1;

            if (type == typeof (short))
                return VarEnum.VT_I2;

            if (type == typeof (ushort))
                return VarEnum.VT_UI2;

            if (type == typeof (int))
                return VarEnum.VT_I4;

            if (type == typeof (uint))
                return VarEnum.VT_UI4;

            if (type == typeof (long))
                return VarEnum.VT_I8;

            if (type == typeof (ulong))
                return VarEnum.VT_UI8;

            if (type == typeof (float))
                return VarEnum.VT_R4;

            if (type == typeof (double))
                return VarEnum.VT_R8;

            if (type == typeof (decimal))
                return VarEnum.VT_CY;

            if (type == typeof (bool))
                return VarEnum.VT_BOOL;

            if (type == typeof (DateTime))
                return VarEnum.VT_DATE;

            if (type == typeof (string))
                return VarEnum.VT_BSTR;

            if (type == typeof (object))
                return VarEnum.VT_EMPTY;

            if (type == typeof (sbyte[]))
                return VarEnum.VT_ARRAY | VarEnum.VT_I1;

            if (type == typeof (byte[]))
                return VarEnum.VT_ARRAY | VarEnum.VT_UI1;

            if (type == typeof (short[]))
                return VarEnum.VT_ARRAY | VarEnum.VT_I2;

            if (type == typeof (ushort[]))
                return VarEnum.VT_ARRAY | VarEnum.VT_UI2;

            if (type == typeof (int[]))
                return VarEnum.VT_ARRAY | VarEnum.VT_I4;

            if (type == typeof (uint[]))
                return VarEnum.VT_ARRAY | VarEnum.VT_UI4;

            if (type == typeof (long[]))
                return VarEnum.VT_ARRAY | VarEnum.VT_I8;

            if (type == typeof (ulong[]))
                return VarEnum.VT_ARRAY | VarEnum.VT_UI8;

            if (type == typeof (float[]))
                return VarEnum.VT_ARRAY | VarEnum.VT_R4;

            if (type == typeof (double[]))
                return VarEnum.VT_ARRAY | VarEnum.VT_R8;

            if (type == typeof (decimal[]))
                return VarEnum.VT_ARRAY | VarEnum.VT_CY;

            if (type == typeof (bool[]))
                return VarEnum.VT_ARRAY | VarEnum.VT_BOOL;

            if (type == typeof (DateTime[]))
                return VarEnum.VT_ARRAY | VarEnum.VT_DATE;

            if (type == typeof (string[]))
                return VarEnum.VT_ARRAY | VarEnum.VT_BSTR;

            if (type == typeof (object[]))
                return VarEnum.VT_ARRAY | VarEnum.VT_VARIANT;

            // Special cases
            if (type == typeof (IllegalType))
                return (VarEnum) Enum.ToObject(typeof (VarEnum), 0x7FFF);

            if (type == typeof (Type))
                return VarEnum.VT_I2;

            return VarEnum.VT_EMPTY;
        }
    }
}