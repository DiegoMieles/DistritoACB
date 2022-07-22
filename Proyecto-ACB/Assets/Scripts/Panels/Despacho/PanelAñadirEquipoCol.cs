using Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebAPI;

namespace Panels
{
    /// <summary>
    /// Controla el panel de añadir carta de colección al equipo
    /// </summary>
    public class PanelAñadirEquipoCol : Panel
    {
        [SerializeField] [Tooltip("Determina si es una colección")]
        private bool isColeccion;
        [SerializeField] [Tooltip("Contenedor de las colecciones")]
        private GridLayoutGroup scrollRectTransactions;
        [SerializeField] [Tooltip("Panel de datos de colección")]
        private PanelColeccionData panelColeccionData;
        [SerializeField] [Tooltip("Acción que se ejecuta al fallar un llamado a backend")]
        private UnityEvent onFailed;
        [SerializeField] [Tooltip("Datos de la colección")]
        private ColectionContainer colectionDataContainer = new ColectionContainer();
        [SerializeField] [Tooltip("Spinner de carga")]
        private GameObject spinner;
        [SerializeField] [Tooltip("Determina si el panel debe cerrarse junto con los paneles que le anteceden")]
        private bool canClose = true;
        [SerializeField, TextArea] [Tooltip("Texto que se muestra cuando la traida de datos de colecciones es fallida")]
        private string textFail;
        [SerializeField] [Tooltip("Texto que se muestra cuando no hay datos de colección")]
        private Text textNoColeccion;

        /// <summary>
        /// Suscribe acción de cerrar panel del equipo competitivo
        /// </summary>
        private void OnEnable()
        {
            CallInfo();
            if(canClose)
                PanelTeamCompetitivo.OnClose += Close;
        }

        /// <summary>
        /// Desuscribe el evento de cerrar panel al destruirse el objeto
        /// </summary>
        private void OnDestroy()
        {
            if (canClose)
                PanelTeamCompetitivo.OnClose -= Close;
        }

        /// <summary>
        /// Carga la información del panel de colecciones
        /// </summary>
        private void CallInfo()
        {
            if (isColeccion)
            {
                WebProcedure.Instance.GetCollectionCL(snapshot =>
                {
                    Debug.Log(snapshot.RawJson);
                    JsonConvert.PopulateObject(snapshot.RawJson, colectionDataContainer);

                    if (colectionDataContainer.collectionData.collectionItems != null && colectionDataContainer.collectionData.collectionItems.Count > 0)
                    {
                        foreach (var collecionData in colectionDataContainer.collectionData.collectionItems)
                        {
                            if (scrollRectTransactions)
                            {
                                var prefab= Instantiate(panelColeccionData, scrollRectTransactions.transform);
                                prefab.ShowInfo(collecionData);
                                textNoColeccion.text = string.Empty;
                            }
                        }
                    }
                    else
                    {
                        textNoColeccion.text = textFail;
                    }
                    ClosedSpinner();

                }, error =>
                {
                    onFailed.Invoke();
                    ClosedSpinner();
                });
            }
            else
            {
                WebProcedure.Instance.GetUserCollectionTC(snapshot =>
                {
                    JsonConvert.PopulateObject(snapshot.RawJson, colectionDataContainer);
                    Debug.Log(snapshot.RawJson);
                    
                    if (colectionDataContainer.collectionData.collectionItems != null && colectionDataContainer.collectionData.collectionItems.Count > 0)
                    {
                        foreach (var collecionData in colectionDataContainer.collectionData.collectionItems)
                        {
                            var prefab= Instantiate(panelColeccionData, scrollRectTransactions.transform);
                            prefab.ShowInfo(collecionData/*, null*/);
                            textNoColeccion.text = string.Empty;
                        }
                    }
                    else
                    {
                        textNoColeccion.text = textFail;
                    }
                    ClosedSpinner();

                }, error =>
                {
                    onFailed.Invoke();
                    ClosedSpinner();
                });
            }
        }

        /// <summary>
        /// Desactiva el spinner de carga
        /// </summary>
        private void ClosedSpinner()
        {
            for (int i = 0; i < spinner.transform.childCount; i++)
            {
                spinner.transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }
}