using System;
using UnityEngine;

public static class Extensions
{
    private const double Epsilon = 1e-10;

    public static bool IsZero(this float d)
    {
        return Math.Abs(d) < Epsilon;
    }
    
    public static float Cross(this Vector2 v,Vector2 z)
    {
        return z.x * v.y - z.y * v.x;
    }
    
    public static float Multiply(this Vector2 v, Vector2 w)
    {
        return v.x * w.x + v.y * w.y;
    }

    public static Vector2 Multiply(this Vector2 v, float mult)
    {
        return new Vector2(v.x * mult, v.y * mult);
    }

    public static Vector2 Multiply(this float mult, Vector2 v)
    {
        return new Vector2(v.x * mult, v.y * mult);
    }
    
    /// <summary>
    /// Test whether two line segments intersect. If so, calculate the intersection point.
    /// <see cref="http://stackoverflow.com/a/14143738/292237"/>
    /// </summary>
    /// <param name="p">Vector to the start point of p.</param>
    /// <param name="p2">Vector to the end point of p.</param>
    /// <param name="q">Vector to the start point of q.</param>
    /// <param name="q2">Vector to the end point of q.</param>
    /// <param name="intersection">The point of intersection, if any.</param>
    /// <param name="considerOverlapAsIntersect">Do we consider overlapping lines as intersecting?
    /// </param>
    /// <returns>True if an intersection point was found.</returns>
    public static bool LineSegementsIntersect(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2,
        out Vector2 intersection, bool considerCollinearOverlapAsIntersect = false)
    {
        intersection = Vector2.negativeInfinity;

        var r = p2 - p;
        var s = q2 - q;
        var rxs = r.Cross(s);
        var qpxr = (q - p).Cross(r);

        // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
        if (rxs.IsZero() && qpxr.IsZero())
        {
            // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
            // then the two lines are overlapping,
            if (considerCollinearOverlapAsIntersect)
                if (0 <= (q - p).Multiply(r) && (q - p).Multiply(r) <= r.Multiply(r) 
                     || 0 <= (p - q).Multiply(s) && (p - q).Multiply(s) <= s.Multiply(s))
                    return true;

            // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
            // then the two lines are collinear but disjoint.
            // No need to implement this expression, as it follows from the expression above.
            return false;
        }

        // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
        if (rxs.IsZero() && !qpxr.IsZero())
            return false;

        // t = (q - p) x s / (r x s)
        var t = (q - p).Cross(s) / rxs;

        // u = (q - p) x r / (r x s)

        var u = (q - p).Cross(r) / rxs;

        // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
        // the two line segments meet at the point p + t r = q + u s.
        if (!rxs.IsZero() && (0 <= t && t <= 1) && (0 <= u && u <= 1))
        {
            // We can calculate the intersection point using either t or u.
            intersection = p + t * r;

            // An intersection was found.
            return true;
        }

        // 5. Otherwise, the two line segments are not parallel but do not intersect.
        return false;
    }
}