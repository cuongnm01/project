using Common.Resources;
using System.ComponentModel.DataAnnotations;

namespace Common.Annotations
{
    public class CxRegularExpressionAttribute : RegularExpressionAttribute
    {
        public CxRegularExpressionAttribute(string pattern) : base(pattern)
        {
            //The field {0} must match the regular expression '{1}'.
            ErrorMessage = AnnotationResource.RegularExpression;
        }
    }
}
