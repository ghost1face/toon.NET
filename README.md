# toon.NET

Token-Oriented Object Notation (TOON) for .NET.

**Token-Oriented Object Notation** is a compact, human-readable format designed for passing structured data to Large Language Models with significantly reduced token usage. It's intended for LLM input, not output.

TOON's sweet spot is **uniform arrays of objects** – multiple fields per row, same structure across items. It borrows YAML's indentation-based structure for nested objects and CSV's tabular format for uniform data rows, then optimizes both for token efficiency in LLM contexts. For deeply nested or non-uniform data, JSON may be more efficient.

> [!TIP]
> Think of TOON as a translation layer: use JSON programmatically, convert to TOON for LLM input.

## Usage

```cs
var serializer = new ToonSerializer();

var data = new {
  user = new {
    id = 123,
    name = "Ada",
    tags = new[] { "reading", "gaming" },
    active = true,
    preferences = new[] { }
  }
}

var dataString = serializer.Serialize(data);

Console.WriteLine(dataString);

//// example output:
// user:
//   id: 123
//   name: Ada
//   tags[2]: reading,gaming
//   active: true
//   preferences[0]:
```

## Why TOON?

AI is becoming cheaper and more accessible, but larger context windows allow for larger data inputs as well. **LLM tokens still cost money** – and standard JSON is verbose and token-expensive:

```json
{
  "users": [
    { "id": 1, "name": "Alice", "role": "admin" },
    { "id": 2, "name": "Bob", "role": "user" }
  ]
}
```

TOON conveys the same information with **fewer tokens**:

```
users[2]{id,name,role}:
  1,Alice,admin
  2,Bob,user
```

## Key Features

- 💸 **Token-efficient:** typically 30–60% fewer tokens than JSON
- 🤿 **LLM-friendly guardrails:** explicit lengths and field lists help models validate output
- 🍱 **Minimal syntax:** removes redundant punctuation (braces, brackets, most quotes)
- 📐 **Indentation-based structure:** replaces braces with whitespace for better readability
- 🧺 **Tabular arrays:** declare keys once, then stream rows without repetition

## Acknowledgements

This is a port of https://github.com/johannschopplich/toon to .NET