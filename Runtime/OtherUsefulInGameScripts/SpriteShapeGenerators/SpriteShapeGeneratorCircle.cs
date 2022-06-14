using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.U2D;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpriteShapeGeneratorCircle : MonoBehaviour {

    public float radius = 4;

    public int division = 8;

    [Range(0, 360)]
    public float angle = 360;
    
    public bool outerCircle = true;

    public float AngleToGenerate => Mathf.Clamp(angle, 0, 360);

    [Button()]
    public void Generate() {

        var controller = GetComponent<SpriteShapeController>();

#if UNITY_EDITOR
        Undo.RecordObject(controller, "SpriteShapeCircle");
#endif

        var spine = controller.spline;
        spine.Clear();
        var points = outerCircle ? GeneratePointsOuterCircle() : GeneratePointsInnerCircle();

        for (var i = 0; i < points.Count; ++i) {
            spine.InsertPointAt(i, points[i]);
        }

        // var jh = controller.BakeMesh();
        // jh.Complete();
#if UNITY_EDITOR

        if (PrefabUtility.IsPartOfAnyPrefab(gameObject)) {
            PrefabUtility.RecordPrefabInstancePropertyModifications(controller);
            // Debug.Log("TEst");
        }
        EditorUtility.SetDirty(controller);
#endif
        // controller.RefreshSpriteShape();
        // controller.UpdateSpriteShapeParameters();
        // var jh = controller.BakeMesh();
        // jh.Complete();
    }

    protected List<Vector3> GeneratePointsOuterCircle() {

        var result = new List<Vector3>();
        if (division <= 0) {
            return result;
        }

        var deltaAngle = AngleToGenerate / division;
        var initOffset = (360 - AngleToGenerate) / 2 + deltaAngle * 0.5f;
        // var startPoint = transform.position;
        for (var i = 0; i < division; ++i) {
            var angle = -i * deltaAngle - initOffset;
            var rot = Quaternion.AngleAxis(angle, Vector3.forward);
            var offset = rot * Vector3.up * radius;
            result.Add(offset);
        }

        return result;
    }

    protected List<Vector3> GeneratePointsInnerCircle() {

        var result = new List<Vector3>();
        if (division <= 0) {
            return result;
        }

        var deltaAngle = AngleToGenerate / division;
        var initOffset = (360 - AngleToGenerate) / 2 + deltaAngle * 0.5f;
        // var startPoint = transform.position;
        for (var i = 0; i < division; ++i) {
            var angle = i * deltaAngle + initOffset;
            var rot = Quaternion.AngleAxis(angle, Vector3.forward);
            var offset = rot * Vector3.up * radius;
            result.Add(offset);
        }

        var gap = result[^1] - result[0];
        
        result.Add(result[^1] + Vector3.up);
        result.Add(result[^1] + Vector3.right * (radius - Mathf.Abs(gap.x / 2) + 1));
        result.Add(result[^1] + Vector3.down * (radius + 1) * 2);
        result.Add(result[^1] + Vector3.left * (radius + 1) * 2);
        result.Add(result[^1] + Vector3.up * (radius + 1) * 2);
        result.Add(result[^1] + Vector3.right * (radius - Mathf.Abs(gap.x / 2) + 1));
        return result;
    }
}