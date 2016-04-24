# KnightMoves.Hierarchical

A lightweight library of a handful of interfaces/classes that implement everything an entity needs to participate as a node in a tree. 
The purpose of this implementation is to take care of all things necessary for creating tree structures and recursively processing the
nodes in the tree. Some scenarios where trees are useful can be category trees, account/subaccount trees, organization trees, management/employee 
hierarchies, etc. 

## License

MIT License. Have fun. You're welcome. =]

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

All you have to do is inherit from 'TreeNode<T>' and your 'Person' class becomes a fully capable node in a hierarchy. 

	// Just inherit from TreeNode<T>
    public class Person : TreeNode<Person>
    {
        // Person is now able to function as a tree node
        // ID and ParentID are inherited properties

        public string Name { get; set; }
    }

One instance of your class is not useful as a tree. So to illustrate, we start with a collection of 'Person' objects.

	List<ITreeNode<Person>> persons = _personRepository.GetPersons();

The persons collection can contain the 'Person' objects in any order. As long as they have an 'Id' and a 'ParentId' with a single 
'Person' object having a 'null' 'ParentId' representing the root node, you can create the tree like this.

	ITreeNode<ITreeNode<Person>> hierarchy = TreeNode<ITreeNode<Person>>.CreateTree(persons)

The 'hierarchy' object is a single ITreeNode<T> object that contains all other objects in its 'Children' collection. Some of 
those children will have child objects in their 'Children' collection so on and so forth. 

That's all you need at its most basic level.

## A Complete Example

