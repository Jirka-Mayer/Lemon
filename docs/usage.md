Používání knihovny
==================

Parser je něco, co dostane vstup a on oznámí, že vstup splňuje jeho kritéria. Stejně jako fungují regulární výrazy.
Lemon je kolekce základních parserů které lze kombinovat pro stavbu dalších, složitějších.

> Hlavní místo použití knihovny v programu jsou soubory [JP.cs](Convertor/Json/JP.cs) a [XP.cs](Convertor/Xml/XP.cs).

Většinou když potřebuju parsovat složitý vstup, tak použiju několik parserů, ty si pojmenuju a poskládám je dohromady.
Konvence je taková, že jeden parser je reprezentovaný jednou metodou. Metody budou statické a budou v
jedné třídě pohromadě.

Takže řekněme, že stavíme kolekci mých parserů (MyParsers) `MP.cs`:

```csharp
using Lemon;

static class MP
{
    // ... moje parsery ...
}
```

Můžeme chtít otestovat, že vstup začíná textem `Hello world!`.

```csharp
static class MP
{
    public static ParserFactory<string> HelloWorld()
    {
        return P.Literal("Hello world!");
    }
}
```

Nebo že se jedná o číslo:

```csharp
static class MP
{
    public static ParserFactory<string> NumberString()
    {
        return P.StringRegex("\d+");
    }
}
```

A možná nás nezajímá text čísla, ale jeho vlastní hodnota:

```csharp
static class MP
{
    public static ParserFactory<int> Number()
    {
        return P.Regex<int>("\d+")
            .Process(p => int.Parse(p .GetMatchedText()));
    }
}
```

Nebo chceme tyhle dva naše parsery aplikovat po sobě a složený celek má mít hodnotu toho čísla:

```csharp
static class MP
{
    public static ParserFactory<int> BothParsers()
    {
        return P.Concat<int>(
            MP.HelloWorld(),
            MP.Number()
        ).Process(p => ((Parser<int>)p.Parts[1]).Value);
    }
}
```

K dispozici jsou následující parsery, ostatní si člověk musí poskládat z nich:

- `P.Literal<TValue>(string literal)` Uspěje, pokud vstup obsahuje doslova zadaný řetězec.
- `P.Regex<TValue>(string pattern)` Uspěje, když uspěje regulární výraz.
- `P.Concat<TValue>(params ParserFactory[] parts)` Spojení více parserů za sebe. Konjunkce.
- `P.Any<TValue>(params ParserFactory<TValue>[] parts)` Aplikuje parsery jeden po druhém, dokud není nějaký úspěšný. Disjunkce nebo alternace.
- `P.Repeat<TValue, TSubValue>(Parser<TSubValue> parser, quantification)` Konkatenace jednoho parseru v daném množství.
- `P.Optional<TValue>(Parser<TValue> parser)` Vždy úspěšné; může nebo nemusí aplikovat parser.
- `P.Cast<TTo, TFrom>(Parser<TFrom> parser)` Přetypuje hodnotu vracenou parserem.

Když chci parser zavolat na vstupu, tak musím vytvořit jeho instanci, zavolat metodu `Parse` a potom z něho můžu vyčíst výsledek parsování.

```csharp
Parser<int> p = MP.Number().CreateAbstractValuedParser();

p.Parse("42");

p.Success // podařilo se matchnout vstup?
p.Value // pokud ano, tak jakou má hodnotu (to co vrátí argument metody .Process(...))
p.Exception // pokud ne, tak co se nepovedlo
```
