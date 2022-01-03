using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;




public static class CurveRelatedExtensions
{
    public static Vector4 ToVector4(this Vector3 vec, float z)
    {
        return new Vector4(vec.x, vec.y, vec.z, z);
    }


    public static Quaternion ToQuaternion(this Matrix4x4 m)
    {
        // Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
        Quaternion q = new Quaternion();
        q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
        q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
        q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
        q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
        q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
        q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
        q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
        return q;
    }
}

public class Curves
{


    // The CurvePoint object stores information about a point on a curve
    // after it has been tesselated: the vertex (V), the tangent (T), the
    // normal (N), and the binormal (B).  It is the responsiblility of
    // functions that create these objects to fill in all the data.
    public struct CurvePoint
    {
        public Vector3 V; // Vertex
        public Vector3 T; // Tangent  (unit)
        public Vector3 N; // Normal   (unit)
        public Vector3 B; // Binormal (unit)
        public float t; // time	 (necessary only for the adaptive tessellation extra)

        public Quaternion ToQuaternion()
        {
            Matrix4x4 rot = new Matrix4x4();
            rot.SetColumn(0, N);
            rot.SetColumn(1, B);
            rot.SetColumn(2, T);
            rot.SetColumn(3, new Vector4(0, 0, 0, 1));
            return rot.ToQuaternion();
        }
    };

    // Approximately equal to.  We don't want to use == because of
    // precision issues with floating point.
    public static bool Approx(Vector3 lhs, Vector3 rhs)
    {
        const float eps = 1e-8f;
        return (lhs - rhs).sqrMagnitude < eps;
    }



    // This is the core routine of the curve evaluation code. Unlike
    // evalBezier, this is only designed to work on 4 control points.
    // Furthermore, it requires you to specify an initial binormal
    // Binit, which is iteratively propagated throughout the curve as
    // the curvepoints are generated. Any other function that creates
    // cubic splines can use this function by a corresponding change
    // of basis.
    protected static List<CurvePoint> CoreBezier(Vector3 p0,
                     Vector3 p1,
                     Vector3 p2,
                     Vector3 p3,
                     Vector3 initNormal,
                     int steps)
    {

        List<CurvePoint> R = new List<CurvePoint>(steps + 1);

        // build the basis matrix and loop the given number of steps,
        // computing points on the spline

        Matrix4x4 B = new Matrix4x4(), pointsMat = new Matrix4x4(), M = new Matrix4x4(), dB = new Matrix4x4(), dM = new Matrix4x4();

        B.SetRow(0, new Vector4(1, -3, 3, -1));
        B.SetRow(1, new Vector4(0, 3, -6, 3));
        B.SetRow(2, new Vector4(0, 0, 3, -3));
        B.SetRow(3, new Vector4(0, 0, 0, 1));

        pointsMat.SetRow(0, new Vector4(p0.x, p1.x, p2.x, p3.x));
        pointsMat.SetRow(1, new Vector4(p0.y, p1.y, p2.y, p3.y));
        pointsMat.SetRow(2, new Vector4(p0.z, p1.z, p2.z, p3.z));
        pointsMat.SetRow(3, new Vector4(1, 1, 1, 1));

        dB.SetRow(0, new Vector4(-3, 6, -3, 0));
        dB.SetRow(1, new Vector4(3, -12, 9, 0));
        dB.SetRow(2, new Vector4(0, 6, -9, 0));
        dB.SetRow(3, new Vector4(0, 0, 3, 0));

        M = pointsMat * B;
        dM = pointsMat * dB;


        for (int i = 0; i <= steps; ++i)
        {
            CurvePoint curvePoint;

            curvePoint.t = (1.0f * i) / steps;
            Vector4 tVec = new Vector4(1, curvePoint.t, curvePoint.t * curvePoint.t, curvePoint.t * curvePoint.t * curvePoint.t);

            Vector4 point = M * tVec;
            var temp = point / point.w;
            curvePoint.V = new Vector3(temp.x, temp.y, temp.z);
            temp = dM * tVec;
            curvePoint.T = new Vector3(temp.x, temp.y, temp.z);
            curvePoint.T.Normalize();
            if (i == 0)
            {
                if (Approx(initNormal.normalized, curvePoint.T.normalized))
                {
                    curvePoint.B = Vector3.Cross(new Vector3(initNormal.x, initNormal.y, initNormal.z), curvePoint.T).normalized;
                }
                else
                {
                    curvePoint.B = Vector3.Cross(initNormal, curvePoint.T).normalized;
                }
            }
            else
            {
                curvePoint.B = Vector3.Cross(R[i - 1].N, curvePoint.T).normalized;
            }
            curvePoint.N = Vector3.Cross(curvePoint.T, curvePoint.B).normalized;

            R.Add(curvePoint);
        }
        return R;
    }



