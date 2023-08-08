using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
    public static readonly Color BACKGROUND_COLOR = new(0.6f, 0.6f, 0.6f);
    public static readonly float COLOR_FADE_DURATION = 0.6f;

    private UnityEvent fadedOutFinishedCanDestroy;

    [Tooltip("Name of the entity. Used as an identifier.")]
    public string entityName;
    [Tooltip("Whether the entity is in the background.")]
    public bool fadedOut;
    [Tooltip("The image component of this entity.")]
    public Image entityImage;
    //public SpriteRenderer entitySprite;
    [Tooltip("The image component of this entity.")]
    public EntityPositionEnum curEntityPos = EntityPositionEnum.Middle;
    [Tooltip("Whether the entity should use the attached animator and play animations from it.")]
    public bool animated;
    [Tooltip("Animation state the entity is in. Corresponds to animation name in attached animator.")]
    public string animatedState;
    private Animator _animator;

    public void Awake()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        _animator.enabled = false;
    }

    public void SwitchTexture(Sprite texture, bool animated, string animatedState)
    {
        // StopPlayback() doesn't seem to work? Default state still overrides texture
        // to first sprite of the default animation on the prefab
        _animator.StopPlayback();
        // fuck you, using nuclear option
        _animator.enabled = false;

        entityImage.sprite = texture;
        //entitySprite.sprite = texture;

        this.animated = animated;
        this.animatedState = animatedState;
        if (animated)
        {
            _animator.enabled = true;
            _animator.Play(animatedState);
        }
    }

    public void Teleport(EntityPositionEnum relativeMovePos)
    {
        if (relativeMovePos == curEntityPos)
            return;

        // TODO: move in y units too
        int moveX = GetRelativeXUnitsToMove(relativeMovePos);
        //int moveY = GetRelativeYUnitsMove(relativeMovePos);

        gameObject.transform.SetLocalPositionAndRotation(new(
            gameObject.transform.localPosition.x + moveX,
            gameObject.transform.localPosition.y
        ), Quaternion.identity);

        curEntityPos = relativeMovePos;
    }

    public void Move(EntityPositionEnum relativeMovePos, float duration)
    {
        if (relativeMovePos == curEntityPos)
            return;

        // TODO: move in y units too
        int moveX = GetRelativeXUnitsToMove(relativeMovePos);
        //int moveY = GetRelativeYUnitsMove(relativeMovePos);
        StartCoroutine(StartMove(
                duration,
                gameObject.transform.localPosition,
                new(gameObject.transform.localPosition.x + moveX, gameObject.transform.localPosition.y)
            )
        );

        curEntityPos = relativeMovePos;
    }

    private int GetRelativeXUnitsToMove(EntityPositionEnum goal)
    {
        switch (curEntityPos)
        {
            case EntityPositionEnum.Middle:
                if (goal == EntityPositionEnum.Left) return (int)EntityPositionEnum.Left;
                return (int)EntityPositionEnum.Right;
            case EntityPositionEnum.Left:
                if (goal == EntityPositionEnum.Middle) return (int)EntityPositionEnum.Right;
                return (int)EntityPositionEnum.Right * 2;
            case EntityPositionEnum.Right:
                if (goal == EntityPositionEnum.Middle) return (int)EntityPositionEnum.Left;
                return (int)EntityPositionEnum.Left * 2;
        }

        return 0;
    }

    private IEnumerator StartMove(float duration, Vector3 startPos, Vector3 goalPos)
    {
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, currentTime / duration);
            transform.localPosition = Vector3.Lerp(startPos, goalPos, t);
            yield return null;
        }

        transform.localPosition = goalPos;
        yield break;
    }

    public void PutInBackground()
    {
        fadedOut = true;
        StartCoroutine(StartFade(COLOR_FADE_DURATION, entityImage.color, BACKGROUND_COLOR));
    }

    public void PutInForeground()
    {
        fadedOut = false;
        StartCoroutine(StartFade(COLOR_FADE_DURATION, entityImage.color, new(1f, 1f, 1f)));
    }

    public void FadeInstantiate(Color targetColor)
    {
        StartCoroutine(StartFade(COLOR_FADE_DURATION, entityImage.color, targetColor));
    }

    public void FadeDestroy()
    {
        StartCoroutine(StartFade(COLOR_FADE_DURATION, entityImage.color, new(0, 0, 0, 0)));
        fadedOutFinishedCanDestroy = new();
        fadedOutFinishedCanDestroy.AddListener(DestroySelf);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
        fadedOutFinishedCanDestroy.RemoveAllListeners();
    }

    private IEnumerator StartFade(float duration, Color startColor, Color targetColor)
    {
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            entityImage.color = Color.Lerp(startColor, targetColor, currentTime / duration);
            yield return null;
        }

        fadedOutFinishedCanDestroy?.Invoke();

        yield break;
    }
}
