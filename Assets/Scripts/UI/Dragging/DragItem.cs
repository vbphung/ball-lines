using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragItem<T> : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler where T : class
{
    Vector3 startPosition;
    Transform originalParent;
    IDragSource<T> source;

    Canvas parentCanvas;

    private void Awake()
    {
        parentCanvas = GetComponentInParent<Canvas>();
        source = GetComponentInParent<IDragSource<T>>();
    }

    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        startPosition = transform.position;
        originalParent = transform.parent;
        GetComponent<CanvasGroup>().blocksRaycasts = false;
        transform.SetParent(parentCanvas.transform, true);
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        transform.position = eventData.position;
    }

    void IEndDragHandler.OnEndDrag(PointerEventData eventData)
    {
        transform.position = startPosition;

        GetComponent<CanvasGroup>().blocksRaycasts = true;
        transform.SetParent(originalParent, true);

        IDragDestination<T> container;
        if (!EventSystem.current.IsPointerOverGameObject())
            container = parentCanvas.GetComponent<IDragDestination<T>>();
        else
            container = GetContainer(eventData);

        if (container != null)
            DropItemIntoContainer(container);
    }

    private IDragDestination<T> GetContainer(PointerEventData eventData)
    {
        if (eventData.pointerEnter)
        {
            var container = eventData.pointerEnter.GetComponentInParent<IDragDestination<T>>();
            return container;
        }
        return null;
    }

    private void DropItemIntoContainer(IDragDestination<T> destination)
    {
        if (object.ReferenceEquals(destination, source))
            return;

        AttemptSimpleTransfer(destination);
    }

    private void AttemptSimpleTransfer(IDragDestination<T> destination)
    {
        if (destination.Acceptable(source.GetIndex()))
        {
            destination.AddItems(source.GetItem());
            source.RemoveItems();
        }
    }
}