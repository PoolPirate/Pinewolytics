using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel;

namespace Pinewolytics.Utils;

public class CommaDelimitedArrayModelBinder : IModelBinder
{
    public Task BindModelAsync(ModelBindingContext bindingContext)
    {
        var modelName = bindingContext.ModelName;
        var valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);

        var elementType = bindingContext.ModelType.GetElementType();

        if (elementType is null)
        {
            throw new ArgumentNullException(nameof(elementType));
        }

        var converter = TypeDescriptor.GetConverter(elementType);

        var s = valueProviderResult.FirstValue ?? "";
        var values = Array.ConvertAll(s.Split(',', StringSplitOptions.RemoveEmptyEntries),
            x => converter.ConvertFromString(x != null ? x.Trim() : ""));

        var typedValues = Array.CreateInstance(elementType, values.Length);

        values.CopyTo(typedValues, 0);

        bindingContext.Result = ModelBindingResult.Success(typedValues);
        return Task.CompletedTask;
    }
}