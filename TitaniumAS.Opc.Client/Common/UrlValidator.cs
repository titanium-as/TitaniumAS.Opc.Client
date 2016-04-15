using System;

namespace TitaniumAS.Opc.Client.Common
{
    /// <summary>
    /// Represents an OPC server URL validator.
    /// </summary>
    public static class UrlValidator
    {
        /// <summary>
        /// Checks if an OPC DA server URL is correct.
        /// </summary>
        /// <param name="opcServerUrl">The OPC server URL.</param>
        /// <exception cref="System.ArgumentNullException">opcServerUrl</exception>
        /// <exception cref="UriFormatException">Wrong OPC server URL. Format: opcda://host/progIdOrCLSID.</exception>
        public static void CheckOpcUrl(Uri opcServerUrl)
        {
            UrlValidationResult result = Validate(opcServerUrl);

            if (0 != (result & UrlValidationResult.UrlIsNull))
            {
                throw new ArgumentNullException("opcServerUrl");
            }

            if (0 != (result & UrlValidationResult.MoreThanTwoSegments))
            {
                throw new UriFormatException("Wrong OPC server URL. Format: opcda://host/progIdOrCLSID.");
            }

            if (0 != (result & UrlValidationResult.WrongScheme))
            {
                throw new UriFormatException("Wrong OPC server URL. Format: opcda://host/progIdOrCLSID.");
            }
        }

        /// <summary>
        /// Validates if an OPC DA server URL is correct and returns valivation result.
        /// </summary>
        /// <param name="opcServerUrl">The OPC server URL.</param>
        /// <returns>OPC DA server URL validation result.</returns>
        public static UrlValidationResult Validate(Uri opcServerUrl)
        {
            var result = UrlValidationResult.Ok;

            if (opcServerUrl == null)
            {
                result |= UrlValidationResult.UrlIsNull;
                return result;
            }

            if (opcServerUrl.Segments.Length < 2)
            {
                result |= UrlValidationResult.MoreThanTwoSegments;
            }

            if (opcServerUrl.Scheme != UrlBuilder.OpcDaScheme)
            {
                result |= UrlValidationResult.WrongScheme;
            }

            return result;
        }
    }
}