using UnityEngine;
using System;
using Data;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json.Linq;

/// <summary>
/// Clase que resuelve la conversión de los datos de backend con respecto a los objetos que se pueden comprar en la tienda
/// </summary>
public class BaseSpecifiedConcreteClassTankConverter : DefaultContractResolver
{
    /// <summary>
    /// Resuleve la conversión del objteto de tienda especificado
    /// </summary>
    /// <param name="objectType">Clase con los datos del tipo de objeto de la tienda</param>
    /// <returns></returns>
    protected override JsonConverter ResolveContractConverter(Type objectType)
    {
        if (typeof(MallContainerData.MallData.MallItems).IsAssignableFrom(objectType) && !objectType.IsAbstract)
            return null;
        return base.ResolveContractConverter(objectType);
    }
}

/// <summary>
/// Clase base que se encarga de la conversión de los datos de backend a un objeto específico de la tienda
/// </summary>
public class MallConvert : JsonConverter
{
    static JsonSerializerSettings SpecifiedSubclassConversion = new JsonSerializerSettings() { ContractResolver = new BaseSpecifiedConcreteClassTankConverter() };

    /// <summary>
    /// Determina si los datos del objeto se encuentran de los que pueden ser convertidos
    /// </summary>
    /// <param name="objectType">Datos del objeto a intentar convertir a objeto de la tienda</param>
    /// <returns>Devuelve si los datos pueden ser convertidos o no</returns>
    public override bool CanConvert(Type objectType)
    {
        return typeof(MallContainerData.MallData.MallItems).IsAssignableFrom(objectType);

    }

    /// <summary>
    /// Sobreescritura de método base de lectura de Json enfocado a la conversión de los datos del objeto de la tienda
    /// </summary>
    /// <param name="reader">Lector de Json</param>
    /// <param name="objectType">Datos del objeto en particular</param>
    /// <param name="existingValue">Valor previo existente del objeto</param>
    /// <param name="serializer">Serializador de Json</param>
    /// <returns>Devuelve el objteo de la tienda</returns>
    public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
    {
        JObject jo = JObject.Load(reader);
        Debug.Log(jo.ToString());
        switch (GetEnumMallObject(jo["elementType"]?.ToString()))
        {

            case ItemType.ACBALL:
                return JsonConvert.DeserializeObject<MallContainerData.MallData.MallItemsAcBall>(jo.ToString(), SpecifiedSubclassConversion);

            case ItemType.SKIN:
                return JsonConvert.DeserializeObject<MallContainerData.MallData.MallItemsAvatar>(jo.ToString(), SpecifiedSubclassConversion);

            case ItemType.TOKEN:
                return JsonConvert.DeserializeObject<MallContainerData.MallData.MallItemsToken>(jo.ToString(), SpecifiedSubclassConversion);

            case ItemType.HIGTHLIGHT:
                return JsonConvert.DeserializeObject<MallContainerData.MallData.MallItemsHight>(jo.ToString(), SpecifiedSubclassConversion);

            case ItemType.BOOSTER:
                return JsonConvert.DeserializeObject<MallContainerData.MallData.MallItemsBooster>(jo.ToString(), SpecifiedSubclassConversion);
            
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
    /// Convierte los valores en cadena de texto de un JSON a valores enum del tipo de objeto de la tienda
    /// </summary>
    /// <param name="val">Datos dentro del Json en forma de cadena de texto</param>
    /// <returns>Devuelve el enumerador del objeto en específico</returns>
    private ItemType GetEnumMallObject(string val)
    {
        if (string.IsNullOrEmpty(val))
        {
            return ItemType.NONE;
        }
        return (ItemType)Enum.Parse(typeof(ItemType), val, true);
    }
}


