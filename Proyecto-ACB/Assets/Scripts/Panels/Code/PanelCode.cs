using Data;
using WebAPI;
using UnityEngine;
using UnityEngine.UI;
using Newtonsoft.Json;

public class PanelCode : Panel
{

	[HideInInspector]
	public MissionsData.MissionItemData currentMissionData;

	
    #region Public Methods

    public void CodeKeyFinished(string dataText)
	{
		MissionBody scan = new MissionBody()
		{
			mission_id = currentMissionData.id,
			keyword = dataText,
		};
		Debug.Log(JsonConvert.SerializeObject(scan));
		ACBSingleton.Instance.ActivateMainSpinner(true);
		WebProcedure.Instance.PostSaveMissionComplete(JsonConvert.SerializeObject(scan), OnSuccess, OnFailed);
	}
    
    #endregion

    #region Inner Methods

    private void OnFailed(WebError obj)
	{
		ACBSingleton.Instance.AlertPanel.SetupPanel("Hubo un error, por favor intenta nuevamente", "", false, CloseCodePanel);
	}

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
			CloseCodePanel();
			Firebase.Analytics.FirebaseAnalytics.LogEvent("mission_ok");
			Debug.Log("Analytic mission_ok logged");
		}
		else
        {
			ACBSingleton.Instance.AlertPanel.SetupPanel(cached.message, "", false, CloseCodePanel);
        }
	}

	private void CloseCodePanel()
    {
	    Close();
    }

    #endregion
}
