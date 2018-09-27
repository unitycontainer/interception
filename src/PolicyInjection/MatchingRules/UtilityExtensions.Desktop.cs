

using System.Globalization;
using System.Reflection;

namespace Unity.Interception.PolicyInjection.MatchingRules
{
    /// <summary>
    /// Some utility extension methods to make things portable to Silverlight.
    /// </summary>
    internal static class UtilityExtensions
    {
        internal static bool IsReturn(this ParameterInfo parameterInfo)
        {
            return parameterInfo.IsRetval;
        }

        internal static bool IsInvariantCulture(this CultureInfo cultureInfo)
        {
            return cultureInfo.LCID == CultureInfo.InvariantCulture.LCID;
        }

        internal static bool IsSameAs(this CultureInfo cultureInfo, CultureInfo otherCultureInfo)
        {
            return cultureInfo.LCID == otherCultureInfo.LCID;
        }
    }
}
