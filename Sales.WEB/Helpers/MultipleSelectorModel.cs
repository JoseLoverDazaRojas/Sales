namespace Sales.WEB.Helpers
{

    #region Import
    
    #endregion Import

    /// <summary>
    /// The class MultipleSelectorModel
    /// </summary>

    public class MultipleSelectorModel
    {

        #region Attributes

        public string Key { get; set; }

        public string Value { get; set; }

        #endregion Attributes

        #region Methods

        public MultipleSelectorModel(string key, string value)
        {
            Key = key;
            Value = value;
        }

        #endregion Methods

    }
}