Parser
======

Parser je objekt, který dostane vstupní řetězec a pokusí se na jeho začátku nalézt
podřetězec, který splňuje kritéria parseru. Pokud hledání dopadne kladně, tak se provede
zpracování řetězce a parser vrátí objekt, který řetězec nějak reprezentuje.

Parser je po svém vytvořený nedotčený `p.IsPristine`, dokud se na něm nezavolá medota `Parse`.
Poté se do něho uloží výsledek parsování a nelze použít znovu. To proto, aby se neměnil jeho vnitřní stav.

Po provedení parsování je k dispozici několik metod a vlastností pro dotazování výsledku:

- `Success` zda se parsování zdařilo a vstupní řetězec odpovídal formátu, který parser očekává
- `Exception` pokud se nezdařilo, tak jak vypadá problém, který vznikl (kde ve vstupu, co nebylo nalezeno apod.)
- `Value` hodnota reprezentující matchnutý vstupní řetězec, pokud parser provádí transformaci z textu na objekt
- `MatchedLength` kolik znaků vstupu se podařilo matchnout
- `GetMatchedString()` vrátí text, který se matchnul
- `AlmostMatchedLength` kolik znaků se podařilo matchnout, než došlo k chybě (protože s chybou může být `MatchedLength` rovna nule)

Specifické parsery (třeba `ConcatParser`) mají ještě další vlastnosti se kterými lze pracovat.

> **Poznámka:** Parser bere jako vstup `string` a ne `Stream`, protože chci mít k dispozici
regulární výrazy a také mít přístup ke vstupnímu textu i potom, co parser seběhl. To znamená,
že pro používání knihovny na souborech musím parsovat buď po nějakých celcích (např. řádky),
nebo mít rozumně velké (malé) soubory, aby se celé vešly do paměti.


## Processing

Pokud nás zajímá pouze, zda vstup splňuje nějaká pravidla, tak je hodnota `Success` jediné co nás zajímá.
Většinou ale používáme parsery k vytvoření nějaké abstraktní reprezentace vstupu. Tedy chceme po parseru, aby
nám tuto reprezentaci rovnou vrátil.

Problém se hezky demonstruje na parsování čísel. Tedy zavedeme koncept jakési návrtové hodnoty parseru,
která bude uložená ve vlastnosti `Value`. Jak se ovšem vypočítá musí spacifikovat uživatel předáním nějaké metody.

Tuto metodu parseru předá factory po jeho vytvoření. Více o parser factories pozdeji.

> Pokud po parseru nechceme, aby něco vracel, tak můžeme říct, že vrací typ `P.Void`. Ono bychom mohli říct,
že vrací `string` a prostě neposkytnout žádný processor a kdybychom se nedotazovali na `Value`, tak by se nic
nestalo. Ale s explicitním `Parser<P.Void>` je na první pohled vidět, že parser nemá nic vracet.


## Definice parseru

Parser lze definovat skládáním z jiných parserů, jako je to v ukázce použití frameworku, nebo může být napsán přímo.
Stačí dědit od abstracktní třídy `Parser`, nebo `Parser<TValue>` a implementovat metodu `PerformParsing`.

Metoda dostane vstupní řetězec, index od kterého má parsovat a jejím úkolem je nastavit hodnotu vlastnosti `MatchedLength` na odpovídající hodnotu a vrátit `null`. V případě chyby vrátí instanci třídy `ParsingException`.
