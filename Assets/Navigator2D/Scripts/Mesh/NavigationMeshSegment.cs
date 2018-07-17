using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NavigationMeshSegment
{
    public NavigationVertex Start, End;

    public float Distance
    {
        get { return Vector2.Distance(Start.Point, End.Point); }
    }
    
    public NavigationMeshSegment(NavigationVertex start, NavigationVertex end)
    {
        Start = start;
        End = end;
    }

    public NavigationMeshSegment(Vector2 start, Vector2 end)
    {
        Start = new NavigationVertex(start);
        End = new NavigationVertex(end);
    }

    public bool IsIntersectWithAnySegment(List<NavigationMeshSegment> segments, out Vector2 intersection)
    {
        var _intersection = Vector2.negativeInfinity;

        var isIntersection = segments.Any(x =>
            Extensions.LineSegmentsIntersection(Start.Point, End.Point, x.Start.Point, x.End.Point, out _intersection));

        intersection = _intersection;

        return isIntersection;
    }
}
