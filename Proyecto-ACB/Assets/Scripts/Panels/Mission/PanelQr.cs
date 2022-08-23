using Data;
using WebAPI;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

/// <summary>
/// Controla los datos y funcionamiento del panel QR
/// </summary>
public class PanelQr : Panel
{
	[Tooltip("Clase encargada de la decodificación del código QR")]
	public QRCodeDecodeController e_qrController;

	[Tooltip("Botón de reseteo de escaneo de código QR")]
	public GameObject resetBtn;

	[Tooltip("Linea visual de escaneo de QR")]
	public GameObject scanLineObj;

	[SerializeField] [Tooltip("Botón que se encarga del cerrado del panel")]
	private Button exitButton;

	[HideInInspector] 
	public MissionsData.MissionItemData currentMissionData; //Datos de la misión actual (la seleccionada por el jugador)

    #region Unity Methods

    private void Awake()
    {
		GetComponent<QRCodeDecodeController>().onQRScanFinished.AddListener(qrScanFinished);
	}
    /// <summary>
    /// Se ejecuta cuando el panel ha sido iniciado por primera vez en escena, configurando el botón de cerrado de panel
    /// </summary>
    private void Start()
    {
	    exitButton.onClick.AddListener(() => { CloseQRPanel(); });
    }
    
    #endregion

    #region Public Methods

	/// <summary>
	/// Se llama una vez el QR ha sido leido y abre el panel de misión completada
	/// </summary>
	/// <param name="dataText"></param>
    public void qrScanFinished(string dataText)
	{
		MissionBody scan = new MissionBody()
		{
			mission_id = currentMissionData.id,
			uuid_r = dataText,
		};
		Debug.Log(JsonConvert.SerializeObject(scan));
		ACBSingleton.Instance.ActivateMainSpinner(true);
		WebProcedure.Instance.PostSaveMissionComplete(JsonConvert.SerializeObject(scan), OnSuccess, OnFailed);
	}

    #endregion

    #region Inner Methods

	/// <summary>
	/// Detiene la lectura del código QR
	/// </summary>
	private void Stop()
	{
		if (e_qrController != null)
		{
			e_qrController.StopWork();
		}

		if (resetBtn != null)
		{
			resetBtn.SetActive(false);
		}
		if (scanLineObj != null)
		{
			scanLineObj.SetActive(false);
		}
	}

	/// <summary>
	/// Método que regresa backend cuando un QR ha fallado en la lectura
	/// </summary>
	/// <param name="obj">Clase con los datos del error de lectura</param>
    private void OnFailed(WebError obj)
	{
		ACBSingleton.Instance.AlertPanel.SetupPanel("Hubo un error, por favor intenta nuevamente", "", false, CloseQRPanel);
		e_qrController.StopWork();
	}

	/// <summary>
	/// Método que se ejecuta cuando la lectura del QR ha sido exitosa, verifica los datos traidos desde backend
	/// y actualiza las analíticas
	/// </summary>
	/// <param name="obj"></param>
	private void OnSuccess(DataSnapshot obj)
	{
		Debug.Log(obj.RawJson);
		MissionRewardData cached = new MissionRewardData();
		JsonConvert.PopulateObject(obj.RawJson, cached);

		ACBSingleton.Instance.ActivateMainSpinner(false);

		if (cached.code == 200)
        {
			JsonConvert.PopulateObject(obj.RawJson, ACBSingleton.Instance.AccountData);
			ACBSingleton.Instance.RewardPanel.SetMissionRewardToOpen(cached, null);
			CloseQRPanel();
			Firebase.Analytics.FirebaseAnalytics.LogEvent("mission_ok");
			Debug.Log("Analytic mission_ok logged");
		}
		else
        {
			ACBSingleton.Instance.AlertPanel.SetupPanel(cached.message, "", false, CloseQRPanel);
			e_qrController.StopWork();
        }
	}

	/// <summary>
	/// Detiene la lectura del código QR y cierra el panel
	/// </summary>
	public void CloseQRPanel()
    {
	    e_qrController.StopWork();
	    Close();
    }

    #endregion
}
