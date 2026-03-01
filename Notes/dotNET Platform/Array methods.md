# Arrays — Algo Cheatsheet

Fixed size, contiguous memory, O(1) index access, O(n) search/insert/delete.

---

## Quick API

```csharp
int[] a = new int[5];               // default 0
int[] b = { 3, 1, 4, 1, 5 };       // inline init
int   n = b.Length;                 // 5

// Statics — all in-place unless noted
Array.Sort(b);                      // O(n log n) IntroSort
Array.Reverse(b);                   // O(n)
Array.Fill(b, 0);                   // fill entire array
Array.Fill(b, 0, 2, 3);            // fill 3 elements starting at index 2

int idx = Array.BinarySearch(b, 4); // O(log n) — requires sorted, returns ~idx if missing
int idx = Array.IndexOf(b, 4);      // O(n) linear scan, -1 if not found

Array.Copy(src, dst, n);            // shallow copy n elements
int[] copy = (int[])b.Clone();      // full shallow clone

// Span — zero-allocation slice (no copy)
Span<int> slice = b.AsSpan(1, 3);   // elements at index 1,2,3
```

---

## Initialisation Patterns

```csharp
// Fill with computed values
int[] squares = Enumerable.Range(1, 5).Select(x => x * x).ToArray();
// { 1, 4, 9, 16, 25 }

// 2D matrix
int[,] grid = new int[3, 4];        // 3 rows, 4 cols
grid[1, 2] = 7;
int rows = grid.GetLength(0);       // 3
int cols = grid.GetLength(1);       // 4

// Jagged (array of arrays — rows can differ in length)
int[][] jagged = new int[3][];
jagged[0] = new int[] { 1, 2 };
jagged[1] = new int[] { 3, 4, 5 };
```

---

## Pattern — Two Pointers

O(n) time, O(1) space. Works on **sorted** arrays or problems where left/right converge.

```csharp
// Two sum in sorted array
(int, int) TwoSum(int[] a, int target)
{
    int lo = 0, hi = a.Length - 1;
    while (lo < hi)
    {
        int sum = a[lo] + a[hi];
        if (sum == target) return (lo, hi);
        if (sum < target) lo++;
        else hi--;
    }
    return (-1, -1);
}

// Reverse in-place (no extra space)
void Reverse(int[] a)
{
    int lo = 0, hi = a.Length - 1;
    while (lo < hi)
        (a[lo++], a[hi--]) = (a[hi], a[lo]); // tuple swap
}
```

---

## Pattern — Sliding Window

O(n) time, O(1) space. Fixed or variable-size window that slides right.

```csharp
// Max sum of k consecutive elements — fixed window
int MaxSumK(int[] a, int k)
{
    int sum = a[..k].Sum();          // first window
    int max = sum;
    for (int i = k; i < a.Length; i++)
    {
        sum += a[i] - a[i - k];     // slide: add right, drop left
        max = Math.Max(max, sum);
    }
    return max;
}
```

---

## Pattern — Prefix Sum

O(n) build, O(1) range query. Pre-compute cumulative sums to answer "sum from i to j" in O(1).

```csharp
int[] BuildPrefix(int[] a)
{
    var prefix = new int[a.Length + 1]; // prefix[0] = 0 sentinel
    for (int i = 0; i < a.Length; i++)
        prefix[i + 1] = prefix[i] + a[i];
    return prefix;
}

// Sum from index l to r inclusive — O(1)
int RangeSum(int[] prefix, int l, int r) => prefix[r + 1] - prefix[l];
```

---

## Pattern — Binary Search (sorted array)

O(log n). Always use `lo + (hi - lo) / 2` — avoids int overflow.

```csharp
int BinarySearch(int[] a, int target)
{
    int lo = 0, hi = a.Length - 1;
    while (lo <= hi)
    {
        int mid = lo + (hi - lo) / 2;
        if (a[mid] == target) return mid;
        if (a[mid] < target) lo = mid + 1;
        else hi = mid - 1;
    }
    return -1;
}

// Find first position where condition is true (generalised)
int FirstTrue(int[] a, Func<int, bool> cond)
{
    int lo = 0, hi = a.Length - 1, ans = -1;
    while (lo <= hi)
    {
        int mid = lo + (hi - lo) / 2;
        if (cond(a[mid])) { ans = mid; hi = mid - 1; }
        else lo = mid + 1;
    }
    return ans;
}
```

---

## Pattern — Frequency Map

O(n). Count occurrences without sorting.

```csharp
int[] FindDuplicates(int[] a)
{
    var freq = new Dictionary<int, int>();
    foreach (var x in a)
        freq[x] = freq.GetValueOrDefault(x) + 1;

    return freq.Where(kv => kv.Value > 1)
               .Select(kv => kv.Key)
               .ToArray();
}
```

---

## Gotchas

```csharp
// Arrays are reference types — assignment copies the reference, not the data
int[] a = { 1, 2, 3 };
int[] b = a;
b[0] = 99;
// a[0] is now 99 — same object

// Safe copy
int[] c = (int[])a.Clone();         // shallow clone
int[] d = new int[a.Length];
Array.Copy(a, d, a.Length);

// Array.BinarySearch returns bitwise complement (~) if not found
int idx = Array.BinarySearch(sorted, 7);
if (idx < 0) idx = ~idx;            // ~idx = insertion point

// Jagged vs 2D — interviewers sometimes ask
// int[,] = true matrix, single allocation
// int[][] = array of arrays, each row separate allocation — more flexible
```

---

## LINQ shortcuts useful in algos

```csharp
a.Sum()
a.Min() / a.Max()
a.Count(x => x > 0)
a.Where(x => x % 2 == 0).ToArray()
a.OrderBy(x => x).ToArray()         // returns new array, original unchanged
a.Distinct().ToArray()              // remove duplicates (order not guaranteed)
a.Reverse().ToArray()               // LINQ Reverse — returns IEnumerable, not in-place
Array.Reverse(a);                   // in-place — these are different!
```