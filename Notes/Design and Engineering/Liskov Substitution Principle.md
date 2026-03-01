### _Liskov Substitution Principle (LSP): Subtypes must be substitutable for their base types without breaking the program._

If `B` inherits from `A`, any code using `A` must work correctly with `B` — no surprises, no broken contracts.

### _Classic violation — Square/Rectangle problem_

```csharp
// ❌ Square violates Rectangle's contract
public class Rectangle
{
    public virtual int Width { get; set; }
    public virtual int Height { get; set; }
    public int Area => Width * Height;
}

public class Square : Rectangle
{
    public override int Width
    {
        get => base.Width;
        set { base.Width = value; base.Height = value; } // forces both to change
    }
    public override int Height
    {
        get => base.Height;
        set { base.Width = value; base.Height = value; }
    }
}

void Resize(Rectangle r)
{
    r.Width = 5;
    r.Height = 10;
    // Expects Area == 50, but if r is a Square, Area == 100 — LSP violated
}
```

### _Practical violation — throwing NotImplementedException_

```csharp
// ❌ ReadOnlyRepository breaks the IRepository contract
public interface IRepository<T>
{
    T GetById(int id);
    void Add(T entity);
    void Delete(int id);
}

public class ReadOnlyRepository<T> : IRepository<T>
{
    public T GetById(int id) => ...;
    public void Add(T entity) => throw new NotImplementedException(); // 💥
    public void Delete(int id) => throw new NotImplementedException(); // 💥
}
```

Code expecting `IRepository<T>` will crash when it calls `Add`. The subtype doesn't honor the contract.

### _Fix — split the interface (applies ISP too)_

```csharp
// ✅ Separate read and write contracts
public interface IReadRepository<T>
{
    T GetById(int id);
}

public interface IWriteRepository<T>
{
    void Add(T entity);
    void Delete(int id);
}

public class ReadOnlyRepository<T> : IReadRepository<T>
{
    public T GetById(int id) => ...; // no need to fake Add/Delete
}
```

### _LSP rules of thumb_

- Derived classes must not throw unexpected exceptions.
- Derived classes must not strengthen preconditions (require more).
- Derived classes must not weaken postconditions (promise less).
- If you see `NotImplementedException` or `NotSupportedException` in an override — suspect LSP violation.

[[SOLID]]
[[Interface Segregation Principle]]
[[Inheritance]]