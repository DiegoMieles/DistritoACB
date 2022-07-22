using Data;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebAPI;


namespace Panels
{
    /// <summary>
    /// Controla el panel de colección de equipos
    /// </summary>
    public class PanelColeccionData : MonoBehaviour
    {
        #region Fields and Properties
        //Evento que se dispara al completar la publicación en el mercadillo
        public event PanelConfirmPublish.VoidDelegate OnConfirmedPublish;
        [Header("Panel properties")]
        [SerializeField] [Tooltip("Botón de colección")]
        private Button button;
        [SerializeField] [Tooltip("Imágen de la colección")]
        private Image image;
        [SerializeField] [Tooltip("Texto de la colección")]
        private Text textConcept;
        [SerializeField] [Tooltip("Se ejecuta cuando la traida de datos es fallida")]
        private UnityEvent onFailed;
        [SerializeField] [Tooltip("Datos de la colección actual")]
        private CollectionData.CollectionItemData currentCollection;

        [Space(5)]
        [Header("On clicked button properties")]
        [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
        private PanelOpener panelOpener;
        [SerializeField] [Tooltip("Panel de añadir colección")]
        private GameObject panelAddCollection;
        [SerializeField] [Tooltip("Panel de añadir colección")]
        private GameObject panelSubCollectionHighlights;
        [SerializeField] [Tooltip("Panel de añadir cartas")]
        private GameObject panelAddCards;
        
        [SerializeField] [Tooltip("Nombre del spinner de carga")]
        private string spinner_name;

        #endregion

        #region Public Methods

        /// <summary>
        /// Muestra los datos de la colección
        /// </summary>
        /// <param name="collecionData">Datos de la colección</param>
        public void ShowInfo([CanBeNull] CollectionData.CollectionItemData collecionData,bool isJumbleSale = false, bool isHighlight = false)
        {
            currentCollection = collecionData;
            textConcept.text = currentCollection.name;
            
            if (!string.IsNullOrEmpty(currentCollection.pathImg))
            {
                WebProcedure.Instance.GetSprite(currentCollection.pathImg,sprite =>
                {
                    if (image)
                    {
                        image.sprite = sprite;
                        ClosedSpinner();
                    }
                 
                    if (collecionData.hasSubcollections)
                    {
                        button.onClick.AddListener(()=>
                        {
                            OnSelectedCollection(isHighlight? panelSubCollectionHighlights: panelAddCollection,collecionData.hasSubcollections, isHighlight? ItemType.HIGTHLIGHT: collecionData.type, isJumbleSale,isHighlight);
                        });
                    }
                    else
                    {
                        button.onClick.AddListener(()=>
                        {
                            OnSelectedCollection(panelAddCards,collecionData.hasSubcollections,collecionData.type,  isJumbleSale);
                        });
                    }
                }, error =>
                {
                    onFailed.Invoke();
                    ClosedSpinner();
                });
            }
            else
            {
                ClosedSpinner();
            }
        }

        #endregion

        #region Inner Methods

        /// <summary>
        /// Abre las cartas o la colección de acuerdo a lo seleccionado
        /// </summary>
        /// <param name="panelAddCollection">Panel de añadir carta a la colección</param>
        /// <param name="hascolletion">Determina si hay subcolecciones</param>
        /// <param name="type">Tipo de item</param>
        private void OnSelectedCollection(GameObject panelAddCollection, bool hascolletion, ItemType type, bool isJumbleSale = false,bool isHighlight = false)
        {
            panelOpener.popupPrefab = panelAddCollection;
            panelOpener.OpenPopup();
            if (hascolletion)
            {
                if (type != ItemType.HIGTHLIGHT)
                {
                    if (isJumbleSale)
                    {
                        panelOpener.popup.GetComponent<PanelSubCollectionsToPublish>().SetCollectionData(currentCollection.id, currentCollection.name);
                        panelOpener.popup.GetComponent<PanelSubCollectionsToPublish>().OnConfirmedPublish += () => { OnConfirmedPublish?.Invoke(); };
                    }
                    else panelOpener.popup.GetComponent<PanelAñadirEquipoLiga>().SetCollectionData(currentCollection.id, currentCollection.name);
                }
                else if (isJumbleSale)
                {
                        panelOpener.popup.GetComponent<PanelCardsToPublish>().CallInfo(currentCollection.id, currentCollection.name);
                    panelOpener.popup.GetComponent<PanelCardsToPublish>().OnConfirmedPublish += () => { OnConfirmedPublish?.Invoke(); };
                }
                if(!isHighlight)
                {
                    if (isJumbleSale)
                    {
                        panelOpener.popup.GetComponent<PanelSubCollectionsToPublish>().SpinnerClosing();
                    }
                    else panelOpener.popup.GetComponent<PanelAñadirEquipoLiga>().SpinnerClosing();
                }
              
            }
            else
            {
                if (type != ItemType.HIGTHLIGHT)
                {
                    if ( isJumbleSale)panelOpener.popup.GetComponent<PanelCardsToPublish>().CallInfoC(currentCollection.id,currentCollection.name); 
                    else panelOpener.popup.GetComponent<PanelAñadirEquipo>().CallInfoC(currentCollection.id,currentCollection.name);  
                }
                else
                {
                   if(isJumbleSale) panelOpener.popup.GetComponent<PanelCardsToPublish>().CallInfoCHighLight(currentCollection.id,currentCollection.name);   
                    else panelOpener.popup.GetComponent<PanelAñadirEquipo>().CallInfoCHighLight(currentCollection.id,currentCollection.name);    
                }
            }
        }

        /// <summary>
        /// Oculta el panel de carga
        /// </summary>
        private void ClosedSpinner()
        {
            GameObject spinner = GameObject.Find(spinner_name);
            for (int i = 0; i < spinner.transform.childCount; i++)
            {
                spinner.transform.GetChild(i).gameObject.SetActive(false);
            }
        }

        #endregion

    }
}
