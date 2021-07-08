# KnightMoves.Hierarchical

A lightweight library of a handful of interfaces/classes that implement everything an entity needs to participate as a node in a tree. 
The purpose of this implementation is to take care of all things necessary for creating tree structures and recursively processing the
nodes in the tree. Some scenarios where trees are useful can be category trees, account/subaccount trees, organization trees, management/employee 
hierarchies, etc. 

## License

MIT License

## Getting Started

You can pull the code from the repository and build it yourself or consume the NuGet package in your solution.

	PM> Install-Package KnightMoves.Hierarchical

It is basically just a single DLL. Here's what you get. 

Suppose you have the following entity class in your application.

    public class Person
    {
        public string Name { get; set; }
        // And a bunch of other properties not included for brevity
    }

All you have to do is inherit from `TreeNode<TId, T>` and your `Person` class becomes a fully capable node in a hierarchy. 

	// Just inherit from TreeNode<TId, T>
    // TId is the type of the entity's Id field. If the name of your entity's identifier property
    // is "Id" then you should override the inherited "Id" property since it is virtual or remove it

    public class Person : TreeNode<string, Person>
    {
        // Person is now able to function as a tree node
        // Id and ParentID are inherited properties or 
        // override it if you name it the same

        public override string Id { get; set; }
        public string Name { get; set; }
    }

One instance of your class is not useful as a tree. So to illustrate, we start with a collection of `Person` objects.

	List<Person> persons = _personRepository.GetPersons();

The persons collection can contain the `Person` objects in any order. As long as they have an `Id` and a `ParentId` with a single 
'Person' object having a `null` `ParentId` representing the root node, you can create the tree like this.

	var hierarchy = TreeNode<string, Person>.CreateTree(persons)

The `hierarchy` object is a single `ITreeNode<TId, T>` object that contains all other objects in its `Children` collection. Some of 
those children will have child objects in their `Children` collection so on and so forth. 

That's all you need at its most basic level.

## A Complete Example

Here is an example console application.

	namespace MyApp
	{
        // Just inherit from TreeNode<TId, T>
        public class Person : TreeNode<string, Person>
        {
	        // Person is now able to function as a tree node
	        // Id and ParentId are inherited properties

	        public string Name { get; set; }
        }

        public class MyFamilyTreeApp
        {
            public static void Main(string[] args)
            {
                var grandpa = new Person { Id = "Grandpa", Name = "Richard" };
                var dad = new Person { Id = "Dad", ParentId = "Grandpa", Name = "Richard Jr." };
                var uncle = new Person { Id = "UncleJohn", ParentId = "Grandpa", Name = "John" };
                var cousin = new Person { Id = "CousinAnn", ParentId = "UncleJohn", Name = "Ann" };
                var sister = new Person { Id = "SisterJane", ParentId = "Dad", Name = "Jane" };
                var me = new Person { Id = "Me", ParentId = "Dad", Name = "MeMyselfAndI" };

                var familyMembers = new List<Person>();

                // Let's add them in random order just to prove a point
                familyMembers.Add(sister);
                familyMembers.Add(dad);
                familyMembers.Add(uncle);
                familyMembers.Add(grandpa);
                familyMembers.Add(me);
                familyMembers.Add(cousin);

                // Not linked in tree structure
                Console.WriteLine(grandpa.Children.Count);      // prints 0
                Console.WriteLine(dad.Parent == null);          // prints true
                Console.WriteLine(me.Parent == null);           // prints true

                // Creates tree structure automatically by using Id and ParentId of objects.
                // Even though the family members were added in random order to the collection 
                // the tree is still created properly. It returns a single Person object. In this 
                // case the root Person object is grandpa and it will return it with all other 
                // child objects arranged under it in the hierarchy.

                var familyTree = TreeNode<string, Person>.CreateTree(familyMembers);

                Console.WriteLine(familyTree == grandpa);                           // prints true
                Console.WriteLine(familyTree.Children.Count);                       // prints 2
                Console.WriteLine(familyTree.Children[0] == dad);                   // prints true
                Console.WriteLine(familyTree.Children[1] == uncle);                 // prints true
                Console.WriteLine(familyTree.Children[0].Children.Count);           // prints 2
                Console.WriteLine(familyTree.Children[0].Children[0] == sister);    // prints true
                Console.WriteLine(familyTree.Children[0].Children[1] == me);        // prints true
                Console.WriteLine(familyTree.Children[1].Children.Count);           // prints 1
                Console.WriteLine(familyTree.Children[1].Children[0] == cousin);    // prints true

                // Linked in tree now
                Console.WriteLine(grandpa.Children.Count);      // prints 2
                Console.WriteLine(dad.Parent == grandpa);       // prints true
                Console.WriteLine(me.Parent == dad);            // prints true
            }
        }
	}

## Persisting To Your Database

