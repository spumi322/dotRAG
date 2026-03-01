*Basic data types such as int, long, short, byte, decimal, float, char, bool, and others, as well as their literals.*

### Primitive/Built-in Types

1. **int** (32-bit integer): Represents an integer value.

```C#
int number = 42;
```

2. **double** (64-bit floating-point number): Represents a double-precision floating-point number.

```C#
double pi = 3.14159;
```

3. **bool** (Boolean):Represents a Boolean value (true or false).

```C#
bool isTrue = true;
```

4. **char** (16-bit Unicode character): Represents a single Unicode character.

```C#
char letter = 'A';
```

5. **string** (sequence of characters): Represents a sequence of Unicode characters.

```C#
string name = "DotNet";
```

6. **decimal** (128-bit precise decimal): Represents a high-precision decimal number.

```C#
decimal price = 99.99m;
```

7. **float** (32-bit floating-point number): Represents a single-precision floating-point number.

```C#
float temperature = 23.4f;
```

8. **long** (64-bit integer): Represents a long integer value.

```C#
long bigNumber = 123456789L;
```

9. **byte** (8-bit unsigned integer): Represents an unsigned integer from 0 to 255.

```C#
byte level = 255;
```

10. **short** (16-bit integer): Represents a short integer value.

```C#
short smallNumber = 10000;
```
### User-Defined Value Types

11. **struct** (Structure): Represents a complex data type that can contain members like fields, methods, and constructors.

```C#
struct Point { public int X;, public int Y; }

Point P = new Point { X = 1, Y = 2 }
```


12. **enum** (Enumeration): Represents a set of named constants (usually integers).

```C#
enum Color { Red, Green, blue }

Color favColor = Color.Green
```

13. **Tuples** (C# 7+): Lightweight grouping of multiple values without defining a class/struct. Value type (`System.ValueTuple`).

```csharp
// Named tuple
(int Id, string Name) product = (1, "Widget");
Console.WriteLine(product.Name); // "Widget"

// Return multiple values from a method
(decimal Min, decimal Max) GetPriceRange(List<Product> products)
    => (products.Min(p => p.Price), products.Max(p => p.Price));

var range = GetPriceRange(products);
Console.WriteLine($"{range.Min} - {range.Max}");

// Deconstruction into separate variables
var (min, max) = GetPriceRange(products);
```

Use tuples for quick multi-value returns in private/internal methods. For public APIs, prefer a named type (record, class) for clarity.

[[Reference and Value Types]]
[[Nullable Types]]
[[Enums]]
[[Boxing and Unboxing]]
[[System.Object]]
[[Dynamic Type & Object Type]]
[[Array methods]]
[[Theories/Design and Engineering/Character Encoding|Character Encoding]]
