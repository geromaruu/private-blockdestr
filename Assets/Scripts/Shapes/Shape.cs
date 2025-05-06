using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.EventSystems;

public class Shape : MonoBehaviour, IPointerClickHandler, IPointerUpHandler, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerDownHandler
{
    public GameObject squareShapeImage;
    public Vector3 shapeSelectedScale;
    public Vector2 offset = new Vector2(0f, 700f);


    [SerializeField] private Grid _grid;

    [HideInInspector]
    public ShapeData CurrentShapeData;
    public int TotalSquareNumber;
    private void OnEnable()
    {
        GameEvents.MoveShapeToStartPosition += MoveShapeToStartPosition;
        GameEvents.SetShapeInactive += SetShapeInactive;
    }

    private void OnDisable()
    {
        GameEvents.MoveShapeToStartPosition -= MoveShapeToStartPosition;
        GameEvents.SetShapeInactive -= SetShapeInactive;
    }


    private List<GameObject> _currentShape = new List<GameObject>();
    private Vector3 _shapeStartScale;
    private RectTransform _transform;
    private bool _shapeDraggable = true;
    private Canvas _canvas;
    private Vector3 _startPosition;
    private bool _shapeActive = true;
    public void Awake()
    {
        _shapeStartScale = this.GetComponent<RectTransform>().localScale;
        _transform = this.GetComponent<RectTransform>();
        _canvas = GetComponentInParent<Canvas>();
        _shapeDraggable = true;
        _startPosition = transform.localPosition;
        _shapeActive = true;

    }

    void Start()
    {
        if (_grid == null)
            _grid = FindObjectOfType<Grid>();
    }

    public bool IsOnStartPosition()
    {
        return transform.localPosition == _startPosition;
    }

    public bool IsAnyOfShapeSquareActive()
    {
        foreach (var square in _currentShape)
        {
            if (square.gameObject.activeSelf)
                return true;
        }
        return false;
    }

    public void DeactivateShape()
    {
        if (_shapeActive)
        {
            foreach (var square in _currentShape)
            {
                square?.GetComponent<ShapeSquare>().DeactivateShape();
            }


            _shapeActive = false;
        }
    }

    private void SetShapeInactive()
    {
        if (IsOnStartPosition() == false && IsAnyOfShapeSquareActive())
        {
            foreach (var square in _currentShape)
            {
                square.gameObject.SetActive(false);
            }
        }
    }
    public void ActivateShape()
    {
        if (!_shapeActive)
        {
            foreach (var square in _currentShape)
            {
                square?.GetComponent<ShapeSquare>().ActivateShape();
            }
        }
        _shapeActive = true;
    }

    public void RequestNewShape(ShapeData shapeData)
    {
        transform.localPosition = _startPosition;
        CreateShape(shapeData);
    }


    public void CreateShape(ShapeData shapeData)
    {
        CurrentShapeData = shapeData;
        TotalSquareNumber = GetNumberOfSquares(shapeData);
        

        // Исправленный цикл: не создаёт лишний блок
        while (_currentShape.Count < TotalSquareNumber)
        {
            _currentShape.Add(Instantiate(squareShapeImage, transform));
        }

        foreach (var square in _currentShape)
        {
            square.transform.localPosition = Vector3.zero;
            square.SetActive(false);
        }

        RectTransform squareRect = squareShapeImage.GetComponent<RectTransform>();
        Vector2 moveDistance = new Vector2(
            squareRect.rect.width * squareRect.localScale.x,
            squareRect.rect.height * squareRect.localScale.y
        );

        int currentIndexInList = 0;

        Debug.Log($"Создание фигуры: {shapeData.name}, Squares = {GetNumberOfSquares(shapeData)}");

        // Установка позиций блоков
        for (int row = 0; row < shapeData.rows; row++)
        {
            for (int col = 0; col < shapeData.columns; col++)
            {
                if (shapeData.board[row].column[col])
                {
                    float posX = GetXPositionForShapeSquare(shapeData, col, moveDistance);
                    float posY = GetYPositionForShapeSquare(shapeData, row, moveDistance);

                    _currentShape[currentIndexInList].SetActive(true);
                    _currentShape[currentIndexInList].GetComponent<RectTransform>().localPosition = new Vector2(posX, posY);

                    // Лог позиций для отладки
                    // Debug.Log($"Block {currentIndexInList} -> X: {posX}, Y: {posY}");

                    currentIndexInList++;
                }
            }
        }
    }

    private float GetYPositionForShapeSquare(ShapeData shapeData, int row, Vector2 moveDistance)
    {
        int middleIndex = shapeData.rows / 2;
        return (row - middleIndex) * moveDistance.y;
    }

    private float GetXPositionForShapeSquare(ShapeData shapeData, int column, Vector2 moveDistance)
    {
        int middleIndex = shapeData.columns / 2;
        return (column - middleIndex) * moveDistance.x;
    }

    private int GetNumberOfSquares(ShapeData shapeData)
    {
        int number = 0;
        foreach (var rowData in shapeData.board)
        {
            foreach (bool active in rowData.column)
            {
                if (active) number++;
            }
        }
        return number;
    }
    public void OnPointerClick(PointerEventData eventData)
    {

    }
    public void OnPointerUp(PointerEventData eventData)
    {

    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = shapeSelectedScale;
        SoundManager.instance.PlaySound(SoundManager.instance.pickUpSound);
    }

    public void OnDrag(PointerEventData data)
    {
        _transform.anchorMin = new Vector2(0, 0);
        _transform.anchorMax = new Vector2(0, 0);
        _transform.pivot = new Vector2(0, 0);

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(_canvas.transform as RectTransform, data.position, Camera.main, out pos);
        _transform.localPosition = pos + offset;
    
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        this.GetComponent<RectTransform>().localScale = _shapeStartScale;
        GameEvents.CheckIfShapeCanBePlaced();
  


    }
    public void OnPointerDown(PointerEventData eventData)
    {

    }

    private void MoveShapeToStartPosition()
    {
        _transform.transform.localPosition = _startPosition;
    }
}
