using System.ComponentModel.DataAnnotations;

namespace Services.Helpers;

public class ValidationHelper
{
    internal static void ValidateModel<T>(T model)
    {
        if (model is null) throw new ArgumentNullException($"{model} is not supplied");
        var validationContext = new ValidationContext(model);
        var results = new List<ValidationResult>();
        bool isVaild = Validator.TryValidateObject(model, validationContext, results, true);
        if (!isVaild)
            throw new ArgumentException(results.FirstOrDefault()?.ErrorMessage);
    }
}