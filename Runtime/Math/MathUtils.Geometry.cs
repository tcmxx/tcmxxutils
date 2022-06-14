using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace TCUtils {

    public static partial class MathUtils {

        /// <summary>
        /// Raycast on a triangle, return the hit if there is
        /// </summary>
        /// <param name="rayOrigin"></param>
        /// <param name="rayDirection">need to be normalized</param>
        /// <param name="p0"></param>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static RaycastHit? RayTriangleIntersect(float3 rayOrigin, float3 rayDirection, float3 p0, float3 p1, float3 p2) {

            var invA = CreateBarycentricTempMatrix(rayDirection, p0, p1, p2);
            return RayTriangleIntersect(invA, rayOrigin, rayDirection, p0, p1, p2);
        }

        public static RaycastHit? RayTriangleIntersect(float3x3 barycentricTempMatrix, float3 rayOrigin, float3 rayDirection, float3 p0, float3 p1, float3 p2) {

            var b = new float3(p0.x - rayOrigin.x, p0.y - rayOrigin.y, p0.z - rayOrigin.z);
            var x = math.mul(barycentricTempMatrix, b);

            if (x.z >= 0 && x.x >= 0 && x.y >= 0 && x.x + x.y <= 1) {
                return new RaycastHit() {
                    point = rayOrigin + x.z * rayDirection,
                    distance = x.z,
                    barycentricCoordinate = new Vector3(1 - x.x - x.y, x.x, x.y),
                    normal = math.normalizesafe(math.cross(p1 - p0, p2 - p0))
                };
            }

            return null;
        }

        // matrix used by RayTriangleIntersect(). we can cache this result of this to optimize the performance if needed.
        public static float3x3 CreateBarycentricTempMatrix(float3 rayDirection, float3 p0, float3 p1, float3 p2) {
            float3x3 a;
            a.c0 = new float3(p0.x - p1.x, p0.y - p1.y, p0.z - p1.z);
            a.c1 = new float3(p0.x - p2.x, p0.y - p2.y, p0.z - p2.z);
            a.c2 = new float3(rayDirection.x, rayDirection.y, rayDirection.z);
            return math.inverse(a);
        }

        public static (float3 min, float3 max) GetTriangleAABB(float3 p0, float3 p1, float3 p2) {
            var min = math.min(p0, p1);
            min = math.min(min, p2);

            var max = math.max(p0, p1);
            max = math.max(max, p2);
            return (min, max);
        }

        public static (float2 min, float2 max) GetTriangleAABB(float2 p0, float2 p1, float2 p2) {
            var min = math.min(p0, p1);
            min = math.min(min, p2);

            var max = math.max(p0, p1);
            max = math.max(max, p2);
            return (min, max);
        }
    }
}