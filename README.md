# Konzolová aplikace ukazující použití Fluent Validation
Fluent je typ zápisu, kdy metoda vrací instanci objektu, ve které se nachází. Často je použit v programovacích modelech např. [Builder pattern](https://ucitel.sps-prosek.cz/~maly/PRG/materials/csharp/#builder), využívá ho také LINQ.

Umožňuje zápis pomocí tečkové notace viz. následující příklad PersonBuilder.

---
## Person Builder
Metody WithEmail a WithFirstName vrací instanci třídy PersonBuilder, tedy this.
```csharp
class PersonBuilder
{
    String email;
    String firstName;
    String lastName;
    DateTime dateOfBirth = new DateTime();
    ReferencedClassFromPerson referencedClassFromPerson;

    public PersonBuilder WithEmail(string email)
    {
        this.email = email;
        return this;
    }

    public PersonBuilder WithFirstName(string firstName)
    {
        this.firstName = firstName;
        return this;
    }
...
}
```
### Vytvoření instance Person
Díky tomu, že metody vrací this, je možné provést zápis pomocí tečkové notace.
```csharp
PersonBuilder pValidPersonBuilder = new PersonBuilder()
    .WithFirstName("Jan")
    .WithLastName("Novák")
    .WithDateOfBirth(new DateTime(1993, 1, 1))
    .WithEmail("john.doe@email.com")
    .WithReferencedClass(new ReferencedClassFromPerson()
    {
        Property = "some value"
    });

Person pValid = pValidPersonBuilder.Build();
```
---
## Fluent Validation
![Validation Output](Resources/ValidationOutput.png)

Pro zápis pravidel, která určují správnost dat ve třídě, je ve knihovně [FluentValidation](https://github.com/JeremySkinner/fluentvalidation) použit právě tento styl zápisu.
```csharp
RuleFor(person => person.LastName)
    .NotEmpty()
    .NotNull()
    .NotEqual(person => person.FirstName)
    .WithMessage("First name and last name could not be same");
```
Pro použití je nutné:
1) Přidat referenci na knihovnu FluentValidation do projektu
2) Vytvořit Validator pro entitu
```csharp
class PersonValidator : AbstractValidator<Person>
{
    public PersonValidator()
    {
        RuleFor(person => person.FirstName)
            .NotEmpty();

        RuleFor(person => person.LastName)
            .NotEmpty()
            .NotEqual(person => person.FirstName)
            .WithMessage("First name and last name could not be same");
...
}
```
3) Vytvořit instanci validátoru
```csharp
PersonValidator validator = new PersonValidator();
```
4) Zavolat metodu Validate, předat jí instanci entity a zkontrolovat validitu, či zpracovat chyby
```csharp
// p is instance of Person
ValidationResult results = validator.Validate(p);
if (results.IsValid)
{
    Console.WriteLine("Valid");
}
else
{
    results.Errors.ToList().ForEach(i => Console.WriteLine(i.ToString()));
}
```
---
## Rozšíření validace
### Validace pouze jedné vlastnosti (Property)
Někdy je potřeba validovat pouze jednu vlastnost entity.

1) Vytvořit extension metodu. Ta může být umístěna v jakékoliv třídě.
```csharp
/// <param name="validator">Validator for class</param>
/// <param name="instance">Instance of class to be validated</param>
/// <param name="properties">Name of property to validate</param>
public static ValidationResult Validate<T>(this IValidator validator, T instance, params string[] properties)
{
    var context = new ValidationContext<T>(instance, new PropertyChain(), ValidatorOptions.ValidatorSelectors.MemberNameValidatorSelectorFactory(properties));
    return validator.Validate(context);
}
```
2) Předat ji validátor entity, instanci entity a jméno vlastnosti ke kontrole
```csharp
ValidationExtensions.Validate(personValidator, pValid, "Email");
```

### Validace závislých objektů
Přidání pravidla, které využívá pomocné metody. Ta vrací True/False, dle úspěšnosti validace závislého objektu.
```csharp
RuleFor(person => person.ReferenceToAnotherClass)
    .Cascade(CascadeMode.StopOnFirstFailure)
    .NotNull()
        .WithMessage("ReferenceToAnotherClass is null from SimplePropertyValidator")
    .Must(SimplePropertyValidator)
        .WithMessage("ReferenceToAnotherClass is not valid by SimplePropertyValidator ");
```
Validující metoda vytvoří instanci validátoru pro závislý objekt a zkontroluje ho.
```csharp
private bool SimplePropertyValidator(ReferencedClassFromPerson SpecialProperty)
{
    // Validator for given class
    ReferencedClassFromPersonValidator specialPropertyValidator = new ReferencedClassFromPersonValidator();
    return specialPropertyValidator.Validate(SpecialProperty).IsValid;
}
```
Tato metoda, ale nevypisuje chyby závislého objektu.
### Vlastní validátor
Pokud nestačí metody, které poskytuje knihovna FluentValidation, je možné napsat vlastní.

Následující kód validuje objekt Person spolu s jeho vlastnotí, která je typu jiného objektu (_ReferencedClassFromPerson_). 

Vyskytnou-li se chyby při validaci závislého objektu, tak je instnace třídy Person neplatná a chyby jsou vypsány (vč. chyb validovovaného závislého objektu).
1) Vlastní validátor
```csharp
// Validate referenced object and print errors
RuleFor(person => person.ReferenceToAnotherClass)
    .Cascade(CascadeMode.StopOnFirstFailure)
    .NotNull()
        .WithMessage("ReferenceToAnotherClass is null from ExtendedPropertyValidator")
    .Custom((person, context) =>
    {
        ValidationResult result = ExtendedPropertyValidator(person);   
        if (!result.IsValid)
        {
            context.AddFailure(
                string.Join(",", result.Errors)
                +" by ExtendedPropertyValidator");
        } 
    });
```
2) Pomocná metoda

Metoda validující závislý objekt a vracející výsledek validace.
```csharp
private ValidationResult ExtendedPropertyValidator(ReferencedClassFromPerson SpecialProperty)
{
    // Validator for given class
    ReferencedClassFromPersonValidator specialPropertyValidator = new ReferencedClassFromPersonValidator();
    return specialPropertyValidator.Validate(SpecialProperty);
}
```