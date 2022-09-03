using System.Collections.Generic;
using Data;
using WebAPI;
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

/// <summary>
/// Controla el minipanel específico de compra de ACBall
/// </summary>
public class BuyMiniPanelACBall : BuyMiniPanel
{
    #region Fields and Properties

    [Header("ACBall UI components")]
    [SerializeField] [Tooltip("Prefab de los premios internos de la ACBall")]
    private GameObject rewardImagePrefab;
    [SerializeField] [Tooltip("Objeto contenedor de los premios dentro de la ACBall")]
    private RectTransform imagePrefabContainer;

    private MallContainerData.MallData.MallItemsAcBall acballData; //Datos de la acball a comprar
    private List<GameObject> acballPossibleRewardsList; //Lista de premios dentro de la ACBall posibles

    #endregion

    #region Public Methods

    /// <summary>
    /// Activa el minipanel a nivel general
    /// </summary>
    /// <param name="productSprite">Imagen del producto a comprar</param>
    /// <param name="itemData">Datos del producto</param>
    /// <param name="description">Descripción del producto</param>
    /// <param name="onFailedLoading">Acción que se ejecuta cuando la carga del producto falla</param>
    public override void ShowMiniPanel(Sprite productSprite, MallContainerData.MallData.MallItems itemData, string description, Action onFailedLoading)
    {
        base.ShowMiniPanel(productSprite, itemData, description, onFailedLoading);

        if (itemData.random)
            return;

        acballPossibleRewardsList = new List<GameObject>();
        acballData = new MallContainerData.MallData.MallItemsAcBall();
        WebProcedure.Instance.GetPostMallElement(itemData.id.ToString(), OnSuccessLoadingItemData, (error) => { ACBSingleton.Instance.AlertPanel.SetupPanel(error.Message, "", false, onFailedLoading); });
    }   
    /// <summary>
    /// Activa el minipanel a nivel general
    /// </summary>
    /// <param name="productSprite">Imagen del producto a comprar</param>
    /// <param name="itemData">Datos del producto</param>
    /// <param name="description">Descripción del producto</param>
    /// <param name="onFailedLoading">Acción que se ejecuta cuando la carga del producto falla</param>
    public override void ShowMiniPanel(Sprite productSprite, JumbleSaleResult.JumbleItems itemData, string description, Action onFailedLoading)
    {
        base.ShowMiniPanel(productSprite, itemData, description, onFailedLoading);


        acballPossibleRewardsList = new List<GameObject>();
        acballData = new MallContainerData.MallData.MallItemsAcBall();
        WebProcedure.Instance.GetJumbleSaleInfoItem(itemData.id.ToString(), OnSuccessLoadingItemDataJumbleSale, (error) => { ACBSingleton.Instance.AlertPanel.SetupPanel(error.Message, "", false, onFailedLoading); });
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Método que se ejecuta cuando se ha cargado la imagen de la ACBall exitosamente
    /// </summary>
    /// <param name="obj">Datos de la ACBall cargados</param>
    private void OnSuccessLoadingItemData(DataSnapshot obj)
    {
        JsonConvert.PopulateObject(obj.RawJson, acballData);
        Debug.Log($"ACBall data = { obj.RawJson}");

        imagePrefabContainer.sizeDelta = new Vector2(0, imagePrefabContainer.sizeDelta.y);

        foreach (var url in from assets in acballData.element.assets.Keys
                            select acballData.element.assets.ContainsKey(assets) ? acballData.element.assets[assets] : null)
        {
            if (!string.IsNullOrEmpty(url))
            {
                GameObject rewardImage = Instantiate(rewardImagePrefab, imagePrefabContainer);
                rewardImage.GetComponent<ACBallReward>().SetupRewardImage(url);
                acballPossibleRewardsList.Add(rewardImage);
                imagePrefabContainer.sizeDelta += new Vector2(rewardImage.GetComponent<LayoutElement>().preferredWidth, 0);
            }
        }

        int buttonPos = 0;

        foreach (var title in from tit_elements in acballData.element.tit_elements.Keys
                              select acballData.element.tit_elements.ContainsKey(tit_elements) ? acballData.element.tit_elements[tit_elements] : null)
        {
            if (!string.IsNullOrEmpty(title))
            {
                acballPossibleRewardsList[buttonPos].GetComponentInChildren<Button>().onClick.AddListener(() => { descriptionText.text = title; });
                buttonPos++;
            }
        }


    }
    /// <summary>
    /// Método que se ejecuta cuando se ha cargado la imagen de la ACBall exitosamente en el mercadillo
    /// </summary>
    /// <param name="obj">Datos de la ACBall cargados</param>
    private void OnSuccessLoadingItemDataJumbleSale(DataSnapshot obj)
    {
      
        JumbleSaleResult.JumbleItemData acballJumbleData = new JumbleSaleResult.JumbleItemData();
        JsonConvert.PopulateObject(obj.RawJson, acballJumbleData);
        Debug.Log($"ACBall data = { obj.RawJson}");

        imagePrefabContainer.sizeDelta = new Vector2(0, imagePrefabContainer.sizeDelta.y);

        foreach (var url in from assets in acballJumbleData.data.assets.Keys
                            select acballJumbleData.data.assets.ContainsKey(assets) ? acballJumbleData.data.assets[assets] : null)
        {
            if (!string.IsNullOrEmpty(url))
            {
                GameObject rewardImage = Instantiate(rewardImagePrefab, imagePrefabContainer);
                rewardImage.GetComponent<ACBallReward>().SetupRewardImage(url);
                acballPossibleRewardsList.Add(rewardImage);
                imagePrefabContainer.sizeDelta += new Vector2(rewardImage.GetComponent<LayoutElement>().preferredWidth, 0);
            }
        }

        int buttonPos = 0;

        foreach (var title in from tit_elements in acballJumbleData.data.tit_elements.Keys
                              select acballJumbleData.data.tit_elements.ContainsKey(tit_elements) ? acballJumbleData.data.tit_elements[tit_elements] : null)
        {
            if (!string.IsNullOrEmpty(title))
            {
                acballPossibleRewardsList[buttonPos].GetComponentInChildren<Button>().onClick.AddListener(() => { if (descriptionText) { descriptionText.text = title; } });
                buttonPos++;
            }
        }
    }


    #endregion

}
