using System;
using System.Globalization;

namespace TitaniumAS.Opc.Client.Interop.Helpers
{
    /// <summary>
    /// Represents helper class for conversion between LocaleID (LCID) and CultureInfo.
    /// </summary>
    internal static class CultureHelper
    {
        /// <summary>
        /// The constant used for neutral locale.
        /// </summary>
        public const int LOCALE_NEUTRAL = 0x0;

        /// <summary>
        /// The constant used for system default locale.
        /// </summary>
        public const int LOCALE_SYSTEM_DEFAULT = 0x800;

        /// <summary>
        /// The constant used for user default locale.
        /// </summary>
        public const int LOCALE_USER_DEFAULT = 0x400;

        /// <summary>
        /// Gets a CultureInfo corresponding to specified LocaleID (LCID).
        /// </summary>
        /// <param name="localeId">The LocaleID (LCID).</param>
        /// <returns>The CultureInfo.</returns>
        public static CultureInfo GetCultureInfo(int localeId)
        {
            try
            {
                if (localeId == LOCALE_NEUTRAL || localeId == LOCALE_SYSTEM_DEFAULT || localeId == LOCALE_USER_DEFAULT)
                {
                    return CultureInfo.InvariantCulture;
                }

                return new CultureInfo(localeId);
            }
            catch (ArgumentOutOfRangeException)
            {
                throw new ArgumentException("Wrong LocaleID (LCID)", "localeId");
            }
            catch (CultureNotFoundException)
            {
                throw new ArgumentException("Wrong LocaleID (LCID)", "localeId");
            }
        }

        /// <summary>
        /// Gets a LocaleID (LCID) corresponding to specified CultureInfo.
        /// </summary>
        /// <param name="cultureInfo">The CultureInfo.</param>
        /// <returns>The LocaleID (LCID).</returns>
        public static int GetLocaleId(CultureInfo cultureInfo)
        {
            if (cultureInfo == null)
            {
                return LOCALE_NEUTRAL;
            }
            return cultureInfo.LCID;
        }
    }
}
