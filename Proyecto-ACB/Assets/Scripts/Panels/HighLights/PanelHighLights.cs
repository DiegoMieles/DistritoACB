using System;
using Data;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;

namespace Panels
{
    /// <summary>
    /// Controla el panel de las cartas tipo highlight
    /// </summary>
    public class PanelHighLights : Panel
    {
        [SerializeField] [Tooltip("Texto del titulo frontal del highlight")]
        private Text textTittleFront;
        [SerializeField] [Tooltip("Texto del titulo reverso del highlight")]
        private Text textTittleBack;
        [SerializeField] [Tooltip("Texto de descripción del highlight")]
        private Text textDescription;
 
        [SerializeField] [Tooltip("Imagen de highlight frontal")]
        private Image imageFront;
        [SerializeField] [Tooltip("Imagen de highlight reverso")]
        private Image imageBack;
        [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
        private PanelOpener panelOpener;
        [SerializeField] [Tooltip("Prefab del panel de videos")]
        private GameObject panelVideo;
        [SerializeField] [Tooltip("Datos del highlight")]
        private HighLightData.HigthlightItems currentHigthlightItems;
        [SerializeField] [Tooltip("Spinner de carga")]
        private GameObject spinner;
        [SerializeField] [Tooltip("Prefab del objeto arrastrable")]
        private GameObject HexDragTemplate;
        [SerializeField] [Tooltip("Botón de girar la carta")]
        private Button flipCardButton;
        [SerializeField] [Tooltip("Grupo contenedor de los highlights")]
        private GridLayoutGroup gridLayoutGroupHightLight;
        [SerializeField] [Tooltip("Grupo contenedor de los highlights")]
        private HigthlightContainer2 higthlightContainer;
        [SerializeField] [Tooltip("Objeto arrastrable de highlight")]
        private PanelTokenItemToggle panelCardItem;
        [Space(10)]
        [Header("Dragable reference")]
        [SerializeField] [Tooltip("Posición final de los arrastrables de highlight")]
        private GameObject dragableTargetPosition;
        [Space(10)]
        [Header("HightLight Part")]
        [SerializeField] [Tooltip("Vista frontal del highlight")]
        private GameObject frontHight;
        [SerializeField] [Tooltip("Vista reversa del highlight")]
        private GameObject backHight;
        
        private PanelTokenItemToggle draggedToogle; //Objeto arrastrable seleccionado
        private int dragablesCount; //Contador de objetos arrastrables disponibles

        /// <summary>
        /// Configura el highlight a nivel gráfico y lógico
        /// </summary>
        /// <param name="higthlightItems">Datos de los highlight</param>
        public void Setup(HighLightData.HigthlightItems higthlightItems)
        {
            HexDragTemplate.gameObject.SetActive(true);
            flipCardButton.interactable = false;
            currentHigthlightItems = higthlightItems;
            textTittleFront.text = currentHigthlightItems.title;
            textTittleBack.text = currentHigthlightItems.title;
            textDescription.text = currentHigthlightItems.description;
            
            var  bodyHightLightContiner = new BodyHightLight() {higthlight_id = higthlightItems.id};
            var json = JsonConvert.SerializeObject(bodyHightLightContiner);
            
            
            Debug.Log(json);
            WebProcedure.Instance.GetHigthlightsTokenCL(json, snapshot =>
            {
                Debug.Log(snapshot.RawJson);
                higthlightContainer = new HigthlightContainer2();
                JsonConvert.PopulateObject(snapshot.RawJson, higthlightContainer);
                
                if (gridLayoutGroupHightLight.transform.childCount > 0)
                {
                    for (int i = 0; i < gridLayoutGroupHightLight.transform.childCount; i++)
                    {
                        Destroy(gridLayoutGroupHightLight.transform.GetChild(i).gameObject);
                    }
                }

                if (higthlightContainer.tokenData.tokenItems != null)
                {
                    dragablesCount = 0;
                    foreach (var transactionData in higthlightContainer.tokenData.tokenItems)
                    {
                        var prefab= Instantiate(panelCardItem, gridLayoutGroupHightLight.transform);

                        if (dragableTargetPosition != null)
                        {
                            gridLayoutGroupHightLight.GetComponent<RectTransform>().sizeDelta = new Vector2((prefab.GetComponent<RectTransform>().sizeDelta.x + 60) * higthlightContainer.tokenData.tokenItems.Count, gridLayoutGroupHightLight.GetComponent<RectTransform>().sizeDelta.y);
                            prefab.GetComponent<RectTransform>().anchoredPosition = new Vector2(((prefab.GetComponent<RectTransform>().sizeDelta.x + 60) * dragablesCount) + (prefab.GetComponent<RectTransform>().sizeDelta.x / 2) + 60, 0);
                            prefab.ShowInfo(transactionData, dragableTargetPosition, () => { UpdateHightLightView(transactionData, prefab); });
                            dragablesCount++;
                        }
                        else
                        {
                            prefab.ShowInfo(transactionData, null, null, null );
                        }
                   
   
                    } 
                }
                else
                {
                    CloseSpinner();
                }
            }, error =>
            {
                CloseSpinner();
            } );
        }

        /// <summary>
        /// Abre el panel de video
        /// </summary>
        public void OpenVideo()
        {
            if (currentHigthlightItems != null)
            {
                panelOpener.popupPrefab = panelVideo;
                panelOpener.OpenPopup();
                panelOpener.popup.GetComponent<PanelVideo>().PlayVideo(currentHigthlightItems.urlvideo);
            }
        }

        /// <summary>
        /// Oculta spinner de carga
        /// </summary>
        private void CloseSpinner()
        {
            spinner.gameObject.SetActive(false);
        }

        /// <summary>
        /// Muestra el spinner de carga
        /// </summary>
        private void ShowSpinner()
        {
            spinner.gameObject.SetActive(true);
        }
        
        /// <summary>
        /// Actualiza la vista y datos de highlight de acuerdo al objeto arrastrable
        /// </summary>
        /// <param name="cardNewData">Datos de los highlights</param>
        /// <param name="draggedToogle">Objeto arrastrable</param>
        private void UpdateHightLightView(HighLightData.HigthlightItems cardNewData, PanelTokenItemToggle draggedToogle)
        {
            flipCardButton.interactable = false;
            HexDragTemplate.gameObject.SetActive(false);
            frontHight.gameObject.gameObject.SetActive(true);
            backHight.gameObject.gameObject.SetActive(false);
            ShowSpinner();
            if (!string.IsNullOrEmpty(cardNewData.pathImgCol))
            {
                WebProcedure.Instance.GetSprite(cardNewData.pathImgCol, sprite =>
                {
                    imageBack.sprite = sprite;
                }, error =>
                {
                    Debug.Log("Error");
                });
            }
            
            if (!string.IsNullOrEmpty(cardNewData.pathImg))
            {
                WebProcedure.Instance.GetSprite(cardNewData.pathImg, sprite =>
                {
                    flipCardButton.interactable = true;
                    imageFront.sprite = sprite;
                    CloseSpinner();

                }, error =>
                {
                    Debug.Log("Error");
                    CloseSpinner();
                });
            }
            
            if (this.draggedToogle != null)
                this.draggedToogle.Dragable.ResetDragable();
            
            gridLayoutGroupHightLight.CalculateLayoutInputHorizontal();
        
            this.draggedToogle = draggedToogle;
        }
        
    }
}

