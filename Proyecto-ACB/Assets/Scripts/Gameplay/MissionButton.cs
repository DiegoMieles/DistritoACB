using UnityEngine;
using UnityEngine.UI;
using Data;
using WebAPI;

/// <summary>
/// Controla todas las marquesinas conocidas como botones de misi�n
/// </summary>
public class MissionButton : BuildingButton
{
    [SerializeField, Header("PROPERTIES")] [Tooltip("Imagen de la marquesina")]
    private Image image;
    [SerializeField] [Tooltip("�ndice de misi�n o id")]
    private int position;
    [SerializeField, Header("DATA")]  [Tooltip("Datos de la misi�n")]
    private MissionsData.MissionItemData mission;
    [SerializeField] [Tooltip("Imagen por defecto cuando no se encuentra la imagen de la marquesina en backend")]
    private Sprite spriteDefault;

    public int Position => position;
    public MissionsData.MissionItemData Mission => mission;

    #region Public Methods

    /// <summary>
    /// Configura la misi�n y asigna la imagen principal de la marquesina
    /// </summary>
    /// <param name="missiondata">Datos de misi�n traidos desde backend</param>
    public void Setup(MissionsData.MissionItemData missiondata)
    {
        mission = missiondata;
        gameObject.SetActive(true);
        WebProcedure.Instance.GetSprite(mission.img, OnSuccess, OnFailed);
    }
    
    /// <summary>
    /// Resetea y oculta la misi�n a nivel visual
    /// </summary>
    public void ResetMission()
    {
        image.sprite = spriteDefault;
        mission = null;
        gameObject.SetActive(false);
    }

    #endregion

    #region Protected Methods

    /// <summary>
    /// Muestra las opciones cuando se selecciona un edificio
    /// </summary>
    protected override void OpenBuildingOptions()
    {
        base.OpenBuildingOptions();
        ACBSingleton.Instance.PanelBuildingSelection.SetMissionData(mission);
        ACBSingleton.Instance.actualCachedMission = this;
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// M�todo que se ejecuta cuando backend no ha podido traer la imagen de la marquesina
    /// </summary>
    /// <param name="obj">Clase con los datos de error traidos desde backend</param>
    private void OnFailed(WebError obj)
    {
        Debug.Log(obj.Message);
    }

    /// <summary>
    /// M�todo que se ejecuta cuando backend consigue satisfactoriamente la imagen de la marquesina
    /// </summary>
    /// <param name="obj">Imagen de la marquesina traido desde backend</param>
    private void OnSuccess(Sprite obj)
    {
        image.sprite = obj;
    }

    #endregion

}