    // the P argument holds the control points and steps gives the amount of uniform tessellation.
    // the rest of the arguments are for the adaptive tessellation extra.
    public static List<CurvePoint> EvalBezier(List<Vector3> P, int steps, float3 initNormal)
    {
        // Check
        if (P.Count < 4 || P.Count % 3 != 1)
        {
            Debug.LogError("evalBezier must be called with 3n+1 control points.");
            return null;
        }

        // YOUR CODE HERE (R1):
        // You should implement this function so that it returns a Curve
        // (e.g., a vector<CurvePoint>).  The variable "steps" tells you
        // the number of points to generate on each piece of the spline.
        // At least, that's how the sample solution is implemented and how
        // the SWP files are written.  But you are free to interpret this
        // variable however you want, so long as you can control the
        // "resolution" of the discretized spline curve with it.

        // EXTRA CREDIT NOTE:
        // Also compute the other Vec3fs for each CurvePoint: T, N, B.
        // A matrix [N, B, T] should be unit and orthogonal.
        // Also note that you may assume that all Bezier curves that you
        // receive have G1 continuity. The T, N and B vectors will not
        // have to be defined at points where this does not hold.

        List<CurvePoint> result = new List<CurvePoint>();
        result.Capacity = (P.Count * steps);
        for (int i = 0; i < P.Count / 3; ++i)
        {
            List<CurvePoint> tmp;
            if (i == 0)
            {
                tmp = CoreBezier(P[i * 3], P[i * 3 + 1], P[i * 3 + 2], P[i * 3 + 3], initNormal, steps);
            }
            else
            {
                tmp = CoreBezier(P[i * 3], P[i * 3 + 1], P[i * 3 + 2], P[i * 3 + 3], result[result.Count - 1].N, steps);
            }
            if (i == 0)
            {
                result.AddRange(tmp);
            }
            else
            {
                tmp.RemoveAt(0);
                result.AddRange(tmp);
            }
        }

        return result;
    }


