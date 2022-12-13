using Common.Resources;
using System.ComponentModel.DataAnnotations;

namespace Common.Annotations
{
    public class CxMaxLengthAttribute : MaxLengthAttribute
    {
        public CxMaxLengthAttribute()
        {
            //The field {0} must be a string or array type with a maximum length of '{1}'.
            ErrorMessage = AnnotationResource.MaxLength;
        }

        public CxMaxLengthAttribute(int length) : base(length)
        {
            ErrorMessage = AnnotationResource.MaxLength;
        }
    }
}
