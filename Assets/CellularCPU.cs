using System;
using UnityEngine;

public class CellularCPU : MonoBehaviour
{
    [SerializeField] int height = 256;
    [SerializeField] int width = 256;
    Texture2D tex;
    Material mat;
    Camera cam;

    CellBuffer cellBuffer;
    Color[] colors;

    void Awake()
    {
        cellBuffer = new CellBuffer(width, height);
        Renderer rend = GetComponent<Renderer>();
        mat = rend.material;

        tex = new Texture2D(width, height)
        {
            filterMode = FilterMode.Point,
        };

        mat.mainTexture = tex;
        colors = new Color[width * height];
    }

    void Start()
    {
        cam = Camera.main;

        // Fill(25, CellType.Sand);
    }

    void Update()
    {
        cellBuffer.ClearBuffer();

        Render();
        Step();
        if (Input.GetMouseButton(0))
        {
            Vector2 pos = Input.mousePosition; // Mouse position
            Ray ray = cam.ScreenPointToRay(pos);

            Physics.Raycast(cam.transform.position, ray.direction, out RaycastHit hit, 10000.0f);
            if (hit.collider)
            {
                GetPixelLocation(hit.textureCoord, out int x, out int y);
                // Debug.Log($"X:{x}, Y:{y}");

                cellBuffer.Set(x, y, CellType.Sand);
            }
        }

        if (Input.GetMouseButton(1))
        {
            Vector2 pos = Input.mousePosition; // Mouse position
            Ray ray = cam.ScreenPointToRay(pos);

            Physics.Raycast(cam.transform.position, ray.direction, out RaycastHit hit, 10000.0f);
            if (hit.collider)
            {
                GetPixelLocation(hit.textureCoord, out int x, out int y);
                // Debug.Log($"X:{x}, Y:{y}");

                cellBuffer.Set(x, y, CellType.Empty);
            }
        }

        cellBuffer.SwapBuffers();
    }

    void Step()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                CellType cellType = cellBuffer.Get(x, y).Type;

                // Debug.Log(cellType);
                switch (cellType)
                {
                    case CellType.Empty:
                        break;

                    case CellType.Sand:
                        if (cellBuffer.Get(x, y - 1).Type == CellType.Empty)
                        {
                            cellBuffer.Set(x, y - 1, CellType.Sand);
                        }
                        else if (cellBuffer.Get(x - 1, y - 1).Type == CellType.Empty)
                        {
                            cellBuffer.Set(x - 1, y - 1, CellType.Sand);
                        }
                        else if (cellBuffer.Get(x + 1, y - 1).Type == CellType.Empty)
                        {
                            cellBuffer.Set(x + 1, y - 1, CellType.Sand);
                        }
                        else
                        {
                            cellBuffer.Set(x, y, CellType.Sand);
                        }

                        break;
                    case CellType.Invalid:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
            }
        }
    }

    void Render()
    {
        var buffer = cellBuffer.GetBuffer();
        for (int i = 0; i < colors.Length; i++)
        {
            colors[i] = buffer[i].Color();

            // Debug.Log(buffer[i].Color());
        }

        tex.SetPixels(colors);

        tex.Apply();
    }

    void Fill(float amountInPercentage, CellType cellType)
    {
        amountInPercentage = Mathf.Clamp(amountInPercentage, 0, 100);

        int fillAmount = (int)(width * height * amountInPercentage / 100);

        for (int i = 0; i < fillAmount; i++)
        {
            cellBuffer.Set(i, cellType);
        }

        cellBuffer.SwapBuffers();
    }

    void GetPixelLocation(Vector2 position, out int x, out int y)
    {
        x = (int)Mathf.Floor(position.x * width);
        y = (int)Mathf.Floor(position.y * height);
    }
}