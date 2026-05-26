// Vector3Variable.cs
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[UnityValueVariable]
[CreateAssetMenu(menuName = "Variables/Vector3IntVariable")]
public class Vector3IntVariable : ValueVariable<Vector3Int>
{
    protected override JsonConverter GetConverter() => new Vector3IntConverter();

    private class Vector3IntConverter : JsonConverter<Vector3Int>
    {
        public override void WriteJson(JsonWriter w, Vector3Int v, JsonSerializer s)
            => JObject.FromObject(new { v.x, v.y, v.z }).WriteTo(w);

        public override Vector3Int ReadJson(JsonReader reader, Type objectType, Vector3Int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Vector3Int(obj["x"].Value<int>(), obj["y"].Value<int>(), obj["z"].Value<int>());
        }
    }
}
