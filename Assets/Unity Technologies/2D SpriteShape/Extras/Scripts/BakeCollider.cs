using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;
using ExtrasClipperLib;

public enum ColliderCornerType
{
    Square,
    Round,
    Sharp
}

[ExecuteAlways]
public class BakeCollider : MonoBehaviour
{
    [SerializeField]
    ColliderCornerType m_ColliderCornerType = ColliderCornerType.Square;
    [SerializeField]
    float m_ColliderOffset = 0;


    const float s_ClipperScale = 100000.0f;
    int m_HashCode = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void SampleCurve(float colliderDetail, Vector3 startPoint, Vector3 startTangent, Vector3 endPoint, Vector3 endTangent, ref List<IntPoint> path)
    {
        
        if (startTangent.sqrMagnitude > 0f || endTangent.sqrMagnitude > 0f)
        {
            for (int j = 0; j <= colliderDetail; ++j)
            {
                float t = j / (float)colliderDetail;
                Vector3 newPoint = BezierUtility.BezierPoint(startPoint, startTangent + startPoint, endTangent + endPoint, endPoint, t) * s_ClipperScale;

                path.Add(new IntPoint((System.Int64)newPoint.x, (System.Int64)newPoint.y));
            }
        }
        else
        {
            Vector3 newPoint = startPoint * s_ClipperScale;
            path.Add(new IntPoint((System.Int64)newPoint.x, (System.Int64)newPoint.y));

            newPoint = endPoint * s_ClipperScale;
            path.Add(new IntPoint((System.Int64)newPoint.x, (System.Int64)newPoint.y));
        }
    }

    // Update is called once per frame
    void Update()
    {
        var sc = gameObject.GetComponent<SpriteShapeController>();
        
        if ( sc != null )
        {
            List<IntPoint> path = new List<IntPoint>();        
            int splinePointCount = sc.spline.GetPointCount();
            int pathPointCount = splinePointCount;
            int hashCode = sc.spline.GetHashCode() + m_ColliderCornerType.GetHashCode() + m_ColliderOffset.GetHashCode();
            if (m_HashCode == hashCode)
                return;

            m_HashCode = hashCode;

            if (sc.spline.isOpenEnded)
                pathPointCount--;

            for (int i = 0; i < pathPointCount; ++i)
            {
                int nextIndex = SplineUtility.NextIndex(i, splinePointCount);
                SampleCurve(sc.colliderDetail, sc.spline.GetPosition(i), sc.spline.GetRightTangent(i), sc.spline.GetPosition(nextIndex), sc.spline.GetLeftTangent(nextIndex), ref path);
            }

            if (m_ColliderOffset != 0f)
            {
                List<List<IntPoint>> solution = new List<List<IntPoint>>();
                ClipperOffset clipOffset = new ClipperOffset();

                EndType endType = EndType.etClosedPolygon;

                if (sc.spline.isOpenEnded)
                {
                    endType = EndType.etOpenSquare;

                    if (m_ColliderCornerType == ColliderCornerType.Round)
                        endType = EndType.etOpenRound;
                }

                clipOffset.ArcTolerance = 200f / sc.colliderDetail;
                clipOffset.AddPath(path, (ExtrasClipperLib.JoinType)m_ColliderCornerType, endType);
                clipOffset.Execute(ref solution, s_ClipperScale * m_ColliderOffset);

                if (solution.Count > 0)
                    path = solution[0];
            }

            List<Vector2> pathPoints = new List<Vector2>(path.Count);

            for (int i = 0; i < path.Count; ++i)
            {
                IntPoint ip = path[i];
                pathPoints.Add(new Vector2(ip.X / s_ClipperScale, ip.Y / s_ClipperScale));
            }

            var pc = GetComponent<PolygonCollider2D>();
            if (pc)
                pc.SetPath(0, pathPoints.ToArray());

            var ec = GetComponent<EdgeCollider2D>();
            if (ec)
            {
                if (m_ColliderOffset > 0f || m_ColliderOffset  < 0f && !sc.spline.isOpenEnded)
                    pathPoints.Add(pathPoints[0]);

                ec.points = pathPoints.ToArray();
            }
        }

    }
}
