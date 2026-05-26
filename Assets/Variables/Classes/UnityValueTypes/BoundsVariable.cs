// Vector3Variable.cs
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using UnityEngine;

[UnityValueVariable]
[CreateAssetMenu(menuName = "Variables/BoundsVariable")]
public class BoundsVariable : ValueVariable<Bounds>
{
    protected override JsonConverter GetConverter() => new BoundsConverter();

    private class BoundsConverter : JsonConverter<Bounds>
    {
        public override void WriteJson(JsonWriter w, Bounds v, JsonSerializer s)
            => JObject.FromObject(new { center = new { v.center.x, v.center.y, v.center.z }, size = new { v.size.x, v.size.y, v.size.z } }).WriteTo(w);

        public override Bounds ReadJson(JsonReader reader, Type objectType, Bounds existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            var obj    = JObject.Load(reader);
            var center = new Vector3(obj["center"]["x"].Value<float>(), obj["center"]["y"].Value<float>(), obj["center"]["z"].Value<float>());
            var size   = new Vector3(obj["size"]["x"].Value<float>(),   obj["size"]["y"].Value<float>(),   obj["size"]["z"].Value<float>());
            return new Bounds(center, size);
        }
    }

}
