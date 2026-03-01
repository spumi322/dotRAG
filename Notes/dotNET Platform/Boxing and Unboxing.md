The process of converting a value type instance to a reference type (typically to an object or to an interface the value type implements).

Boxing wraps the value type within a System.Object and stores it on the heap.

```csharp
int value = 123;
object boxed = val; // Boxing

int unboxed = (int)boxed; // Unboxing
```

The reverse process of extracting the value type from the object on the heap.
Unboxing requires an explicit cast to the correct value type.
### _Boxing and Unboxing -- converting between value types and reference types_

**Boxing** is the process of converting a value type instance to a reference type (typically to `object` or to an interface the value type implements).

Boxing wraps the value type within a `System.Object` and stores it on the heap.

**Unboxing** is the reverse process of extracting the value type from the object on the heap.
Unboxing requires an explicit cast to the correct value type.

### _Why boxing exists_

C# has a unified type system where **everything** derives from `System.Object`. This means even value types (int, bool, struct) must be treatable as objects. Boxing is the mechanism that makes this work.
```csharp
int num = 42;           // value type, lives on stack (if local variable)
object obj = num;       // BOXING â†' wraps 42 in System.Object, allocates on heap
int unwrapped = (int)obj; // UNBOXING â†' extracts int from heap object
```

### _What happens during boxing_

1. **Heap allocation**: A new object is allocated on the heap
2. **Value copy**: The value type's data is copied into the new heap object
3. **Reference returned**: You get a reference to the heap object

Result: The value now exists in TWO places â€" original stack location + heap copy.

### _What happens during unboxing_

1. **Type check**: CLR verifies the boxed object actually contains the target type
2. **Value extraction**: The value is copied from the heap object back to a value type variable
3. **InvalidCastException**: If you try to unbox to the wrong type
```csharp
object boxed = 123;
int correct = (int)boxed;      // âœ… Works
double wrong = (double)boxed;  // âŒ InvalidCastException at runtime
```

### _Performance cost_

Boxing and unboxing are **expensive operations**:

- **Heap allocation**: Every box allocates a new object on the heap
- **Garbage collection pressure**: Boxed objects create garbage that GC must clean up
- **Type checking overhead**: Unboxing requires runtime type verification
- **Memory copying**: Value is copied twice (once to box, once to unbox)

**Example of boxing in a loop â€" performance disaster:**
```csharp
ArrayList list = new ArrayList(); // stores object, not int
for (int i = 0; i < 100000; i++)
{
    list.Add(i); // boxes EVERY int â†' 100,000 heap allocations!
}
```

### _Common scenarios that cause boxing_

**1. Using non-generic collections:**
```csharp
ArrayList arr = new ArrayList();
arr.Add(5);           // int â†' object (BOXING)
int x = (int)arr[0];  // object â†' int (UNBOXING)
```

**2. Passing value type to method expecting object:**
```csharp
void PrintObject(object obj) => Console.WriteLine(obj);

int num = 10;
PrintObject(num); // BOXING â€" int becomes object
```

**3. Interface implementation on value types:**
```csharp
struct Point : IComparable
{
    public int X, Y;
    public int CompareTo(object obj) => /* ... */;
}

Point p = new Point { X = 1, Y = 2 };
IComparable comparable = p; // BOXING â€" Point becomes heap object
```

**4. String concatenation with value types:**
```csharp
int age = 25;
string message = "Age: " + age; // age is boxed to call ToString()
```

**5. Using `object` type variable:**
```csharp
int val = 123;
object boxed = val;      // Boxing
int unboxed = (int)boxed; // Unboxing
```

### _Boxing with nullable types â special behavior_

When you box a nullable type, the CLR boxes **the underlying value**, not the `Nullable<T>` wrapper:
```csharp
int? nullable = 42;
object boxed = nullable;
Console.WriteLine(boxed.GetType()); // System.Int32 (not Nullable<Int32>!)

// Boxing null produces null reference, NOT a boxed Nullable<T>
int? nullValue = null;
object boxedNull = nullValue;
Console.WriteLine(boxedNull == null); // True
```

### _Interview red flags_

- "Boxing converts stack to heap is imprecise. Boxing **copies** a value type to a heap-allocated object.
- "Boxing happens automatically so it's fine - NO. It's a performance cost to be aware of.
- "Boxing wraps a value type in System.Object and allocates on the heap, which is expensive in loops."

### _Related topics_

[[Reference and Value Types]]
[[System.Object]] 
[[Data Types]] 