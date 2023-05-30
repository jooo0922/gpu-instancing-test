using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstanceSpawner : MonoBehaviour
{
    public GameObject prefab; // GPU 인스턴싱에 사용할 프리팹

    public int instanceCount = 100000; // GPU 인스턴스 개수

    [Header("Instance Position")]
    public float minX = -5000f;
    public float maxX = 5000f;
    public float minY = 3000f;
    public float maxY = 7000f;
    public float minZ = -4000f;
    public float maxZ = 4000f;

    [Header("Instance Scale")]
    public float minScale = 0.1f;
    public float maxScale = 2.5f;

    // GPU 인스턴싱을 위한 멤버변수 초기화 및 각 인스턴스 변환 계산
    private void Setup()
    {
        MaterialPropertyBlock props = new MaterialPropertyBlock();
        MeshRenderer renderer;

        for (int i = 0; i < instanceCount; i++)
        {
            Vector3 position = new Vector3(Random.Range(minX, maxX), Random.Range(minY, maxY), Random.Range(minZ, maxZ));
            Quaternion rotation = Quaternion.identity;
            Vector3 scale = Vector3.one;

            GameObject obj = Instantiate(prefab, position, rotation);

            float r = Random.Range(0.0f, 1.0f);
            float g = Random.Range(0.0f, 1.0f);
            float b = Random.Range(0.0f, 1.0f);
            props.SetColor("_Color", new Color(r, g, b));

            renderer = obj.GetComponent<MeshRenderer>();
            renderer.SetPropertyBlock(props);
        }
    }

    private void Start()
    {
        Setup();
    }

    private void Update()
    {

    }
}
