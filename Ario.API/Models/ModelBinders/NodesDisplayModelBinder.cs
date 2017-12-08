using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;
using Ario.API.Models.DisplayModels;
using Ario.API.Models.Converters;

namespace Ario.API.Models.ModelBinders
{
    public class NodesDisplayModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext) {
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

            JsonConverter[] converters = { new NodeConverter()};
            NodesDisplay disp = JsonConvert.DeserializeObject<NodesDisplay>(valueFromBody,
                                new JsonSerializerSettings() { Converters = converters});

            bindingContext.Result = ModelBindingResult.Success(disp);

            return Task.CompletedTask; 
        }
    }
}
