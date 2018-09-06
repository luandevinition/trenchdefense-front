using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

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
            LineSegmentsIntersection(Start.Point, End.Point, x.Start.Point, x.End.Point, out _intersection));

        intersection = _intersection;

        return isIntersection;
    }
    
    
    public bool LineSegmentsIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector3 p4,
        out Vector2 intersection)
    {
        intersection = Vector2.zero;

        var d = (p2.x - p1.x) * (p4.y - p3.y) - (p2.y - p1.y) * (p4.x - p3.x);

        if (d == 0.0f)
        {
            return false;
        }

        var u = ((p3.x - p1.x) * (p4.y - p3.y) - (p3.y - p1.y) * (p4.x - p3.x)) / d;
        var v = ((p3.x - p1.x) * (p2.y - p1.y) - (p3.y - p1.y) * (p2.x - p1.x)) / d;

        if (u < 0.0f || u > 1.0f || v < 0.0f || v > 1.0f)
        {
            return false;
        }

        intersection.x = p1.x + u * (p2.x - p1.x);
        intersection.y = p1.y + u * (p2.y - p1.y);

        return true;
    }
}
