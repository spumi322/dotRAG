Delegate is like a pointer to a method that has a certain set of parameters and a return type. It lets you safely pass methods around as if they were variables and set up methods that can be called later.

### *What a multicast delegate is and how to use it to invoke multiple methods simultaneously.*

A multicast delegate is a type of delegate that can hold references to more than one method at a time. When you invoke a multicast delegate, it calls all the methods it references in the order.


### *What anonymous methods and lambda expressions are.*

1.  *Anonymus method:*
   
An anonymous method is a method without a name, used to create an inline delegate instance.

 1. _Lambda expression:_
    
A lambda expression is a shorthand syntax for defining anonymous methods.


2. _Predicate delegates_:
   
```Predicate<T>```: Represents a method that defines a set of criteria and determines whether the specified object meets those criteria. It returns a Boolean value and takes one parameter.


4.  _Comparison delegates_:
   
```Comparison<T>```: Represents a method that compares two objects of the same type. It is often used for sorting and comparison purposes in collections.


### *What events are and how they are used in .NET to notify of specific actions in a class or changes in an object's state.*

Events in .NET are a way for a class or object to notify other classes or objects when something interesting happens. They are based on the delegate model and are typically used to implement the observer pattern, allowing subscribers to be notified of and respond to specific actions or changes in the state of an object.


### *The mechanism of closure when using delegates.*

A closure is a feature that allows a delegate or an anonymous method to capture and use variables from its surrounding scope. When you use a delegate or a lambda expression, it can "capture" local variables from the enclosing method where it is defined.


