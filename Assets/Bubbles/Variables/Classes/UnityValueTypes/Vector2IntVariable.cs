// Vector2IntVariable.cs
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[UnityValueVariable]
[CreateAssetMenu(menuName = "Variables/Vector2IntVariable")]
public class Vector2IntVariable : ValueVariable<Vector2Int>
{
    protected override JsonConverter GetConverter() => new Vector2IntConverter();

    private class Vector2IntConverter : JsonConverter<Vector2Int>
    {
        public override void WriteJson(JsonWriter w, Vector2Int v, JsonSerializer s)
            => JObject.FromObject(new { v.x, v.y }).WriteTo(w);

        public override Vector2Int ReadJson(JsonReader reader, Type objectType, Vector2Int existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Vector2Int(obj["x"].Value<int>(), obj["y"].Value<int>());
        }
    }
}
