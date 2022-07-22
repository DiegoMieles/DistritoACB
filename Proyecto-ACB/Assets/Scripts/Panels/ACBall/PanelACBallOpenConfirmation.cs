using Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using WebAPI;

/// <summary>
/// Panel encargado del control de apertura y mostrado de datos que contiene una ACBall en específico
/// </summary>
public class PanelACBallOpenConfirmation : Panel
{
    [Header("Panel components")]
    [SerializeField] [Tooltip("Botón que se encarga del cerrado del panel")]
    private Button closePanelButton;
    [SerializeField] [Tooltip("Botón de apertura de la ACBall")]
    private Button openACBallButton;
    [SerializeField] [Tooltip("Imagen de la ACBall a abrir")]
    private Image acballImage;
    [SerializeField] [Tooltip("Prefab de objeto que se puede mostrar como uno de los items que puede contener una ACBall")]
    private GameObject rewardImagePrefab;
    [SerializeField] [Tooltip("Contenedor de las imagenes de los objetos que puede contener una ACBall")]
    private RectTransform imagePrefabContainer;
    [SerializeField] [Tooltip("Texto con el valor de costo de abrir la ACBall")]
    private Text acballCostText;
    [SerializeField] [Tooltip("Texto con la cantidad actual de monedas del jugador")]
    private Text actualACBCoins;
    [SerializeField] [Tooltip("Valor de máximo de monedas que se puede mostrar en pantalla, en caso de excederse muestra el valor límite únicamente a nivel gráfico y no lógico")]
    private float limit;
    [SerializeField] [Tooltip("Texto que contiene los nombres de los items que pueden salir dentro de la ACBall")]
    private Text innerRewardsName;
    [SerializeField] [Tooltip("Clase con los datos de los items dentro de la ACBall")]
    private AcbBallContainer.AcbBallsData.AcBallsItems acballItemData;
    [SerializeField] [Tooltip("Referencia al Spinner de carga")]
    private GameObject spinner;

    private List<GameObject> acballPossibleRewardsList; //Lista de los posibles items que puede contener una ACBall
    private Action onFinishedOpeningACBall; //Acción que se llama al terminar de abrir una ACBall

    #region Public Methods

    /// <summary>
    /// Configura el panel con los datos de la ACBall que se quiere abrir
    /// </summary>
    /// <param name="acballItemData">Datos de los items de ACBall a mostrar</param>
    /// <param name="onFinishedOpeningACBall">Método que se llama al terminar de abrir un ACBall</param>
    /// <param name="onGoBack">"Método que se ejecuta al cerrar el panel"</param>
    public void SetupPanel(AcbBallContainer.AcbBallsData.AcBallsItems acballItemData, Action onFinishedOpeningACBall, Action onGoBack)
    {
        this.onFinishedOpeningACBall = onFinishedOpeningACBall;
        acballPossibleRewardsList = new List<GameObject>();
        Debug.Log(JsonConvert.SerializeObject(acballItemData));
        closePanelButton.onClick.AddListener(() => { onGoBack?.Invoke(); Close(); });
        acballCostText.text = acballItemData.cost.ToString();
        this.acballItemData = acballItemData;
        WebProcedure.Instance.GetSprite(acballItemData.path_img, OnSuccessLoadingACBallImage, OnFailedLoadingACBallImage);
    }

    #endregion

    #region Inner Methods

