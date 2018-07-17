using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UniRx;

[CreateAssetMenu]
public class NavigationMesh : ScriptableObject
{
    public Dictionary<int, List<NavigationMeshSegment>> Boundaries
    {
        get { return _boundaries; }
    }

    [SerializeField] private float _navigatorOffset = 0.1f;

    [HideInInspector][SerializeField]
    private readonly Dictionary<int, List<NavigationMeshSegment>> _boundaries = new Dictionary<int, List<NavigationMeshSegment>>();

    private NavigationGraph _navigationGraph;
    
    [ContextMenu ("Bake navigator mesh")]
    void BakeNavigatorMesh ()
    {
        _boundaries.Clear();
        
        // Find static collider
        var staticColliders = FindStaticCollider();

        // Calculate new edge collider boundary base on offset
        foreach (var collider in staticColliders)
        {
            var boundary = CalculateNewBoundary(collider);
            _boundaries.Add(collider.GetInstanceID(),boundary);
        }
        
        // Bake paths
        BakePaths();
    }

    EdgeCollider2D[] FindStaticCollider()
    {
        return GameObject.FindObjectsOfType<EdgeCollider2D>().Where(x => x.gameObject.isStatic).ToArray();
    }

    List<NavigationMeshSegment> CalculateNewBoundary(EdgeCollider2D collider)
    {
        var points = new List<Vector2>(collider.points);

        if (points.Count <= 1) return ConvertToSegments(points);
        
        points.Add(collider.points[0]);

        var newBoundaries = new List<Vector2>();
        for (int i = 1; i < collider.points.Length; i++)
        {
            var aPoint = points[i];
            var bPoint = points[i - 1];
            var cPoint = points[i + 1];

            var center = new Vector2((cPoint.x + bPoint.x) / 2f, (cPoint.y + bPoint.y) / 2f);

            var direction = (aPoint - center).normalized;

            var newAPoint = new Vector2(collider.transform.position.x, collider.transform.position.y) + aPoint +
                            direction * _navigatorOffset;
            newBoundaries.Add(newAPoint);
        }

        return ConvertToSegments(newBoundaries);
    }
    
    List<NavigationMeshSegment> ConvertToSegments(List<Vector2> points)
    {
        var closedPath = new List<Vector2>(points);
        closedPath.Add(points[0]);
        var segments = new List<NavigationMeshSegment>();
        for (int i = 0; i < points.Count; i++)
        {
            if (i == 0)
            {
                segments.Add(new NavigationMeshSegment(closedPath[i], closedPath[i + 1]));
            }
            else
            {
                var startVertex = segments[i - 1].End;
                var endVertex = new NavigationVertex(closedPath[i + 1]);
                segments.Add(new NavigationMeshSegment(startVertex, endVertex));
            }
        }

        return segments;
    }

    void BakePaths()
    {
        var paths = new List<NavigationPath>();
        foreach (var startBoundary in _boundaries)
        {
            var connectBoundaries = _boundaries.Where(x => x.Key != startBoundary.Key).ToList();

            connectBoundaries.ForEach(connectBoundary =>
            {
                startBoundary.Value.ForEach(startSegment =>
                {
                    var segments = new List<NavigationMeshSegment>();
                    connectBoundary.Value.ForEach(connectSegment =>
                    {
                        var connectedSegment = new NavigationMeshSegment(startSegment.Start,connectSegment.Start);

                        var intersection = Vector2.negativeInfinity;
                        if (!IsIntersectAnySegment(connectedSegment, out intersection))
                        {
                            segments.Add(connectedSegment);
                        }
                    });
                    
                    var path = new NavigationPath(startSegment.Start,segments);
                    paths.Add(path);
                });
            });
        }
        
        _navigationGraph = new NavigationGraph(paths);
    }

    bool IsIntersectAnySegment(NavigationMeshSegment segment,out Vector2 intersection)
    {
        var segments = _boundaries.SelectMany(x => x.Value).Where(x =>
            x.Start.InstanceId != segment.Start.InstanceId && x.End.InstanceId != segment.Start.InstanceId
            && x.Start.InstanceId != segment.End.InstanceId && x.End.InstanceId != segment.End.InstanceId).ToList();
        
        var _intersection = Vector2.negativeInfinity;
        var isIntersection = segment.IsIntersectWithAnySegment(segments,out _intersection);

        intersection = _intersection;

        return isIntersection;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        
        foreach (var keyValuePair in _boundaries)
        {
            var boundary = keyValuePair.Value;
            for (int i = 0; i < boundary.Count; i++)
            {
                Gizmos.DrawLine(boundary[i].Start.Point, boundary[i].End.Point);
            }
        }

        if(_navigationGraph == null) return;
        
        _navigationGraph.Paths.SelectMany(path => path.Paths).ToList().ForEach(path =>
        {
            Gizmos.DrawLine(path.Start.Point, path.End.Point);
        });
    }
}
