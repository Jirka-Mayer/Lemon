ParsingException
================

Pokud se parseru nepovede matchnout vstup, tak vrátí instanci třídy `ParsingException`,
aby se snáz hledaly chyby.

Problém je, že selhání parseru je typicky častá věc. Pokud mám nějaký parser definovaný jako
alternaci více parserů `P.Any(...)`, tak očekávám, že jich hodně neprojde, pouze jeden
bude úspěšný. A kdybychom výjimku vyhazovali pomocí `throw`, tak tím budeme zbytečně brzdit běh
programu a implementace parserů bude zamořená `try` blokama.

Na druhou stranu `ParsingException` dědí od třídy `Exception` takže vyhodit lze. Protože typicky
používáme Lemon jako součást většího programu. Máme nějaký parser na celý dokument, ten zavoláme
a stane se, že neuspěje. V dokumentu je někde chyba. Můžeme vzít instanci výjimky a vyhodit ji,
pokud ji nechceme zpracovávat hned na místě.

```csharp
Parser<Document> documentParser = MP.Document().CreateAbstractValuedParser();
documentParser.Parse(documentContent);

if (!documentParser.Success)
    throw documentParser.Exception;
```

Výjimka s sebou nese vstupní řetězec, pozici ve vstupu na které vznikla. Z pozice dopočítá řádek a znak na řádku.

Navíc obsahuje zádobník parserů - něco jako volací zásobník. Např. pro `ConcatParser`, když jeden z jeho parserů selže,
tak nevytváří novou výjimku, ale tu vzniklou pošle nahoru a do jejího zásobníku přidá sebe.

Všechny informace se vypíšou naformátované pomocí metody `ToString()`.

Problém je se selháním `AnyParseru`, protože typicky není jasné, který parser je ten, co měl nejspíš uspět, ale selhal.
Takže se dělá to, že se porovná `AlmostMatchedLength` jednotivých parserů, vybere se první největší a jeho
výjimka se pošle nahoru. To problém řeší docela dobře.

Stejně ale nastává problém s `RepeatParserem`. Protože když je opravdová chyba někde v opakovaném prvku, tak ale
`RepeatParser` většinou uspěje, vadný prvek neukousne a tak na něm spadne první parser, který je po něm.
Tedy chyba se pak tváří, jakože vznikla úplně jinde, ačkoliv ve skutečnosti vznikla v repeat parseru.

> K vyřešení problému bych musel nejspíš předělat `ConcatParser`, aby on řešil opakování nějakého sub-parseru
a aby si hlídal `AlmostMatchedLength` jednotlivých opakování a nějakou heuristikou se rozhodl podobně
jako `AnyParser`. Nebo nevracet jednu větev parsování, ale celý strom, to je ale zase hodně nepřehledných dat.
