# Konzolová aplikace ukazující použití Fluent Validation
Fluent je typ zápisu, kdy metoda vrací instanci objektu, ve které se nachází. Èasto je použit v programovacích modelech napø. [Builder pattern](https://ucitel.sps-prosek.cz/~maly/PRG/materials/csharp/#builder), využívá ho také LINQ.

Umožòuje zápis pomocí teèkové notace viz. následující pøíklad PersonBuilder.

---
## Person Builder
Metody WithEmail a WithFirstName vrací instanci tøídy PersonBuilder, tedy this.
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
### Vytvoøení instance Person
Díky tomu, že metody vrací this, je možné provést zápis pomocí teèkové notace.
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
Pro zápis pravidel, která urèují správnost dat ve tøídì, je ve knihovnì [FluentValidation](https://github.com/JeremySkinner/fluentvalidation) použit právì tento styl zápisu.
```csharp
RuleFor(person => person.LastName)
    .NotEmpty()
    .NotNull()
    .NotEqual(person => person.FirstName)
    .WithMessage("First name and last name could not be same");
```
Pro použití je nutné:
1) Pøidat referenci na knihovnu FluentValidation do projektu
2) Vytvoøit Validator pro entitu
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
3) Vytvoøit instanci validátoru
```csharp
PersonValidator validator = new PersonValidator();
```
4) Zavolat metodu Validate, pøedat jí instanci entity a zkontrolovat validitu, èi zpracovat chyby
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
## Rozšíøení validace
### Validace pouze jedné vlastnosti (Property)
Nìkdy je potøeba validovat pouze jednu vlastnost entity.

1) Vytvoøit extension metodu. Ta mùže být umístìna v jakékoliv tøídì.
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
2) Pøedat ji validátor entity, instanci entity a jméno vlastnosti ke kontrole
```csharp
ValidationExtensions.Validate(personValidator, pValid, "ReferenceToAnotherClass");
```

### Validace závislých objektù
Pøidání pravidla, které využívá metodu, která validuje závislý objekt.
```csharp
RuleFor(person => person.ReferenceToAnotherClass)
    .Cascade(CascadeMode.StopOnFirstFailure)
    .NotNull()
        .WithMessage("ReferenceToAnotherClass is null from SimplePropertyValidator")
    .Must(SimplePropertyValidator)
        .WithMessage("ReferenceToAnotherClass is not valid by SimplePropertyValidator ");
```
Validující metoda:
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
Pokud nestaèí metody, které poskytuje knihovna FluentValidation, je možné napsat vlastní.

Následující kód validuje objekt _ReferencedClassFromPerson_, který je vlastností tøídy Person, spolu s celou instancí tøídy Person. Vyskytnou-li se chyby pøi validaci závislého objektu, tak je instnace tøídy Person neplatná a chyby jsou vypsány (vè. chyb validovovaného závislého objektu).
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