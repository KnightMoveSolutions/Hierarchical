using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace KnightMoves.Hierarchical
{
    public class TreeNodeJsonConverter : JsonConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null) return null;

            var jObj = JObject.Load(reader);

            var nodeType = Type.GetType((string)jObj["TypeName"]);

            var treeNode = nodeType.Assembly.CreateInstance(nodeType.FullName);

            reader = jObj.CreateReader();

            serializer.Populate(reader, treeNode);

            return treeNode;
        }

        public override bool CanWrite => false;

        public override bool CanConvert(Type objectType) => throw new NotImplementedException();

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) 
            => throw new NotImplementedException();
    }
}
