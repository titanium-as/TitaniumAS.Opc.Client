using System;
using System.Runtime.InteropServices;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TitaniumAS.Opc.Client.Interop.Helpers;

namespace TitaniumAS.Opc.Client.Tests.Interop.Helpers
{
    [TestClass]
    public class TypeConverterTests
    {
        [TestMethod]
        public void Is_Should_Convert_From_VT_EMPTY_To_Null()
        {
            Type result = TypeConverter.FromVarEnum(VarEnum.VT_EMPTY);
            result.Should().BeNull();
        }

        [TestMethod]
        public void Is_Should_Convert_From_VT_I4_To_Int32()
        {
            Type result = TypeConverter.FromVarEnum(VarEnum.VT_I4);
            result.Should().Be(typeof (int));
        }

        [TestMethod]
        public void Is_Should_Convert_From_ARRAY_Of_VT_R8_To_DoubleArray()
        {
            Type result = TypeConverter.FromVarEnum(VarEnum.VT_ARRAY | VarEnum.VT_R8);
            result.Should().Be(typeof (double[]));
        }

        [TestMethod]
        public void Is_Should_Convert_From_Null_To_VT_EMPTY()
        {
            VarEnum result = TypeConverter.ToVarEnum(null);
            result.Should().Be(VarEnum.VT_EMPTY);
        }

        [TestMethod]
        public void Is_Should_Convert_From_Int32_To_VT_I4()
        {
            VarEnum result = TypeConverter.ToVarEnum(typeof (int));
            result.Should().Be(VarEnum.VT_I4);
        }

        [TestMethod]
        public void Is_Should_Convert_From_StringArray_To_VT_BSTR()
        {
            VarEnum result = TypeConverter.ToVarEnum(typeof (string[]));
            result.Should().Be(VarEnum.VT_ARRAY | VarEnum.VT_BSTR);
        }

        [TestMethod]
        public void Is_Should_Convert_From_Type_To_VT_I2()
        {
            VarEnum result = TypeConverter.ToVarEnum(typeof (Type));
            result.Should().Be(VarEnum.VT_I2);
        }
    }
}