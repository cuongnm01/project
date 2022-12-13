using Common.Resources;
using System.ComponentModel.DataAnnotations;

namespace Common.Annotations
{
    public class CxStringLengthAttribute : StringLengthAttribute
    {
        public CxStringLengthAttribute(int maximumLength) : base(maximumLength)
        {
            //The field {0} must be a string with a maximum length of {1}.
            ErrorMessage = AnnotationResource.StringLength;
        }
    }
}
