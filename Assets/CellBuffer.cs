using System;
using UnityEngine;

[Serializable]
public class CellBuffer
{
    [SerializeField] Cell[] activeBuffer;
    [SerializeField] Cell[] passiveBuffer;

    readonly int width;
    readonly int height;
    readonly int size;

    public CellBuffer(int width, int height)
    {
        this.width = width;
        this.height = height;

        activeBuffer = new Cell[this.width * this.height];
        passiveBuffer = new Cell[this.width * this.height];
        size = activeBuffer.Length;
        for (int i = 0; i < size; i++)
        {
            activeBuffer[i] = new Cell(CellType.Empty);
            passiveBuffer[i] = new Cell(CellType.Empty);
        }

        // Array.Fill(activeBuffer, new Cell(CellType.Empty));
        // Array.Fill(passiveBuffer, new Cell(CellType.Sand));
    }

    public void SwapBuffers()
    {
        (activeBuffer, passiveBuffer) = (passiveBuffer, activeBuffer);
    }

    public Cell[] GetBuffer()
    {
        return passiveBuffer;
    }

    public void ClearBuffer()
    {
        for (int i = 0; i < activeBuffer.Length; i++)
        {
            activeBuffer[i].Type = CellType.Empty;
        }
    }

    public Cell Get(int x, int y)
    {
        if (CoordIsValid(x, y))
        {
            return passiveBuffer[width * y + x];
        }

        // It might be a good idea to return a cell with Invalid type.
        return new Cell(CellType.Invalid);
    }

    public void Set(int i, CellType cellType)
    {
        if (CoordIsValid(i))
        {
            activeBuffer[i].Type = cellType;
        }
    }

    public void Set(int x, int y, CellType cellType)
    {
        if (!CoordIsValid(x, y))
            return;

        activeBuffer[width * y + x].Type = cellType;
    }

    bool CoordIsValid(int x, int y)
    {
        return x >= 0 && x < width && y >= 0 && y < height;
    }

    bool CoordIsValid(int i)
    {
        return i >= 0 && i < size;
    }
}