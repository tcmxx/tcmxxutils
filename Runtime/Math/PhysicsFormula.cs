using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TCUtils
{
    public static class PhysicsFormula
    {
        public static float CompleteInelasticCollision(float v1, float v2, float m1, float m2)
        {
            return (v1 * m1 + v2 * m2) / (m1 + m2);
        }

        public static (float v1After, float v2After) ElasticCollision(float v1, float v2, float m1, float m2)
        {
            var v11 = (v1 * (m1 - m2) + 2 * m2 * v2) / (m1 + m2);
            var v22 = (v2 * (m2 - m1) + 2 * m1 * v1) / (m1 + m2);

            return (v11, v22);
        }
    }
}