In order to recreate the tree later on, it is important to persist the `Id` and `ParentId` properties in the database. When the 
collection of objects of your entity type is retrieved, the `Id` and `ParentId` properties are used to rebuild the tree. The 
only other requirement is that the collection must have only one object with the `ParentId` set to `null`, which represents the 
root object. Other properties of `ITreeNode<TId, T>` are not required to save in the database but can be very useful.

## Useful Methods Your Entity Class Inherits

* `FindById` - Finds and returns the ITreeNode<T> object where the Id value is equal to the nodeId value provided as an argument. It will search the tree recursively until it is found.
* `IsAncestor` - Determines if the node provided as the treeNode is an ancestor of this node up the tree.
* `IsDescendent` - Determines if the node provided as the treeNode is an descendant of this node down the tree.
* `IsSibling`	- Determines if the node provided as the treeNode is a sibling of this node.
* `MakeChildOfUpperSibling` - Makes this node a child of the sibling higher in the order of the Children collection of its parent.
* `MakeSiblingOfParent` - Makes this node a sibling of its parent.
* `MoveDownInSiblingOrder` - Moves this node down in the sibling order.
* `MoveUpInSiblingOrder` - Moves this node up in the sibling order.
* `ProcessChildren(Func<ITreeNode<T>, Boolean>)` - Passes each child of this node to the nodeProcessor provided recursively down the tree. It does not include this node.
* `ProcessChildren(ITreeNodeProcessor<T>)` - Passes each child of this node to the nodeProcessor provided recursively down the tree. It does not include this node.
* `ProcessTree(Func<ITreeNode<T>, Boolean>)` - Passes each child of this node to the nodeProcessor provided recursively down the tree. Unlike ProcessChildren, this method will start with (include) this node.
* `ProcessTree(ITreeNodeProcessor<T>)` - Passes each child of this node to the nodeProcessor provided recursively down the tree. Unlike ProcessChildren, this method will start with (include) this node.
* `ProcessAncestors(ITreeNodeProcessor<TId, T> nodeProcessor, ITreeNode<TId, T> treeNode, int maxLevel = 1)` - Passes each node up the tree of ancestors to the `nodeProcessor` starting with `treeNode` and stopping at the node where `DepthFromRoot' = 1 (i.e. the top) or the specified maxLevel.
* `ProcessAncestors(Func<ITreeNode<TId, T>, bool> nodeProcessor, ITreeNode<TId, T> treeNode, int maxLevel = 1)` - Passes each node up the tree of ancestors to the `nodeProcessor function` starting with `treeNode` and stopping at the node where `DepthFromRoot' = 1 (i.e. the top) or the specified maxLevel.
* `ProcessAncestors(Func<ITreeNode<TId, T>, bool> nodeProcessor, ITreeNode<TId, T> treeNode, Func<ITreeNode<TId, T>, bool> stopFunction)` - Passes each node up the tree of ancestors to the `nodeProcessor function` starting with `treeNode` and stopping at the node that causes the `stopFunction` to return true.

## Useful Properties Your Entity Class Inherits

* `Children` - The Child objects of this node in the hierarchy
* `DepthFromRoot`	- The number of levels in the tree where the ROOT node is Level 1. Whatever contains the Root (i.e. whole tree) is Level 0.
* `HasChildren` - True if this node has Children false if not.
* `Id` - The ID of this node. It's data type is customized by the `TId` generic type specification.
* `IndentCharacter` - The character used for indentation. It will be repeated once for every Level in the tree, which is reported by the DepthFromRoot property by the IndentString property.
* `IndentString` - Repeats the IndentCharacterDepthFromRoot number of times to produce the indentation string for pretty display in text output.
* `Parent` - The Parent node of this node
* `ParentId` - The Id of the Parent object of this node.
* `PathId` - A unique identifier representing the unique path from the root to the current node. Essentially, it is a hash of the chain of hashes of ancestor Ids.
* `Root` - A reference to the Root object in the tree. All objects in the tree will have the same Root.
* `RoodId` - The Id of the Root object. Useful for persisting to the database when there are multiple hierarchies in the same table differentiated by their shared Root.
* `Siblings` - The other objects in the Parent objects Children collection. It returns all objects in that collection except for this node.
* `SortableTreePath` - A string representation of the location of this object in the tree in the form of a "Path".
* `TreeNodeId` - A unique identifer for the node. Not required but can be useful.

## About the 'SortableTreePath' Property

This property implements the Materialized Path Pattern. A well-known example of Materialized Path is the common file system folder path. C:\Users\Bob\Documents is an example of a Materialized Path.
The SortableTreePath produces a path using integers starting with 1. See the example below.

    Tree                     SortableTreePath
    ----------------------   ---------------------------
    RootNode                 1
      ItemA                  1.1
        ItemA1               1.1.1
        ItemA2               1.1.2
      ItemB                  1.2
        ItemB1               1.2.1
          ItemB1A            1.2.1.1
      ItemC                  1.3
      ItemD                  1.4
        ItemD1               1.4.1
        ItemD2               1.4.2
      ...
      etc.
            
