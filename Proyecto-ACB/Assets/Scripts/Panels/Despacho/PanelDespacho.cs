/// <summary>
/// Controla el panel de despacho (vista del despacho)
/// </summary>
public class PanelDespacho : Panel
{
    #region Unity Methods

    /// <summary>
    /// Pone la vista del despacho por delante de los demás paneles
    /// </summary>
    private void Start() => ACBSingleton.Instance.PanelBuildingSelection.transform.SetAsLastSibling();

    #endregion
}
