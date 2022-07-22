using UnityEngine;
using System;
using Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

/// <summary>
/// Clase que resuelve la conversión de los datos de backend con respecto a las recompensas de una misión
/// </summary>
public class BaseSpecifiedConcreteMissionRewardConverter : DefaultContractResolver
{
    /// <summary>
    /// Resuleve la conversión de la recompensa de una misión
    /// </summary>
    /// <param name="objectType">Clase con los datos del tipo de recompensa de una misión</param>
    /// <returns></returns>
    protected override JsonConverter ResolveContractConverter(Type objectType)
    {
        if (typeof(MissionRewardData.RewardData).IsAssignableFrom(objectType) && !objectType.IsAbstract)
            return null;
        return base.ResolveContractConverter(objectType);
    }
}

/// <summary>
/// Clase base que se encarga de la conversión de los datos de backend a una recompensa específica de una misión
/// </summary>
public class MissionRewardConvert : JsonConverter
{
    static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteMissionRewardConverter() };

    /// <summary>
    /// Determina si los datos del objeto se encuentran de los que pueden ser convertidos
    /// </summary>
    /// <param name="objectType">Datos del objeto a intentar convertir a recompensa de misión</param>
    /// <returns>Devuelve si los datos pueden ser convertidos o no</returns>
    public override bool CanConvert(Type objectType)
    {
        return typeof(MissionRewardData.RewardData).IsAssignableFrom(objectType);
    }

    /// <summary>
    /// Sobreescritura de método base de lectura de Json enfocado a la conversión de los datos de la misión
    /// </summary>
    /// <param name="reader">Lector de Json</param>
    /// <param name="objectType">Datos del objeto en particular</param>
    /// <param name="existingValue">Valor previo existente del objeto</param>
    /// <param name="serializer">Serializador de Json</param>
    /// <returns>Devuelve la recompensa de la misión</returns>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);
        Debug.Log(jo.ToString());
        switch (GetEnumACBallObject(jo["rewardType"]?.ToString()))
        {

            case ItemType.COINS:
                return JsonConvert.DeserializeObject<MissionRewardData.RewardData.RewardItemsCoin>(jo.ToString(), SpecifiedSubclassConversion);

            case ItemType.SKIN:
                return JsonConvert.DeserializeObject<MissionRewardData.RewardData.RewardItemsAvatar>(jo.ToString(), SpecifiedSubclassConversion);

            case ItemType.TOKEN:
                return JsonConvert.DeserializeObject<MissionRewardData.RewardData.RewardItemsToken>(jo.ToString(), SpecifiedSubclassConversion);

            case ItemType.HIGTHLIGHT:
                return JsonConvert.DeserializeObject<MissionRewardData.RewardData.RewardItemsHighlight>(jo.ToString(), SpecifiedSubclassConversion);

            case ItemType.BOOSTER:
                return JsonConvert.DeserializeObject<MissionRewardData.RewardData.RewardItemsBooster>(jo.ToString(), SpecifiedSubclassConversion);

            case ItemType.ACBALL:
                return JsonConvert.DeserializeObject<MissionRewardData.RewardData.RewardItemsACBall>(jo.ToString(), SpecifiedSubclassConversion);

            default:
                throw new Exception();
        }
    }

    /// <summary>
    /// Determina si un Json puede ser sobreescrito
    /// </summary>
    public override bool CanWrite
    {
        get { return false; }
    }

    /// <summary>
    /// Sobreescritura de método de escritura de Json
    /// </summary>
    /// <param name="writer">Sobreescritor de Json</param>
    /// <param name="value">Valores del Json</param>
    /// <param name="serializer">Serializador de JSON</param>
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Convierte los valores en cadena de texto de un JSON a valores enum del tipo de recompensa de una misión
    /// </summary>
    /// <param name="val">Datos dentro del Json en forma de cadena de texto</param>
    /// <returns>Devuelve el enumerador del objeto en específico</returns>
    private ItemType GetEnumACBallObject(string val)
    {
        if (string.IsNullOrEmpty(val))
        {
            return ItemType.NONE;
        }
        return (ItemType)Enum.Parse(typeof(ItemType), val, true);
    }
}
