using System.ComponentModel.DataAnnotations;

namespace Quickaid.Utils
{
    // Atrybut walidacyjny do porównywania dwóch pól hasła
    // Sprawdza, czy wartość pola jest równa wartości innego pola (potwierdzenie hasła)
    public class ComparePasswordsAttribute : ValidationAttribute
    {
        private readonly string _otherProperty;

        // Konstruktor przyjmujący nazwę drugiego pola do porównania
        public ComparePasswordsAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
            ErrorMessage = "Hasła nie są zgodne";
        }

        // Waliduje, czy wartość pola jest równa wartości drugiego pola
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            // Pobranie właściwości do porównania po nazwie
            var otherProp = validationContext.ObjectType.GetProperty(_otherProperty);
            if (otherProp == null)
                return new ValidationResult($"Nie znaleziono właściwości {_otherProperty}");

            var otherValue = otherProp.GetValue(validationContext.ObjectInstance)?.ToString();

            // Porównanie wartości pól
            if (value?.ToString() != otherValue)
                return new ValidationResult(ErrorMessage);

            return ValidationResult.Success;
        }
    }
}
