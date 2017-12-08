using System.Collections.Generic;
using Ario.API.Models.Objects;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Ario.API.Models.ModelBinders
{
    public class NodeComponentDataModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context.Metadata.ModelType == typeof(List<NodeComponentData>))
                return new NodeComponentDataModelBinder();

            return null;
        }
    }
}
