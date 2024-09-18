using System; 
using UnityEngine;
using UnityEngine.UIElements;

namespace AtomEngine.SystemFunc.Extensions
{
    public static class VisualElementExtensions
    {
        public static void StartFadeInAnimation(this VisualElement element, float fadeDuration = 2.5f, Action onComplete = null)
        {
            float elapsedTime = 0f;
            element.style.opacity = 0f;

            element.schedule.Execute(() =>
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / fadeDuration);
                element.style.opacity = progress;
                if (progress >= 1f)
                {
                    element.style.opacity = 1f;
                    onComplete?.Invoke();
                }
            }).Until(() => element.style.opacity.value >= 1f);
        }
        public static void StartFadeOutAnimation(this VisualElement element, float fadeDuration = 2.5f, Action onComplete = null)
        {
            float elapsedTime = 0f;
            element.style.opacity = 1f;

            element.schedule.Execute(() =>
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / fadeDuration);
                element.style.opacity = 1f - progress;
                if (progress >= 1f)
                {
                    element.style.opacity = 0f;
                    onComplete?.Invoke();
                }
            }).Until(() => element.style.opacity.value <= 0f);
        }
        public static void StartUnfoldAnimation(this VisualElement element, float originalHeight, float fadeDuration = 2.5f, Action onComplete = null)
        {
            float elapsedTime = 0f;
            element.style.height = 0;

            element.schedule.Execute(() =>
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / fadeDuration);
                element.style.height = Mathf.Lerp(0, originalHeight, progress);

                if (progress >= 1f)
                {
                    element.style.height = originalHeight;
                    onComplete?.Invoke();
                }
            }).Until(() => element.style.height == originalHeight);
        }
        public static void StartFoldAnimation(this VisualElement element, float originalHeight, float fadeDuration = 2.5f, Action onComplete = null)
        {
            float elapsedTime = 0f;

            element.schedule.Execute(() =>
            {
                elapsedTime += Time.deltaTime;
                float progress = Mathf.Clamp01(elapsedTime / fadeDuration);
                element.style.height = Mathf.Lerp(originalHeight, 0, progress);

                if (progress >= 1f)
                {
                    element.style.height = 0;
                    onComplete?.Invoke();
                }
            }).Until(() => element.style.height == 0);
        }
    }
}
