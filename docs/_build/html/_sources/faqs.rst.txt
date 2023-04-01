`Documentation Home <https://docs.knightmovesolutions.com>`_

==================
FAQs
==================

**I can utilize recursion with the SQL syntax of my database. Why not use that?**

You can certainly use that to produce or process your tree. However, you will find that for every 
type of entity that must participate in a tree you will need to write that kind of recursive code 
again. With this library it is done once. Also, having a business logic layer implementation makes 
it database agnostic. You can port your data to another database easier because your hierarchical 
processing is done in the application code. 

The output of the ``SortableTreePath`` also makes reporting easier. Sometimes the people creating 
the reports are not developers. You can use a reporting tool like Tableau and easily create a tree 
with indented children simply by sorting on the ``SortableTreePath``. This is implemented once for 
all of your hierarchical entities. 

Lastly, if you're like me and you prefer not to put business logic in the database, then this makes 
it easier to write code to process a tree recursively and keep the business logic in the application 
code.

**My entity already inherits from something. C# doesn't support multiple inheritance so what do I do?**

Use the ``TreeNodeWrapper<TId, T>`` class like so.

Instead of ..

.. code-block:: csharp

    var grandpa = new Person { Id = "Grandpa", Name = "Richard" }
    var dad = new Person { Id = "Dad", ParentId = "Grandpa" Name = "Richard Jr." };
    // ... etc.

 ... you would code ...

.. code-block:: csharp 

    var grandpa = new TreeNodeWrapper<string, Person>
    (
        new Person 
        { 
            Id = "Grandpa", 
            Name = "Richard" 
        }, 
        "Grandpa", 
        null
    );

    var dad = new TreeNodeWrapper<string, Person>
    (
        new Person 
        { 
            Id = "Dad", 
            ParentId = "Grandpa", 
            Name = "Richard Jr." 
        }, 
        "Dad", 
        "Grandpa"
    );

    // ... etc.

 ... and the rest is the same.

