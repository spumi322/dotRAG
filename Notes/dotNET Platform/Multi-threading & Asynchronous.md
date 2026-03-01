
### *Differences between multi-threading and asynchronous programming and scenarios in which these approaches are used.*

Multi-threading uses multiple threads for **parallel execution** — doing multiple things at the same time on different cores. Asynchronous programming is about **not blocking** the calling thread while waiting for I/O — it frees the thread to do other work. Async does not mean single-threaded: `Task.Run` dispatches work to thread pool threads, and `await` may resume on a different thread.

- **Multi-threading** → CPU-bound work (calculations, image processing). Goal: **parallelism**.
- **Async/await** → I/O-bound work (HTTP calls, DB queries, file reads). Goal: **scalability** (don't waste threads waiting).

```csharp
// CPU-bound → use threads/Parallel
Parallel.ForEach(images, img => ProcessImage(img));

// I/O-bound → use async/await
var data = await _httpClient.GetStringAsync(url); // thread is FREE while waiting
```
1. _Multi-threading:_

Multi-threading means having multiple threads of execution in a single process doing different tasks at the same time, in parallel.
Use Cases:
Running calculations, like rendering graphics or processing data, faster by splitting the work among threads.

  
2. _Asynchronous Programming:_

Asynchronous programming allows you to start a task and continue with other work without waiting for it to finish. It's handy when you don't want to block your program while waiting for something to complete.
Use Cases:
Fetching data from the internet while the rest of the program keeps running.
Handling many requests in a web server efficiently without slowing down responses.

### *Basic concepts of multi-threaded programming, such as process, thread, thread pool, competition for resources, thread synchronization.*


1. Process:_ A process is a program in execution with its own address space, memory, data stack, etc. The operating system allocates resources to the processes and manages their execution. 

2. Thread:_ A thread is the smallest unit of execution within a process. Each thread has its own register state and stack, but shares the process’s memory and resources.
 
3. Thread Pool: A thread pool is a group of pre-instantiated, idle threads which stand ready to be used for a parallel task.

4. Competition for Resources: When multiple threads require access to a shared resource, there can be competition, which needs to be managed to prevent conflicts. To manage this, synchronization mechanisms such as locks or semaphores are employed to ensure that only one thread can access the resource at a time, preventing data corruption or unexpected behavior.

5. Thread Synchronization: Synchronization is used to control the access of multiple threads to shared resources. Without synchronization, one thread could modify a shared variable while another thread is in the process of using or updating that same variable.
   
6. Deadlock: Two or more threads are unable to proceed because each is waiting for the other to release a resource. Preventing Deadlocks
1. **Avoid Nested Locks**: Avoid acquiring multiple locks at the same time. If you must, acquire all locks in a consistent order.
2. **Use Timeout**: Use timeouts for acquiring locks so that a thread can back off and try again.
3. **Use Lock Hierarchies**: Establish a global order in which locks must be acquired and ensure all threads adhere to this order.
4. **Deadlock Detection**: Implement deadlock detection algorithms to identify and resolve deadlocks when they occur.

**Lock Statement:**

- The `lock` statement is a simple and convenient way to protect critical sections of code. It is based on the `Monitor` class and ensures that only one thread can enter a specified section of code at a time.

```C#
object lockObject = new object();

lock (lockObject)
{
    // Code inside this block is protected
}

```

**Semaphore Class:**

- The `Semaphore` class is used to control access to a resource by limiting the number of threads that can access it simultaneously.

```C#
using System.Threading;

Semaphore semaphore = new Semaphore(initialCount: 1, maximumCount: 1);

semaphore.WaitOne(); // Acquire the semaphore
try
{
    // Code inside this block is protected
}
finally
{
    semaphore.Release(); // Release the semaphore
}

```


### *How the async/await construct works and the advantages of using it.*


The async/await construct simplifies asynchronous code, resembling synchronous code for cleaner and more understandable logic. It mitigates callback hell, enhances error handling through try/catch blocks, and facilitates debugging by preserving the call stack.

1. _Cleaner code:_ Async/await helps avoid callback hell by making the code look much cleaner and easier to understand.

2. _Error handling:_ With async/await, you can use try/catch blocks to handle errors, which is a more familiar and standardized form of error handling.

3. _Debugging:_ Debugging is easier with async/await because the call stack is not lost.

### *Basics of asynchronous programming in .NET using async and await keywords, understanding asynchronous delegates and tasks (Task).*


Asynchronous programming is a technique of executing tasks in the same time, which can improve the performance and responsiveness of the application.

The **async** keyword is used to mark a method as asynchronous. An async method can contain await expressions and will return a Task or ```
Task<T>.``` The async keyword does not make a method asynchronous; it allows the use of await.

**Task and ```Task<T>:```** The Task class represents an asynchronous operation. A Task is essentially a “promise” that an operation will be completed in the future. If the operation returns a result, ```Task<T>``` is used, where T is the type of the result.

The **await** Keyword: The async keyword itself does not make a method asynchronous. It is the await keyword that makes the method asynchronous by suspending its execution until the awaited task completes. The compiler transforms the code following the await into a continuation that runs after the awaited task finishes.

### *Common async/await pitfalls — mistakes that cause real production issues.*

### 1. Deadlock with `.Result` / `.Wait()`

Blocking on async code in a synchronization context (ASP.NET, WinForms) causes a **deadlock**: the thread waits for the task to finish, but the task needs that same thread to continue.

```csharp
// ❌ DEADLOCK in ASP.NET controller
public IActionResult GetProduct()
{
    var product = _productService.GetProductAsync(1).Result; // blocks thread
    return Ok(product);
}

// ✅ Correct — async all the way down
public async Task<IActionResult> GetProduct()
{
    var product = await _productService.GetProductAsync(1);
    return Ok(product);
}
```

**Real-life scenario:** A junior dev writes a synchronous controller action that calls `.Result` on `HttpClient.GetAsync()`. Works fine in unit tests (no sync context), but **deadlocks in production** under IIS/Kestrel. The endpoint hangs forever, returning no response.

**Rule: async all the way down.** Once you go async, every caller must be async too.

### 2. Fire-and-forget — swallowed exceptions

```csharp
// ❌ Exception is silently lost
public void SendWelcomeEmail(User user)
{
    _ = _emailService.SendAsync(user.Email, "Welcome!"); // no await
}

// ✅ If you truly need fire-and-forget, at least log errors
public void SendWelcomeEmail(User user)
{
    _ = Task.Run(async () =>
    {
        try { await _emailService.SendAsync(user.Email, "Welcome!"); }
        catch (Exception ex) { _logger.LogError(ex, "Email send failed"); }
    });
}
```

**Real-life scenario:** User registration endpoint returns 200, but the welcome email silently fails. No error in logs, no alert. Customer complains they never received it. The `SendAsync` threw a `SmtpException` that was never observed.

### 3. `async void` — avoid it

`async void` methods cannot be awaited, and unhandled exceptions crash the process.

```csharp
// ❌ Crashes the application if it throws
async void OnButtonClick(object sender, EventArgs e) { ... }

// ✅ Use async Task — only exception: event handlers (WinForms/WPF)
async Task OnButtonClickAsync() { ... }
```

### 4. `ValueTask` — when to use it

`ValueTask<T>` avoids heap allocation when the result is already available (cached, synchronous path). Use it when a method **frequently completes synchronously**.

```csharp
// Hot path: cache hit returns synchronously, cache miss awaits DB
public ValueTask<Product?> GetProductAsync(int id)
{
    if (_cache.TryGetValue(id, out var product))
        return ValueTask.FromResult(product); // no Task allocation

    return new ValueTask<Product?>(LoadFromDbAsync(id));
}
```

⚠️ `ValueTask` can only be awaited **once** and must not be stored. When in doubt, use `Task<T>`.

### 5. `ConfigureAwait(false)` — library code

In **library code** (not ASP.NET controllers), use `ConfigureAwait(false)` to avoid capturing the synchronization context, improving performance and preventing deadlocks:

```csharp
// In a library/service class
var data = await _httpClient.GetStringAsync(url).ConfigureAwait(false);
```

In **ASP.NET Core** (no sync context), it has no effect — but it's still good practice in shared libraries that might be used from WinForms/WPF.