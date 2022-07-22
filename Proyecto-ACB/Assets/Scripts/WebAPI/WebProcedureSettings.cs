using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
#endif

namespace WebAPI
{
    [CreateAssetMenu(fileName = "ScriptableWebSettings", menuName = "ScriptableObjects/ScriptableWebSettings", order = 1)]
    class WebProcedureSettings : ScriptableObject
    {
        [Header("SETTINGS")]
        public string Ip = string.Empty;
        public bool ShowDebug = true;
        public static WebProcedureSettings Instance => Resources.Load<WebProcedureSettings>("Scriptables/ScriptableWebSettings");
        
#if UNITY_EDITOR
        [MenuItem("WebAPI/Settings")]
        private static void Select()
        {
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = Instance;
        }
#endif
   
    }
}