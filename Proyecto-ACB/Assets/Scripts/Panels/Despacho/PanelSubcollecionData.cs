using Data;
using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using WebAPI;

namespace Panels
{
    /// <summary>
    /// Controla el panel con los datos de subcolecciones
    /// </summary>
    public class PanelSubcollecionData : MonoBehaviour
    {
        #region Fields and Properties
        //Evento que se dispara al completar la publicación en el mercadillo
        public event PanelConfirmPublish.VoidDelegate OnConfirmedPublish;
        [Header("Button components")]
        [SerializeField] [Tooltip("Botón de subcolecciones")]
        protected Button button;
        [SerializeField] [Tooltip("Imagen de las subcolecciones")]
        protected Image image;
        [SerializeField] [Tooltip("Nombre de las subcolecciones")]
        protected Text textConcept;
        [SerializeField] [Tooltip("Se ejecuta cuando la traida de datos es fallida")]
        protected UnityEvent onFailed;
        [SerializeField] [Tooltip("Datos de subcolecciones")]
        protected SubCollectionData.SubCollectionItemData currentSubcollection;

        [Space(5)]
        [Header("Panel opener components")]
        [SerializeField] [Tooltip("Clase que controla la apertura de nuevos paneles a mostrar")]
        protected PanelOpener panelOpener;
        [SerializeField] [Tooltip("Panel de añadir al equipo competitivo")]
        protected GameObject panelAddTeamPrefab;

        [SerializeField] [Tooltip("Nombre del spinner de carga")]
        protected string spinner_name;

        protected Action onClickedButton; //Acción que se ejecuta al hacer click al botón

        #endregion

        #region Public Methods

        /// <summary>
        /// Configura y muestra la información de la subcolección
        /// </summary>
        /// <param name="subcollecionData">Datos de la subcolección</param>
        /// <param name="onClickedButton">Acción que se ejecuta al hacer click al botón</param>
        public void ShowInfo(SubCollectionData.SubCollectionItemData subcollecionData, Action onClickedButton)
        {
            this.onClickedButton = onClickedButton;
            currentSubcollection = subcollecionData;
            textConcept.text = currentSubcollection.name;

            if (!string.IsNullOrEmpty(currentSubcollection.pathImg))
            {
                WebProcedure.Instance.GetSprite(currentSubcollection.pathImg,sprite =>
                {
                    if (image)
                    {
                        image.sprite = sprite;
                        ClosedSpinner();
                    }
                    else
                    {
                        ClosedSpinner();
                    }
                    
                    if (subcollecionData.type != ItemType.HIGTHLIGHT || subcollecionData.type != ItemType.H)
                    {
                        button.onClick.AddListener(OpenAddTeamPanel);
                    }
                    else
                    {
                        button.onClick.AddListener(OpenAddTeamHightLightPanel);
                    }
          
                }, error =>
                {
                    onFailed.Invoke();
                    ClosedSpinner();
                }
                );
            }
            else
            {
                ClosedSpinner();
            }
        }

        #endregion

        #region Inner Methods

        /// <summary>
        /// Abre panel de añadir carta al equipo competitivo
        /// </summary>
        protected virtual void OpenAddTeamPanel()
        {
            panelOpener.popupPrefab = panelAddTeamPrefab;
            panelOpener.OpenPopup();
            panelOpener.popup.GetComponent<PanelAñadirEquipo>().CallInfo(currentSubcollection.id,currentSubcollection.name );
            onClickedButton?.Invoke();
        }

        // <summary>
        /// Abre panel de highlight
        /// </summary>
        protected void OpenAddTeamHightLightPanel()
        {
            panelOpener.popupPrefab = panelAddTeamPrefab;
            panelOpener.OpenPopup();
            panelOpener.popup.GetComponent<PanelAñadirEquipo>().CallInfoCHighLightSub(currentSubcollection.id,currentSubcollection.name);   
            onClickedButton?.Invoke();
        }

        /// <summary>
        /// Oculta el spinner de carga
        /// </summary>
        protected void ClosedSpinner()
        {
            GameObject spinner = GameObject.Find(spinner_name);
            if (spinner)
            {
                for (int i = 0; i < spinner.transform.childCount; i++)
                {
                    spinner.transform.GetChild(i).gameObject.SetActive(false);
                }
            }
    
        }
        /// <summary>
        /// Invoca el evento OnConfirmedPublish
        /// </summary>
        protected void OnConfirmedPublishEvent()
        {
            OnConfirmedPublish?.Invoke();
        }
        #endregion

    }
}

