`Documentation Home <https://docs.knightmovesolutions.com>`_

=========
Recursion
=========

You can process the nodes of your tree of objects recursively without having to code a recursive function.

First you would create the code that processes the nodes of the tree by implementing ``ITreeNodeProcessor<TId, T>``, 
which requires the implementation of one method called ``bool ProcessNode(ITreeNode<TId, T> node)``.

For example:

.. code-block:: csharp

    class MyTreeNodeProcessor : ITreeNodeProcessor<string, Person>
    {
        public IList<string> TestOutput;

        public MyTreeNodeProcessor()
        {
            TestOutput = new List<string>();
        }

        public bool ProcessNode(ITreeNode<string, Person> node)
        {
            TestOutput.Add(node.Id + " Processed");
            return true;
        }
    }

Then you just feed it to the top of the node you want to process. In the example below, we process every 
node in the tree because we start at the root node.

.. code-block:: csharp 

    var myProcessor = new MyTreeNodeProcessor();
    familyTree.ProcessTree(myProcessor);

However, if you want to process only a branch, then you can pass the tree node processor to the root of 
that branch only. If you want to exclude the root node (or root of branch node) and process starting with 
its children, then you can use the ``ProcessChildren(ITreeNode<TId, T> node)`` method instead.

.. code-block:: csharp 

    var myProcessor = new MyTreeNodeProcessor();
    familyTree.ProcessChildren(myProcessor);

As you can see, you can just write your business logic and forget about having to recurse. Your tree node 
processor could act on the node or not depending on the logic. If your entity is a View Model, you can set 
property values to drive user interface changes such as setting ``IsHighlighted = true`` if the object 
matches a search string.

You can make this shorter by using a lambda.

.. code-block:: csharp

    IList<string> testOutput = new List<string>();

    familyTree.ProcessTree(node => {
        testOutput.Add(node.Id + " Processed");
        return true;
    });

The return value of the ``ProcessTree`` method is the logical AND result of all the resulting processing of 
the nodes. To illustrate consider the following example:

.. code-block:: csharp 

    IList<string> testOutput = new List<string>();

    bool okay = familyTree.ProcessTree(node => {

        bool result = true;

        // Test the node for something 
        if(node.IsManager == false)
            result = false;

        testOutput.Add(node.Id + " Processed");
        return result;
    });

    if(okay) 
    {
        // All nodes processed returned true
    } else {
        // At least one of the nodes that were processed failed
        // whatever condition you coded in the function
    }

You can also process ancestor nodes up the tree as easy as you can process children. Just use one of the 
similar ``ProcessAncestors`` methods.

The example below uses the sample hierarchy above where the me ``Person`` object is the third level down 
(i.e. grandchild):

.. code-block:: csharp 

    IList<string> testOutput = new List<string>();

    // Starts with the `me` object and processes up to the 
    // root (grandpa object) because maxLevel == 1 by default 
    bool okay = familyTree.ProcessAncestors(
        (node) => 
        {
            testOutput.Add(node.Id + " Processed");
            return result;
        },
        me
    );

You can cause it to stop recursing up the ancestor line early by setting the maxLevel value

.. code-block:: csharp 

    IList<string> testOutput = new List<string>();

    // Starts with the `me` object and processes up to the 
    // 2nd level (dad object) because maxLevel == 2 
    bool okay = familyTree.ProcessAncestors(
        (node) => 
        {
            testOutput.Add(node.Id + " Processed");
            return result;
        },
        me,
        2
    );

If the logic to stop it from going up the ancestor tree early is more complicated, then you can 
provide a stopFunction with more robust logic to determine if it should stop climbing the tree.

.. code-block:: csharp 

    IList<string> testOutput = new List<string>();

    // Starts with the `me` object and processes up to the 
    // level that satisfies the stopFunction 
    bool okay = familyTree.ProcessAncestors(
        (node) => 
        {
            testOutput.Add(node.Id + " Processed");
            return result;
        },
        me,
        (node) => 
        {
            return node.Id == "Dad";
        }
    );

The stopping logic first processes the current node and then evaluates whether it should stop or not. 
If true, it will return without climbing to the next level. Keep in mind that it does that only after 
executing your ``ITreeNodeProcessor<TId, T>`` or your ``nodeProcessor`` lambda function against the 
current node first.

So you can see that with one quick function in a method call you can test an entire tree for something 
whether you're going down recursively to all children/branches or up the ancestor line of the tree.

There is no equivalent for a logical OR processing of the nodes, where it would return true if ANY 
of the tests in the condition resulted in true. However, you can code this yourself as part of your 
function. Just keep OR'ing some boolean that is outside of the function. This sets you up to do all 
kinds of neat things with trees easily.