    /// <summary>
    /// Método que se ejecuta al cargar la imagen de la ACBall de forma satisfactoria, carga igualmente la imagen de los items de la ACBall 
    /// y actualiza los datos de la ACBall a abrir
    /// </summary>
    /// <param name="obj">Imagen de la ACBall cargada</param>
    private void OnSuccessLoadingACBallImage(Sprite obj)
    {
        acballImage.sprite = obj;
        openACBallButton.onClick.AddListener(OpenACBallRewardPanel);
        actualACBCoins.text = Mathf.Clamp(ACBSingleton.Instance.AccountData.statsData.coinsBalance, 0, limit).ToString();

        imagePrefabContainer.sizeDelta = new Vector2(0, imagePrefabContainer.sizeDelta.y);

        foreach (var url in from assets in acballItemData.assets.Keys
                            select acballItemData.assets.ContainsKey(assets) ? acballItemData.assets[assets] : null)
        {
            if(!string.IsNullOrEmpty(url))
            {
                GameObject rewardImage = Instantiate(rewardImagePrefab, imagePrefabContainer);
                rewardImage.GetComponent<ACBallReward>().SetupRewardImage(url);
                acballPossibleRewardsList.Add(rewardImage);
                imagePrefabContainer.sizeDelta += new Vector2(rewardImage.GetComponent<LayoutElement>().preferredWidth, 0);
            }
        }

        int buttonPos = 0;

        foreach (var title in from tit_elements in acballItemData.tit_elements.Keys
                            select acballItemData.tit_elements.ContainsKey(tit_elements) ? acballItemData.tit_elements[tit_elements] : null)
        {
            if(!string.IsNullOrEmpty(title))
            {
                acballPossibleRewardsList[buttonPos].GetComponentInChildren<Button>().onClick.AddListener(() => { innerRewardsName.text = title; });
                buttonPos++;
            }
        }
    }

    /// <summary>
    /// Método que se llama al no poder cargar la imagen desde backend
    /// </summary>
    /// <param name="obj">Clase con los datos de error</param>
    private void OnFailedLoadingACBallImage(WebError obj)
    {
        ACBSingleton.Instance.AlertPanel.SetupPanel("Algo ha salido mal, vuelve al menú anterior y selecciona nuevamente la acball", "", false, Close);
    }

    /// <summary>
    /// Controla la apertura del panel que muestra la recompensa dentro de la ACBall
    /// </summary>
    private void OpenACBallRewardPanel()
    {
        spinner.gameObject.SetActive(true);
        if (ACBSingleton.Instance.AccountData.statsData.coinsBalance < acballItemData.quantity)
        {
            ACBSingleton.Instance.AlertPanel.SetupPanel("No tienes suficiente dinero", "vuelve cuando tengas más monedas", false, () => { spinner.gameObject.SetActive(false); Close(); });
            return;
        }

        AcbBallContainer.AcBallBody acballBodyInfo = new AcbBallContainer.AcBallBody() { acball_id = acballItemData.id.ToString() };
        Debug.Log(acballBodyInfo);
        ACBSingleton.Instance.AlertPanel.SetupPanel("Confirmación de transacción", "Esta operación te costará " + acballItemData.cost.ToString() + " acbcoins", true,
            () => WebProcedure.Instance.PostOpenACBall(JsonConvert.SerializeObject(acballBodyInfo), OnSuccessOpeningACBall, OnFailedOpeningACBall), () => { spinner.gameObject.SetActive(false); });
    }

    /// <summary>
    /// Método que se llama desde backend que actualiza los datos del jugador con respecto a sus monedas y ACBalls al abrirla
    /// </summary>
    /// <param name="obj">Clase con los datos devueltos de backend de la ACBall abierta</param>
    private void OnSuccessOpeningACBall(DataSnapshot obj)
    {
        AcbBallContainer.AcbBallReward reward = new AcbBallContainer.AcbBallReward();
        Debug.Log(obj.RawJson);
        JsonConvert.PopulateObject(obj.RawJson, ACBSingleton.Instance.AccountData);
        JsonConvert.PopulateObject(obj.RawJson, reward);
        ACBSingleton.Instance.RewardPanel.SetACBallOpen(acballItemData, reward, onFinishedOpeningACBall, Close);
        Firebase.Analytics.FirebaseAnalytics.LogEvent("open_acball");
        Debug.Log("Analytic open_acball logged");
    }

    /// <summary>
    /// Método que se llama al fallar la apertura de una ACBall en backend
    /// </summary>
    /// <param name="obj">Clase con los datos de los errores de la ACBall</param>
    private void OnFailedOpeningACBall(WebError obj)
    {
        ACBSingleton.Instance.AlertPanel.SetupPanel("Algo ha salido mal, por favor intenta abrir la acball nuevamente", "", false, Close);
    }

    /// <summary>
    /// Desactiva el spinner de carga
    /// </summary>
    public void ClosingSpinner()
    {
        spinner.gameObject.SetActive(false);
    }

    #endregion
}
