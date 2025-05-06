using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.LowLevel;

public class Grid : MonoBehaviour
{

    public ShapeStorage shapeStorage;
    public int columns = 0;
    public int rows = 0;
    public float squaresGap = 0;
    public GameObject gridSquare;
    public float squareScale = 1.0f;
    public float everySquareOffset = 0.0f;

    private Vector2 _offset = new Vector2(0.0f, 0.0f);
    private List<GameObject> _gridSquares = new List<GameObject>();

    private LineIndicator _lineIndicator;
    private bool playerLost = false;

    private void OnShapesReady()
    {
        CheckIfPlayerLost(); 
    }
    private void OnEnable()
    {
        GameEvents.CheckIfShapeCanBePlaced += CheckIfShapeCanBePlaced;
        GameEvents.RequestNewShapes += CheckIfPlayerLost;
        GameEvents.OnShapesReady += OnShapesReady;
    }

    private void OnDisable()
    {
        GameEvents.CheckIfShapeCanBePlaced -= CheckIfShapeCanBePlaced;
        GameEvents.RequestNewShapes -= CheckIfPlayerLost;
        GameEvents.OnShapesReady -= OnShapesReady;
    }

    void Start()
    {
        _lineIndicator = GetComponent<LineIndicator>();
        CreateGrid();
    }

    private void CreateGrid()
    {
        SpawnGridSquares();
        SetGridSquaresPositions();
    }

    private void SpawnGridSquares()
    {
        int square_index = 0;

        for (var row = 0; row < rows; ++row)
        {
            for (var column = 0; column < columns; ++column)
            {
                _gridSquares.Add(Instantiate(gridSquare) as GameObject);

                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SquareIndex = square_index;
                _gridSquares[_gridSquares.Count - 1].transform.SetParent(this.transform);
                _gridSquares[_gridSquares.Count - 1].transform.localScale = new Vector3(x: squareScale, y: squareScale, z: squareScale);
                _gridSquares[_gridSquares.Count - 1].GetComponent<GridSquare>().SetImage(_lineIndicator.GetGridSquareIndex(square_index) % 2 == 0);

                square_index++;
            }
        }

    }

    private void SetGridSquaresPositions()
    {
        if (_gridSquares.Count == 0) return;

        int columnNumber = 0;
        int rowNumber = 0;

        var squareRect = _gridSquares[0].GetComponent<RectTransform>();

        _offset.x = (squareRect.rect.width * squareScale) + everySquareOffset;
        _offset.y = (squareRect.rect.height * squareScale) + everySquareOffset;

        float gridWidth = columns * _offset.x;
        float gridHeight = rows * _offset.y;
        Vector2 startPosition = new Vector2(-gridWidth / 2 + _offset.x / 2, gridHeight / 2 - _offset.y / 2);

        // Debug.Log($"Grid Start Pos: {startPosition}, Offset: {_offset}");

        for (int i = 0; i < _gridSquares.Count; i++)
        {
            if (columnNumber >= columns)
            {
                columnNumber = 0;
                rowNumber++;
            }

            float posX = startPosition.x + (_offset.x * columnNumber);
            float posY = startPosition.y - (_offset.y * rowNumber);

            _gridSquares[i].GetComponent<RectTransform>().anchoredPosition = new Vector3(posX, posY, 0.0f);

            //   Debug.Log($"Square {i}: Position -> X: {posX}, Y: {posY}");

            columnNumber++;
        }

    }

    private void CheckIfShapeCanBePlaced()
    {
        var squareIndexes = new List<int>();

        foreach (var square in _gridSquares)
        {
            var gridSquare = square.GetComponent<GridSquare>();

            if (gridSquare.Selected && !gridSquare.SquareOccupied)
            {
                squareIndexes.Add(gridSquare.SquareIndex);
                gridSquare.Selected = false;
                //gridSquare.ActivateSquare();
            }
        }

        var CurrentSelectedShape = shapeStorage.GetCurrentSelectedShape();
        if (CurrentSelectedShape == null) return;

        if (CurrentSelectedShape.TotalSquareNumber == squareIndexes.Count)
        {

            foreach (var squareIndex in squareIndexes)
            {
                _gridSquares[squareIndex].GetComponent<GridSquare>().PlaceShapeOnBoard();
            }

            var shapeLeft = 0;

            foreach (var shape in shapeStorage.shapeList)
            {
                if (shape.IsOnStartPosition() && shape.IsAnyOfShapeSquareActive())
                {
                    shapeLeft++;
                }
            }

            CheckIfAnyLineIsCompleted();

            if (shapeLeft == 0)
            {
                GameEvents.RequestNewShapes();
            }
            else
            {
                GameEvents.SetShapeInactive();
            }

            CheckIfPlayerLost();

        }
        else
        {
            GameEvents.MoveShapeToStartPosition();
        }


    }

