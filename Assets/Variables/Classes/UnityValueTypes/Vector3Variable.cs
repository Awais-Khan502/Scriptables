// Vector3Variable.cs
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[UnityValueVariable]
[CreateAssetMenu(menuName = "Variables/Vector3Variable")]
public class Vector3Variable : ValueVariable<Vector3>
{
    protected override JsonConverter GetConverter() => new Vector3Converter();

    private class Vector3Converter : JsonConverter<Vector3>
    {
        public override void WriteJson(JsonWriter w, Vector3 v, JsonSerializer s)
            => JObject.FromObject(new { v.x, v.y, v.z }).WriteTo(w);

        public override Vector3 ReadJson(JsonReader reader, Type objectType, Vector3 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Vector3(obj["x"].Value<float>(), obj["y"].Value<float>(), obj["z"].Value<float>());
        }
    }
}
