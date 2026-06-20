# simplesound C# code formatting!

## Identifier Name Casing:

```
Constants: UPPERCASE_SNAKE_CASE
Variables / Arguments: snake_case
Methods / Actions / Enums / Structs / Classes: PascalCase
Private Indication (exclude constants): _[Identifier]
```

## Spacing

* All tabs / indents must be 4 spaces (this is default in vscode).

* No adding in extra spaces than what is required for the code to compile.

* All curly braces must be formatted in [K&R](https://en.wikipedia.org/wiki/Indentation_style#K&R)

* braces that only contain one statement should be collapsed to a one liner unless it is too long or unable to compile.

## Reminders

* Any (variable / method / action) that isn't public or is used by a child class should be made private

* All C# file names should be in PascalCase and named the same as the class / struct they contain.

* Document functions and variables when you can.

* Use built in godot functions as much as possible.