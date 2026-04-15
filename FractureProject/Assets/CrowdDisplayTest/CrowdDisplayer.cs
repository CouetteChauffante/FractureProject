using UnityEngine;
using UnityEngine.U2D;

public class CrowdDisplayer : MonoBehaviour
{
    struct CharacterData {
        public Vector3 randomOffset;
        public float initialProgress;
        public Vector4 uvRect;
    }
    
    public Transform A;
    public Transform B;

    public SpriteAtlas atlas;
    public Sprite[] characters;
    public Mesh characterMesh; 
    public Material crowdMaterial;
    
    public int characterCount = 10000;
    public bool isMoving = false;
    public float moveSpeed = 0.1f;
    
    private ComputeBuffer crowdBuffer;
    private ComputeBuffer argsBuffer;
    private float globalOffset = 0f;

    void Start()
    {
        InitializeCrowd();
    }

    void InitializeCrowd()
    {
        CharacterData[] data = new CharacterData[characterCount];
        
        for (int i = 0; i < characterCount; i++)
        {
            Sprite s = characters[Random.Range(0, characters.Length)];
            
            Rect r = s.textureRect;
            float texW = s.texture.width;
            float texH = s.texture.height;

            data[i] = new CharacterData {
                randomOffset = new Vector3(
                    Random.Range(-1f, 1f),
                    Random.Range(0f, 10f),
                    0
                ),
                initialProgress = Random.value,
                uvRect = new Vector4(r.x / texW, r.y / texH, r.width / texW, r.height / texH)
            };
        }

        crowdBuffer = new ComputeBuffer(characterCount, 32);
        crowdBuffer.SetData(data);
        crowdMaterial.SetBuffer("_CrowdBuffer", crowdBuffer);

        crowdMaterial.SetTexture("_MainTex", characters[0].texture);

        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        uint[] args = new uint[5] { characterMesh.GetIndexCount(0), (uint)characterCount, 0, 0, 0 };
        argsBuffer.SetData(args);
    }

    void Update()
    {
        if (A != null && B != null)
        {
            crowdMaterial.SetVector("_PointA", A.position);
            crowdMaterial.SetVector("_PointB", B.position);
        }
        
        if (isMoving) {
            globalOffset += Time.deltaTime * moveSpeed;
            crowdMaterial.SetFloat("_GlobalOffset", globalOffset);
        }
        Graphics.DrawMeshInstancedIndirect(characterMesh, 0, crowdMaterial, new Bounds(Vector3.zero, Vector3.one * 1000), argsBuffer);
    }

    void OnDisable() {
        if (crowdBuffer != null) crowdBuffer.Release();
        if (argsBuffer != null) argsBuffer.Release();
    }
}