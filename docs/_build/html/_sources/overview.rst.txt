`Documentation Home <https://docs.knightmovesolutions.com>`_

========
Overview
========

What is it?
-----------

This is a lightweight library of a handful of interfaces/classes that implement everything an 
entity needs to participate as a node in a tree. 

What problem does it solve?
---------------------------

The purpose of this library is to take care of all the things that are necessary for creating 
tree structures and recursively processing the nodes in the tree. To be clear, this isn't to be 
confused with low-level structures like B-Trees. It facilitates the creation of the Composite 
Object-Oriented design pattern for C# objects by simply inheriting from a base tree node object.

When do I use it?
-----------------

Through the course of developing business applications across various industries, it is inevitable
that you will run into scenarios where trees are anywhere from useful to necessary. Some scenarios 
where trees can be utilized include category trees, account/subaccount trees, organization trees, 
management/employee hierarchies, etc. 

An object-oriented tree structure can also be useful to 
drive hierarchical user interfaces. This library makes it easy to wire user interface actions to 
the underlying tree structure and ultimately for persitence in a file, JSON document, or database.
User interface actions that might be needed to manage a tree structure include moving whole branches
up, down, left, or right in the tree and rearranging the sibling order. 

What Next?
----------

If you are familiar with this library and need a reminder on the steps necessary to implement
or if you just want to get crackin' then go to the :doc:`quick-start`
