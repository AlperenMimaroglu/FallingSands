using System;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Serialization;

public class CellularAutomata : MonoBehaviour
{
    [FormerlySerializedAs("cell")] [SerializeField]
    ComputeShader cellShader;

    [SerializeField] int width;
    [SerializeField] int height;

    RenderTexture _renderTexture;
    Material _material;
    int _kernel;
    static readonly int Result = Shader.PropertyToID("result");

    uint _groupX;
    uint _groupY;
    static readonly int MainTex = Shader.PropertyToID("_UnlitColorMap");
    ComputeBuffer _cellBuffer;
    static readonly int Cells = Shader.PropertyToID("cells");

    struct Cell
    {
        public uint type;
        public Vector2 Position;
    }

    void Awake()
    {
        _renderTexture = new RenderTexture(width, height, 0)
        {
            enableRandomWrite = true
        };

        _renderTexture.Create();
        _kernel = cellShader.FindKernel("Step");
    }

    void Start()
    {
        var rend = GetComponent<Renderer>();
        _material = rend.material;
        _material.SetTexture(MainTex, _renderTexture);

        cellShader.GetKernelThreadGroupSizes(_kernel, out _groupX, out _groupY, out _);
        cellShader.SetTexture(_kernel, Result, _renderTexture);

        _cellBuffer = new ComputeBuffer(width * height, Marshal.SizeOf(typeof(int)), ComputeBufferType.Default,
            ComputeBufferMode.Dynamic);

        var cells = new int[width * height];
        for (int i = 0; i < cells.Length; i++)
        {
            cells[i] = 1;
            if (i % 8 == 0)
            {
                cells[i] = 0;
            }
        }

        _cellBuffer.SetData(cells);
        var paintKernel = cellShader.FindKernel("Paint");
        cellShader.SetBuffer(paintKernel, Cells, _cellBuffer);
        cellShader.SetTexture(paintKernel, Result, _renderTexture);

        cellShader.Dispatch(paintKernel, (int)(width / _groupX), (int)(height / _groupY), 1);
    }

    void Update()
    {
        // TODO: Get the converted mouse position from click
        // Spawn sand on left click
        // Remove on right click
    }

    void OnDestroy()
    {
        _cellBuffer.Release();
    }
}