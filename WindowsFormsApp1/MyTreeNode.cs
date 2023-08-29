using System;
using System.Collections.Generic;
using System.Linq;

public class MyTreeNode
{
    public string Data { get; set; }
    public MyTreeNode Parent { get; private set; }
    public List<MyTreeNode> Children { get; } = new List<MyTreeNode>();

    public MyTreeNode(string data)
    {
        this.Data = data;
    }

    public void AddChild(MyTreeNode child)
    {
        child.Parent = this;
        Children.Add(child);
    }

    public string GetPath()
    {
        var path = new List<string>();
        var currentNode = this;
        while (currentNode != null)
        {
            path.Add(currentNode.Data);
            currentNode = currentNode.Parent;
        }
        path.Reverse();
        return string.Join("\\", path);
    }

    public List<string> GetParents()
    {
        var parents = new List<string>();
        var currentNode = this.Parent;
        while (currentNode != null)
        {
            parents.Add(currentNode.Data);
            currentNode = currentNode.Parent;
        }
        return parents;
    }

    public List<string> GetChildren()
    {
        return Children.Select(child => child.Data).ToList();
    }
}
