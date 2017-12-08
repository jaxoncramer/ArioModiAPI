using System;
using Ario.API.Models.DisplayModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ario.API.Models.ModelBinders
{
    public class NodesDisplayModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context) 
        {
            if (context.Metadata.ModelType == typeof(NodesDisplay))
                return new NodesDisplayModelBinder();

            return null;
        }
    }
}
