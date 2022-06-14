using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Forms;
using DG.Tweening;
using Unity.Mathematics;
using UnityEngine;

// [Serializable]
// public class Camera2DBackgroundSet {
//     public float2 originalSize;
//     public float2 scaling;
//     public List<Sprite> backgrounds;
// }

public class Camera2DMovingBackground : MonoBehaviour {
    public Vector2 referencePosition;

    [Range(-1, 1)]
    [SerializeField]
    private float moveFactor;

    public Transform backgroundRef;
    [SerializeField]
    protected List<BackgroundGroup> backgroundGroups;

    private int currentGroup = 0;

    private void Awake() {
        GameController.Instance.initCharacterSpawned += OnInitCharacterSpawned;
    }

    private void Start() {

        GameController.Instance.chapterChanged += InstanceOnChapterChanged;
    }

    private void InstanceOnChapterChanged(int oldChapter, int newChapter) {
        SwitchGroup(newChapter);
    }

    private void OnInitCharacterSpawned() {
        SetGroupImmediately(GameController.Instance.CurrentChapterIndex);
    }

    // Update is called once per frame
    void LateUpdate() {
        var offset = (Vector2)transform.position - referencePosition;
        var z = backgroundRef.position.z;
        var moved = offset * moveFactor;
        var newPos = referencePosition + moved;
        backgroundRef.position = new Vector3(newPos.x, newPos.y, z);
    }

    private void OnDestroy() {
        if (GameController.Instance) {
            GameController.Instance.initCharacterSpawned -= OnInitCharacterSpawned;
            GameController.Instance.chapterChanged -= InstanceOnChapterChanged;
        }
    }

    public void SwitchGroup(int groupIndex) {
        if (groupIndex == currentGroup) {
            return;
        }

        foreach (var rend in backgroundGroups[currentGroup].backgrounds) {
            rend.DOFade(0, 1).onComplete += () => rend.gameObject.SetActive(false);
        }

        foreach (var rend in backgroundGroups[groupIndex].backgrounds) {
            rend.gameObject.SetActive(true);
            rend.DOFade(1, 1);
        }

        currentGroup = groupIndex;
    }

    public void SetGroupImmediately(int groupIndex) {
        foreach (var rend in backgroundGroups[currentGroup].backgrounds) {
            var color = rend.color;
            color.a = 0;
            rend.color = color;
            rend.gameObject.SetActive(false);
        }

        foreach (var rend in backgroundGroups[groupIndex].backgrounds) {
            var color = rend.color;
            color.a = 1;
            rend.color = color;
            rend.gameObject.SetActive(true);
        }

        currentGroup = groupIndex;
    }
    //
    // public Vector2 CalculateOffsets(Camera2DBackgroundSet backGroundSet) {
    //     var currentPos = (float2)(Vector2)transform.localPosition;
    //     var offset = currentPos - referencePosition;
    //     offset = offset * moveFactor;
    //     var backgroundSize = backGroundSet.originalSize * backGroundSet.scaling;
    //
    //     var remain = math.fmod(offset, backgroundSize);
    //     var cell = (int2)(offset / backgroundSize);
    // }
    //
    // public Sprite GetNewSpriteForCell(int2 cell) {
    //     
    // }

    [Serializable]
    protected struct BackgroundGroup {
        public List<SpriteRenderer> backgrounds;
    }
}