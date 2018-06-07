# Konzolov� aplikace ukazuj�c� pou�it� Fluent Validation
Fluent je typ z�pisu, kdy metoda vrac� instanci objektu, ve kter� se nach�z�. �asto je pou�it v programovac�ch modelech nap�. [Builder pattern](https://ucitel.sps-prosek.cz/~maly/PRG/materials/csharp/#builder), vyu��v� ho tak� LINQ.

Umo��uje z�pis pomoc� te�kov� notace viz. n�sleduj�c� p��klad PersonBuilder.

---
## Person Builder
Metody WithEmail a WithFirstName vrac� instanci t��dy PersonBuilder, tedy this.
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
### Vytvo�en� instance Person
D�ky tomu, �e metody vrac� this, je mo�n� prov�st z�pis pomoc� te�kov� notace.
```csharp
PersonBuilder pValidPersonBuilder = new PersonBuilder()
    .WithFirstName("Jan")
    .WithLastName("Nov�k")
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
Pro z�pis pravidel, kter� ur�uj� spr�vnost dat ve t��d�, je ve knihovn� [FluentValidation](https://github.com/JeremySkinner/fluentvalidation) pou�it pr�v� tento styl z�pisu.
```csharp
RuleFor(person => person.LastName)
    .NotEmpty()
    .NotNull()
    .NotEqual(person => person.FirstName)
    .WithMessage("First name and last name could not be same");
```
Pro pou�it� je nutn�:
1) P�idat referenci na knihovnu FluentValidation do projektu
2) Vytvo�it Validator pro entitu
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
3) Vytvo�it instanci valid�toru
```csharp
PersonValidator validator = new PersonValidator();
```
4) Zavolat metodu Validate, p�edat j� instanci entity a zkontrolovat validitu, �i zpracovat chyby
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
## Roz���en� validace
### Validace pouze jedn� vlastnosti (Property)
N�kdy je pot�eba validovat pouze jednu vlastnost entity.

1) Vytvo�it extension metodu. Ta m��e b�t um�st�na v jak�koliv t��d�.
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
2) P�edat ji valid�tor entity, instanci entity a jm�no vlastnosti ke kontrole
```csharp
ValidationExtensions.Validate(personValidator, pValid, "ReferenceToAnotherClass");
```

### Validace z�visl�ch objekt�
P�id�n� pravidla, kter� vyu��v� metodu, kter� validuje z�visl� objekt.
```csharp
RuleFor(person => person.ReferenceToAnotherClass)
    .Cascade(CascadeMode.StopOnFirstFailure)
    .NotNull()
        .WithMessage("ReferenceToAnotherClass is null from SimplePropertyValidator")
    .Must(SimplePropertyValidator)
        .WithMessage("ReferenceToAnotherClass is not valid by SimplePropertyValidator ");
```
Validuj�c� metoda:
```csharp
private bool SimplePropertyValidator(ReferencedClassFromPerson SpecialProperty)
{
    // Validator for given class
    ReferencedClassFromPersonValidator specialPropertyValidator = new ReferencedClassFromPersonValidator();
    return specialPropertyValidator.Validate(SpecialProperty).IsValid;
}
```
Tato metoda, ale nevypisuje chyby z�visl�ho objektu.
### Vlastn� valid�tor
Pokud nesta�� metody, kter� poskytuje knihovna FluentValidation, je mo�n� napsat vlastn�.

N�sleduj�c� k�d validuje objekt _ReferencedClassFromPerson_, kter� je vlastnost� t��dy Person, spolu s celou instanc� t��dy Person. Vyskytnou-li se chyby p�i validaci z�visl�ho objektu, tak je instnace t��dy Person neplatn� a chyby jsou vyps�ny (v�. chyb validovovan�ho z�visl�ho objektu).
1) Vlastn� valid�tor
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
2) Pomocn� metoda

Metoda validuj�c� z�visl� objekt a vracej�c� v�sledek validace.
```csharp
private ValidationResult ExtendedPropertyValidator(ReferencedClassFromPerson SpecialProperty)
{
    // Validator for given class
    ReferencedClassFromPersonValidator specialPropertyValidator = new ReferencedClassFromPersonValidator();
    return specialPropertyValidator.Validate(SpecialProperty);
}
```