using System.Collections.Generic;
using Godot;

namespace Support.Extensions;

public static class NodeExtensions
{
    public static T? FindChildOfType<T>(this Node node, bool recursive = true) where T : Node
    {
        var q = new Queue<Node>();
        q.Enqueue(node);
        while (q.TryDequeue(out var currentNode))
        {
            for (int i = 0, count = currentNode.GetChildCount(); i < count; i++)
            {
                Node child = currentNode.GetChild(i);
                if (recursive && child.GetChildCount() > 0) { q.Enqueue(child); }
                if (child is T childOfType)
                {
                    return childOfType;
                }
            }
        }
        return null;
    }
    public static T? FindParentOfType<T>(this Node node) where T : Node
    {
        var root = node.GetTree().Root;
        var currentNode = node;
        while (currentNode != root)
        {
            currentNode = node.GetParent();
            if (currentNode is T parentOfType)
            {
                return parentOfType;
            }
        }
        return null;
    }
    public static T? SearchNodeOfType<T>(this Node node) where T : Node
    {
        return FindChildOfType<T>(node.GetTree().Root);
    }
    public static SignalAwaiter TimerAwaiter(this Node node, double seconds)
    {
        return node.ToSignal(node.GetTree().CreateTimer(seconds), Timer.SignalName.Timeout);
    }
    public static SignalAwaiter NextFrameAwaiter(this Node node)
    {
        return node.ToSignal(node.GetTree(), SceneTree.SignalName.ProcessFrame);
    }
}