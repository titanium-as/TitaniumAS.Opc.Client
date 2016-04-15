using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TitaniumAS.Opc.Client.Common;

namespace TitaniumAS.Opc.Client.Tests.Common
{
    [TestClass]
    public class HRESULTTests
    {
        [TestMethod]
        public void It_Should_Create_Correct_Itf_Error_Code()
        {
            HRESULT hresult = HRESULT.AddItfError(0x0200, "My error");
            hresult.Should().Be(-2147220992);
        }

        [TestMethod]
        public void It_Should_Add_New_Itf_Error_To_The_Dictionary()
        {
            HRESULT hresult = HRESULT.AddItfError(0x0200, "My error");
            HRESULT.GetDescription(hresult).Should().Be("My error");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void It_Should_Throw_Argument_Out_Of_Range_Exception_When_A_Code_Less_Than_0x0200()
        {
            HRESULT.AddItfError(0x01FF, "My error");
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentOutOfRangeException))]
        public void It_Should_Throw_Argument_Out_Of_Range_Exception_When_A_Code_Greater_Than_0xFFFF()
        {
            HRESULT.AddItfError(0x10000, "My error");
        }
    }
}
