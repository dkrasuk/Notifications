using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.ModelBinding;
using Newtonsoft.Json;

namespace Notifications.WebAPI.Models.Binders
{
    /// <summary>
    /// Model binder for array in url
    /// </summary>
    public class ArrayModelBinder : IModelBinder
    {
        public bool BindModel(HttpActionContext actionContext, ModelBindingContext bindingContext)
        {
            var key = bindingContext.ModelName;
            if (actionContext.Request.Method != HttpMethod.Get)
            {
                var stringBody = actionContext.Request.Content.ReadAsStringAsync().Result;
                var type = bindingContext.ModelType;
                bindingContext.Model = JsonConvert.DeserializeObject(stringBody, type);
                return true;
            }
            else
            {
                var val = bindingContext.ValueProvider.GetValue(key);
                if (val != null)
                {
                    var s = val.AttemptedValue;
                    if (s != null)
                    {
                        var elementType = bindingContext.ModelType.GetElementType();
                        var converter = TypeDescriptor.GetConverter(elementType);
                        var values = Array.ConvertAll(s.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries),
                            x => { return converter.ConvertFromString(x != null ? x.Trim() : x); });

                        var typedValues = Array.CreateInstance(elementType, values.Length);

                        values.CopyTo(typedValues, 0);

                        bindingContext.Model = typedValues;
                    }
                    else
                    {
                        // change this line to null if you prefer nulls to empty arrays 
                        bindingContext.Model = Array.CreateInstance(bindingContext.ModelType.GetElementType(), 0);
                    }
                    return true;
                }
            }
            return false;
        }
    }
}
