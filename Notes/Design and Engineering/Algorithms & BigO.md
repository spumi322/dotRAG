### _Time & Space Complexity (Big O)_

Big O describes how an algorithm's resource usage grows as input size (n) increases. It measures the **worst-case** scenario.

| Notation   | Name         | Example                               | 10 items | 1,000 items |
| ---------- | ------------ | ------------------------------------- | -------- | ----------- |
| O(1)       | Constant     | Dictionary lookup, array index access | 1        | 1           |
| O(log n)   | Logarithmic  | Binary search                         | 3        | 10          |
| O(n)       | Linear       | Single loop, `List.Contains()`        | 10       | 1,000       |
| O(n log n) | Linearithmic | Merge sort, `Array.Sort()`            | 33       | 10,000      |
| O(n²)      | Quadratic    | Nested loops, bubble sort             | 100      | 1,000,000   |
| O(2ⁿ)      | Exponential  | Recursive Fibonacci (naive)           | 1,024    | ∞           |

### _How to analyze — count the loops_

```csharp
// O(1) — constant
int first = arr[0];

// O(n) — single loop
for (int i = 0; i < n; i++) { ... }

// O(n²) — nested loop
for (int i = 0; i < n; i++)
    for (int j = 0; j < n; j++) { ... }

// O(log n) — halving the input each step
while (n > 1) { n /= 2; }

// O(n log n) — common in efficient sorting
Array.Sort(arr); // uses IntroSort internally
```

### _Space complexity_

How much **extra memory** the algorithm uses (excluding the input).

```csharp
// O(1) space — only a few variables
int sum = 0;
for (int i = 0; i < n; i++) sum += arr[i];

// O(n) space — creates a new collection proportional to input
var copy = arr.ToList();
```

### _Data structure operation complexities_

|Operation|Array|List<T>|Dictionary<K,V>|HashSet<T>|LinkedList<T>|
|---|---|---|---|---|---|
|Access by index|O(1)|O(1)|—|—|O(n)|
|Search|O(n)|O(n)|O(1) avg|O(1) avg|O(n)|
|Insert at end|—|O(1) amortized|O(1) avg|O(1) avg|O(1)|
|Insert at index|—|O(n)|—|—|O(1)*|
|Delete|O(n)|O(n)|O(1) avg|O(1) avg|O(1)*|

* O(1) if you already have the node reference; O(n) to find the node first.

### _Essential algorithms to know_

**Binary search** — O(log n), requires sorted input:

```csharp
int BinarySearch(int[] sorted, int target)
{
    int lo = 0, hi = sorted.Length - 1;
    while (lo <= hi)
    {
        int mid = lo + (hi - lo) / 2; // avoids overflow
        if (sorted[mid] == target) return mid;
        if (sorted[mid] < target) lo = mid + 1;
        else hi = mid - 1;
    }
    return -1; // not found
}
```

**Two-pointer technique** — O(n), common in sorted array problems:

```csharp
// Find two numbers that sum to target in a sorted array
(int, int) TwoSum(int[] sorted, int target)
{
    int lo = 0, hi = sorted.Length - 1;
    while (lo < hi)
    {
        int sum = sorted[lo] + sorted[hi];
        if (sum == target) return (lo, hi);
        if (sum < target) lo++;
        else hi--;
    }
    return (-1, -1);
}
```

**Recursion** — a function calling itself with a smaller problem:

```csharp
// O(n) — linear recursion
int Factorial(int n) => n <= 1 ? 1 : n * Factorial(n - 1);

// ⚠️ O(2ⁿ) — naive Fibonacci, exponential, never use in production
int Fib(int n) => n <= 1 ? n : Fib(n - 1) + Fib(n - 2);

// ✅ O(n) — iterative Fibonacci
int FibIterative(int n)
{
    if (n <= 1) return n;
    int prev = 0, curr = 1;
    for (int i = 2; i <= n; i++)
        (prev, curr) = (curr, prev + curr);
    return curr;
}
```

### _Sorting algorithms awareness_

|Algorithm|Time (avg)|Time (worst)|Space|Stable|Notes|
|---|---|---|---|---|---|
|Bubble Sort|O(n²)|O(n²)|O(1)|✅|Educational only|
|Insertion Sort|O(n²)|O(n²)|O(1)|✅|Fast for small/nearly sorted|
|Merge Sort|O(n log n)|O(n log n)|O(n)|✅|Consistent performance|
|Quick Sort|O(n log n)|O(n²)|O(log n)|❌|Fastest in practice|
|.NET `Array.Sort`|O(n log n)|O(n log n)|O(log n)|❌|IntroSort (hybrid)|

**Stable sort** = equal elements maintain their original relative order.

[[Collections]] 