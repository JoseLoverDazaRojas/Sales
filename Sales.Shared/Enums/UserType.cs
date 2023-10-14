namespace Sales.Shared.Enums
{

    #region Import

    using System.ComponentModel;

    #endregion Import

    /// <summary>
    /// The enum UserType
    /// </summary>

    public enum UserType
    {

        [Description("Administrador")]
        Admin,

        [Description("Usuario")]
        User

    }
}