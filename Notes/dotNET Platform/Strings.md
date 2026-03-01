
## Immutability

Strings cannot be changed after they're created. Every operation that looks like it modifies a string actually creates a new one.

```csharp
string name = "ESL";
name.ToUpper();          // does nothing to name
name = name.ToUpper();   // creates a new string, reassigns
```

This matters for performance. Concatenating in a loop creates a new string object on every iteration:

```csharp
// ❌ Creates 1000 string objects
string result = "";
for (int i = 0; i < 1000; i++)
    result += i;

// ✅ One mutable buffer, one final string
var sb = new StringBuilder();
for (int i = 0; i < 1000; i++)
    sb.Append(i);
string result = sb.ToString();
```

---

## StringBuilder

Mutable buffer for building strings — use it when you're doing repeated concatenation, especially in loops.

```csharp
var sb = new StringBuilder();
sb.Append("Tournament: ");
sb.AppendLine("ESL Pro League");
sb.Insert(0, ">> ");
sb.Remove(0, 3);        // removes ">> "
string final = sb.ToString();
```

Rule of thumb: 3+ concatenations in a loop → use `StringBuilder`.

---

## Interning

String literals that have the same value share the same memory location. The CLR stores them in an intern pool and reuses them instead of allocating duplicates.

```csharp
string a = "hello";
string b = "hello";
ReferenceEquals(a, b); // true — same object in intern pool

string c = new string(new char[] { 'h','e','l','l','o' });
ReferenceEquals(a, c); // false — explicitly allocated, not interned
```

You'll never need to manage this manually. It just explains why string literals are memory-efficient.

---

## Equality

`==` on strings compares content, not references — the operator is overloaded for strings. This is the exception to the usual rule that `==` on reference types compares references.

```csharp
string a = "ESL";
string b = "ESL";
a == b;                  // true — content comparison
ReferenceEquals(a, b);  // true — interned, same object anyway

string c = new string(new char[] { 'E','S','L' });
a == c;                  // true — content still matches
ReferenceEquals(a, c);  // false — different objects
```

---

## Regex

Used for pattern matching and validation. Awareness level for junior interviews — know what it is and one practical use case.

```csharp
// Validate email format
bool isValid = Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");

// Extract numbers from a string
var matches = Regex.Matches("Round 3, Match 7", @"\d+");
// matches[0] = "3", matches[1] = "7"
```

Regex is in `System.Text.RegularExpressions`. The `@` prefix is a verbatim string — backslashes are treated literally, which is what regex patterns need.

---

## Interview traps

- **"Is string a value or reference type?"** — Reference type. But behaves like a value type because it's immutable — you can't observe the difference through shared mutation.
- **"What does immutability mean for performance?"** — Concatenation in a loop is O(n²). Each `+=` allocates a new string. Use `StringBuilder` for repeated modifications.
- **"What's the difference between `==` and `ReferenceEquals` on strings?"** — `==` compares content (overloaded). `ReferenceEquals` checks if they're literally the same object in memory.
- **"When would two equal strings NOT be the same reference?"** — When one is constructed at runtime (`new string(...)`, read from DB, user input) rather than a compile-time literal.

---

### Related

[[String Methods Cheat Sheet]] [[StringBuilder]] [[Reference and Value Types]]