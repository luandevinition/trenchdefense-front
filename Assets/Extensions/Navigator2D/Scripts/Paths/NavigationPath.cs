using System.Collections.Generic;

public class NavigationPath
{
    public NavigationVertex Root
    {
        get { return _root; }
    }

    public List<NavigationMeshSegment> Paths
    {
        get { return _paths; }
    }

    private readonly List<NavigationMeshSegment> _paths;

    private NavigationVertex _root;

    public NavigationPath(NavigationVertex root,List<NavigationMeshSegment> paths)
    {
        _root = root;
        _paths = paths;
    }
}
