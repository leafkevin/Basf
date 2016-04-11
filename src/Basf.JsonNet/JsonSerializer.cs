using Basf.Serializing;
using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using Newtonsoft.Json.Converters;

namespace Basf.JsonNet
{
    public class JsonSerializer : IJsonSerializer
    {
        public JsonSerializerSettings Settings { get; private set; }
        public JsonSerializer()
        {
            this.Settings = new JsonSerializerSettings();
            IsoDateTimeConverter dateConvertor = new IsoDateTimeConverter();
            dateConvertor.DateTimeFormat = "yyyy-MM-dd HH:mm:ss";
            StringEnumConverter enumConverter = new StringEnumConverter();
            this.Settings.Converters = new List<JsonConverter> { dateConvertor, enumConverter };
        }
        public object Deserialize(string value, Type type)
        {
            return JsonConvert.DeserializeObject(value, type, Settings);
        }
        public TObject Deserialize<TObject>(string value) where TObject : class
        {
            return JsonConvert.DeserializeObject<TObject>(JObject.Parse(value).ToString(), Settings);
        }
        public string Serialize(object obj)
        {
            return JsonConvert.SerializeObject(obj, Settings);
        }
    }
}
