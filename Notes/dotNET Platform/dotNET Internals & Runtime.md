[Open interactive version](dotnet-internals.html)




1. **Code gets compiled into [[CIL]] within an [[Assemblies|Assembly]]:**
    - The source code is compiled into CIL bytecode, which is platform-independent.
    
2. **[[Assemblies|Assembly]] is loaded into the [[CLR]]:**
    - The CLR loads the Assembly, containing compiled code, resources, and metadata.
    
3. **JIT Compilation (Just-In-Time):**
    - The CLR uses the Just-In-Time (JIT) compiler to translate CIL code into native code for the specific machine architecture. This improves performance as code is optimized for the target system.
    
4. **[[CLR]] Executes the Code:**
    - The CLR executes the native code, allocating memory on the Stack for local variables and function calls.
    - Objects are allocated on the Heap based on their type and lifetime.
    
5. **Common Type System (CTS) Ensures Consistency:**
    - The CLR uses the Common Type System (CTS) to define [[Theories/dotNET Platform/Data Types]] and their behavior. This ensures consistency and interoperability across different programming languages within the .NET ecosystem.
    
6. **Base Class Library (BCL) Provides Foundation:**
    - The CLR utilizes the Base Class Library (BCL) to access common functionalities like file I/O, networking, and [[Theories/Design and Engineering/Data Structures]]. This simplifies development by providing pre-built components.
    
7. **[[GC]] Monitors and Reclaims Memory:**
    - The Garbage Collector (GC) continuously monitors memory usage on the [[Stack  & Heap|Heap]].
    - When objects become unreachable, the GC reclaims their memory, preventing leaks and ensuring efficient resource management.


[[dotNET Framework & dotNET Core]]
[[Assemblies]]
[[GC]]
[[Stack  & Heap]]
[[CLR]]
[[CIL]]
[[Theories/dotNET Platform/Data Types]]


