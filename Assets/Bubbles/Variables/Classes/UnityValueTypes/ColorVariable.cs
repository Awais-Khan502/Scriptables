// Vector3Variable.cs
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[UnityValueVariable]
[CreateAssetMenu(menuName = "Variables/ColorVariable")]
public class ColorVariable : ValueVariable<Color>
{
    protected override JsonConverter GetConverter() => new ColorConverter();

    private class ColorConverter : JsonConverter<Color>
    {
        public override void WriteJson(JsonWriter w, Color v, JsonSerializer s)
            => JObject.FromObject(new { v.r, v.g, v.b, v.a }).WriteTo(w);

        public override Color ReadJson(JsonReader reader, Type objectType, Color existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj = JObject.Load(reader);
            return new Color(obj["r"].Value<float>(), obj["g"].Value<float>(), obj["b"].Value<float>(), obj["a"].Value<float>());
        }
    }

}
