using UnityEngine;
using UnityEngine.U2D;

public class CrowdDisplayer : MonoBehaviour
{
    struct CharacterData {
        public Vector3 randomOffset;
        public float initialProgress;
        public Vector4 uvRect;
    }

    public Crowd targetCrowd;
    
    public SpriteAtlas atlas;
    public Sprite[] characters;
    public Mesh characterMesh; 
    public Material crowdMaterialTemplate; 
    
    public int characterCount = 10000;
    
    public float baseMoveSpeed = 3f;
    public float acceleration = 5f;
    
    private ComputeBuffer crowdBuffer;
    private ComputeBuffer argsBuffer;
    private ComputeBuffer waypointBuffer; 
    private Vector4[] waypointPositions;  
    
    private float currentSpeed = 0f;
    private float lastTotalDistance = 0f;
    
    private float globalOffset = 0f;
    private MaterialPropertyBlock propertyBlock;
    
    private int currentWaypointCount = 0;

    void Start()
    {
        if (targetCrowd == null) {
            Debug.LogError("Le CrowdDisplayer a besoin d'une référence à un objet Crowd !");
            return;
        }
        InitializeCrowd();
    }

    void InitializeCrowd()
    {
        propertyBlock = new MaterialPropertyBlock();

        CharacterData[] data = new CharacterData[characterCount];
        for (int i = 0; i < characterCount; i++)
        {
            Sprite s = characters[Random.Range(0, characters.Length)];
            Rect r = s.textureRect;
            float texW = s.texture.width;
            float texH = s.texture.height;

            data[i] = new CharacterData {
                randomOffset = new Vector3(Random.Range(-1f, 1f), Random.Range(0f, 10f), 0),
                initialProgress = Random.value,
                uvRect = new Vector4(r.x / texW, r.y / texH, r.width / texW, r.height / texH)
            };
        }

        crowdBuffer = new ComputeBuffer(characterCount, 32);
        crowdBuffer.SetData(data);
        propertyBlock.SetBuffer("_CrowdBuffer", crowdBuffer);
        
        if (characters.Length > 0 && characters[0] != null) {
            propertyBlock.SetTexture("_MainTex", characters[0].texture);
        }

        int maxPossibleNodes = targetCrowd.allNodes.Length;
        waypointPositions = new Vector4[maxPossibleNodes];
        waypointBuffer = new ComputeBuffer(maxPossibleNodes, 16); 
        
        propertyBlock.SetBuffer("_WaypointBuffer", waypointBuffer);

        argsBuffer = new ComputeBuffer(1, 5 * sizeof(uint), ComputeBufferType.IndirectArguments);
        uint[] args = new uint[5] { characterMesh.GetIndexCount(0), (uint)characterCount, 0, 0, 0 };
        argsBuffer.SetData(args);
    }
    
    void Update()
    {
        if (targetCrowd?.rootNode == null) return;

        int currentCount = 0;
        float accumulatedDistance = 0f;
        CrowdNode current = targetCrowd.rootNode;
        Vector3 lastPos = current.position;

        while (current != null) {
            float dist = Vector3.Distance(lastPos, current.position);
            accumulatedDistance += dist;
            waypointPositions[currentCount] = new Vector4(current.position.x, current.position.y, current.position.z, accumulatedDistance);
            lastPos = current.position;
            currentCount++;
            current = current.nextNode;
        }

        if (lastTotalDistance > 0 && Mathf.Abs(accumulatedDistance - lastTotalDistance) > 0.01f) {
            globalOffset *= (accumulatedDistance / lastTotalDistance);
        }
        lastTotalDistance = accumulatedDistance;

        if (currentCount < 2) return;

        float targetSpeed = 0f;
        if (targetCrowd.rootNode.state == CrowdState.Flowing) {
            targetSpeed = baseMoveSpeed; 
        }

        currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        
        globalOffset += Time.deltaTime * currentSpeed;

        waypointBuffer.SetData(waypointPositions);
        propertyBlock.SetInt("_WaypointCount", currentCount);
        propertyBlock.SetFloat("_TotalPathLength", accumulatedDistance);
        propertyBlock.SetFloat("_GlobalOffset", globalOffset / Mathf.Max(0.001f, accumulatedDistance));

        if (characters.Length > 0) propertyBlock.SetTexture("_MainTex", characters[0].texture);

        Graphics.DrawMeshInstancedIndirect(characterMesh, 0, crowdMaterialTemplate, new Bounds(Vector3.zero, Vector3.one * 1000), argsBuffer, 0, propertyBlock);
    }

    void OnDisable() 
    {
        if (crowdBuffer != null) crowdBuffer.Release();
        if (argsBuffer != null) argsBuffer.Release();
        if (waypointBuffer != null) waypointBuffer.Release(); 
    }
}