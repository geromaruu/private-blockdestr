using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShapeStorage : MonoBehaviour
{
    public List<ShapeData> shapeData;
    public List<Shape> shapeList;

    public Grid grid;

    private void OnEnable()
    {
        GameEvents.RequestNewShapes += RequestNewShapes;
    }

    private void OnDisable()
    {
        GameEvents.RequestNewShapes -= RequestNewShapes;
    }

    private ShapeData GetRandomShapeData()
    {
        int randomIndex = UnityEngine.Random.Range(0, shapeData.Count);
        return shapeData[randomIndex];
    }

    public void RequestNewShapes()
    {

        Debug.Log("ShapeStorage.Start() — создаём стартовые фигуры");

        for (int i = 0; i < shapeList.Count; i++)
        {
            var data = GetRandomShapeData();
            shapeList[i].RequestNewShape(data);
        }

        GameEvents.OnShapesReady?.Invoke();
    }

    void Start()
    {
        foreach (var shape in shapeList)
        {
            var data = GetRandomShapeData();
            shape.CreateShape(data);
        }
    }

    public Shape GetCurrentSelectedShape()
    {
        foreach (var shape in shapeList)
        {
            if (!shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                return shape;
        }

        Debug.LogError("There is no place for shape");
        return null;
    }
}
