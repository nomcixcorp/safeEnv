using System.ComponentModel.DataAnnotations;
using envSafe.Api.Models;

namespace envSafe.Api.Extensions;

public static class ModelValidationExtensions
{
    public static bool TryValidate(this GenerateRequest request, out string error)
    {
        var validationContext = new ValidationContext(request);
        var validationResults = new List<ValidationResult>();
        var valid = Validator.TryValidateObject(request, validationContext, validationResults, validateAllProperties: true);

        if (!valid)
        {
            error = validationResults.First().ErrorMessage ?? "Invalid request.";
            return false;
        }

        if (!Enum.IsDefined(request.ValueType))
        {
            error = "valueType is invalid.";
            return false;
        }

        if (!Enum.IsDefined(request.Mode))
        {
            error = "mode is invalid.";
            return false;
        }

        error = string.Empty;
        return true;
    }
}
