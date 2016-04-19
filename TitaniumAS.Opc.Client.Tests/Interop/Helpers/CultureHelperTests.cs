using System.Globalization;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TitaniumAS.Opc.Client.Interop.Helpers;

namespace TitaniumAS.Opc.Client.Tests.Interop.Helpers
{
    [TestClass]
    public class CultureHelperTests
    {
        [TestMethod]
        public void Is_Should_Provide_InvariantCulture_By_LOCALE_NEUTRAL()
        {
            CultureInfo cultureInfo = CultureHelper.GetCultureInfo(CultureHelper.LOCALE_NEUTRAL);
            cultureInfo.Should().Be(CultureInfo.InvariantCulture);
        }

        [TestMethod]
        public void Is_Should_Provide_InvariantCulture_By_LOCALE_SYSTEM_DEFAULT()
        {
            CultureInfo cultureInfo = CultureHelper.GetCultureInfo(CultureHelper.LOCALE_SYSTEM_DEFAULT);
            cultureInfo.Should().Be(CultureInfo.InvariantCulture);
        }

        [TestMethod]
        public void Is_Should_Provide_InvariantCulture_By_LOCALE_USER_DEFAULT()
        {
            CultureInfo cultureInfo = CultureHelper.GetCultureInfo(CultureHelper.LOCALE_USER_DEFAULT);
            cultureInfo.Should().Be(CultureInfo.InvariantCulture);
        }

        [TestMethod]
        public void Is_Should_Provide_CultureInfo_By_LocaleId()
        {
            int localeId = new CultureInfo("ru-RU").LCID;
            CultureInfo cultureInfo = CultureHelper.GetCultureInfo(localeId);
            cultureInfo.Name.Should().Be("ru-RU");
        }

        [TestMethod]
        public void Is_Should_Provide_LOCALE_NEUTRAL_By_Null_CultureInfo()
        {
            int localeId = CultureHelper.GetLocaleId(null);
            localeId.Should().Be(CultureHelper.LOCALE_NEUTRAL);
        }

        [TestMethod]
        public void Is_Should_Provide_LocaleId_By_CultureInfo()
        {
            var cultureInfo = new CultureInfo("ru-RU");
            int expectedLocaleId = cultureInfo.LCID;
            int localeId = CultureHelper.GetLocaleId(cultureInfo);
            localeId.Should().Be(expectedLocaleId);
        }
    }
}