You can see that if you sort by the `SortableTreePath` it will result in a list where every object is in order by its position in the tree.
This value is incredibly useful when persisting to the database. It allows for the use of very simple SQL queries to get nodes or segments of the tree.

Using the dataset above you can do the following.

NOTE: Keep in mind that if you have multiple trees in the table, you will have to add WHERE RootId = 'Something' to avoid mixing nodes from different
trees. Your data access model would have to persist the RootId from the entity in order to accommodate this. 

Get all nodes in the tree in hierarchical order:

	SELECT IndentString + Name FROM MyTreeTable ORDER BY SortableTreePath

Result:

    Name
    -------------
    RootNode  
      ItemA 
        ItemA1 
        ItemA2
      ItemB
        ItemB1
          ItemBA
      ItemC 
      ItemD
        ItemD1
        ItemD2

Get the ItemB tree

	SELECT IndentString + Name FROM MyTreeTable WHERE SortableTreePath LIKE '1.2%' ORDER BY SortableTreePath

Result:

    Name
    -------------
      ItemB
        ItemB1
          ItemBA

Get children of ItemD:

	SELECT Name FROM MyTreeTable WHERE SortableTreePath LIKE '1.4.%' ORDER BY SortableTreePath

Result:

    Name
    -------------
    ItemD1
    ItemD2   
            
Get Ancestors of ItemBA:

	SELECT @itemBAPath = SortableTreePath FROM MyTreeTable WHERE Name = 'ItemBA' -- Noramlly use ID but you get the idea
	SELECT Name FROM MyTreeTable WHERE @itemBAPath LIKE SortableTreePath + '%' ORDER BY SortableTreePath

Result:

    Name
    -------------
    RootNode  
      ItemB
        ItemB1
          ItemBA

## Process the Tree Recursively

You can process the nodes of your tree of objects recursively without having to code a recursive function. 

First you would create the code that processes the nodes of the tree by implementing `ITreeNodeProcessor<TId, T>`, which requires the 
implementation of one method called `bool ProcessNode(ITreeNode<TId, T> node)`. 

For example:

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

Then you just feed it to the top of the node you want to process. In the example below, we process every node in the tree because 
we start at the root node.

    var myProcessor = new MyTreeNodeProcessor();
    familyTree.ProcessTree(myProcessor);

However, if you want to process only a branch, then you can pass the tree node processor to the root of that branch only. If you 
want to exclude the root node (or root of branch node) and process starting with its children, then you can use the 
`ProcessChildren(ITreeNode<TId, T> node)` method instead. 

    var myProcessor = new MyTreeNodeProcessor();
    familyTree.ProcessChildren(myProcessor);

As you can see, you can just write your business logic and forget about having to recurse. Your tree node processor could act on 
the node or not depending on the logic. If your entity is a View Model, you can set property values to drive user interface changes 
such as setting 'IsHighlighted = true' if the object matches a search string.  

You can make this shorter by using a lambda.

	IList<string> testOutput = new List<string>();

    familyTree.ProcessTree(node => {
        testOutput.Add(node.Id + " Processed");
        return true;
    });

The return value of the `ProcessTree` method is the logical AND result of all the resulting processing of the nodes. To 
illustrate consider the following example:

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

You can also process ancestor nodes up the tree as easy as you can process children. Just use one of 
the similar `ProcessAncestors` methods. 

The example below uses the sample hierarchy above where the `me` Person object is the third level down (i.e. grandchild):

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

    IList<string> testOutput = new List<string>();

    // Starts with the `me` object and processes up to the 
    // 2nd level (dad object) because maxLevel == 2 by default 
    bool okay = familyTree.ProcessAncestors(
        (node) => 
        {
            testOutput.Add(node.Id + " Processed");
            return result;
        },
        me,
        2
    );

If the logic to stop it from going up the ancestor tree early is more complicated, then you can provide a 
`stopFunction` with more robust logic to determine if it should stop climbing the tree.

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

The stopping logic first processes the current node and then evaluates whether it should stop or not. If true,
it will return without climbing to the next level. Keep in mind that it does that only after executing your 
`ITreeNodeProcessor<TId, T>` or your `nodeProcessor` lambda function against the current node first.

So you can see that with one quick function in a method call you can test an entire tree for something whether you're
going down recursively to all children/branches or up the ancestor line of the tree. 

There is no equivalent for a logical OR processing of the nodes, where it would return true if ANY of the tests in the condition resulted in true. 
However, you can code this yourself as part of your function. Just keep OR'ing some boolean that is outside of the function. This 
sets you up to do all kinds of neat things with trees easily. 

