using System;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.ComponentModel;


namespace KnightMoves.Hierarchical
{
    /// <summary>
    /// This class takes an ITreeNode&lt;&gt; object that is the root node of a tree and wraps each and every 
    /// ITreeNode&lt;&gt; object recursively down the tree with a TreeNodeViewModel&lt;&gt; object, thereby implementing 
    /// a ViewModel pattern for each node of the entire tree for use in an MVVM WPF application.
    /// </summary>
    /// <typeparam name="T">An ITreeNode&lt;T&gt; that represents the root node of a tree or branch of a tree.</typeparam>
    public class TreeNodeWpfViewModel<T> : TreeNode<TreeNodeWpfViewModel<T>>, INotifyPropertyChanged where T : ITreeNode<T>
    {
        // Fields
        private bool _isExpanded;
        private bool _isSelected;

        // Events
        public event PropertyChangedEventHandler PropertyChanged;

        // Methods
        public TreeNodeWpfViewModel(ITreeNode<T> model)
        {
            ModelEntity = model;

            foreach (ITreeNode<T> node in ModelEntity.Children)
            {
                Children.Add(new TreeNodeWpfViewModel<T>(node));
            }

            AddCommand = new RoutedUICommand("Add Node", "AddNode", typeof(Button));
        }

        public void AddNode(string parentId)
        {
            T model = (T)Assembly.GetAssembly(typeof(T)).CreateInstance(typeof(T).FullName);

            if (model == null)
                throw new TypeLoadException("Could not get assembly to create an object of type T: " + typeof(T).FullName);

            model.Id = Guid.NewGuid().ToString();
            ITreeNode<TreeNodeWpfViewModel<T>> child = new TreeNodeWpfViewModel<T>(model);
            ModelEntity.FindById(parentId).Children.Add(model);
            FindById(parentId).Children.Add(child);
        }

        private void NotifyPropertyChanged(string property)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(property));
        }

        // Properties
        public ICommand AddCommand { get; private set; }

        public override string Id
        {
            get
            {
                return ModelEntity.Id;
            }
            set
            {
                if (ModelEntity.Id != value)
                {
                    ModelEntity.Id = value;
                    NotifyPropertyChanged("Id");
                }
            }
        }

        public bool IsExpanded
        {
            get
            {
                return _isExpanded;
            }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        public bool IsSelected
        {
            get
            {
                return _isSelected;
            }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        public ITreeNode<T> ModelEntity { get; set; }
    }


}
