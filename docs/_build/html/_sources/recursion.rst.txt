`Documentation Home <https://docs.knightmovesolutions.com>`_

=========
Recursion
=========

You can process the nodes of your tree of objects recursively without having to code a recursive function.

-----------------------
Using a Lambda Function
-----------------------

You can easily recurse the tree by using a lambda function.

.. code-block:: csharp

    IList<string> testOutput = new List<string>();

    familyTree.ProcessTree(node => testOutput.Add(node.Id + " Processed"));

You can also recursively process ancestor nodes up the tree as easy as you can process children. Just use 
one of the similar ``ProcessAncestors`` methods.

The example below uses the sample hierarchy above where the ``me`` ``Person`` object is the third level down 
(i.e. grandchild):

.. code-block:: csharp 

    IList<string> testOutput = new List<string>();

    // Starts with the `me` object and processes up to the 
    // root (grandpa object) because maxLevel == 1 by default 
    familyTree.ProcessAncestors( node => testOutput.Add(node.Id + " Processed"), me );

You can cause it to stop recursing up the ancestor line early by setting the maxLevel value

.. code-block:: csharp 

    IList<string> testOutput = new List<string>();

    // Starts with the `me` object and processes up to the 
    // 2nd level (dad object) because maxLevel == 2 
    familyTree.ProcessAncestors( node => testOutput.Add(node.Id + " Processed"), me, 2 );

If the logic to stop it from going up the ancestor tree early is more complicated, then you can 
provide a stopFunction with more robust logic to determine if it should stop climbing the tree.

.. code-block:: csharp 

    IList<string> testOutput = new List<string>();

    // Starts with the `me` object and processes up to the 
    // level that satisfies the stopFunction 
    familyTree.ProcessAncestors
    ( 
        node => testOutput.Add(node.Id + " Processed"), 
        me, 
        node => return node.Id == "Dad" 
    );

The stopping logic first processes the current node and then evaluates whether it should stop or not. 
If true, it will return without climbing to the next level. Keep in mind that it does that only after 
executing your ``ITreeNodeProcessor<TId, T>`` or your ``nodeProcessor`` lambda function against the 
current node first.

So you can see that with one quick function in a method call you can test an entire tree for something 
whether you're going down recursively to all children/branches or up the ancestor line of the tree.

-------------
Using a Class
-------------

First you would create the code that processes the nodes of the tree by implementing ``ITreeNodeProcessor<TId, T>``, 
which requires the implementation of one method called ``void ProcessNode(T node)``.

For example:

.. code-block:: csharp

    class MyTreeNodeProcessor : ITreeNodeProcessor<string, Person>
    {
        public IList<string> TestOutput;

        public MyTreeNodeProcessor()
        {
            TestOutput = new List<string>();
        }

        public void ProcessNode(Person node)
        {
            TestOutput.Add(node.Id + " Processed");
        }
    }

Then you just feed it to the top of the node you want to process. In the example below, we process every 
node in the tree because we start at the root node.

.. code-block:: csharp 

    var myProcessor = new MyTreeNodeProcessor();
    familyTree.ProcessTree(myProcessor);

However, if you want to process only a branch, then you can pass the tree node processor to the root of 
that branch only. If you want to exclude the root node (or root of branch node) and process starting with 
its children, then you can use the ``ProcessChildren(ITreeNodeProcessor<TId, T> nodeProcessor)`` method instead.

.. code-block:: csharp 

    var myProcessor = new MyTreeNodeProcessor();
    familyTree.ProcessChildren(myProcessor);

As you can see, you can just write your business logic and forget about having to recurse. Your tree node 
processor could act on the node or not depending on the logic. If your entity is a View Model, you can set 
property values to drive user interface changes such as setting ``IsHighlighted = true`` if the object 
matches a search string.

----------------
Class vs. Lambda
----------------

Since the most common practice is to use a lambda function it begs the question, why go through the trouble of 
creating a class to do the same thing? Wouldn't that be a lot of work when you can just shortcut it with a 
lambda function to do the same thing? 

It depends.

The feature to be able to use a class was provided for cases when the logic necessary to process the nodes in 
the tree is very elaborate and may require injecting dependencies to get that logic to work. It may be a better 
design to encapsulate that in a class whose responsibility is to deal with that more complex logic. 

Also, using a class gives you the option to use a Strategy Pattern by coding to the interface of the node 
processing class. When writing a lambda function, you cannot swap out that implementation neither through some 
configuration nor at runtime by selecting from a number of available implementing classes. 

In general, if the logic is straight forward and short, which is probably the most common use case, then go with 
the lambda function. If the logic is elaborate, requires the use of dependencies sepecific to the logic that 
processes the nodes, and/or you want to be able to swap processors using a Strategy Pattern, then go with the 
class. 

If you want to swap functions using functional programming techniques instead of the object-oriented practice of 
using a Strategy Pattern then go ahead, do your thing, and avoid classes altogether. Choice is yours.