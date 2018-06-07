# Konzolov� aplikace ukazuj�c� pou�it� Fluent Validation
Fluent validace je typ z�pisu, kdy metoda vrac� instanci objektu, ve kter� se nach�z�.

```csharp
RuleFor(person => person.LastName)
    .NotEmpty()
    .NotNull()
    .NotEqual(person => person.FirstName)
    .WithMessage("First name and last name could not be same");
```


![Validation Output](Resources/ValidationOutput.png)