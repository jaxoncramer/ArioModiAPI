using System;
using System.Collections;
using System.Collections.Generic;
using Ario.API.Models.DisplayModels;
using Ario.API.Models.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ario.API.Models.Converters
{
    public class NodeComponentDataConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return (objectType == typeof(NodeComponentData));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            JToken jo = JToken.Load(reader);

            if (jo is JObject)
            {
                if (((JObject)jo).GetValue("type", StringComparison.InvariantCultureIgnoreCase) != null)
                {
                    switch (((JObject)jo).GetValue("type", StringComparison.InvariantCultureIgnoreCase).Value<string>())
                    {
                        case "label":
                            LabelNodeComponentData labelData = new LabelNodeComponentData();
                            labelData.type = ((JObject)jo).GetValue("type", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                            if (((JObject)jo).GetValue("description", StringComparison.InvariantCultureIgnoreCase) != null)
                            {
                                labelData.description = ((JObject)jo).GetValue("description", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                            }
                            if (((JObject)jo).GetValue("color", StringComparison.InvariantCultureIgnoreCase) != null)
                            {
                                labelData.color = ((JObject)jo).GetValue("color", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                            }
                            if (((JObject)jo).GetValue("screenshot", StringComparison.InvariantCultureIgnoreCase) != null)
                            {
                                labelData.screenshot = ((JObject)jo).GetValue("screenshot", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                            }
                            return labelData;

                        case "qranchor":
                            QRAnchorNodeComponentData anchorData = new QRAnchorNodeComponentData();
                            anchorData.type = ((JObject)jo).GetValue("type", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                            if (((JObject)jo).GetValue("qranchorid", StringComparison.InvariantCultureIgnoreCase) != null)
                            {
                                anchorData.qrAnchorID = ((JObject)jo).GetValue("qranchorid", StringComparison.InvariantCultureIgnoreCase).Value<Int64>();
                            }
                            return anchorData;

                        case "pdf":
                            PDFNodeComponentData pdfData = new PDFNodeComponentData();
                            pdfData.type = ((JObject)jo).GetValue("type", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                            if (((JObject)jo).GetValue("pdflink", StringComparison.InvariantCultureIgnoreCase) != null)
                            {
                                pdfData.pdfLink = ((JObject)jo).GetValue("pdflink", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                            }
                            if (((JObject)jo).GetValue("title", StringComparison.InvariantCultureIgnoreCase) != null)
                            {
                                pdfData.title = ((JObject)jo).GetValue("title", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                            }
                            if (((JObject)jo).GetValue("description", StringComparison.InvariantCultureIgnoreCase) != null)
                            {
                                pdfData.description = ((JObject)jo).GetValue("description", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                            }
                            return pdfData;
                    }
                }
            }
            return null;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }
    }
}