    void CheckIfAnyLineIsCompleted()
    {
        List<int[]> lines = new List<int[]>();




        //columns

        foreach (var column in _lineIndicator.coulmnIndexes)
        {
            lines.Add(_lineIndicator.GetVerticalLine(column));
        }

        //rows

        for (var row = 0; row < 9; row++)
        {
            List<int> data = new List<int>(9);
            for (var index = 0; index < 9; index++)
            {
                data.Add(_lineIndicator.line_data[row, index]);
            }

            lines.Add(data.ToArray());
        }


        var completedLines = CheckIfSquaresAreCompleted(lines);

        if (completedLines > 2)
        {
            //TODO: PLAY BONUS ANIMATIONS
        }

        var totalScores = 100 * completedLines;
        GameEvents.AddScores(totalScores);
        CheckIfPlayerLost();
    }

    private int CheckIfSquaresAreCompleted(List<int[]> data)
    {
        List<int[]> completedLines = new List<int[]>();
        var linesCompleted = 0;
        foreach (var line in data)
        {
            var lineCompleted = true;
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                if (comp.SquareOccupied == false)
                {
                    lineCompleted = false;
                }
            }

            if (lineCompleted)
            {
                completedLines.Add(line);
            }
        }

        foreach (var line in completedLines)
        {
            var completed = false;

            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.Deactivate();
                completed = true;

            }
            foreach (var squareIndex in line)
            {
                var comp = _gridSquares[squareIndex].GetComponent<GridSquare>();
                comp.ClearOccupied();
            }

            if (completed)
            {
                linesCompleted++;
            }
        }
        return linesCompleted;
    }
    public void CheckIfPlayerLost()
    {
        int validShapes = 0;

        for (int i = 0; i < shapeStorage.shapeList.Count; i++)
        {
            Shape currentShape = shapeStorage.shapeList[i];

            // Пропускаем, если фигура null или полностью использована
            if (currentShape == null || !currentShape.IsAnyOfShapeSquareActive())
                continue;

            // 🔍 Проверяем возможность размещения
            bool canBePlaced = CheckIfShapeCanBePlacedOnGrid(currentShape);
            Debug.Log($"Фигура {i} может быть размещена? {canBePlaced}");

            if (canBePlaced)
            {
                validShapes++;
            }
        }

        // ❌ Если нет доступных фигур для размещения — это Game Over
        if (validShapes == 0)
        {
            Debug.LogWarning("🎮 GAME OVER — ни одна фигура не может быть размещена!");
            playerLost = true;
            GameEvents.GameOver?.Invoke(true);
        }
        else
        {
            Debug.Log($"✅ Доступных фигур: {validShapes}. Игра продолжается.");
        }

    }

    private bool CheckIfShapeCanBePlacedOnGrid(Shape currentShape)
    {
        ShapeData data = currentShape.CurrentShapeData;
        int shapeRows = data.rows;
        int shapeCols = data.columns;

        for (int y = 0; y <= rows - shapeRows; y++)
        {
            for (int x = 0; x <= columns - shapeCols; x++)
            {
                bool canPlace = true;

                for (int row = 0; row < shapeRows; row++)
                {
                    var shapeRow = data.board[row];
                    if (shapeRow == null || shapeRow.column == null)
                    {
                        Debug.LogWarning($"ShapeData row {row} is null");
                        canPlace = false;
                        goto EndCheck;
                    }

                    for (int col = 0; col < shapeCols; col++)
                    {
                        if (col >= shapeRow.column.Length) continue;

                        if (shapeRow.column[col])
                        {
                            int gridX = x + col;
                            int gridY = y + row;
                            int index = gridY * columns + gridX;

                            if (index < 0 || index >= _gridSquares.Count)
                            {
                                canPlace = false;
                                goto EndCheck;
                            }

                            var square = _gridSquares[index].GetComponent<GridSquare>();
                            if (square == null || square.SquareOccupied)
                            {
                                canPlace = false;
                                goto EndCheck;
                            }
                        }
                    }
                }

            EndCheck:
                if (canPlace)
                {
                    return true;
                }
            }
        }

        return false;
    }



    private List<int[]> GetAllSquaresCombination(int columns, int rows)
    {
        var squareList = new List<int[]>();
        var lastColumnIndex = 0;
        var lastRowIndex = 0;

        int safeIndex = 0;

        while (lastRowIndex + (rows - 1) < 9)
        {
            var rowData = new List<int>();

            for (var row = lastRowIndex; row < lastRowIndex + rows; row++)
            {
                for (var column = lastColumnIndex; column < lastColumnIndex + columns; column++)
                {
                    rowData.Add(_lineIndicator.line_data[row, column]);
                }
            }

            squareList.Add(rowData.ToArray());

            lastColumnIndex++;

            if (lastColumnIndex + (columns - 1) >= 9)
            {
                lastRowIndex++;
                lastColumnIndex = 0;
            }

            safeIndex++;
            if (safeIndex > 100)
            {
                break;
            }
        }
        return squareList;
    }

}