Here is an example console application.

	namespace MyApp
	{
		// Just inherit from TreeNode<T>
		public class Person : TreeNode<Person>
		{
			// Person is now able to function as a tree node
			// ID and ParentID are inherited properties

			public string Name { get; set; }
		}

		public class MyFamilyTreeApp
		{
			public static void Main(string[] args)
			{
				Person grandpa = new Person { Id = "Grandpa", Name = "Richard" };
				Person dad = new Person { Id = "Dad", ParentId = "Grandpa", Name = "Richard Jr." };
				Person uncle = new Person { Id = "UncleJohn", ParentId = "Grandpa", Name = "John" };
				Person cousin = new Person { Id = "CousinAnn", ParentId = "UncleJohn", Name = "Ann" };
				Person sister = new Person { Id = "SisterJane", ParentId = "Dad", Name = "Jane" };
				Person me = new Person { Id = "Me", ParentId = "Dad", Name = "MeMyselfAndI" };

				List<Person> familyMembers = new List<Person>();

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

				// Creates tree structure automatically by using ID and ParentID of objects.
				// Even though the family members were added in random order to the collection 
				// the tree is still created properly.
				ITreeNode<ITreeNode<Person>> familyTree = TreeNode<ITreeNode<Person>>.CreateTree(familyMembers);

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

In order to recreate the tree later on, it is important to persist the 'Id' and 'ParentId' properties in the database. When the 
collection of objects of your entity type is retrieved, the 'Id' and 'ParentId' properties are used to rebuild the tree. The 
only other requirement is that the collection must have only one object with the 'ParentId' set to 'null', which represents the 
root object. Other properties of 'ITreeNode<T>' are not required to save in the database but can be very useful.

## Useful Methods Your Entity Class Inherits

* FindById - Finds and returns the ITreeNode<T> object where the Id value is equal to the nodeId value provided as an argument. It will search the tree recursively until it is found.
* IsAncestor - Determines if the node provided as the treeNode is an ancestor of this node up the tree.
* IsDescendent - Determines if the node provided as the treeNode is an descendant of this node down the tree.
* IsSibling	- Determines if the node provided as the treeNode is a sibling of this node.
* MakeChildOfUpperSibling - Makes this node a child of the sibling higher in the order of the Children collection of its parent.
* MakeSiblingOfParent - Makes this node a sibling of its parent.
* MoveDownInSiblingOrder - Moves this node down in the sibling order.
* MoveUpInSiblingOrder - Moves this node up in the sibling order.
* ProcessChildren(Func<ITreeNode<T>, Boolean>) - Passes each child of this node to the nodeProcessor provided recursively down the tree. It does not include this node.
* ProcessChildren(ITreeNodeProcessor<T>) - Passes each child of this node to the nodeProcessor provided recursively down the tree. It does not include this node.
* ProcessTree(Func<ITreeNode<T>, Boolean>) - Passes each child of this node to the nodeProcessor provided recursively down the tree. Unlike ProcessChildren, this method will start with (include) this node.
* ProcessTree(ITreeNodeProcessor<T>) - Passes each child of this node to the nodeProcessor provided recursively down the tree. Unlike ProcessChildren, this method will start with (include) this node.

## Useful Properties Your Entity Class Inherits

* Children - The Child objects of this node in the hierarchy
* DepthFromRoot	- The number of levels in the tree where the ROOT node is Level 1. Whatever contains the Root (i.e. whole tree) is Level 0.
* HasChildren - True if this node has Children false if not.
* Id - The ID of this node
* IndentCharacter - The character used for indentation. It will be repeated once for every Level in the tree, which is reported by the DepthFromRoot property by the IndentString property.
* IndentString - Repeats the IndentCharacterDepthFromRoot number of times to produce the indentation string for pretty display in text output.
* Parent - The Parent node of this node
* ParentId - The Id of the Parent object of this node.
* Root - A reference to the Root object in the tree. All objects in the tree will have the same Root.
* Siblings - The other objects in the Parent objects Children collection. It returns all objects in that collection except for this node.
* SortableTreePath - A string representation of the location of this object in the tree in the form of a "Path".
* TreeNodeId - A unique identifer for the node. Not required but can be useful.

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
            
You can see that if you sort by the SortableTreePath it will result in a list where every object is in order by its position in the tree.
This value is incredibly useful when persisting to the database. It allows for the use of very simple SQL queries to get nodes or segments of the tree.

Using the dataset above you can do the following.

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

First you would create the code that processes the nodes of the tree by implementing ITreeNodeProcessor<T>, which requires the 
implementation of one method called `bool ProcessNode(ITreeNode<T> node)`. 

For example:

    class MyTreeNodeProcessor : ITreeNodeProcessor<Person>
    {
        public IList<string> TestOutput;

        public MyTreeNodeProcessor()
        {
            TestOutput = new List<string>();
        }

        public bool ProcessNode(ITreeNode<Person> node)
        {
            TestOutput.Add(node.Id + " Processed");
            return true;
        }
    }

Then you just feed it to the top of the node you want to process. In the example below, we process every node in the tree because 
we start at the root node.

    var myProcessor = new MyTreeNodeProcessor();
    hierarchy.ProcessTree(myProcessor);

However, if you want to process only a branch, then you can pass the tree node processor to the root of that branch only. If you 
want to exclude the root node (or root of branch node) and process starting with its children, then you can use the 
`ProcessChildren(ITreeNode<T> node)` method instead. 

    var myProcessor = new MyTreeNodeProcessor();
    hierarchy.ProcessChildren(myProcessor);

As you can see, you can just write your business logic and forget about having to recurse. Your tree node processor could act on 
the node or not depending on the logic. If your entity is a View Model, you can set property values to drive user interface changes 
such as setting 'IsHighlighted = true' if the object matches a search string.  

## Documentation

Full documentation is provided in the KnightMoves.Hierarchical.Help project, which is a SandCastle Help file builder project. 
Therein you will find the HTML pages in MSDN style and also the HtmlHelp .chm file. Just download it and open it up. 

## Q & A

### I can utilize recursion with the SQL syntax of my database. Why not use that?

You can certainly use that to produce or process your tree. However, you will find that for every type of entity that must 
participate in a tree you will need to write that kind of recursive code again. With this library it is done once. Also, 
having a business logic layer implementation makes it database agnostic. You can port your data to another database easier 
because your hierarchical processing is done in the application code. The output of the SortableTreePath also makes reporting 
easier. Sometimes the people creating the reports are not developers. You can use a reporting tool like Tableau and easily 
create a tree with indented children simply by sorting on the SortableTreePath. This is implemented once for all of your 
hierarchical entities. Lastly, if you're like me and you prefer not to put business logic in the database, then this makes 
it easier to write code to process a tree recursively and keep the business logic in the application code.

### My entity already inherits from something. C# doesn't support multiple inheritance so what do I do?

Use the 'TreeNodeWrapper<T>' class. The Help Documentation fully explains it. 

