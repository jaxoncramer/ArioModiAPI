using System;
using System.Collections;
using System.Collections.Generic;
using Ario.API.Models.DisplayModels;
using Ario.API.Models.Objects;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Ario.API.Models.Converters
{
    public class NodeConverter : JsonConverter
    {

        public override bool CanConvert(Type objectType) 
        {
            return (objectType == typeof(NodesDisplay));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer) 
        {

            JObject o = JObject.Load(reader);
            NodesDisplay disp = new NodesDisplay(o);

            if (o.GetValue("components", StringComparison.InvariantCultureIgnoreCase) != null)
            {
                foreach (JObject jo in o.GetValue("components", StringComparison.InvariantCultureIgnoreCase))
                {
                    if (jo.GetValue("type", StringComparison.InvariantCultureIgnoreCase) != null)
                    {
                        switch (jo.GetValue("type", StringComparison.InvariantCultureIgnoreCase).Value<string>())
                        {
                            case "label":
                                LabelNodeComponentData labelData = new LabelNodeComponentData();
                                labelData.type = jo.GetValue("type", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                                if (jo.GetValue("nodecomponentid", StringComparison.InvariantCultureIgnoreCase) != null)
                                {
                                    labelData.nodeComponentID = jo.GetValue("nodecomponentid", StringComparison.InvariantCultureIgnoreCase).Value<Int64>();
                                }
                                if (jo.GetValue("description", StringComparison.InvariantCultureIgnoreCase) != null)
                                {
                                    labelData.description = jo.GetValue("description", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                                }
                                if (jo.GetValue("color", StringComparison.InvariantCultureIgnoreCase) != null)
                                {
                                    labelData.color = jo.GetValue("color", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                                }
                                if (jo.GetValue("screenshot", StringComparison.InvariantCultureIgnoreCase) != null)
                                {
                                    labelData.screenshot = jo.GetValue("screenshot", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                                }
                                disp.components.Add(labelData);
                                break;

                            case "qranchor":
                                QRAnchorNodeComponentData anchorData = new QRAnchorNodeComponentData();
                                anchorData.type = jo.GetValue("type", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                                if (jo.GetValue("nodecomponentid", StringComparison.InvariantCultureIgnoreCase) != null)
                                {
                                    anchorData.nodeComponentID = jo.GetValue("nodecomponentid", StringComparison.InvariantCultureIgnoreCase).Value<Int64>();
                                }

                                if (jo.GetValue("qranchorid", StringComparison.InvariantCultureIgnoreCase) != null)
                                {
                                    anchorData.qrAnchorID = jo.GetValue("qranchorid", StringComparison.InvariantCultureIgnoreCase).Value<Int64>();
                                }
                                disp.components.Add(anchorData);
                                break;

                            case "pdf":
                                PDFNodeComponentData pdfData = new PDFNodeComponentData();
                                pdfData.type = jo.GetValue("type", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                                if (jo.GetValue("nodecomponentid", StringComparison.InvariantCultureIgnoreCase) != null)
                                {
                                    pdfData.nodeComponentID = jo.GetValue("nodecomponentid", StringComparison.InvariantCultureIgnoreCase).Value<Int64>();
                                }

                                if (jo.GetValue("pdflink", StringComparison.InvariantCultureIgnoreCase) != null)
                                {
                                    pdfData.pdfLink = jo.GetValue("pdflink", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                                }

                                if (jo.GetValue("title", StringComparison.InvariantCultureIgnoreCase) != null)
                                {
                                    pdfData.title = jo.GetValue("title", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                                }

                                if (jo.GetValue("description", StringComparison.InvariantCultureIgnoreCase) != null)
                                {
                                    pdfData.description = jo.GetValue("description", StringComparison.InvariantCultureIgnoreCase).Value<string>();
                                }
                                disp.components.Add(pdfData);
                                break;
                        }
                    }
                }
            }

            return disp;
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override void WriteJson (JsonWriter writer, object value, JsonSerializer serializer) 
        {
            throw new NotImplementedException();
        }
    }
}
