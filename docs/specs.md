Specifikace
===========

Detailní specifikace zápočtového programu na C# ZS 2018 - Jiří Mayer.

Rozhodl jsem se udělat knihovnu na psaní parserů. Inspiroval jsem se
knihovnami [PEG.js](https://pegjs.org/) a [Nearley.js](https://nearley.js.org/).
Nebudu ovšem vymýšlet vlastní jazyk pro popis gramatiky, ale pokusím se sestavit
takové API, které bude pochopitelné přímo v C# kódu.

Jako zápočtový program nelze odevzat knihovnu, takže s její pomocí ještě napíšu
překladač z JSONu do XML.

Nejprve popíšu knihovnu, překladač bude specifikovaný dole.


Lemon
-----

Parser je objekt, který dostane vstupní řetězec a pokusí se na jeho začátku nalézt
podřetězec, který splňuje kritéria parseru. Pokud hledání dopadne kladně, tak se provede
zpracování řetězce a parser vrátí objekt, který řetězec nějak reprezentuje. Typ vraceného
objektu je hodnota typového parametru parseru.

```csharp
// mějme nějaký parser na čtení celých čísel
Parser<int> parser = someParserSomehowCreated;

// zkusíme parsovat nějaký vstup
parser.Parse("42 and the rest of the input string");

// výsledek parsování se drží uvnitř parseru jako stav
bool success = parser.Success; // máme match?
int number = parser.Value; // pokud ano, tak jakou má hodnotu
int len = parser.MatchedLength; // kolik znaků ze vstupu si parser sebral?
string m = parser.GetMatchedString(); // vrátí sebraný řetězec ze vstupu
```

Parser je určený na jednorázové použití, jelikož si drží stav. Můžeme se ho dotázat, zda ještě
nedostal žádný vstup `parser.IsPristine`. Pokud se zeptáme na výsledek parsování před předáním
vstupu, tak dostaneme výjimku `ParserStillPristineException`. Pokud mu naopak už vstup
předán byl a my mu dáváme další, tak dostaneme výjimku `ParserAlreadyUsedException`.

> Myšlenka je taková, aby byl jeho stav immutable.

> **Poznámka:** Parser bere jako vstup `string` a ne `Stream`, protože chci mít k dispozici
regulární výrazy a také mít přístup ke vstupnímu textu i potom, co parser seběhl. To znamená,
že pro používání knihovny na souborech musím parsovat buď po nějakých celcích (např. řádky),
nebo mít rozumně velké (malé) soubory, aby se celé vešly do paměti.


### Definice parseru

Parser mohu vyvořit přímou specifikací chování tak, že budu dědit od `Parser<T>`. Běžnější způsob
ale je poskládat parser z více jednodušších parserů.

Řekněme, že chceme parsovat desetinná čísla. Tedy chceme načíst celé číslo, tečku a celé číslo.

```csharp
Parser<double> parser = P.Concat<double>(
    MP.IntegerNumber(),
    P.Literal("."),
    MP.IntegerNumber()
).Process(
    (p) => double.Parse(p.GetMatchedString())
).CreateParser();
```

> **Poznámka:** `P` znamená "parsers" a `MP` zase "my parsers".

Tedy máme nějaký `"Concat"` parser, který reprezentuje konkatenaci více parserů za sebou a je úspěšný
pouze pokud všechny jeho vnitřní parsery jsou úspěšné.

Jenže volání `P.Concat<double>(...)` není vytvoření instance parseru, takže co se tu děje?

Nejjednodušší způsob, jak implementovat takovýto konkatenační parser by bylo předat mu jeho sub-parsery
jako argumenty v kontruktoru. Něco jako `new ConcatParser(someParser, new OtherParser())`. Jenže to
s sebou nese problémy, pokud budeme chtít udělat rekurzi. Nemůžeme vyvořit nekonečně instancí.

Takže řešením by bylo předat do konstruktoru concat parseru pouze nějaké factory, které by
příslušný sub-parser vytvořily až na poslední chvíli, kdy je opravdu potřeba.

Tedy volání metody `P.Concat<double>(...)` vrátí instanci `ParserFactory<TParser, TValue>`.
A teprve když na ní zavoláme metodu `CreateParser()`, tak dostaneme samotný parser.

Poslední problém nastává, když chceme udělat nějaký parser reusable (znovupoužitelný?)
a to navíc s nějakými parametry. Vrátíme se k příkladu s parsováním desetinných čísel.
Řekněme, že cheme někam zaznamenat, jak náš parser vypadá, abychom ho nemuseli definovat
v každém místě použití. Tak ho prostě extrahujeme do nějaké (statické) metody a budeme volat:

```csharp
Parser<double> p = MP.DecimalNumber().CreateParser();
```

Takovéhle statické metodě budeme říkat *ParserFactoryBuilder*. Protože nám vlastně sestaví factory na parser.

Takže máme tři termíny: `Parser`, `ParserFactory` a `ParserFactoryBuilder`.

Builder teď můžeme udělat parametrický, např. pokud chceme jako desetinný oddělovač použít čárku místo tečky:

```csharp
Parser<double> p = MP.DecimalNumber(",").CreateParser();
```

K čemu nám ale je takovéhle zabalení?

Výhod je několik.

- umožní znovupoužití parseru na více místech
- nemusíme pro každý parser dělat samostatnou třídu, protože parserů je typicky hodně moc a nedědí od sebe
- hlavně se tím ale zabalí implementace převedení matched textu na nějaký objekt = processing


### Processing

Processing je proces převodu matchnutého textu na nějaký objekt. Například při parsování celých čísel
musíme nejdříve ze vstupu odříznout posloupnost číslic a potom z nich vytvořit objekt typu `int`.

Čili obecně pro parser `Parser<TValue>` se jedná o nějakou metodu `TValue Process(string matchedString)`.
Jenže není třeba se omezovat pouze na řetězec samotný. Takže koncept zobecníme na metodu s API
`TValue Process(Parser<TValue> parser)`.

Metodu musíme poskytnout parseru shora při jeho vytvoření.

Tedy když se vrátíme k definici parseru na desetinná čísla:

```csharp
public static ParserFactory<ConcatParser, double> DecimalNumber(string delimiter = ".")
{
    return P.Concat<double>(
        MP.IntegerNumber(),
        P.Literal(delimiter),
        MP.IntegerNumber()
    ).Process((p) => {
        double intPart = ((Parser<int>)p.part[0]).Value;
        double decPart = ((Parser<int>)p.part[2]).Value;
        double decimalPlaces = p.part[2].MatchedLength
        return intPart + decPart / Math.pow(10.0, decimalPlaces);
    });
}
```

Zavoláním metody `Process` řekneme parser factory, jak vypadá funkce na processing a factory ji předá
parseru v okamžiku jeho vytvoření. Argument processingové metody je v našem případě `ConcatParser` a my
tak můžeme přistupovat ke všem jeho vlastnostem.

> **Poznámka:** `ConcatParser` nezná typ svých sub-parserů a pracuje s nimi pouze abstraktně jako `Parser`.
Takže když z nich chceme dostat více informací, musíme si je příslušně přetypovat.

> Kde samozřejmě `Parser<TValue>` dědí od `Parser`.


### Selhání parseru

Když parser selže, tak jeho vlastnost `.Success` bude mít hodnotu `false`. V běžném případě nechceme,
aby selhání skončilo výjimkou. Můžeme mít nějaký parser, který zkouší více možností a stačí mu,
aby jedna byla správná. Nechceme, aby první neúspěch shodil celý proces parsování.

Zároveň by se ale hodilo vědět, když nějaký parser spadne, proč spadne. Pokud totiž neúspěch probublá
až do kořenového parseru (třeba ten, co parsuje celý dokument), tak chceme vědět, jak vypadá posloupnost
volání parserů a co se nelíbí parseru na špičce volání (například očekával středník).

Jelikož je chování těsně analogické výjimkám v .NET, tak jako poslíčka chyby
(ten kdo bude držet informace o typu problému) použijeme instanci výjimky. Konkrétně zavedeme třídu
`ParsingException`, od které budou všechny takovéto výjimky dědit.

Výjimky se ovšem nebudou vyhazovat, protože jejich vznik je velmi častá záležitost. Bude možné
výjimku získat při neúspěchu z vlastnosti `parser.Exception`.

V případě, že výjimku dostane kořenový parser, tak je na něm co s ní udělá. Může s ní nějak pracovat dál
a nebo ji vyhodit, pokud se to považuje za nečekaný stav.

```csharp
Parser<HtmlPage> parser = MP.HtmlPage().CreateParser();
parser.Parse(html);

if (!parser.Suceess)
    throw parser.Exception;
```

> Vyhození výjimky je věc aplikace mimo parsovací framework. Parsery neobsahují try bloky,
takže pokud nějaký parser vyhodí výjimku, tak ta shodí celý parsovací proces.

Během processingu by se neměly vyhazovat tyto výjimky. Processing se zavolá ve chvíli, kdy parser uspěl
a už není možnost, jak mu říct, že vlastně ve skutečnosti neuspěl. Navíc vyhození výjimky uvnitř
processingu je nejspíš věc sémantiky, ale to s parsováním nemá co dělat. Parsery řeší pouze syntax.


### Sada výchozích parserů

Všechny parser factory buildery těchto parserů jsou uvnitř statické třídy `Lemon.P`. Ke každému z parserů
můžeme přidat processingovou metodu, která určí jak vypadá návratová hodnota.

- `P.Literal<TValue>(string literal)` Uspěje, pokud vstup obsahuje doslova zadaný řetězec.
- `P.Regex<TValue>(string pattern)` Uspěje, když uspěje regulární výraz na začátku vstupu.
- `P.Concat<TValue>(params Parser[] parts)` Spojení více parserů za sebe. Konjunkce.
- `P.Any<TValue>(params Parser[] parts)` Aplikuje parsery jeden po druhém, dokud není nějaký úspěšný. Disjunkce.
- `P.Repeat<TValue>(Parser parser, quantification)` Konkatenace jednoho parseru v daném množství.
- `P.Optional<TValue>(Parser parser)` Vždy úspěšné; může nebo nemusí aplikovat parser.


Convertor
---------

Program bude schopný načíst soubor typu JSON nebo XML a vytvořit příslušný abstract syntax tree.
Každý AST se bude schopný vytisknout na výstup v naformátovaném tvaru (takže převaděč půjde použít také
jako pretty-printer). Navíc bude obsahovat logiku pro převod z jednoho stromu (třeba JSON)
na druhý (XML) a naopak.


### JSON AST

    JsonEntity
        void .Stringify(formatOptions, stream)
    JsonNumber : JsonEntity
        bool .IsDecimal
        double .DoubleValue
        int .IntValue
    JsonBoolean : JsonEntity
        bool .Value
    JsonString : JsonEntity
        string .Value
    JsonNull : JsonEntity
    JsonArray : JsonEntity
        ...
    JsonObject : JsonEntity
        ...


### XML AST

    XmlNode
    XmlElement : XmlNode
    XmlText : XmlNode
    XmlAttribute

Nebudu implementovat:

- `<!-- komentáře -->`
- `<!DOCTYPE ...>`
- `<?xml ...>`

Protože převaděč není cílem zápočťáku. Tím je parsovací knihovna.


### Převod JSON -> XML

```json
{
    "foo": 42,
    "bar": ["lorem", "ipsum"]
}
```

Se převede na:

```xml
<object>
    <item key="foo">
        <number>42</number>
    </item>
    <item key="bar">
        <array>
            <string>lorem</string>
            <string>ipsum</string>
        </array>
    </item>
</object>
```


### Převod XML -> JSON

```xml
<foo>
    Text before
    <bar baz="asd">Lorem ipsum dolor.</bar>
    Text after
</foo>
```

Se převede na:

```json
{
    "tag": "foo",
    "attributes": {},
    "pair": true,
    "content": [
        "\n\tText before\n\t",
        {
            "tag": "bar",
            "attributes": {
                "baz": "asd"
            },
            "pair": true,
            "content": []
        },
        "\n\tText after\n"
    ]
}
```
