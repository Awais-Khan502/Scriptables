// Vector3Variable.cs
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[UnityValueVariable]
[CreateAssetMenu(menuName = "Variables/RectVariable")]
public class RectVariable : ValueVariable<Rect>
{
    protected override JsonConverter GetConverter() => new RectConverter();

    private class RectConverter : JsonConverter<Rect>
    {
        public override void WriteJson(JsonWriter w, Rect v, JsonSerializer s)
            => JObject.FromObject(new { v.x, v.y, v.width, v.height }).WriteTo(w);

        public override Rect ReadJson(JsonReader reader, Type objectType, Rect existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Rect(obj["x"].Value<float>(), obj["y"].Value<float>(), obj["width"].Value<float>(), obj["height"].Value<float>());
        }
    }
}
