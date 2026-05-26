// StringVariable.cs
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[UnityValueVariable]
[CreateAssetMenu(menuName = "Variables/QuaternionVariable")]
public class QuaternionVariable : ValueVariable<Quaternion>
{
    protected override JsonConverter GetConverter() => new QuaternionConverter();

    private class QuaternionConverter : JsonConverter<Quaternion>
    {
        public override void WriteJson(JsonWriter w, Quaternion v, JsonSerializer s)
            => JObject.FromObject(new { v.x, v.y, v.z, v.w }).WriteTo(w);

        public override Quaternion ReadJson(JsonReader reader, Type objectType, Quaternion existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Quaternion(obj["x"].Value<float>(), obj["y"].Value<float>(), obj["z"].Value<float>(), obj["w"].Value<float>());
        }
    }
}