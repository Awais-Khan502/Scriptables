// Vector2Variable.cs
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[UnityValueVariable]
[CreateAssetMenu(menuName = "Variables/Vector2Variable")]
public class Vector2Variable : ValueVariable<Vector2>
{
    protected override JsonConverter GetConverter() => new Vector2Converter();
    private class Vector2Converter : JsonConverter<Vector2>
    {
        public override void WriteJson(JsonWriter w, Vector2 v, JsonSerializer s)
            => JObject.FromObject(new { v.x, v.y }).WriteTo(w);

        public override Vector2 ReadJson(JsonReader reader, Type objectType, Vector2 existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Vector2(obj["x"].Value<float>(), obj["y"].Value<float>());
        }
    }
}