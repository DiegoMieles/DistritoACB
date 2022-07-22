using Data;
using Newtonsoft.Json;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebAPI;

namespace Panels
{
    /// <summary>
    /// Controla el panel de añadir carta de liga al equipo
    /// </summary>
    public class PanelSubCollectionsToPublish : Panel
    {
        //Evento que se dispara al completar la publicación en el mercadillo
        public event PanelConfirmPublish.VoidDelegate OnConfirmedPublish;
        [SerializeField] [Tooltip("Determina si es una colección")]
        private bool isColeccion;
        [SerializeField] [Tooltip("Título del panel")]
        private Text textTitle;
        [SerializeField] [Tooltip("Contenedor de botones de ligas")]
        private GridLayoutGroup scrollRectTransactions;
        [SerializeField] [Tooltip("Prefab del panel de subcolecciones")]
        private PanelSubCollectionToPublishData panelSubcoleccionData;
        [SerializeField]
        [Tooltip("Prefab del panel de subcolecciones")]
        private PanelCardsToPublish panelSubcoleccionHighlightData;
        [SerializeField] [Tooltip("Se ejecuta cuando la traida de datos de liga es fallida")]
        private UnityEvent onFailed;
        [SerializeField] [Tooltip("Contenedor de cartas de subcolección")]
        private SubCollectionDataToPublish subcolectionDataContainer = new SubCollectionDataToPublish();
        [SerializeField]
        [Tooltip("Contenedor de cartas de subcolección de highlights ")]
        private SubCollectionHiglightDataToPublish subcolectionHiglightDataContainer = new SubCollectionHiglightDataToPublish();
        [SerializeField, TextArea] [Tooltip("Texto que se muestra cuando la traida de datos de liga es fallida")]
        private string textFail;
        [SerializeField] [Tooltip("Texto que se muestra cuando no hay datos de liga")]
        private Text textNoSucolecciones;
        [SerializeField] [Tooltip("Spinner de carga")]
        private GameObject spinner;

        /// <summary>
        /// Suscribe acción de cerrar panel del equipo competitivo
        /// </summary>
        private void OnEnable()
        {
            PanelTeamCompetitivo.OnClose += Close;
        }

        /// <summary>
        /// Desuscribe el evento de cerrar panel al destruirse el objeto
        /// </summary>
        private void OnDestroy()
        {
            PanelTeamCompetitivo.OnClose -= Close;
        }

        /// <summary>
        /// Carga los datos de la colección
        /// </summary>
        /// <param name="collectionID">Id de la colección</param>
        /// <param name="title">Título de la colección</param>
        public void SetCollectionData(int collectionID, string title)
        {
            if (isColeccion)
            {
                textTitle.text = title;
                var cardsubcol = JsonConvert.SerializeObject(new BodySubcollection() {collection_id = collectionID});

                WebProcedure.Instance.GetSubCollectionsToSell(cardsubcol, snapshot =>
                {
                    JsonConvert.PopulateObject(snapshot.RawJson, subcolectionDataContainer);
                    Debug.Log(snapshot.RawJson);
                    if (subcolectionDataContainer.cardItems != null)
                    {
                        foreach (var subcolecciondata in subcolectionDataContainer.cardItems)
                        {
                            if (scrollRectTransactions)
                            {
                                var prefab = Instantiate(panelSubcoleccionData, scrollRectTransactions.transform);
                                prefab.ShowInfo(subcolecciondata, null);
                                prefab.OnConfirmedPublish += () => { OnConfirmedPublish?.Invoke(); Close(); };
                                textNoSucolecciones.text = string.Empty;
                            }
                            else
                            {
                                ClosedSpinner();
                                textNoSucolecciones.text = textFail;
                            }
                        }
                    }
                    else
                    {
                        textNoSucolecciones.text = textFail;
                        ClosedSpinner();
                    }
                }, error =>
                {
                    onFailed.Invoke();
                    ClosedSpinner();
                });
            }
            else
            {
                textTitle.text = title;
                var cardsubcol = JsonConvert.SerializeObject(new BodySubcollection() {collection_id = collectionID});

                WebProcedure.Instance.GetSubCollectionHighlightsToSell(cardsubcol, snapshot =>
                {
                    JsonConvert.PopulateObject(snapshot.RawJson, subcolectionHiglightDataContainer);
                    Debug.Log(snapshot.RawJson);

                    if (subcolectionHiglightDataContainer.items != null)
                    {
                        foreach (var subcolecciondata in subcolectionHiglightDataContainer.items)
                        {
                            var prefab = Instantiate(panelSubcoleccionHighlightData, scrollRectTransactions.transform);
                            prefab.OnConfirmedPublish += () => { OnConfirmedPublish?.Invoke(); Close(); };
                            prefab.CallInfo(subcolecciondata.id, null);
                        }

                    }
                    else
                    {
                        ClosedSpinner();
                    }
                }, error =>
                {
                    ClosedSpinner();
                    onFailed.Invoke();
                }); 
            }
        }

        /// <summary>
        /// Oculta el spinner de carga
        /// </summary>
        private void ClosedSpinner()
        {
            for (int i = 0; i < spinner.transform.childCount; i++)
            {
                spinner.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// Activa corrutina que retrasa la desactivación del spinner
        /// </summary>
        public void SpinnerClosing()
        {
            StartCoroutine(CloseWait());
        }

        /// <summary>
        /// Corrutina que retrasa la desactivación del spinner
        /// </summary>
        /// <returns></returns>
        IEnumerator CloseWait()
        {
            yield return new WaitForSeconds(3.0f);
            ClosedSpinner();
        }
    }
}

