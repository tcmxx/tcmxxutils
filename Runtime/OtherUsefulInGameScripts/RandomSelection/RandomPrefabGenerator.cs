using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class RandomPrefabGenerator : ScriptableObject {

    public abstract void Reset();

    public abstract GameObject NextRandom(object context = null);
    public abstract List<GameObject> NextRandomN(int number, object context = null);

    public RandomPrefabGenerator CreateInstance() {
        return ScriptableObject.Instantiate(this);
    }
}
