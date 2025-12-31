using UnityEngine;
using UnityEngine.UI;

public class DropDrag : MonoBehaviour
{
    [SerializeField] private Image _imgFoodDrag;
    [SerializeField] private Canvas _canvas;

    private FoodSlot _currentFood, _cacheFood;
    private bool _isDragging;
    private Vector2 _lastMousePos;

    void Update()
    {
        // ================== MOUSE DOWN ==================
        if (Input.GetMouseButtonDown(0))
        {
            _currentFood = Utils.GetRaycastUI<FoodSlot>(Input.mousePosition);

            if (_currentFood != null && _currentFood.HasFood)
            {
                _isDragging = false;
                _cacheFood = _currentFood;
                _lastMousePos = Input.mousePosition;
            }
        }

        // ================== START DRAG KHI CHUỘT DI CHUYỂN ==================
        if (Input.GetMouseButton(0) && _currentFood != null)
        {
            if (!_isDragging && (Vector2)Input.mousePosition != _lastMousePos)
            {
                StartDrag();
            }

            _lastMousePos = Input.mousePosition;
        }

        // ================== DRAGGING ==================
        if (_isDragging)
        {
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _canvas.transform as RectTransform,
                Input.mousePosition,
                _canvas.worldCamera,   // KHÔNG thêm biến
                out Vector2 localMousePos
            );

            _imgFoodDrag.rectTransform.anchoredPosition = localMousePos;
        }

        // ================== MOUSE UP ==================
        if (Input.GetMouseButtonUp(0))
        {
            if (_isDragging)
            {
                _imgFoodDrag.gameObject.SetActive(false);

                FoodSlot dropSlot = Utils.GetRaycastUI<FoodSlot>(Input.mousePosition);

                bool dropped = false;

                if (dropSlot != null && dropSlot != _currentFood && !dropSlot.HasFood)
                {
                    dropSlot.OnSetSlot(_currentFood.GetSpriteFood);
                    _currentFood.Clear();
                    dropped = true;
                }

                if (!dropped)
                {
                    _currentFood.OnActiveFood(true);
                }
            }
            _isDragging = false;
            _currentFood = null;
            _cacheFood = null;
        }


    }

    void StartDrag()
    {
        _isDragging = true;

        _imgFoodDrag.gameObject.SetActive(true);
        _imgFoodDrag.sprite = _currentFood.GetSpriteFood;

        RectTransform canvasRect = _canvas.transform as RectTransform;
        RectTransform foodRect = _currentFood.transform as RectTransform;
        RectTransform dragRect = _imgFoodDrag.rectTransform;

        // Chuột trong Canvas
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvasRect,
            Input.mousePosition,
            _canvas.worldCamera,
            out Vector2 mousePos
        );

        // Tọa độ local trong item
        Vector2 localPointInItem;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            foodRect,
            Input.mousePosition,
            _canvas.worldCamera,
            out localPointInItem
        );

        // 👉 TÍNH PIVOT THEO ĐIỂM CLICK
        Vector2 size = foodRect.rect.size;
        Vector2 pivot = new Vector2(
            (localPointInItem.x / size.x) + foodRect.pivot.x,
            (localPointInItem.y / size.y) + foodRect.pivot.y
        );

        dragRect.pivot = pivot;

        // Đặt đúng vị trí chuột
        dragRect.anchoredPosition = mousePos;

        _currentFood.OnActiveFood(false);
    }

}
