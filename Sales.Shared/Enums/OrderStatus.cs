namespace Sales.Shared.Enums
{

    #region Import

    using System.ComponentModel;

    #endregion Import

    /// <summary>
    /// The enum OrderStatus
    /// </summary>

    public enum OrderStatus
    {
                
        [Description("Nuevo")]
        New,

        [Description("Despachado")]
        Dispatched,

        [Description("Enviado")]
        Sent,

        [Description("Confirmado")]
        Confirmed,

        [Description("Cancelado")]
        Cancelled

    }

}