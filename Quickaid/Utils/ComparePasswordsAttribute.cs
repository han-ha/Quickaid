using System.ComponentModel.DataAnnotations;
namespace Quickaid.Utils;

public class ComparePasswordsAttribute : ValidationAttribute
{
    private readonly string _otherProperty;

    public ComparePasswordsAttribute(string otherProperty)
    {
        _otherProperty = otherProperty;
        ErrorMessage = "Hasła nie są zgodne";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        var otherProp = validationContext.ObjectType.GetProperty(_otherProperty);
        if (otherProp == null) return new ValidationResult($"Nie znaleziono właściwości {_otherProperty}");

        var otherValue = otherProp.GetValue(validationContext.ObjectInstance)?.ToString();
        if (value?.ToString() != otherValue)
            return new ValidationResult(ErrorMessage);

        return ValidationResult.Success;
    }
}
