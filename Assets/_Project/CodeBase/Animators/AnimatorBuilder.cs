using UnityEngine;

namespace AtomEngine.Animators
{
    public class AnimatorBuilder : MonoBehaviour
    {
        [SerializeField] private Avatar avatar;
        [SerializeField] private Animator animator;

        public void SetupAnimatorAndPlayAnimation(Transform root, Avatar avatar, AnimationClip animationClip)
        { 
            Animator animator = root.GetComponent<Animator>();
            if (animator == null)
            {
                animator = root.gameObject.AddComponent<Animator>();
            }
             
            animator.avatar = avatar; 
            if (!animator.avatar.isValid || !animator.avatar.isHuman)
            {
                Debug.LogError("Invalid or non-human avatar.");
                return;
            }

            // Создаем AnimatorController
            AnimatorOverrideController animatorOverrideController = new AnimatorOverrideController();

            // Создаем RuntimeAnimatorController, если у тебя есть уже контроллер
            RuntimeAnimatorController runtimeController = Resources.Load<RuntimeAnimatorController>("YourAnimatorController");
            if (runtimeController != null)
            {
                animator.runtimeAnimatorController = runtimeController;
            }
            else
            {
                Debug.LogError("Animator Controller не найден!");
                return;
            }

            // Проигрываем анимацию
            animator.Play(animationClip.name);
        }
    }
}
