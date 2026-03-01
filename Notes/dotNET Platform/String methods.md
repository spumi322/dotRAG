# String Methods Cheat Sheet

Quick reference for coding exercises. Not for memorisation.

---

## Null / empty checks

```csharp
string.IsNullOrEmpty(s)       // true if null or ""
string.IsNullOrWhiteSpace(s)  // true if null, "", or "   "
```

Prefer `IsNullOrWhiteSpace` for user input — spaces-only is still empty for practical purposes.

---

## Searching

```csharp
s.Contains("ESL")             // true/false
s.StartsWith("ESL")           
s.EndsWith("Pro")             
s.IndexOf("Pro")              // first index, -1 if not found
s.LastIndexOf("o")            // last occurrence
```

---

## Modifying (returns new string)

```csharp
s.ToUpper()
s.ToLower()
s.Trim()                      // removes whitespace both ends
s.TrimStart() / s.TrimEnd()
s.Replace("ESL", "BLAST")
s.Remove(0, 3)                // remove 3 chars starting at index 0
s.Insert(0, ">> ")            // insert at index
s.Substring(2)                // from index 2 to end
s.Substring(2, 4)             // 4 chars starting at index 2
```

---

## Splitting and joining

```csharp
"a,b,c".Split(',')                        // ["a", "b", "c"]
"a,,b".Split(',', StringSplitOptions.RemoveEmptyEntries) // ["a", "b"]

string.Join(", ", new[] { "a", "b", "c" }) // "a, b, c"
string.Join("-", teams.Select(t => t.Name))
```

---

## Formatting

```csharp
// String interpolation (preferred)
$"Match {matchId} won by {team.Name}"

// Composite formatting
string.Format("Match {0} won by {1}", matchId, team.Name)

// Number formatting
score.ToString("F2")    // 2 decimal places: "3.14"
date.ToString("yyyy-MM-dd")
```

---

## Useful static methods

```csharp
string.IsNullOrEmpty(s)
string.Join(separator, collection)
string.Concat("a", "b", "c")          // "abc"
string.Compare("a", "b")              // -1, 0, 1
```

---

## Span<char> — modern slicing (no allocation)

```csharp
// Avoids creating a new string object when you only need a slice
ReadOnlySpan<char> slice = "ESL Pro League".AsSpan(4, 3); // "Pro"
```

Useful in performance-sensitive code. Awareness level for interviews.

---

## Common algo patterns

```csharp
// Reverse a string
string reversed = new string(s.Reverse().ToArray());

// Check palindrome
bool isPalindrome = s == new string(s.Reverse().ToArray());

// Character frequency
var freq = s.GroupBy(c => c).ToDictionary(g => g.Key, g => g.Count());

// Remove duplicates preserving order
string unique = new string(s.Distinct().ToArray());
```

---

### Related

[[Strings]] [[LINQ]]