    public static List<CurvePoint> EvalBspline(List<Vector3> P, int steps, bool averageBNT, float3 initNormal)
    {
        // Check
        if (P.Count < 4)
        {
            Debug.LogError("evalBspline must be called with 4 or more control points.");
            return null;
        }

        // Change of basis from
        // B-spline to Bezier.
        Matrix4x4 Bbs = new Matrix4x4();
        Bbs.SetRow(0, new Vector4(1, -3, 3, -1) / 6);
        Bbs.SetRow(1, new Vector4(4, 0, -6, 3) / 6);
        Bbs.SetRow(2, new Vector4(1, 3, 3, -3) / 6);
        Bbs.SetRow(3, new Vector4(0, 0, 0, 1) / 6);

        Matrix4x4 Bbz = new Matrix4x4();
        Bbz.SetRow(0, new Vector4(1, -3, 3, -1));
        Bbz.SetRow(1, new Vector4(0, 3, -6, 3));
        Bbz.SetRow(2, new Vector4(0, 0, 3, -3));
        Bbz.SetRow(3, new Vector4(0, 0, 0, 1));

        Matrix4x4 transMat = Bbs * Bbz.inverse;

        List<CurvePoint> result = new List<CurvePoint>(P.Count * steps);

        for (int i = 0; i < P.Count - 3; ++i)
        {

            Matrix4x4 cps = new Matrix4x4(), transformedCps = new Matrix4x4();
            cps.SetColumn(0, P[i].ToVector4(1));
            cps.SetColumn(1, P[i + 1].ToVector4(1));
            cps.SetColumn(2, P[i + 2].ToVector4(1));
            cps.SetColumn(3, P[i + 3].ToVector4(1));
            transformedCps = cps * transMat;

            List<CurvePoint> tmp;
            if (i == 0)
            {
                tmp = CoreBezier(new Vector3(transformedCps.m00, transformedCps.m10, transformedCps.m20),
                    new Vector3(transformedCps.m01, transformedCps.m11, transformedCps.m21),
                    new Vector3(transformedCps.m02, transformedCps.m12, transformedCps.m22),
                    new Vector3(transformedCps.m03, transformedCps.m13, transformedCps.m23), initNormal, steps);
            }
            else
            {
                tmp = CoreBezier(new Vector3(transformedCps.m00, transformedCps.m10, transformedCps.m20),
                    new Vector3(transformedCps.m01, transformedCps.m11, transformedCps.m21),
                    new Vector3(transformedCps.m02, transformedCps.m12, transformedCps.m22),
                    new Vector3(transformedCps.m03, transformedCps.m13, transformedCps.m23), result[result.Count - 1].N, steps);
            }

            if (i == 0)
            {
                result.AddRange(tmp);
            }
            else
            {
                tmp.RemoveAt(0);
                result.AddRange(tmp);
            }
        }

        if (averageBNT)
        {
            result.RemoveAt(result.Count - 1);
            for (int n = 0; n < 10; ++n)
            {
                for (int i = 0; i < result.Count; ++i)
                {
                    Matrix4x4 beforeRot = new Matrix4x4(), afterRot = new Matrix4x4();
                    if (i == 0)
                    {
                        beforeRot.SetColumn(0, result[result.Count - 1].N);
                        beforeRot.SetColumn(1, result[result.Count - 1].B);
                        beforeRot.SetColumn(2, result[result.Count - 1].T);
                        beforeRot.SetColumn(3, new Vector4(0, 0, 0, 1));

                        afterRot.SetColumn(0, result[i + 1].N);
                        afterRot.SetColumn(1, result[i + 1].B);
                        afterRot.SetColumn(2, result[i + 1].T);
                        afterRot.SetColumn(3, new Vector4(0, 0, 0, 1));

                    }
                    else if (i == result.Count - 1)
                    {
                        beforeRot.SetColumn(0, result[i - 1].N);
                        beforeRot.SetColumn(1, result[i - 1].B);
                        beforeRot.SetColumn(2, result[i - 1].T);
                        beforeRot.SetColumn(3, new Vector4(0, 0, 0, 1));
                        afterRot.SetColumn(0, result[0].N);
                        afterRot.SetColumn(1, result[0].B);
                        afterRot.SetColumn(2, result[0].T);
                        afterRot.SetColumn(3, new Vector4(0, 0, 0, 1));
                    }
                    else
                    {
                        beforeRot.SetColumn(0, result[i - 1].N);
                        beforeRot.SetColumn(1, result[i - 1].B);
                        beforeRot.SetColumn(2, result[i - 1].T);
                        beforeRot.SetColumn(3, new Vector4(0, 0, 0, 1));
                        afterRot.SetColumn(0, result[i + 1].N);
                        afterRot.SetColumn(1, result[i + 1].B);
                        afterRot.SetColumn(2, result[i + 1].T);
                        afterRot.SetColumn(3, new Vector4(0, 0, 0, 1));
                    }
                    Quaternion beforeQ = beforeRot.ToQuaternion();
                    Quaternion afterQ = afterRot.ToQuaternion();
                    Quaternion avQ = Quaternion.Slerp(afterQ, beforeQ, 0.5f);
                    Matrix4x4 avMatrix = Matrix4x4.TRS(Vector3.zero, avQ, Vector3.one);
                    CurvePoint p = result[i];
                    p.N = avMatrix.GetColumn(0).normalized;
                    p.B = avMatrix.GetColumn(1).normalized;
                    p.T = avMatrix.GetColumn(2).normalized;
                    result[i] = p;
                }
            }
        }

        // Return an empty curve right now.
        return result;
    }

