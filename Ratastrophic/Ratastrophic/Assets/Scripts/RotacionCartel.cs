
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RotacionCartel : MonoBehaviour
{
    [Header("Referencias")]
    [SerializeField] private Transform barrierPivot;
    [SerializeField] private GameObject cartelVisual;

    [Header("Animación")]
    [SerializeField] private float openAngle = 90f;
    [SerializeField] private float duration = 1f;

    private Quaternion closedRotation;
    private Coroutine animationCoroutine;

    private readonly HashSet<Collider> objectsInside =
        new HashSet<Collider>();

    private void Awake()
    {
        if (barrierPivot == null || cartelVisual == null)
        {
            Debug.LogError(
                "Asigna BarrierPivot y Cartel Visual en el Inspector.",
                this);
            enabled = false;
            return;
        }

        closedRotation = barrierPivot.localRotation;
    }

    private bool IsValidObject(Collider other)
    {
        // Solo detectar el collider físico del objeto padre.
        if (other.transform != other.transform.root &&
            other.transform.parent != null)
            return false;

        return other.CompareTag("NPC") ||
               other.CompareTag("MEOW");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!cartelVisual.activeInHierarchy)
            return;

        if (!IsValidObject(other))
            return;

        bool wasEmpty = objectsInside.Count == 0;
        objectsInside.Add(other);

        if (wasEmpty && objectsInside.Count > 0)
            Open();
    }

    private void OnTriggerExit(Collider other)
    {
        if (!cartelVisual.activeInHierarchy)
            return;

        if (!IsValidObject(other))
            return;

        objectsInside.Remove(other);

        if (objectsInside.Count == 0)
            Close();
    }

    private void Open()
    {
        Quaternion target = closedRotation *
            Quaternion.Euler(openAngle, 0f, 0f);

        AnimateTo(target);
    }

    private void Close()
    {
        AnimateTo(closedRotation);
    }

    private void AnimateTo(Quaternion target)
    {
        if (animationCoroutine != null)
            StopCoroutine(animationCoroutine);

        animationCoroutine = StartCoroutine(RotateBarrier(target));
    }

    private IEnumerator RotateBarrier(Quaternion target)
    {
        Quaternion start = barrierPivot.localRotation;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / duration);

            barrierPivot.localRotation =
                Quaternion.Slerp(start, target, t);

            yield return null;
        }

        barrierPivot.localRotation = target;
        animationCoroutine = null;
    }
}
