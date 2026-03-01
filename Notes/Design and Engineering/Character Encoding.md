### *Character encoding and different types of encoding (UTF8, UTF16, Unicode).*

**Types of Encoding in .NET:**

1. **ASCII Encoding:**
    
    - Uses 7 bits to represent characters, primarily for basic Latin characters.

    
    `Encoding ascii = Encoding.ASCII;`
    
2. **UTF-8 Encoding:**
    
    - Variable-width encoding using 8-bit code units, capable of representing all Unicode characters.

    
    `Encoding utf8 = Encoding.UTF8;`
    
3. **UTF-16 Encoding:**
    
    - Variable-width encoding using 16-bit code units, supports the entire Unicode character set.

    
    `Encoding utf16 = Encoding.Unicode; // Little-endian UTF-16`
    
4. **UTF-16 Big-Endian:**
    
    - Similar to UTF-16 but with big-endian byte order.

    
    `Encoding utf16BE = Encoding.BigEndianUnicode;`