    public static List<CurvePoint> EvalCRspline(List<Vector3> P, int steps, float3 initNormal)
    {
        // Check
        if (P.Count < 4)
        {
            Debug.LogError("evalCRspline must be called with 4 or more control points.");
            return null;
        }
        Matrix4x4 Bbz = new Matrix4x4();
        Bbz.SetRow(0, new Vector4(1, -3, 3, -1));
        Bbz.SetRow(1, new Vector4(0, 3, -6, 3));
        Bbz.SetRow(2, new Vector4(0, 0, 3, -3));
        Bbz.SetRow(3, new Vector4(0, 0, 0, 1));


        Matrix4x4 CRz = new Matrix4x4();
        CRz.SetRow(0, new Vector4(0, -1, 2, -1) * 0.5f);
        CRz.SetRow(1, new Vector4(2, 0, -5, 3) * 0.5f);
        CRz.SetRow(2, new Vector4(0, 1, 4, -3) * 0.5f);
        CRz.SetRow(3, new Vector4(0, 0, -1, 1) * 0.5f);
        Matrix4x4 transMat = CRz * Bbz.inverse;

        List<CurvePoint> result = new List<CurvePoint>(P.Count * steps);
        for (int i = 0; i < P.Count - 3; ++i)
        {

            Matrix4x4 cps = new Matrix4x4(), transformedCps = new Matrix4x4();
            cps.SetColumn(0, P[i].ToVector4(1));
            cps.SetColumn(1, P[i + 1].ToVector4(1));
            cps.SetColumn(2, P[i + 2].ToVector4(1));
            cps.SetColumn(3, P[i + 3].ToVector4(1));
            transformedCps = cps * transMat;

            List<CurvePoint> tmp;
            if (i == 0)
            {
                tmp = CoreBezier(new Vector3(transformedCps.m00, transformedCps.m10, transformedCps.m20),
                    new Vector3(transformedCps.m01, transformedCps.m11, transformedCps.m21),
                    new Vector3(transformedCps.m02, transformedCps.m12, transformedCps.m22),
                    new Vector3(transformedCps.m03, transformedCps.m13, transformedCps.m23), initNormal, steps);
            }
            else
            {
                tmp = CoreBezier(new Vector3(transformedCps.m00, transformedCps.m10, transformedCps.m20),
                    new Vector3(transformedCps.m01, transformedCps.m11, transformedCps.m21),
                    new Vector3(transformedCps.m02, transformedCps.m12, transformedCps.m22),
                    new Vector3(transformedCps.m03, transformedCps.m13, transformedCps.m23), result[result.Count - 1].N, steps);
            }

            if (i == 0)
            {
                result.AddRange(tmp);
            }
            else
            {
                tmp.RemoveAt(0);
                result.AddRange(tmp);
            }
        }
        // Return an empty curve right now.
        return result;
    }




    public static Quaternion LerpOrientation(List<CurvePoint> curve, float t)
    {
        // Use De Casteljau with spherical interpolation (slerp) to interpolate between orientation control points, 
        // and convert interpolated quaternion to an orientation matrix
        if (curve.Count == 0)
            Debug.LogError("Lerped curve has zero point");
        if (curve.Count < 1)
        {
            return curve[0].ToQuaternion();
        }

        int totalSegment = curve.Count - 1;
        float segmentLength = 1.0f / totalSegment;
        int segmentNumFort = Mathf.Clamp((int)(t / segmentLength), 0, totalSegment - 1);
        float portionForThisSegment = t / segmentLength - segmentNumFort;
        return Quaternion.Slerp(curve[segmentNumFort].ToQuaternion(), curve[segmentNumFort + 1].ToQuaternion(), portionForThisSegment);
    }

    public static Vector3 LerpTranslation(List<CurvePoint> curve, float t)
    {
        if (curve.Count == 0)
        {
            Debug.LogError("Lerped curve has zero point");
            return Vector3.zero;
        }
        if (curve.Count < 1)
        {
            return curve[0].V;
        }
        t = Mathf.Clamp01(t);
        int totalSegment = curve.Count - 1;
        float segmentLength = 1.0f / totalSegment;
        int segmentNumFort = Mathf.Clamp((int)(t / segmentLength), 0, totalSegment - 1);
        float portionForThisSegment = t / segmentLength - segmentNumFort;
        return Vector3.Lerp(curve[segmentNumFort].V, curve[segmentNumFort + 1].V, portionForThisSegment);
    }
}








