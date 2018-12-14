Lemon
=====

Mini framework pro psaní parserů + jeho použití (pretty printery na XML a JSON).

> Zápočtový program pro C# - ZS 2018, Jiří Mayer


Zadání
------

Parsování bude fungovat podobně jako třeba fungují regulární výrazy,
jen parsery budou schopné načtený text rovnou převést na nějakou
datovou strukturu.

Parser dostane vstupní řetězec a pokusí se na jeho začátku nalézt shodu.
Pokud shodu nalezne, tak vrátí informace o tom, kolik znaků vstupu ukousl
a vrátí datovou položku odpovídající načtenému vstupu.

Ukázka používání frameworku:

```csharp
// načte celočíslenou hodnotu vstupu
Parser<int> parser = P.Regex<int>(@"[0-9]+")
    .Label("integer") // titulek pro chybové hlášky
    .Process((RegexParser p) => int.Parse(p.Text)) // vytvoření hodnoty
    .CreateParser();

parser.Parse("42");
int foo = parser.Success ? parser.Value : -1;

// jiný parser na zpracování C#-pového "using" příkazu
return P.Concat<AST.UsingExpression>(
    P.Literal("using"),
    MP.Whitespace(),        // (MP = MyParsers)
    MP.NamespacePath(),
    MP.Semicolon()
).Process(
    (p) => new AST.UsingExpression(p.part[2].Value)
);
```


Použití frameworku v programu
-----------------------------

Na ukázku použití frameworku naimplementuji dva pretty printery
(tedy programy, co vezmou vstupní kód a vytisknou ho správně naformátovaný).
Jeden jednodušší pro JSON a druhý složitější pro XML. Oba budou schované
v jednom projektu jako jeden spustitelný soubor.

Představa používání:

    $ PrettyPrint.exe input.json output.json
    $ PrettyPrint.exe input.xml output.xml
