using System;
using System.Collections;
using System.Collections.Generic;
using TCUtils;
using UnityEngine;
using UnityEngine.Internal;
using UnityEngine.PlayerLoop;
using Object = UnityEngine.Object;

[Serializable]
public class WeightPrefabPair {
    public float weight;
    public GameObject prefab;
    [Range(0, 1)]
    public float repeatPreventFactor = 0.5f;
    [Range(0, 1)]
    public float repeatPreventLasting = 0.5f;
}

[CreateAssetMenu(menuName = "TCUtils/RandomPrefabGeneration/SimpleRandomSelector")]
public class RandomPrefabsContainer : RandomPrefabGenerator {
    [SerializeField]
    private List<WeightPrefabPair> prefabsPool = new List<WeightPrefabPair>();
    private List<float> actualWeights = new List<float>();

    private void UpdateWeights() {
        for (var i = 0; i < prefabsPool.Count; ++i) {
            var val = prefabsPool[i];
            var recover = Mathf.Clamp01(1 - val.repeatPreventLasting);
            actualWeights[i] = Mathf.Lerp(actualWeights[i], val.weight, recover);
        }
    }

    private void InitializeWeightIfNot() {
        if (actualWeights == null || actualWeights.Count != prefabsPool.Count) {
            InitializeWeights();
        }
    }

    private void InitializeWeights() {
        actualWeights = new List<float>();
        for (var i = 0; i < prefabsPool.Count; ++i) {
            actualWeights.Add(prefabsPool[i].weight);
        }
    }

    public override void Reset() {
        InitializeWeights();
    }

    public override GameObject NextRandom(object context = null) {
        InitializeWeightIfNot();

        var weights = actualWeights.ToArray();

        for (var i = 0; i < actualWeights.Count; ++i) {
            var prefab = prefabsPool[i].prefab;
            if (prefab == null) {
                continue;
            }
            var option = prefab.GetComponent<IOptionalPrefab>();
            if (option != null && !option.CanSelect()) {
                weights[i] = 0;
            }
        }

        var index = MathUtils.IndexByChance(weights);
        var result = prefabsPool[index];

        UpdateWeights();

        actualWeights[index] = Mathf.Lerp(actualWeights[index], 0, Mathf.Clamp01(result.repeatPreventFactor));

        return result.prefab;
    }

    private void OnValidate() {
        for (var i = 0; i < prefabsPool.Count; ++i) {
            var val = prefabsPool[i];
            if (val.weight <= 0) {
                val.weight = 1;
            }

            prefabsPool[i] = val;
        }
    }

    public override List<GameObject> NextRandomN(int number, object context = null) {
        var result = new List<GameObject>(number);
        for (var i = 0; i < number; ++i) {
            result.Add(NextRandom());
        }

        return result;
    }
}