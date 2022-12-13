using Common.Resources;
using System.ComponentModel.DataAnnotations;

namespace Common.Annotations
{
    public class CxMinLengthAttribute : MinLengthAttribute
    {
        public CxMinLengthAttribute(int length) : base(length)
        {
            //The field {0} must be a string or array type with a minimum length of '{1}'.
            ErrorMessage = AnnotationResource.MinLength;
        }
    }
}
