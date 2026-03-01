### _Definition_: Assemblies are compiled code libraries used by .NET applications, essentially containers for Intermediate Language (IL) code, metadata, and an optional manifest.

1. _Executable Assemblies (.exe):_ These are the main application files that can be directly executed. They contain entry points (like the Main method in a C# program).

2. _Class Library Assemblies (.dll):_ These are the Dynamic Link Libraries that contain reusable code. They don't have entry points and are used by other applications or assemblies.
    

1. **Manifest**: Contains metadata such as assembly name, version, and culture, along with information about external dependencies and embedded resources.
    
2. **Type Metadata**: Describes the types defined within the assembly, including classes, interfaces, and methods, along with their properties and attributes.
    
4. **Resources**: Embedded files like images, icons, and localization strings that the application uses.
    
5. **Assembly References**: Information about other assemblies that this assembly depends on, ensuring the correct versions are loaded at runtime.
    
6. **Security Information**: Details any security permissions the assembly requires to execute, aligning with the CLR’s security mechanisms.

### Global Assembly Cache (GAC)

**Definition:** The Global Assembly Cache (GAC) is a machine-wide code cache used in the .NET framework to store assemblies that are intended to be shared by multiple applications on a computer.
### Compilation and Storage

  
_Types of Assemblies:_

1. Private Assemblies: Used by a single application, located in the application's directory.
    
2. Shared Assemblies: Intended for use by multiple applications. Usually stored in the Global Assembly Cache (GAC) and strongly named for uniqueness. . The GAC is a machine-wide store for all dotnet applications to share the assembly, ensuring that each application can access the version of the assembly it was built against, even if other versions are present on the system.