using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[ExecuteInEditMode]
[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshMergeTool : MonoBehaviour
{
    public GameObject rootObject;
    public bool generate = false;
    public bool disableOrigins = false;


    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (generate)
        {
            CombineMeshes();
            generate = false;

        }
    }
    public void CombineMeshes()
    {
        CombineMesh(rootObject, disableOrigins);
    }
    public static void CombineMesh(GameObject rootObject, bool disableOriginalObjects = true)
    {
        Dictionary<Material, List<CombineInstance>> matMeshFilterDic = new Dictionary<Material, List<CombineInstance>>();
        List<Mesh> tempMeshed = new List<Mesh>();
        List<CombineInstance> noMatMeshes = new List<CombineInstance>();


        //Get or create root mesh renderer and filter
        var rootMeshFilter = rootObject.transform.GetComponent<MeshFilter>();
        if (rootMeshFilter == null)
            rootMeshFilter = rootObject.AddComponent<MeshFilter>();
        MeshRenderer rootRenderer = rootObject.GetComponent<MeshRenderer>();
        if (rootRenderer == null)
            rootRenderer = rootObject.AddComponent<MeshRenderer>();


        MeshFilter[] meshFilters = rootObject.GetComponentsInChildren<MeshFilter>();

        foreach (var mf in meshFilters)
        {
            if (mf.sharedMesh == null)
            {
                continue;
            }
            if (mf == rootMeshFilter)
            {
                Debug.LogWarning("MeshFilter on root object is ignored");
                continue;
            }
            //get the material of the mesh
            var renderer = mf.GetComponent<MeshRenderer>();
            Material sharedMat = null;
            if (renderer != null)
                sharedMat = renderer.sharedMaterial;

            //create combine instance
            CombineInstance combine = new CombineInstance();
            combine.subMeshIndex = 0;
            combine.mesh = mf.sharedMesh;
            combine.transform = mf.transform.localToWorldMatrix;

            if (sharedMat == null)
            {
                //no material for that mesh
                noMatMeshes.Add(combine);
            }
            else
            {
                if (!matMeshFilterDic.ContainsKey(sharedMat))
                {
                    matMeshFilterDic[sharedMat] = new List<CombineInstance>();
                }
                matMeshFilterDic[sharedMat].Add(combine);
            }

            if (disableOriginalObjects && mf.gameObject != rootObject)
                mf.gameObject.SetActive(false);
        }

        var keyList = matMeshFilterDic.Keys;
        List<Material> allMat = new List<Material>(keyList);
        //combine the meshes for the same material
        foreach (var k in keyList)
        {
            Mesh newMesh = new Mesh();
            newMesh.CombineMeshes(matMeshFilterDic[k].ToArray(), true);
            tempMeshed.Add(newMesh);
        }
        if (noMatMeshes.Count > 0)
        {
            var noMaterialMesh = new Mesh();
            noMaterialMesh.CombineMeshes(noMatMeshes.ToArray(), true);
            tempMeshed.Add(noMaterialMesh);
            allMat.Add(null);
        }
        //combine all meshes

        var finalCombine = tempMeshed.Select((m) =>
        {
            CombineInstance combine = new CombineInstance();
            combine.subMeshIndex = 0;
            combine.mesh = m;
            combine.transform = Matrix4x4.Translate(-rootObject.transform.position);
            return combine;
        }).ToList();

        Mesh finalMesh = new Mesh();
        finalMesh.CombineMeshes(finalCombine.ToArray(), false, true);
        finalMesh.subMeshCount = finalCombine.Count;

        //set the mesh to the mesh fileter
        rootMeshFilter.mesh = finalMesh;

        //set the material for the final mesh renderer
        rootRenderer.materials = allMat.ToArray();
    }





}
