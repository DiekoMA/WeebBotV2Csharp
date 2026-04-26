using System.Reflection;
using System.Runtime.Serialization;

public static class OddsHelper
{
    /// <summary>
    /// Converts OddsPercentage enum to double (0.0 to 1.0)
    /// </summary>
    public static double ToDouble(OddsPercentage odds)
    {
        var field = typeof(OddsPercentage).GetField(odds.ToString());
        var attr = field?.GetCustomAttribute<EnumMemberAttribute>();
        
        if (attr?.Value != null)
        {
            string value = attr.Value.Replace("%", "");
            if (double.TryParse(value, out double result))
            {
                return result / 100.0;
            }
        }
        
        return 0.0;
    }
 
    /// <summary>
    /// Gets the display value (e.g., "10%")
    /// </summary>
    public static string GetDisplayValue(OddsPercentage odds)
    {
        var field = typeof(OddsPercentage).GetField(odds.ToString());
        var attr = field?.GetCustomAttribute<EnumMemberAttribute>();
        return attr?.Value ?? odds.ToString();
    }
}