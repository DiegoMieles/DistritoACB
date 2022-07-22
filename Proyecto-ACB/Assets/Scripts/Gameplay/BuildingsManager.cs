using System.Collections.Generic;
using UnityEngine;
using Data;

/// <summary>
/// Controlador de los edificios disponibles en el juego
/// </summary>
public class BuildingsManager : MonoBehaviour
{
    [Header("Buildings configuration")]
    [SerializeField] [Tooltip("Listado de edificios disponibles")]
    private BuildingButton[] buildings;
    [SerializeField] [Tooltip("Color del contorno de los edificios al ser de día")]
    private Color buildingDayOutline;
    [SerializeField] [Tooltip("Color del contorno de los edificios al ser de noche")]
    private Color buildingNightOutline;

    private List<MainMenuData.BuildData> backendBuildingsData; //Listado de datos de los edificios
    private bool isNight; //Determina si es de noche en el juego

    private bool isPlayingShiny; //Determina si los edificios deben hacer el efecto de barrido

    #region Public Methods

    /// <summary>
    /// Configura todos los edificios a nivel lógico
    /// </summary>
    /// <param name="setNightOutline">Determina si el color del contorno de los edificios debe tener el color noche</param>
    /// <param name="buildingsData">Lista de los datos del edificios disponible</param>
    public void SetupBuildings(bool setNightOutline, List<MainMenuData.BuildData> buildingsData = null)
    {
        Color buildingOutline = setNightOutline ? buildingNightOutline : buildingDayOutline;
        isNight = setNightOutline;
        isPlayingShiny = false;

        bool hasBackendData = buildingsData != null ? true : false;
        backendBuildingsData = buildingsData;

        foreach (BuildingButton building in buildings)
        {
            if (hasBackendData)
                SetBuildingData(building);

            building.SetupButton(buildingOutline);
        }
    }

    /// <summary>
    /// Muestra el contorno y efecto de barrido de los edificios
    /// </summary>
    public void ActivateBuildingsOutline()
    {
        if (isPlayingShiny)
            return;

        foreach(BuildingButton building in buildings)
        {
            building.SelectionOutline.enabled = true;
            building.Shiny.Play();
        }
        isPlayingShiny = true;
    }

    /// <summary>
    /// Oculta el contorno y efecto de barrido de los edificios
    /// </summary>
    public void DeactivateBuildingOutline()
    {
        if (!isPlayingShiny)
            return;

        foreach (BuildingButton building in buildings)
        {
            building.SelectionOutline.enabled = false;
            building.Shiny.Stop();
        }
        isPlayingShiny = false;
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Asigna los datos de un edificio en específico
    /// </summary>
    /// <param name="building">Edificio al que se le van a asignar los datos</param>
    private void SetBuildingData(BuildingButton building)
    {
        for(int i = 0; i < backendBuildingsData.Count; i++)
        {
            if (building.BuildingId == backendBuildingsData[i].id)
            {
                building.SetBuildingData(backendBuildingsData[i], isNight);
                return;
            }
        }
    }

    #endregion

}
