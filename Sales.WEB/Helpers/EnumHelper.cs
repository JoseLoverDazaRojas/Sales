namespace Sales.WEB.Helpers
{

    #region Import
        
    using System.ComponentModel;

    #endregion Import

    /// <summary>
    /// The class EnumHelper
    /// </summary>

    public class EnumHelper
    {

        #region Methods

        public static string GetEnumDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString())!;
            var attributes = (DescriptionAttribute[])field.GetCustomAttributes(typeof(DescriptionAttribute), false);

            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            else
            {
                return value.ToString();
            }
        }

        #endregion Methods

    }
}