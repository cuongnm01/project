using Common.Resources;
using System.ComponentModel.DataAnnotations;

namespace Common.Annotations
{
    public class CxRequiredAttribute : RequiredAttribute
    {
        public CxRequiredAttribute()
        {
            //The {0} field is required.
            ErrorMessage = AnnotationResource.Required;
        }
    }
}
