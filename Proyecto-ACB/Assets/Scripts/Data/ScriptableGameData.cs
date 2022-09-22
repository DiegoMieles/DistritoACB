#if UNITY_EDITOR
using UnityEditor.Build;
using UnityEditor.Build.Reporting;
#endif
using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using UnityEngine;

namespace Data
{
    /// <summary>
    /// Objeto que almacena los datos principales de la partida
    /// </summary>
    [CreateAssetMenu(fileName = "ScriptableGameData", menuName = "ScriptableObjects/ScriptableGameData", order = 1)]
    public class ScriptableGameData : ScriptableObject
    {
        private const string SCRIPTABLEGAMEDATA = "Scriptables/ScriptableGameData";
        public int hours;
        public string country;
        public int costChallenge;
        public App app;
        public MainMenuData mainMenuData;

        
        private static ScriptableGameData _cached = null;

        public static ScriptableGameData Get()
        {
            if (_cached == null)
            {
                _cached = Resources.Load<ScriptableGameData>(SCRIPTABLEGAMEDATA);
            }

            return _cached;
        }

        public void Clear()
        {
            mainMenuData = new MainMenuData();
        }

    }

    /// <summary>
    /// Enumerador de los tipos de item que se pueden obtener en el juego
    /// </summary>
    public enum ItemType
    {
        NONE,
        ACBALL,
        BODYACCESORY,
        HEADACCESORY,
        ARMACCESORY,
        EYEACCESORY,
        BACKGROUNDACCESORY,
        FOREGROUNDACCESORY,
        SKINCOLOR,
        FACEFORM,
        EYES,
        EAR,
        NOSE,
        MOUTH,
        EYEBROW,
        HAIRSTYLE,
        TOKEN,
        BODYPART,
        FACIALHAIR,
        BODYFORM,
        SHIRTDEFAULT,
        HAIRSTYLEBACK,
        HAIRCOLOR,
        BOOSTER,
        COINS, 
        SKIN,
        HIGTHLIGHT,
        SUBCOLECTION,
            CH,
            H
    }

    /// <summary>
    /// Datos generales de los items disponibles para el jugador
    /// </summary>
    [Serializable]
    public class ItemData
    {
        public int id;
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemType itemType;
        [JsonConverter(typeof(StringEnumConverter))]
        public ItemType type;
        public string title;
        public string description;
        public string path_img;
        public string img_show;
        public bool on_market;
        private const string SPRITEPATH = "SpriteItem/";
        public Sprite GetSprite()
        {
            return Resources.Load<Sprite>(SPRITEPATH + id);
        }
    }

    /// <summary>
    /// Datos generales de los items disponibles
    /// </summary>
    [Serializable]
    public class GeneralData
    {
        public List<ItemData> items;
    }
    /// <summary>
    /// Datos de las Skins que pueden venderse
    /// </summary>
    [Serializable]
    public class SkinsToSell
    {
        public List<ItemData> BODYACCESORY = new List<ItemData>();
        public List<ItemData> HEADACCESORY = new List<ItemData>();
        public List<ItemData> ARMACCESORY = new List<ItemData>();
        public List<ItemData> EYEACCESORY = new List<ItemData>();
        public List<ItemData> BACKGROUNDACCESORY = new List<ItemData>();
    }
    /// <summary>
    /// Enumerador que contiene la franja del día
    /// </summary>
    public enum TransitionTime
    {
        DAY,
        NIGHT
    }

    /// <summary>
    /// Datos principales de juego como horario del día e información de los edificios
    /// </summary>
    [Serializable] 
    public class MainMenuData
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public TransitionTime transitionTime;
        public List<BuildData> buildInfoData;
        
        [Serializable] 
        public class BuildData
        {
            public string id;
            public string title;
            [TextArea]
            public string info;
        }
    }

    /// <summary>
    /// Datos del editor de avatar
    /// </summary>
    [Serializable]
    public class AvatarEditorData
    {
        public AvatarMenuData avatarMenuData;
    }

    /// <summary>
    /// Dalos items de avatar gratis disponibles
    /// </summary>
    [Serializable]
    public class AvatarMenuData
    {
        public List<ItemData> itemsFree;
    }

    /// <summary>
    /// Datos generales de la app
    /// </summary>
    [Serializable]
    public class App
    {
        public string version;
        public string vers_android;
        public string vers_ios;
        public string text;
        public string url_android;
        public string url_ios;
    }

#if UNITY_EDITOR

    public class ResetScriptableGameDataOnBuild : IPreprocessBuildWithReport
    {
        public int callbackOrder
        {
            get { return 0; }
        }

        public void OnPreprocessBuild(BuildReport report)
        {
            Reset();
        }

        private void Reset()
        {
            ScriptableGameData.Get().Clear();
        }

    }
#endif
}