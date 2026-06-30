using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class CharacterVisual : MonoBehaviour
{
    [Header("References")]
    public CharacterSpriteData spriteData;
    public Image characterImage;

    [Header("Sprite Duration")]
    public float damageTime = 0.4f;
    public float deadTime = 2f;

    [Header("Size Settings")]
    public float idleSize = 100f;
    public float aimSize = 80f;

    public float idleDamageSize = 120f;
    public float aimDamageSize = 100f;

    public float deadSize = 150f;

    [Header("Animation Speed")]
    public float sizeSpeed = 10f;
    public float deadSizeSpeed = 3f;

    [Header("Color")]
    public Color normalColor = Color.white;
    public Color deadColor = Color.red;
    public float colorSpeed = 3f;

    private bool isAiming;
    private bool isTemporaryState;

    private Coroutine stateRoutine;
    private RectTransform rectTransform;

    private Vector2 targetSize;
    private Color targetColor;

    private void Start()
    {
        rectTransform = characterImage.rectTransform;

        targetSize = new Vector2(idleSize, idleSize);
        rectTransform.sizeDelta = targetSize;

        targetColor = normalColor;
        characterImage.color = normalColor;

        SetIdle();
    }

    private void Update()
    {
        if (!isTemporaryState)
        {
            if (Mouse.current != null && Mouse.current.rightButton.isPressed)
            {
                if (!isAiming)
                {
                    isAiming = true;
                    SetAim();
                }

                targetSize = new Vector2(aimSize, aimSize);
            }
            else
            {
                if (isAiming)
                {
                    isAiming = false;
                    SetIdle();
                }

                targetSize = new Vector2(idleSize, idleSize);
            }
        }

        float currentSpeed = targetSize == new Vector2(deadSize, deadSize)
            ? deadSizeSpeed
            : sizeSpeed;

        rectTransform.sizeDelta = Vector2.Lerp(
            rectTransform.sizeDelta,
            targetSize,
            Time.deltaTime * currentSpeed
        );

        characterImage.color = Color.Lerp(
            characterImage.color,
            targetColor,
            Time.deltaTime * colorSpeed
        );
    }

    private void SetIdle()
    {
        characterImage.sprite = spriteData.idle;
    }

    private void SetAim()
    {
        characterImage.sprite = spriteData.aim;
    }

    public void ShowDamage()
    {
        if (stateRoutine != null)
            StopCoroutine(stateRoutine);

        targetColor = normalColor;

        stateRoutine = StartCoroutine(ShowTemporary(spriteData.damage, damageTime, false));
    }

    public void ShowDead()
    {
        if (stateRoutine != null)
            StopCoroutine(stateRoutine);

        stateRoutine = StartCoroutine(ShowTemporary(spriteData.dead, deadTime, true));
    }

    private IEnumerator ShowTemporary(Sprite sprite, float duration, bool isDead)
    {
        isTemporaryState = true;

        characterImage.sprite = sprite;

        if (isDead)
        {
            // Grow and become red
            targetSize = new Vector2(deadSize, deadSize);
            targetColor = deadColor;

            // Stay dead for a while
            yield return new WaitForSeconds(duration);

            // Return to normal size and color
            targetSize = new Vector2(idleSize, idleSize);
            targetColor = normalColor;

            // Wait until the animation is almost finished
            while (Vector2.Distance(rectTransform.sizeDelta, targetSize) > 0.5f ||
                   Vector4.Distance(characterImage.color, targetColor) > 0.02f)
            {
                yield return null;
            }

            SetIdle();
            isAiming = false;
            isTemporaryState = false;

            yield break;
        }

        // Damage animation
        if (isAiming)
            targetSize = new Vector2(aimDamageSize, aimDamageSize);
        else
            targetSize = new Vector2(idleDamageSize, idleDamageSize);

        yield return new WaitForSeconds(duration * 0.5f);

        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            targetSize = new Vector2(aimSize, aimSize);
        }
        else
        {
            targetSize = new Vector2(idleSize, idleSize);
        }

        yield return new WaitForSeconds(duration * 0.5f);

        if (Mouse.current != null && Mouse.current.rightButton.isPressed)
        {
            isAiming = true;
            SetAim();
        }
        else
        {
            isAiming = false;
            SetIdle();
        }

        isTemporaryState = false;
    }
}