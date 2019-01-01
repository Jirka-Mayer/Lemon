Lemon
=====

Mini framework pro psaní parserů + jeho použití (pretty printery na XML a JSON).

> Zápočtový program pro C# - ZS 2018, Jiří Mayer

Specifikace zadání je [zde](docs/spec.md).

Dokumentace
-----------

Dokumentace se skládá ze dvou částí - z dokumentace programu (Convertor) a knihovny (Lemon).


### Convertor

Convertor je program na převod souborů mezi formáty XML a JSON.

Používá se následujícím způsobem:

    convertor.exe [xml|json] [inputFile] [outputFile]

První argument je jeden z řetězců `xml`, nebo `json` a označuje formát výstupu.

Druhý argument je název vstupního souboru a jeho typ se odvodí z přípony souboru.

Poslední argument je název výstupního souboru. Je-li místo něho zadaná pomlčka `-`, tak se výsledek
vypíše na standardní výstup.

Pokud jsou formáty vstupního i výstupního souboru stejné (např. oba jsou xml), tak program soubor pouze zformátuje.
Pokud se formáty liší, tak se provede převod.

Pokud je problém s načtením vstupního souboru (IO nebo špatný formát), tak program vypíše chybu.


#### Převod JSON -> XML

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


#### Převod XML -> JSON

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


### Lemon

Lemon je framework na psaní parserů pomocí skládání jednodušších parserů do sebe přímo v C#.

- [Používání knihovny](docs/usage.md)
- [Parser](docs/parser.md)
- [ParserFactory, Rekurze](docs/parser-factory.md)
- [Chyby při parsování - ParsingException](docs/exceptions.md)


### Závěr

Původně jsem chtěl vymyslet knihovně nějaké pěkné API, aby se dala hezky používat i ve staticky
typovaném jazyce jako je C#. Veškeré znalosti které jsem s podobnými věcmi měl jsou z dynamických jazyků.
Výsledkem je moře generických tříd a typových argumentů, ve kterém se člověk ztrácí. Navíc spousta
chyb se stejně odchytí až za běhu, protože je všechno tak modulární a neurčité, že se musí všude přetypovávat.

Takže možná parsování (alespoň takovéhle) opravu sedí lépe na dynamické jazyky. Nebo jsem to spíš napsal
dynmicky ve statickém jazyku. Nevím.

Další problém, na který jsem narazil ke konci programování je, jak přesně šířit výjimky o neúspěšném parsování.
Jakmile totiž někde požijeme alternaci, začne být problém určovat, kdo je viník. A když použijeme opakování
parseru, tak ten začne házet vinu na sousední parsery a tedy data ve výjimce jsou skoro zavádějící.

Problém jsem ale jen trochu zalepil a dál neřešil. Nebyl to cíl práce a neměl jsem to ani nijak
konkrétně rozmyšlené dopředu.
