using UnityEngine;
using System.Collections.Generic;

public class BezierCurve : MonoBehaviour
{
    public List<Vector3> controlPoints = new List<Vector3>();

    void Reset(){
        controlPoints = new List<Vector3>(){
            new Vector3(0, 0, 0),
            new Vector3(1, 0, 0),
            new Vector3(2, 0, 0),
            new Vector3(3, 0, 0)
        };
    }

    public int SegmentCount => Mathf.CeilToInt((controlPoints.Count - 1) / 3f);


    public Vector3 GetPoint(float t){
        int seg = Mathf.FloorToInt(t * SegmentCount);
        seg = Mathf.Clamp(seg, 0, SegmentCount - 1);

        float localT = (t * SegmentCount) - seg;

        int count = controlPoints.Count;
        
        int baseIndex = seg * 3;

        int i0 = Mathf.Min(baseIndex + 0, count - 1);
        int i1 = Mathf.Min(baseIndex + 1, count - 1);
        int i2 = Mathf.Min(baseIndex + 2, count - 1);
        int i3 = Mathf.Min(baseIndex + 3, count - 1);
        Vector3 p0 = transform.TransformPoint(controlPoints[i0]);
        Vector3 p1 = transform.TransformPoint(controlPoints[i1]);
        Vector3 p2 = transform.TransformPoint(controlPoints[i2]);
        Vector3 p3 = transform.TransformPoint(controlPoints[i3]);

        return CubicBezier(p0, p1, p2, p3, localT);
    }

    public void AddPoint(Vector3 point){
        controlPoints.Add(point);
    }

    public static Vector3 CubicBezier(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t){
        float u = 1 - t;
        return
            u*u*u * p0 +
            3*u*u*t * p1 +
            3*u*t*t * p2 +
            t*t*t * p3;
    }

    void OnDrawGizmos(){
        Gizmos.color = Color.yellow;

        // draw the curve
        Vector3 prev = GetPoint(0f);
        for(int i = 1; i <= 50; i++){
            float t = i / 50f;
            Vector3 curr = GetPoint(t);
            Gizmos.DrawLine(prev, curr);
            prev = curr;
        }

        // draw spheres on the local coordinates of the points
        Gizmos.color = Color.red;
        foreach(var cp in controlPoints){
            Gizmos.DrawSphere(transform.TransformPoint(cp), 0.1f);
        }
    }
}
