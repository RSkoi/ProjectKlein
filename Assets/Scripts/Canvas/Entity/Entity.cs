using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class Entity : MonoBehaviour
{
    public static readonly Color BACKGROUND_COLOR = new(0.6f, 0.6f, 0.6f);
    public static readonly float COLOR_FADE_DURATION = 0.6f;

    private UnityEvent _fadedOutFinishedCanDestroy;
    private Coroutine _fadeEntityCoroutine;

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
    [Tooltip("Whether the entity is flipped/mirrored.")]
    public bool flipped = false;
    [Tooltip("The current positional offset of the entity")]
    public Vector2 curOffset = new();

    public void Awake()
    {
        if (_animator == null)
            _animator = GetComponent<Animator>();

        _animator.enabled = false;
    }

    public void Flip()
    {
        Quaternion rot = transform.localRotation;
        rot.y = flipped ? 0 : -180;

        transform.SetLocalPositionAndRotation(transform.localPosition, rot);
        flipped = !flipped;
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

    public void Offset(Vector2 offset)
    {
        Vector3 curPos = transform.localPosition;
        curPos.x += offset.x;
        curPos.y += offset.y;
        transform.localPosition = curPos;
        curOffset += offset;
    }

    public void ResetOffset()
    {
        Vector3 curPos = transform.localPosition;
        curPos.x -= curOffset.x;
        curPos.y -= curOffset.y;
        transform.localPosition = curPos;
        curOffset.x = 0;
        curOffset.y = 0;
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

    public void Move(EntityPositionEnum relativeMovePos, float duration, AnimationCurve speed)
    {
        if (relativeMovePos == curEntityPos)
            return;

        // TODO: move in y units too
        int moveX = GetRelativeXUnitsToMove(relativeMovePos);
        //int moveY = GetRelativeYUnitsMove(relativeMovePos);
        StartCoroutine(StartMove(
                duration,
                speed,
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

    private IEnumerator StartMove(float duration, AnimationCurve speed, Vector3 startPos, Vector3 goalPos)
    {
        float currentTime = 0;
        while (currentTime < duration)
        {
            currentTime += Time.deltaTime;
            float t = Mathf.SmoothStep(0, 1, currentTime / duration);
            float t_speed = speed.length != 0 ? speed.Evaluate(t) : t;
            transform.localPosition = Vector3.Lerp(startPos, goalPos, t_speed);
            yield return null;
        }

        transform.localPosition = goalPos;
        yield break;
    }

    private void ResetFadeOutCoroutine()
    {
        if (_fadeEntityCoroutine != null)
            StopCoroutine(_fadeEntityCoroutine);
        _fadeEntityCoroutine = null;
    }

    public void PutInBackground()
    {
        fadedOut = true;
        ResetFadeOutCoroutine();
        _fadeEntityCoroutine = StartCoroutine(StartFade(COLOR_FADE_DURATION, entityImage.color, BACKGROUND_COLOR));
    }

    public void PutInForeground()
    {
        fadedOut = false;
        ResetFadeOutCoroutine();
        _fadeEntityCoroutine = StartCoroutine(StartFade(COLOR_FADE_DURATION, entityImage.color, new(1f, 1f, 1f)));
    }

    public void FadeInstantiate(Color targetColor)
    {
        ResetFadeOutCoroutine();
        _fadeEntityCoroutine = StartCoroutine(StartFade(COLOR_FADE_DURATION, entityImage.color, targetColor));
    }

    public void FadeDestroy()
    {
        ResetFadeOutCoroutine();
        _fadeEntityCoroutine = StartCoroutine(StartFade(COLOR_FADE_DURATION, entityImage.color, new(0, 0, 0, 0)));
        _fadedOutFinishedCanDestroy = new();
        _fadedOutFinishedCanDestroy.AddListener(DestroySelf);
    }

    private void DestroySelf()
    {
        Destroy(gameObject);
        _fadedOutFinishedCanDestroy.RemoveAllListeners();
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

        _fadedOutFinishedCanDestroy?.Invoke();

        yield break;
    }
}
