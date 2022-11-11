﻿#if UNITY_EDITOR
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
    /// Objeto que almacena los datos de la cuenta del jugador
    /// </summary>
    [CreateAssetMenu(fileName = "ScriptableAccount", menuName = "ScriptableObjects/ScriptableAccount", order = 2)]
    public class  ScriptableAccount : ScriptableObject
    {
        private const string SCRIPTABLEACCOUNT = "Scriptables/ScriptableAccount";
        public StatsData statsData = new StatsData();
        public AvatarData avatarData = new AvatarData();
        public StadiumData stadiumData = new StadiumData();
        public MissionsData missionsData = new MissionsData();
        private static ScriptableAccount _cached = null;

        public static ScriptableAccount Get()
        {

            if (_cached == null)
            {
                _cached = Resources.Load<ScriptableAccount>(SCRIPTABLEACCOUNT);
            }

            return _cached;
        }

        public void Clear()
        {
            statsData = new StatsData();
            avatarData = new AvatarData();
            stadiumData = new StadiumData();
            missionsData = new MissionsData();
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    /// <summary>
    /// Datos básicos del avatar
    /// </summary>
    [Serializable]
    public class AvatarConfigData
    {
        [Header("BODYPART")]
        public string nickName;
        public int skinColor;
        public int bodyForm;
        public int faceForm;
        public int eyes;
        public int ear;
        public int nose;
        public int mouth;
        public int eyeBrow;
        public int hairStyleFront;
        public int hairStyleBack;
        public int hairColor;
        public int facialHair;

        [Header("ACCESORIES")]
        public int bodyAccessory;
        public int headAccessory;
        public int armAccessory;
        public int eyesAccessory;

        [Header("ENVIROMENT")]
        public int backGround;
        public int foreGround;
        public bool isFirstTime;

        [Header("VIEW")]
        public string img_small;
        public string img_big;

    }

    /// <summary>
    /// Datos del listado de items de un avatar
    /// </summary>
    [Serializable]
    public class AvatarData : AvatarConfigData
    {

        [Header("ITEMS")]
        public List<ItemData> avatarItems = new List<ItemData>();


        public int GetDataByType(ItemType _itemType)
        {
            switch (_itemType)
            {
                case ItemType.BODYACCESORY:
                    return bodyAccessory;
                case ItemType.HEADACCESORY:
                    return headAccessory;
                case ItemType.ARMACCESORY:
                    return armAccessory;
                case ItemType.EYEACCESORY:
                    return eyesAccessory;
                case ItemType.BACKGROUNDACCESORY:
                    return backGround;
                case ItemType.FOREGROUNDACCESORY:
                    return foreGround;
                case ItemType.SKINCOLOR:
                    return skinColor;
                case ItemType.FACEFORM:
                    return faceForm;
                case ItemType.EYES:
                    return eyes;
                case ItemType.EAR:
                    return ear;
                case ItemType.NOSE:
                    return nose;
                case ItemType.MOUTH:
                    return mouth;
                case ItemType.EYEBROW:
                    return eyeBrow;
                case ItemType.HAIRSTYLE:
                    return hairStyleFront;
                case ItemType.FACIALHAIR:
                    return facialHair;
                case ItemType.BODYFORM:
                    return bodyForm;
                case ItemType.HAIRSTYLEBACK:
                    return hairStyleBack;
            }
            return -1;
        }
    }

    /// <summary>
    /// Datos de recolección de monedas y tiempos de recolección en cancha callejera
    /// </summary>
    [Serializable]
    public class StadiumData
    {
        public bool collectCoins;
        public long lastCollectTime;
        public long nextCollectTime;
    }

    /// <summary>
    /// Tipo de transacción dentro del banco
    /// </summary>
    public enum TransactionType
    {
        INGRESO,
        EGRESO
    }

    /// <summary>
    /// Clase contenedora del Json de los datos del banco del jugador
    /// </summary>
    [Serializable]
    public class TransactionContainer
    {
        public StatsData statsData = new StatsData();
        public TransactionData transactionData = new TransactionData();
    }

    /// <summary>
    /// Datos de transacción del jugador dentro del juego
    /// </summary>
    [Serializable]
    public class TransactionData
    {
        public List<TransactionItemData> transactionItems = new List<TransactionItemData>();
        [Serializable]
        public class TransactionItemData
        {
            public string date;
            [TextArea]
            public string concept;
            public int spend;
            public int balance;
            [JsonConverter(typeof(StringEnumConverter))]
            public TransactionType type;
        }

    }

    /// <summary>
    /// Datos de estadisticas base del jugador como por ejemplo la monedas y ACBall que tiene
    /// </summary>
    [Serializable]
    public class StatsData
    {
        public int coinsBalance;
        public int acballsBalance;
        public bool acballsNotification;
    }

    /// <summary>
    /// Datos de misiones disponibles para el jugador
    /// </summary>
    [Serializable]
    public class MissionsData
    {
        public enum MissionType
        {
            AR,
            QR,
            CODE
        }

        public List<MissionItemData> missionItems = new List<MissionItemData>();

        [Serializable]
        public class MissionItemData
        {
            public int id;
            public string name;
            [TextArea]
            public string info;
            public string url;
            public string img;
            [JsonConverter(typeof(StringEnumConverter))]
            public MissionType type;
            public int position;
            [JsonConverter(typeof(StringEnumConverter))]
            public ItemType rewardType;
            public string rewardTypeImg;
            public string uuid_r;
            public string lat;
            public string lon;
            public bool valGPS;

            private const string SPRITEPATH = "SpriteType/";

            public Sprite GetSprite()
            {
                return Resources.Load<Sprite>(SPRITEPATH + rewardType.ToString());
            }
        }

    }

    /// <summary>
    /// Datos de geolocalización de una misión
    /// </summary>
    [Serializable]
    public class MissionBody
    {
        public int mission_id;
        public string uuid_r;
        public string keyword;
        public float latitude;
        public float longitude;
    }

    /// <summary>
    /// Datos GPS de una misión
    /// </summary>
    [Serializable]
    public class GPSConfirmBody
    {
        public int mission_id;
        public string uuid_r;
        public float lat;
        public float lon;
    }

    /// <summary>
    /// Datos de recompensa dentro de una misión
    /// </summary>
    [Serializable]
    public class MissionReward
    {
        public string type;
        public string value;
    }

    /// <summary>
    /// Datos de misión previamente completada
    /// </summary>
    [Serializable]
    public class MissionAlreadyComplete
    {
        public int code;
        public string message;
    }

    /// <summary>
    /// Datos de recompensa de una misión
    /// </summary>
    [Serializable]
    public class MissionRewardData
    {
        public int code;
        public string message;
        public RewardData reward = new RewardData();
        public StatsData statsData = new StatsData();
        public AvatarData avatarData = new AvatarData();
        public TransactionData transactionData = new TransactionData();
        public MissionsData missionsData = new MissionsData();
        public StadiumData stadiumData = new StadiumData();
        public int balance;
        [Serializable, JsonConverter(typeof(MissionRewardConvert))]
        public class RewardData
        {
            public int mission_id;
            public string uuid_r;
            [JsonConverter(typeof(StringEnumConverter))]
            public ItemType rewardType;
            public string reward;
            public string img;

            [JsonConstructor]
            public RewardData()
            {
                rewardType = ItemType.NONE;
            }

            public RewardData ChangeType(ItemType types)
            {
                switch (types)
                {
                    case ItemType.COINS:
                        return this as RewardItemsCoin;

                    case ItemType.SKIN:
                        return this as RewardItemsAvatar;

                    case ItemType.TOKEN:
                        return this as RewardItemsToken;

                    case ItemType.HIGTHLIGHT:
                        return this as RewardItemsHighlight;

                    case ItemType.BOOSTER:
                        return this as RewardItemsBooster;

                    case ItemType.ACBALL:
                        return this as RewardItemsACBall;

                    default:
                        throw new Exception();
                }
            }

            [Serializable]
            public class RewardItemsCoin : RewardData
            {
                public AcbBallContainer.ACBallItemsCoin.ACBallCoinData element = new AcbBallContainer.ACBallItemsCoin.ACBallCoinData();

                [JsonConstructor]
                public RewardItemsCoin()
                {
                    rewardType = ItemType.COINS;
                }
            }

            [Serializable]
            public class RewardItemsAvatar : RewardData
            {
                public AcbBallContainer.ACBallItemsAvatar.ACBallSkinData element = new AcbBallContainer.ACBallItemsAvatar.ACBallSkinData();

                [JsonConstructor]
                public RewardItemsAvatar()
                {
                    rewardType = ItemType.SKIN;
                }
            }

            [Serializable]
            public class RewardItemsToken : RewardData
            {
                public AcbBallContainer.ACBallItemsToken.ACBallTokenData element = new AcbBallContainer.ACBallItemsToken.ACBallTokenData();

                [JsonConstructor]
                public RewardItemsToken()
                {
                    rewardType = ItemType.TOKEN;
                }
            }

            [Serializable]
            public class RewardItemsHighlight : RewardData
            {
                public AcbBallContainer.ACBallItemsHighlight.ACBallHighlightData element = new AcbBallContainer.ACBallItemsHighlight.ACBallHighlightData();

                [JsonConstructor]
                public RewardItemsHighlight()
                {
                    rewardType = ItemType.HIGTHLIGHT;
                }
            }

            [Serializable]
            public class RewardItemsBooster : RewardData
            {
                public AcbBallContainer.ACBallItemsBooster.ACBallBoosterData element = new AcbBallContainer.ACBallItemsBooster.ACBallBoosterData();

                [JsonConstructor]
                public RewardItemsBooster()
                {
                    rewardType = ItemType.BOOSTER;
                }
            }

            [Serializable]
            public class RewardItemsACBall : RewardData
            {
                public AcbBallContainer.AcbBallsData.AcBallsItems element = new AcbBallContainer.AcbBallsData.AcBallsItems();

                [JsonConstructor]
                public RewardItemsACBall()
                {
                    rewardType = ItemType.ACBALL;
                }
            }
        }
    }

    /// <summary>
    /// Datos principales de ACBalls disponibles para abrir por el jugador
    /// </summary>
    [Serializable]
    public class AcbBallContainer
    {
        public StatsData statsData;
        public AcbBallsData acballsData;

        [Serializable]
        public class StatsData
        {
            public int acballsBalance;
            public bool acballsNotification;
        }

        [Serializable]
        public class AcbBallsData
        {
            public List<AcBallsItems> acballsItems = new List<AcBallsItems>();

            [Serializable]
            public class AcBallsItems
            {
                public int id;
                public string name;
                public string description;
                public string path_img;
                public int quantity;
                public Dictionary<ItemType, string> assets;
                public Dictionary<ItemType, string> tit_elements;
                public Dictionary<ItemType, string> desc_elements;
                public int cost;
                public string status;
                public bool show;
            }
        }

        [Serializable]
        public class AcBallBody
        {
            public string acball_id;
        }

        [Serializable]
        public class ACBallUpdateShowBody
        {
            public int acball_id;
        }

        [Serializable]
        public class AcbBallReward
        {
            public OpenAcBallReward openACBall;
        }

        [Serializable, JsonConverter(typeof(ACBallConvert))]
        public class OpenAcBallReward
        {
            public ItemType elemenType = ItemType.NONE;
            public string elementId;
            public string img;

            [JsonConstructor]
            public OpenAcBallReward()
            {
                elemenType = ItemType.NONE;
            }

            public OpenAcBallReward ChangeType(ItemType types)
            {
                switch (types)
                {
                    case ItemType.COINS:
                        return this as ACBallItemsCoin;

                    case ItemType.SKIN:
                        return this as ACBallItemsAvatar;

                    case ItemType.TOKEN:
                        return this as ACBallItemsToken;

                    case ItemType.HIGTHLIGHT:
                        return this as ACBallItemsHighlight;

                    case ItemType.BOOSTER:
                        return this as ACBallItemsBooster;

                    default:
                        throw new Exception();
                }
            }
        }
        [Serializable]
        public class ACBallsToSell
        {
            public int page;
            public int balance;
            public int count;
            public List<AcBallsItems> items = new List<AcBallsItems>();

            [Serializable]
            public class AcBallsItems
            {
                public int id;
                public string path_img;
                public acball acball = new acball();
                public OpenAcBallReward content =  new OpenAcBallReward();
                public string status;
                public bool show;
            }
            [Serializable]
            public class acball
            {
                public int id;
                public string name;
                public string info;
                public string path_img;
                public string q_elementype;
             //   public Dictionary<ItemType, string> v_elemcontent;
                public Dictionary<ItemType, string> img_elements;
                public Dictionary<ItemType, string> tit_elements;
                public Dictionary<ItemType, string> desc_elements;
                public int cost;
            }
            public class OpenAcBallReward
            {
                public ItemType elemenType = ItemType.NONE;
                public string elementId;
                public string img;
                public ACBallItemsBooster.ACBallBoosterData element = new ACBallItemsBooster.ACBallBoosterData();
            }
            }
        [Serializable]
        public class ACBallItemsCoin : OpenAcBallReward
        {
            public ACBallCoinData element = new ACBallCoinData();

            [JsonConstructor]
            public ACBallItemsCoin()
            {
                elemenType = ItemType.COINS;
            }

            [SerializeField]
            public class ACBallCoinData
            {
                public int value;
                public string assets;
                public string titulo;
                public string description;
            }
        }

        [Serializable]
        public class ACBallItemsAvatar : OpenAcBallReward
        {
            public ACBallSkinData element = new ACBallSkinData();

            [JsonConstructor]
            public ACBallItemsAvatar()
            {
                elemenType = ItemType.SKIN;
            }

            [SerializeField]
            public class ACBallSkinData
            {
                public int id;
                public string token;
                [JsonConverter(typeof(StringEnumConverter))]
                public ItemType type;
                public string name;
                public string description;
                public string img_path;
                public string img_show;
            }
        }

        [Serializable]
        public class ACBallItemsToken : OpenAcBallReward
        {
            public ACBallTokenData element = new ACBallTokenData();

            [JsonConstructor]
            public ACBallItemsToken()
            {
                elemenType = ItemType.TOKEN;
            }

            [SerializeField]
            public class ACBallTokenData
            {
                public int id;
                public string token;
                public int card_id;
                public string name;
                public string description;
                public string pathImgFront;
                public string pathThumbnail;
                public string pathImgBack;
                public string triples;
                public string freeshots;
                public string rebounds;
                public string assits;
                public string points;
                public string st_triples;
                public string st_freeshots;
                public string st_rebounds;
                public string st_assists;
                public string st_points;
                public string pt_triples;
                public string pt_freeshots;
                public string pt_rebounds;
                public string pt_assists;
                public string pt_points;
                public string user_id;
                public string token_bchain;
                public int subcollection_id;
                [JsonConverter(typeof(StringEnumConverter))]
                public TokenRarety rarity;
                public bool isTeam;
                public string victories;
                public bool isInjured;
                public string textInjured;
                public bool isBooster;
                public bool isBoostTriples;
                public bool isBoostFreeshots;
                public bool isBoostRebounds;
                public bool isBoostAssists;
                public bool isBoostPoints;
                public List<CardVideoData> videos = new List<CardVideoData>();
                public string daysOrTextInjured;
            }
        }

        [Serializable]
        public class ACBallItemsHighlight : OpenAcBallReward
        {
            public ACBallHighlightData element = new ACBallHighlightData();

            [JsonConstructor]
            public ACBallItemsHighlight()
            {
                elemenType = ItemType.HIGTHLIGHT;
            }

            [SerializeField]
            public class ACBallHighlightData
            {
                public int id;
                public string token;
                public string name;
                public string description;
                public string urlvideo;
                public string pathImg;
                public string pathImgThumbnail;
            }
        }

        [Serializable]
        public class ACBallItemsBooster : OpenAcBallReward
        {
            public ACBallBoosterData element = new ACBallBoosterData();

            [JsonConstructor]
            public ACBallItemsBooster()
            {
                elemenType = ItemType.BOOSTER;
            }

            [SerializeField]
            public class ACBallBoosterData
            {
                public int id;
                public string uuid;
                [JsonConverter(typeof(StringEnumConverter))]
                public BoosterType type;
                public string name;
                public string description;
                public string value;
                public string path_img;
                public int idType;
            }
        }
    }

    /// <summary>
    /// Dato de número máximo de jugadores a mostrar en el ranking
    /// </summary>
    [Serializable]
    public class RankingBody
    {
        public int page = 1;
        public int num_items = 100;
    }

    /// <summary>
    /// Datos princiaples del edificio ACB dentro del juego
    /// </summary>
    [Serializable]
    public class HeadquarterContainerData
    {
        public int total_pages;
        public int total_items;
        public List<RankingUserData> items = new List<RankingUserData>();
        public RankingUserData current_user;
        public int code;
        public string headQuartersURL = "";
        public List<RankingUserData> currentUser = new List<RankingUserData>();
        public List<RankingUserData> rankingUsers = new List<RankingUserData>();
        public List<RankingUserData> currentUserClassic = new List<RankingUserData>();
        public List<RankingUserData> currentUserCurrent = new List<RankingUserData>();
        public List<RankingUserData> rankingCurrent = new List<RankingUserData>();
        public List<RankingUserData> rankingClassic = new List<RankingUserData>();
        public string message= "";
        //historico
        public List<RankingUserData> best_leauge = new List<RankingUserData>();
        public string TEXT_EDITION;
        public string MESSAGE;
        public List<DataSeason> data = new List<DataSeason>();
        [Serializable]
        public class DataSeason
        {
            public List<Season> SEASON_2025 = new List<Season>();
            public List<Season> SEASON_2024 = new List<Season>();
            public List<Season> SEASON_2023 = new List<Season>();
            public List<Season> SEASON_2022 = new List<Season>();
                  public List<Season> SEASON_2021 = new List<Season>();
            public List<Season> SEASON_2020 = new List<Season>();
         
         }
        [Serializable]
        public class Season
        {
            public List<RankingUserData> ENE_MAR = new List<RankingUserData>();
            public List<RankingUserData> ABR_JUN = new List<RankingUserData>();
            public List<RankingUserData> JUL_SEP = new List<RankingUserData>();
            public List<RankingUserData> OCT_DEC = new List<RankingUserData>();

        }
        [Serializable]
        public class RankingUserData 
        {
            public int id;
            public string user_id;
            public int points;
            public int frozencoins;
            public int position;
            public int challenge_id;
            public string created;
            public string expiration;
            public string nickName;
            public int skinColor;
            public int faceForm;
            public int eyes;
            public int ear;
            public int nose;
            public int mouth;
            public int eyeBrow;
            public int hairStyleFront;
            public int facialHair;
            public int bodyAccessory;
            public int headAccessory;
            public int armAccessory;
            public int eyesAccessory;
            public int backGround;
            public int foreGround;
            public int hairStyleBack;
            public int bodyForm;
            public int hairColor;
            public int statusId;
            public string status;
            public bool show;
            public int level;
            public int challengeCost;
            public string img_small;
            public string img_big;
            public int quarter;
            //missions
            public int missions_number;
            //tokens
            public int total_tokens;
       
        }
    }
    [Serializable]
    public class TokenDataRankingContainer
    {
        public int total_pages;
        public int total_items;
        public List<TokenDataRanking> items = new List<TokenDataRanking>();
        public List<DataSeason> data = new List<DataSeason>();
        public List<TokenDataRanking> best_leauge = new List<TokenDataRanking>();
        public string TEXT_EDITION;
        public string MESSAGE;
        [Serializable]
        public class DataSeason
        {
            public List<Season> SEASON_2025 = new List<Season>();
            public List<Season> SEASON_2024 = new List<Season>();
            public List<Season> SEASON_2023 = new List<Season>();
            public List<Season> SEASON_2022 = new List<Season>();
            public List<Season> SEASON_2021 = new List<Season>();
            public List<Season> SEASON_2020 = new List<Season>();

        }
        [Serializable]
        public class Season
        {
            public List<TokenDataRanking> ENE_MAR = new List<TokenDataRanking>();
            public List<TokenDataRanking> ABR_JUN = new List<TokenDataRanking>();
            public List<TokenDataRanking> JUL_SEP = new List<TokenDataRanking>();
            public List<TokenDataRanking> OCT_DEC = new List<TokenDataRanking>();

        }
    }
        [Serializable]
    public class TokenDataRanking
    {
       
        public int id;
        public string user_id;
        public string nickName;
        public int skinColor;
        public int faceForm;
        public int eyes;
        public int ear;
        public int nose;
        public int mouth;
        public int eyeBrow;
        public int hairStyleFront;
        public int facialHair;
        public int bodyAccessory;
        public int headAccessory;
        public int armAccessory;
        public int eyesAccessory;
        public int backGround;
        public int foreGround;
        public int hairStyleBack;
        public int bodyForm;
        public int hairColor;
        public int frozencoins;
        public int level;
        public object bch_account;
        public List<ItemData> avatarItems;
        public string img_small;
        public string img_big;
        public string token;
        public int card_id;
        public string player_name;
        public string description;
        public string pathImgFront;
        public string pathThumbnail;
        public string pathImgCol;
        public string path_img_leauge;
        public string path_img;
        public string pathImgBack;
        public string triples;
        public string freeshots;
        public string rebounds;
        public string assists;
        public string points;
        public string st_triples;
        public string st_freeshots;
        public string st_rebounds;
        public string st_assists;
        public string st_points;
        public string pt_triples;
        public string pt_freeshots;
        public string pt_rebounds;
        public string pt_assists;
        public string pt_points;
        public string token_bchain;
        public int subcollection_id;
        [JsonConverter(typeof(StringEnumConverter))]
        public TokenRarety rarity;
        public bool isTeam;
        public string victories;
        public bool isInjured;
        public string textInjured;
        public string daysOrTextInjured;
        public bool isBooster;
        public object videos;
        public int useradmin_id;
        public int status;
        public Dictionary<ItemType, string> assets;
        public Dictionary<ItemType, string> tit_elements;
        public Dictionary<ItemType, string> desc_elements;
        public int position;
    }
    /// <summary>
    /// Datos generales de la tienda, por ejemplo, de los items disponibles y su precio
    /// </summary>
    [Serializable]
    public class MallContainerData
    {
        public MallData mallData;

        [Serializable]
        public class MallData
        {
            public List<MallItems> mallItems = new List<MallItems>();

            [Serializable, JsonConverter(typeof(MallConvert))]
            public class MallItems
            {
                public int id;
                public string title;
                public string description;
                public int price;
                public string postdate;
                public string path_img;
                public bool random;
                public ItemType elementType = ItemType.NONE;

                [JsonConstructor]
                public MallItems()
                {
                    elementType = ItemType.NONE;
                }

                public MallItems ChangeType(ItemType types)
                {
                    switch (types)
                    {

                        case ItemType.ACBALL:
                            return this as MallItemsAcBall;

                        case ItemType.SKIN:
                            return this as MallItemsAvatar;

                        case ItemType.TOKEN:
                            return this as MallItemsToken;

                        case ItemType.HIGTHLIGHT:
                            return this as MallItemsHight;

                        case ItemType.BOOSTER:
                            return this as MallItemsBooster;

                        default:
                            throw new Exception();
                    }
                }
            }

            [Serializable]
            public class MallItemsAcBall : MallItems
            {
                public AcbBallContainer.AcbBallsData.AcBallsItems element = new AcbBallContainer.AcbBallsData.AcBallsItems();

                [JsonConstructor]
                public MallItemsAcBall()
                {
                    elementType = ItemType.ACBALL;
                }
            }

            [Serializable]
            public class MallItemsAvatar : MallItems
            {
                public ItemData element = new ItemData();

                [JsonConstructor]
                public MallItemsAvatar()
                {
                    elementType = ItemType.SKIN;
                }
            }

            [Serializable]
            public class MallItemsToken : MallItems
            {
                public ChallengeAcceptedData.CardChallengeData.CardItems element = new ChallengeAcceptedData.CardChallengeData.CardItems();

                [JsonConstructor]
                public MallItemsToken()
                {
                    elementType = ItemType.TOKEN;
                }
            }

            [Serializable]
            public class MallItemsHight : MallItems
            {
                public HighLightData.HigthlightItems element = new HighLightData.HigthlightItems();

                [JsonConstructor]
                public MallItemsHight()
                {
                    elementType = ItemType.HIGTHLIGHT;
                }
            }

            [Serializable]
            public class MallItemsBooster : MallItems
            {
                public BoosterData.BoosterItemData element = new BoosterData.BoosterItemData();

                [JsonConstructor]
                public MallItemsBooster()
                {
                    elementType = ItemType.BOOSTER;
                }
            }

        }

        [Serializable]
        public class MallBody
        {
            public int idPost;
        }
    }

    /// <summary>
    ///  Dato de número máximo de objetos a mostrar en tienda y panel de ACBall
    /// </summary>
    public class PageBody
    {
        public int page;
    }
    /// <summary>
    ///  Información requerida para pedir los items del mercadillo
    /// </summary>
    public class JumbleSaleRequest
    {
        public string[] types;
        public int num_items;
        public int page;
        public string user_id;
        public List<string[]> order = new List<string[]>();
        public string query = "";

    }
    /// <summary>
    /// retorna los items del mercadillo
    /// </summary>
    public class JumbleUser
    {
        public int id;
        public string user_id;
        public string nickName;
        public int skinColor;
        public int faceForm;
        public int eyes;
        public int ear;
        public int nose;
        public int mouth;
        public int eyeBrow;
        public int hairStyleFront;
        public int facialHair;
        public int bodyAccessory;
        public int headAccessory;
        public int armAccessory;
        public int eyesAccessory;
        public int backGround;
        public int foreGround;
        public int hairStyleBack;
        public int bodyForm;
        public int hairColor;
        public int points;
        public int frozencoins;
    }
    public class JumbleResult
    {
        public int balance;
    }
    public class JumbleSaleResult
    {
        public int total_items;
        public int total_pages;
        public List<JumbleItems> items = new List<JumbleItems>();
        public int balance;
        public string MESSAGE_MAX_POSTS;

        public class JumbleItems
        {
            public int id;
            public string name;
            public string description;
            public string item_type;
            public int item_id;
            public string seller_user_id;
            public string price;
            public string publication_date;
            public string path_img;
            public bool is_booster;
            public bool is_injured;
            public JumbleUser user;
        }
        /// <summary>
        /// Petición para vender algo en el mercadillo
        /// </summary>
        public class JumbleBuyRequest
        {
            public string user_id;
            public int item_id;
            public string item_type;
        }
        /// <summary>
        /// Elimina un elemento del mercadillo
        /// </summary>
        public class JumbleDeleteItemRequest
        {
            public string user_id;
            public int item_id;
        }
        /// <summary>
        /// Información de un item del mercadillo
        /// </summary>
        public class JumbleItemData
        {
            public JumbleCardData data;
        }
        public class JumbleCardData
        {
            public string token;
            public int card_id;
            public string name;
            public string description;
            public string path_img_front;
            public string pathThumbnail;
            public string pathImgCol;
            public string path_img_leauge;
            public string path_img;
            public string path_img_back;
            public string triples;
            public string freeshots;
            public string rebounds;
            public string assists;
            public string points;
            public string st_triples;
            public string st_freeshots;
            public string st_rebounds;
            public string st_assists;
            public string st_points;
            public string pt_triples;
            public string pt_freeshots;
            public string pt_rebounds;
            public string pt_assists;
            public string pt_points;
            public string user_id;
            public string token_bchain;
            public int subcollection_id;
            [JsonConverter(typeof(StringEnumConverter))]
            public TokenRarety rarity;
            public bool isTeam;
            public string victories;
            public bool isInjured;
            public string textInjured;
            public string daysOrTextInjured;
            public bool isBooster;
            public object videos;
            public int useradmin_id;
            public int status;
            public Dictionary<ItemType, string> assets;
            public Dictionary<ItemType, string> tit_elements;
            public Dictionary<ItemType, string> desc_elements;
            //Highlights properties
            public string updated;
            public string title;
            public string created;
            public string path_img_backcard;
            //ACBalls properties
            public int id;
            public int acball_id;

        }
    }
    /// <summary>
    /// Enumerador de los tipos de potenciadores
    /// </summary>
    public enum BoosterType
    {
        TRIPLES,
        FREESHOTS,
        REBOUNDS,
        ASSISTS,
        POINTS,
    }

    /// <summary>
    /// Respuesta de backend que valida si un booster se puede aplicar a un jugador
    /// </summary>
    [Serializable]
    public class ApplyBoosterResponse
    {
        public BoosterData boosterData = new BoosterData();
        public TokenData2 tokenData = new TokenData2();
    }

    /// <summary>
    /// Respuesta de backend que valida si un booster se puede aplicar a un jugador
    /// </summary>
    [Serializable]
    public class ApplyBoosterBody
    {
        public string uuidBooster;
        public string uuidTokenCard;
    }

    /// <summary>
    /// Respuesta de backend que valida si un booster se puede aplicar a un jugador
    /// </summary>
    [Serializable]
    public class BoosterBody
    {
        [JsonConverter(typeof(StringEnumConverter))]
        public BoosterType type;
    }

    /// <summary>
    /// Respuesta de backend que valida si un booster se puede aplicar a un jugador
    /// </summary>
    [Serializable]
    public class BoosterContainer
    {
        public BoosterData boosterData = new BoosterData();
    }

    /// <summary>
    /// Datos de potenciadores disponibles
    /// </summary>
    [Serializable]
    public class BoosterData
    {
        public List<BoosterItemData> boosterItems = new List<BoosterItemData>();
        [Serializable]
        public class BoosterItemData
        {
            public int id;
            public string token;
            public string name;
            public int idType;
            [JsonConverter(typeof(StringEnumConverter))]
            public BoosterType type;
            public string value;
            public string path_img;
            private const string SPRITEPATH = "Boosters/";
            public string description;
            public string user_id;
            public Sprite GetSprite()
            {
                return Resources.Load<Sprite>(SPRITEPATH + type);
            }
        }

    }

    /// <summary>
    /// Datos generales de colecciones del equipo competitivo
    /// </summary>
    [Serializable]
    public class ColectionContainer
    {
        public CollectionData collectionData = new CollectionData();
    }

    /// <summary>
    /// Datos de los items de colección del equipo competitivo
    /// </summary>
    [Serializable]
    public class CollectionData
    {
        public List<CollectionItemData> collectionItems = new List<CollectionItemData>();

        [Serializable]
        public class CollectionItemData
        {
            public int id;
            public string name;
            public string description;
            public string pathImg;
            public string pathImgBackcard;
            public int id_competition;
            public int position;
            public bool hasSubcollections = true;
            [JsonConverter(typeof(StringEnumConverter))]
            public ItemType type;
            public bool current;
        }

    }
    /// <summary>
    ///Contenedor de los potenciadores para publicar 
    /// </summary>
    [Serializable]
    public class BoosterContainerJumbleSale
    {
        public List<BoosterData.BoosterItemData> Asistencias = new List<BoosterData.BoosterItemData>();
        public List<BoosterData.BoosterItemData> Rebotes = new List<BoosterData.BoosterItemData>();
        public List<BoosterData.BoosterItemData> Puntos = new List<BoosterData.BoosterItemData>();
        public List<BoosterData.BoosterItemData> Tiros = new List<BoosterData.BoosterItemData>();
        public List<BoosterData.BoosterItemData> Triples = new List<BoosterData.BoosterItemData>();
    }

    /// <summary>
    /// Datos de los items de colección de highlights para vender
    /// </summary>
    [Serializable]
    public class HighlightCollectionData
    {
        public List<CollectionData.CollectionItemData> items = new List<CollectionData.CollectionItemData>();
    }
    /// <summary>
    /// Datos de los items de colección de highlights para vender
    /// </summary>
    [Serializable]
    public class AllCollectionsContainer
    {
        public List<CollectionData.CollectionItemData> data = new List<CollectionData.CollectionItemData>();
    }
    /// <summary>
    /// Datos de subcolección de un equipo
    /// </summary>
    [Serializable]
    public class BodySubcollection
    {
        public int collection_id;

    }

    /// <summary>
    /// Datos de subcolecciones del equipo competitivo
    /// </summary>
    [Serializable]
    public class SubColectionContainer
    {
        public SubCollectionData subcollectionData = new SubCollectionData();
    }

    /// <summary>
    /// Datos de las subcolecciones disponibles
    /// </summary>
    [Serializable]
    public class SubCollectionData
    {
        public List<SubCollectionItemData> subCollectionItems = new List<SubCollectionItemData>();
        [Serializable]
        public class SubCollectionItemData
        {
            public int id;
            public string name;
            public string pathImg;
            public int collection_id;
            public int position;
            public ItemType type;
        }

    }
    /// <summary>
    /// Datos de las subcolecciones disponibles para vender
    /// </summary>
    [Serializable]
    public class SubCollectionDataToPublish
    {
        public List<SubCollectionData.SubCollectionItemData> cardItems = new List<SubCollectionData.SubCollectionItemData>();
    }
    /// <summary>
    /// Datos de las subcolecciones de highlights disponibles para vender
    /// </summary>
    [Serializable]
    public class SubCollectionHiglightDataToPublish
    {
        public List<HighLightData.HigthlightItems> items = new List<HighLightData.HigthlightItems>();
    }
    /// <summary>
    /// Datos del cuerpo de una carta de equipo competitivo
    /// </summary>
    [Serializable]
    public class BodyCard
    {
        public int subcollection_id;
    }

    /// <summary>
    /// Contenedor de datos generales de las cartas de equipo competitivo
    /// </summary>
    [Serializable]
    public class CardContainer
    {
        public CardData cardData = new CardData();
    }

    /// <summary>
    /// Datos específicos de una carta del equipo competitivo
    /// </summary>
    [Serializable]
    public class CardData
    {
        public List<CardItemData> cardItems = new List<CardItemData>();
        [Serializable]
        public class CardItemData
        {
            public int id;
            public string uuid;
            public string name;
            public string pathImgFront;
            public string pathImgBack;
            public string pathThumbnail;
            public int subcollection_id;
            public string st_triples;
            public string st_freeshots;
            public string st_rebounds;
            public string st_assists;
            public string st_points;
            public bool upgradeable_stats;
            public bool include_team;
            public int useradmin_id;
            public int id_license;
            public bool enter = true;
            [JsonConverter(typeof(StringEnumConverter))]
            public ItemType type;
            public int position;
            public List<CardVideoData> videos = new List<CardVideoData>();

        }

        [Serializable]
        public class CardVideoData
        {
            public string title;
            public string url_video;
        }
    }
    /// <summary>
    /// Datos específicos de una carta del equipo competitivo en el mercadillo
    /// </summary>
    [Serializable]
    public class CardDataToPublish
    {
        public List<CardData.CardItemData> items = new List<CardData.CardItemData>();

    }
    /// <summary>
    ///  información para poder publicar en el mercadillo
    /// </summary>
    [Serializable]
    public class SellData
    {
        public string type;
        public int item_id;
        public int price;
    }
    /// <summary>
    /// mensaje luego de agregar un jugador
    /// </summary>
    [Serializable]
    public class PostSetTeam
    {
        public int code;
        public string message;
        public AllTokensContainer data = new AllTokensContainer();
    }

    /// <summary>
    ///  petición para validar un QR de pase VIP del auditorio 
    /// </summary>
    [Serializable]
    public class RequestQRTicket
    {
        public string code;
        public string user_id;
    }
    /// <summary>
    /// Información que retorna de la petición de la cartelera del auditorio
    /// </summary>
    public class BillBoardReturn
    {
        public BillboardData[] data;
        public class BillboardData
        {
            public int id;
            public string type;
            public string media_path;
            public string title;
            public string description;
            public string init_date;
            public string end_date;
            public int capacity;
            public bool status;
            public int useradmin_id;
            public string created;
            public string updated;
            public string thumbnail;
        }
    }
    /// <summary>
    /// Información que retorna la petición de los pases del jugador en el auditorio
    /// </summary>
    public class VIPPassesReturn
    {
        public VIPPass[] data;
        public class VIPPass
        {
            public int id;
            public string user_id;
            public VIPPassData vip_pass;
            public class VIPPassData
            {
                public int id;
                public string code;
                public string title;
                public string description;
                public string type;
                public string media_id;
            }
        }
    }
    /// <summary>
    ///  petición para validar un QR de pase VIP del auditorio 
    /// </summary>
    [Serializable]
    public class RequestUseVIPPass
    {
        public string user_id;
        public int userpass_id;
    }
    /// <summary>
    ///  petición para salir del auditorio
    /// </summary>
    [Serializable]
    public class RequestLeaveAuditory
    {
        public string user_id;
        public int seat_id;
    }
    /// <summary>
    ///  retorno de la petición de información del asiento en el auditorio
    /// </summary>
    [Serializable]
    public class ReturnSeatInfoAuditory
    {
        public int id;
        public string user_id;
        public bool status;
        public int projection_room_id;
        public int userpass_id;
        public string created;
        public string updated;

    }
    /// <summary>
    /// request para eliminar la cuenta
    /// </summary>
    public class RequestDeleteAccount
    {
        public string user_id;
        public string Authorization;
    }
    /// <summary>
    /// Contenedor con los datos de cartas de jugadores
    /// </summary>
    [Serializable]
    public class AllTokensContainer
    {
        public List<TokenItemData> classical = new List<TokenItemData>();
        public List<TokenItemData> current = new List<TokenItemData>();
    }

    /// <summary>
    /// Contenedor con los datos de cartas de jugadores
    /// </summary>
    [Serializable]
    public class TokenContainer
    {
        public TokenData teamData = new TokenData();
    }

    /// <summary>
    /// Contenedor con los datos de cartas de jugadores
    /// </summary>
    [Serializable]
    public class TokenContainer2
    {
        public TokenData2 tokenData = new TokenData2();
    }

    /// <summary>
    /// Contenedor con el listado de cartas del jugador
    /// </summary>
    [Serializable]
    public class TokenData
    {
        public List<TokenItemData> teamItems = new List<TokenItemData>();
    }

    /// <summary>
    /// Contenedor con el listado de cartas del jugador
    /// </summary>
    [Serializable]
    public class TokenData2
    {
        public List<TokenItemData> tokenItems = new List<TokenItemData>();
    }

    /// <summary>
    /// Datos de una carta tipo highlight
    /// </summary>
    [Serializable]
    public class BodyHightLight
    {
        public int higthlight_id;
    }

    /// <summary>
    /// Dato del id del cuerpo de la carta
    /// </summary>
    [Serializable]
    public class BodyToken
    {
        public int card_id;
    }
    /// <summary>
    /// Dato del id del cuerpo de la carta de un highlight
    /// </summary>
    [Serializable]
    public class BodyTokenHighlight
    {
        public int highlight_id;
    }

    /// <summary>
    /// Dato de la carta
    /// </summary>
    [Serializable]
    public class BodyTokenCard
    {
        public string tokenCard;
    }

    /// <summary>
    /// Enumerador con la rareza de una carta
    /// </summary>
    public enum TokenRarety
    {
        ÉPICA,
        LEGENDARIA,
        COMÚN
    }

    /// <summary>
    /// Datos de una carta de un jugador
    /// </summary>
    [Serializable]
    public class TokenItemData
    {
        public int team_id;
        public int collection_id = 206;
        public int tokencard_id;
        public string tokencard_token;
        public int id;
        public string token;
        public string card_id;
        public string name;
        public string description;
        public string pathImgFront;
        public string pathThumbnail;
        public string pathImgBack;
        public string pathImgCol;
        public string triples;
        public string freeshots;
        public string rebounds;
        public string assists;
        public string assits;
        public string points;
        public string st_triples;
        public string st_freeshots;
        public string st_rebounds;
        public string st_assists;
        public string st_points;
        public string pt_triples;
        public string pt_freeshots;
        public string pt_rebounds;
        public string pt_assists;
        public string pt_points;
        public string user_id;
        public string token_bchain;
        public int subcollection_id;
        [JsonConverter(typeof(StringEnumConverter))]
        public TokenRarety rarity;
        public bool isTeam;
        public string victories;
        public bool isInjured;
        public string textInjured;
        public bool isBooster;
        public bool isBoostTriples;
        public bool isBoostFreeshots;
        public bool isBoostRebounds;
        public bool isBoostAssists;
        public bool isBoostPoints;
        public List<CardVideoData> videos = new List<CardVideoData>();
        public string daysOrTextInjured;
        public bool current = true;
        public CardTokenData card = new CardTokenData();
    }
    [Serializable]
    public class CardTokenData
    {
    public string player_name;
    public string description;
    public string pathImgFront;
    public string pathThumbnail;
    public string st_triples;
    public string st_freeshots;
    public string st_rebounds;
    public string st_assists;
    public string st_points;
    public string subcollection_id;
    public SubCollectionInfo subcollection = new SubCollectionInfo();
    }

    public class SubCollectionInfo
    {
    public int id;
    public CollectionInfo collection = new CollectionInfo();    
    }

    public class CollectionInfo
    {
    public string pathImgBack;
    public string pathImgCol;
    public Edition edition = new Edition();
    }
  [Serializable]
    public class Edition
    {
       public bool current;
    }

    /// <summary>
    /// Datos de videos que contiene la carta de un jugador
    /// </summary>
    [Serializable]
    public class CardVideoData
    {
        public string title;
        public string url_video;
        public string fotograma;
    }

    /// <summary>
    /// Dato con el id del desafio aceptado por el jugador
    /// </summary>
    [Serializable]
    public class ChallengeAccept
    {
        public int challenge_id;
    }

    /// <summary>
    /// Contenedor de los datos generales del tablón de desafios
    /// </summary>
    [Serializable]
    public class ChallengeContainer
    {
        public ChallengesTablon challengeData = new ChallengesTablon();
    }

    /// <summary>
    /// Datos de los desafios disponibles dentro del tablón
    /// </summary>
    [Serializable]
    public class ChallengesTablon
    {
        public bool TeamComplete;
        public bool canPostChallenge;
        
        public bool challengeFree;
        public bool canAcceptChallenge;
        public string challengeFreeMessage;
        public string canPostChallengeMessage;
        public string canAcceptChallengeMessage;
        public List<ChallengesTablonItem> challengeItems = new List<ChallengesTablonItem>();

        [Serializable]
        public class ChallengesTablonItem
        {
            public int challenge_id;
            public string created;
            public string expiration;
            public string user_id;
            public string nickName;
            public int skinColor;
            public int faceForm;
            public int eyes;
            public int ear;
            public int nose;
            public int mouth;
            public int eyeBrow;
            public int hairStyleFront;
            public int facialHair;
            public int bodyAccessory;
            public int headAccessory;
            public int armAccessory;
            public int eyesAccessory;
            public int backGround;
            public int foreGround;
            public int hairStyleBack;
            public int bodyForm;
            public int hairColor;
            public int statusId;
            public string status;
            public bool show;
            public int level;
            public int points;
            public int challengeCost;
            public List<ItemData> avatarItems = new List<ItemData>();
            public string img_small;
            public string img_big;
        }
    }

    /// <summary>
    /// Datos de un desafio aceptado
    /// </summary>
    [Serializable]
    public class ChallengeAcceptedData
    {
        public StatsData statsData;
        public PlayerCardData user;
        public PlayerCardData rival;
        public ChallengeEndData challengeData;

        [Serializable]
        public class PlayerCardData
        {
            public AvatarChallengeData avatarData;
            public CardChallengeData cardData;
        }

        [Serializable]
        public class RivalChallengeData
        {
            public AvatarChallengeData avatarData;
            public CardChallengeData cardData;
        }

        [Serializable]
        public class ChallengeEndData
        {
            public int challenge_id;
            public string win_st_freeshots;
            public string win_st_triples;
            public string win_st_rebounds;
            public string win_st_assists;
            public string win_st_points;
            public string winner;
            public int win_points;
            public int lost_points;
            public AcballChallengeData acballData;
        }

        [Serializable]
        public class AcballChallengeData
        {
            public AcballItemChallenge acballItem;

            [Serializable]
            public class AcballItemChallenge
            {
                public int id;
                public string name;
                public string info;
                public string path_img;
                public string open_img;
                public int quantity;
                public ChallengeContent content;
                public ChallengeAssets assets;
                public ChallengeTitElements tit_elements;
                public ChallengeDescElements desc_elements;
                public int cost;
                public string status;

                [Serializable]
                public class ChallengeContent
                {
                    public string TOKEN;
                }

                [Serializable]
                public class ChallengeAssets
                {
                    public string TOKEN;
                }

                [Serializable]
                public class ChallengeTitElements
                {
                    public string TOKEN;
                }

                [Serializable]
                public class ChallengeDescElements
                {
                    public string TOKEN;
                }
            }
        }

        [Serializable]
        public class AvatarChallengeData
        {
            public int id;
            public string user_id;
            public string nickName;
            public int skinColor;
            public int faceForm;
            public int eyes;
            public int ear;
            public int nose;
            public int mouth;
            public int eyeBrow;
            public int hairStyleFront;
            public int facialHair;
            public int bodyAccessory;
            public int headAccessory;
            public int armAccessory;
            public int eyesAccessory;
            public int backGround;
            public int foreGround;
            public int hairStyleBack;
            public int bodyForm;
            public int hairColor;
            public int points;
            public int frozencoins;
            public int level;
            public object bch_account;
            public List<ItemData> avatarItems;
            public string img_small;
            public string img_big;
        }

        [Serializable]
        public class CardChallengeData
        {
            public CardItems cardItems;

            [Serializable]
            public class CardItems
            {
                public int id;
                public string token;
                public int card_id;
                public string name;
                public string description;
                public string pathImgFront;
                public string pathThumbnail;
                public string pathImgCol;
                public string pathImgBack;
                public string triples;
                public string freeshots;
                public string rebounds;
                public string assits;
                public string points;
                public string st_triples;
                public string st_freeshots;
                public string st_rebounds;
                public string st_assists;
                public string st_points;
                public string pt_triples;
                public string pt_freeshots;
                public string pt_rebounds;
                public string pt_assists;
                public string pt_points;
                public string user_id;
                public string token_bchain;
                public int subcollection_id;
                [JsonConverter(typeof(StringEnumConverter))]
                public TokenRarety rarity;
                public bool isTeam;
                public string victories;
                public bool isInjured;
                public string textInjured;
                public string daysOrTextInjured;
                public bool isBooster;
                public object videos;
                public bool is_clasic = true;
            }
        }
    }

    /// <summary>
    /// Datos de un desafio previamente terminado que se encuentra en la cancha
    /// </summary>
    [Serializable]
    public class ChallengesField
    {
        public ChallengeFieldData challengeData;
        public int page;
        public int rowCount;
        [Serializable]
        public class ChallengeFieldData
        {
            public List<ChallengesFieldItem> challengeItems = new List<ChallengesFieldItem>();

            [Serializable]
            public class ChallengesFieldItem
            {
                public int challenge_id;
                public string jugado;
                public string rival;
                public string nickName;
                public int skinColor;
                public int faceForm;
                public int eyes;
                public int ear;
                public int nose;
                public int mouth;
                public int eyeBrow;
                public int hairStyleFront;
                public int facialHair;
                public int bodyAccessory;
                public int headAccessory;
                public int armAccessory;
                public int eyesAccessory;
                public int backGround;
                public int foreGround;
                public int hairStyleBack;
                public int bodyForm;
                public int hairColor;
                public List<ItemData> avatarItems = new List<ItemData>();
                public string cardName;
                public string description;
                public string pathImgFront;
                public string pathThumbnail;
                public string status;
                public string win_status;
                public string img_small;
                public string img_big;
                public bool isTeam;
                public bool isInjured;
                public bool isBooster;
            }
        }
    }

    /// <summary>
    /// Contenedor de los datos generales de una carta highlight
    /// </summary>
    [Serializable]
    public class HigthlightContainer
    {
        public HighLightData higthlightData = new HighLightData();
    }
    
    /// <summary>
    /// Datos específicios de los highlights disponibles
    /// </summary>
    [Serializable]
    public class HighLightData
    {
        public List<HigthlightItems> higthlightItems = new List<HigthlightItems>();
        [Serializable]
        public class HigthlightItems
        {
            public int id;
            public string token;
            public string title;
            public string name;
            public string description;
            public string urlvideo;
            public string pathImg;
            public string pathImgThumbnail;
            public string pathImgCol;
            public int higthlight_id;
            public string user_id;
            public bool enter;

        }

        public enum ChangeStatus
        {
            NOTPLAYED,
        }
    }

    /// <summary>
    /// Contenedor de los datos generales de una carta highlight
    /// </summary>
    [Serializable]
    public class HigthlightContainer2
    {
        public HighLightData2 tokenData = new HighLightData2();
    }

    /// <summary>
    /// Datos específicios de los highlights disponibles
    /// </summary>
    [Serializable]
    public class HighLightData2
    {
        public List<HighLightData.HigthlightItems> tokenItems = new List<HighLightData.HigthlightItems>();
    }


#if UNITY_EDITOR

    public class ResetScriptableAcccountOnBuild : IPreprocessBuildWithReport
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
            ScriptableAccount.Get().Clear();
        }

    }
#endif
}