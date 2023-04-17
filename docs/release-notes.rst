`Documentation Home <https://docs.knightmovesolutions.com>`_

=============
Release Notes
=============

-----
3.1.0
-----

* Switched to the use of ``T`` instead of ``ITreeNode<TId, T>`` so casting is no longer necessary for the following properties
  and collections 

  * ``Root``
  * ``Parent`` 
  * ``Children``
  * ``Siblings``

* Added the new ``T Find(Func<T, bool> condition)`` method to allow finding of a node anywhere in the tree using an arbitrary 
  lambda function to test for a custom condition

* Added robust examples to documentation 

-----
3.0.0
-----

Breaking Changes
----------------

* Bumped major version to 3 to reflect breaking changes 

* Switched to the use of ``T`` instead of ``ITreeNode<TId, T>`` so casting is no longer necessary in the following places 

  * ``FindById()`` return type 
  * ``ProcessChildren()`` lambda function parameter type 
  * ``ProcessTree()`` lambda function parameter type 

* Changed the return type from ``bool`` to ``void`` for the methods below so that lambdas no longer require a function body.
  One-liner lambda functions are now possible.

  * All ``ProcessChildren()`` methods 
  * All ``ProcessTree()`` methods 
  * All ``ProcessAncestors()`` methods 

Enhancements
------------ 

* Ported to .NET Core 6
* Added Contains() method 
* Added DeepCopy() method 
* Added Filter() method 
* Added ToList() method 


-----
2.1.1
-----
* Fixed bug where TypeName was null when saving to the database using EF Core

-----
2.1.0
-----
* Added the ability to deserialize JSON into the hierarchical object model when the nodes are polymorphic (i.e. different base classes that implement the same base type)

-----
2.0.4
-----
* Fixed serialization bugs

-----
2.0.3
-----
* Fixed serialization bugs

-----
2.0.2
-----
* Added support for JSON Deserialization

-----
2.0.1
-----
* Added support for JSON Serialization

-----
2.0.0
-----
* Ported to .NET Core 3
* Added generic defintion for the Id data type 
* Added PathId property to uniquely identify a path
* Added ProcessAncestors() methods
* Added support for Entity Framework Core
* No longer supporting WPF
* Updated documentation in README.md


