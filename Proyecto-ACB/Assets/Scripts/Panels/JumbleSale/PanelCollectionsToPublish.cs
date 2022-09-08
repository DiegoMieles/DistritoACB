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
    public class PanelCollectionsToPublish : Panel
    {
        //Evento que se dispara al completar la publicación en el mercadillo
        public event PanelConfirmPublish.VoidDelegate OnConfirmedPublish;
        [SerializeField] [Tooltip("Determina si es una colección")]
        private bool isColeccion;
        [SerializeField] [Tooltip("Contenedor de las colecciones")]
        private GridLayoutGroup scrollRectTransactions;
        [SerializeField] [Tooltip("Panel de datos de colección")]
        private PanelColeccionData panelColeccionData;
        [SerializeField] [Tooltip("Acción que se ejecuta al fallar un llamado a backend")]
        private UnityEvent onFailed;
        [SerializeField] [Tooltip("Datos de la colección")]
        private AllCollectionsContainer colectionDataContainer = new AllCollectionsContainer();
        [SerializeField]
        [Tooltip("Datos de la colección de highlights")]
        private HighlightCollectionData highlightColectionDataContainer = new HighlightCollectionData();
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
                WebProcedure.Instance.GetAllCollectionsToSell(snapshot =>
                {
                    Debug.Log(snapshot.RawJson);
                    JsonConvert.PopulateObject(snapshot.RawJson, colectionDataContainer);

                    if (colectionDataContainer.data != null && colectionDataContainer.data.Count > 0)
                    {
                        foreach (var collecionData in colectionDataContainer.data)
                        {
                            if (scrollRectTransactions)
                            {
                                var prefab = Instantiate(panelColeccionData, scrollRectTransactions.transform);
                                prefab.GetComponent<PanelColeccionData>().OnConfirmedPublish += () => { OnConfirmedPublish?.Invoke(); Close(); };
                                prefab.ShowInfo(collecionData, true,collecionData.type == ItemType.H);
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
                    textNoColeccion.text = textFail;
                    onFailed.Invoke();
                    ClosedSpinner();
                });
            }
            else
            {
                WebProcedure.Instance.GetHighlightCollectionsToSell(snapshot =>
                {
                    Debug.Log(snapshot.RawJson);
                    JsonConvert.PopulateObject(snapshot.RawJson, highlightColectionDataContainer);

                    if (highlightColectionDataContainer.items != null && highlightColectionDataContainer.items.Count > 0)
                    {
                        foreach (var collecionData in highlightColectionDataContainer.items)
                        {
                            if (scrollRectTransactions)
                            {
                                var prefab = Instantiate(panelColeccionData, scrollRectTransactions.transform);
                                prefab.GetComponent<PanelColeccionData>().OnConfirmedPublish += () => { OnConfirmedPublish?.Invoke(); Close(); };
                                prefab.ShowInfo(collecionData, true,true);
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
                    textNoColeccion.text = textFail;
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