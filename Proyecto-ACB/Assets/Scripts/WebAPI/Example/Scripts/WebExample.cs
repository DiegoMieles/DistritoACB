using Data;
using UnityEngine;
using UnityEngine.UI;

namespace WebAPI.Example.Scripts
{
    public class WebExample : MonoBehaviour
    {
        public Image images;

        public string strTest;
        public PanelReward rewardPanel;
        public MissionRewardData cached;

        private void Start()
        { 
            WebProcedure.Instance.GetLoadUserData("StubToken",OnSuccess,OnFailed );
        }

        private  void OnFailed(WebError obj)
        {
            Debug.Log(obj.Message);
        }

        private  void OnSuccess(Sprite obj)
        {
            images.sprite = obj;
        }
        
        private  void OnSuccess(DataSnapshot obj)
        {
            Debug.Log(obj.RawJson);
            Debug.Log(cached.ToString());
        }
    }
}
