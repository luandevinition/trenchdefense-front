using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationGraph
{
    public List<NavigationPath> Paths
    {
        get { return _paths; }
    }

    private readonly List<NavigationPath> _paths = new List<NavigationPath>();

    public NavigationGraph(List<NavigationPath> paths)
    {
        _paths = paths;
    }

    public string ToString()
    {
        return string.Format("Number of navigation paths = {0}", _paths.Count);
    }
}
