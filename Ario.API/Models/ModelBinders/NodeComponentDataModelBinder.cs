using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Ario.API.Models.Converters;
using Ario.API.Models.Objects;
using System.Collections.Generic;

namespace Ario.API.Models.ModelBinders
{
    public class NodeComponentDataModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
                throw new ArgumentNullException(nameof(bindingContext));

            string valueFromBody = string.Empty;

            using (var sr = new StreamReader(bindingContext.HttpContext.Request.Body))
            {
                valueFromBody = sr.ReadToEnd();
            }

            if (string.IsNullOrEmpty(valueFromBody))
            {
                return Task.CompletedTask;
            }

            JsonConverter[] converters = { new NodeComponentDataConverter() };
            var data = JsonConvert.DeserializeObject<NodeComponentData>(valueFromBody,
                                new JsonSerializerSettings() { Converters = converters });

            bindingContext.Result = ModelBindingResult.Success(data);

            return Task.CompletedTask;
        }
    }
}