## Using Entity Framework

Utilizing an domain model object that inherits from `TreeNode<TId, T>` is problematic due to the particulars of how
Entity Framwork functions. To accommodate the use of EF so that the `ITreeNode<TId, T>` properties can be persisted
to the database, another abstract class was created to satisfy this need. 

#### TreeNodeEF<TId, T>

If your domain entity is to be persisted to the database using Entity Framework, then it should inherit from 
`TreeNodeEF<TId, T>` instead of `TreeNode<TId, T>`. Certain properties of the `ITreeNode<TId, T>` model 
have been decorated with data annotations to instruct EF on what to do with those properties such as annotating
properties with `[NotMapped]` to prevent EF from persisting them. It is recommended to annotate the `Id` 
property with the `[Key]` attribute or configure it as the primary key using the Fluent API. If the primary key 
of your entity is not named `Id` then you can override the `Id` property with `get` and `set` code that wires the 
`Id` property to your entity's primary key property. You should then mark the `ParentId` as a foreign key 
fluently or with the `[ForeignKey("ParentEntity")]` annotation where the `ParentEntity` has been provided 
to serve as the navigation property for the parent. This makes it a self-referencing foreign key implementing 
the hierarchy in the relational model.

There's some other trickery done under the hood to get it to work, particularly with 
properties that are computed at runtime such as the `SortableTreePath` property. If you're curious you can look
at the source code but it basically takes care of the tree node properties that are persistable to the database 
for you. The rest of your own properties in your subclass are up to you to manage as you see fit but whatever you
want to persist to the database will go along with the underlying inherited properties managed by `TreeNodeEF<TId, T>`.

Lastly, inheriting from `TreeNodeEF<TId, T>` will work with migrations as long as your subclass is defined as a 
`DbSet<T>` property in your application's `DbContext`

## JSON Serialization

Due to the fact that the `TreeNode<TId, T>` implementation includes references to the `Parent`, `Root` and `Siblings`
objects, it introduces complications when serializing the tree for JSON output. It will fail with errors about circular
references. To make things worse, for some reason at the time of this documentation (July 2021), the `[JsonIgnore]`
attribute does not work for NewtonSoft or the `System.Text.Json.Serialization` namespace.

In order to get around this, two methods were implemented to mark the tree as serializable and unmark the tree as 
serializable. 

    familyTree.MarkAsSerializable();

    // Safe to serialize to JSON here. Cannot use some methods.

    familyTree.UnMarkAsSerializable();

    // Restored to original state. Safe to use all methods.

The `MarkAsSerializable()` method sets the `IsSerializable` property to true for all nodes in the tree recursively. 
When `IsSerializable` is `true` then the `Root`, `Parent`, and `Siblings` properties will return `null` in 
order to avoid the circular reference exception during serialization. During this time, methods that rely on `Parent`
or `Root` will not work. Therefore, you should only mark the tree as serializable for the lines of code that 
produce the JSON string. After that, if you want the full functionality of the tree, then you must restore
it by calling the `UnMarkSerializable()` method, which will set the `IsSerializable` property to true for all nodes 
in the tree recursively. After that all methods of the tree that depend on `Parent`, `Root`, and `Siblings` will 
function normally.
    

## Q & A

#### I can utilize recursion with the SQL syntax of my database. Why not use that?

You can certainly use that to produce or process your tree. However, you will find that for every type of entity that must 
participate in a tree you will need to write that kind of recursive code again. With this library it is done once. Also, 
having a business logic layer implementation makes it database agnostic. You can port your data to another database easier 
because your hierarchical processing is done in the application code. The output of the SortableTreePath also makes reporting 
easier. Sometimes the people creating the reports are not developers. You can use a reporting tool like Tableau and easily 
create a tree with indented children simply by sorting on the SortableTreePath. This is implemented once for all of your 
hierarchical entities. Lastly, if you're like me and you prefer not to put business logic in the database, then this makes 
it easier to write code to process a tree recursively and keep the business logic in the application code.

#### My entity already inherits from something. C# doesn't support multiple inheritance so what do I do?

Use the `TreeNodeWrapper<TId, T>` class like so. 

Instead of ...

    var grandpa = new Person { Id = "Grandpa", Name = "Richard" }
    var dad = new Dad { Id = "Dad", ParentId = "Grandpa" Name = "Richard Jr." };
    // ... etc.

... you would code ... 

    var grandpa = new TreeNodeWrapper<string, Person>(new Person { Id = "Grandpa", Name = "Richard" }, "Grandpa", null);
    var dad = new TreeNodeWrapper<string, Person>(new Dad { Id = "Dad", ParentId = "Grandpa" Name = "Richard Jr." }, "Dad", "Grandpa");
    // ... etc.

... and the rest is the same.


