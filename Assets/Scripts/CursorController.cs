#if UNITY_EDITOR
using Lean.Touch;
using UnityEngine;

namespace DefaultNamespace
{
    public class CursorController : MonoBehaviour
    {
        [SerializeField] private Texture2D _texture;
        [SerializeField] private Texture2D _downTexture;
        [SerializeField] private Vector2 _offset;
        private bool _isCursorActive = false;
        
        
        private void EnableCursor()
        {
            if (_texture == null)
            {
                Debug.LogWarning("Please specify the texture to be used as cursor. Disabling for now.");
                return;
            }

            _isCursorActive = true;
            Cursor.SetCursor(_texture, _offset, CursorMode.ForceSoftware);
        }
        
        
        private void DisableCursor()
        {
            Cursor.SetCursor(null, Vector2.zero, CursorMode.ForceSoftware);
            _isCursorActive = false;
        }
        private void Awake()
        {
            EnableCursor();
        }

        private void OnEnable()
        {
            LeanTouch.OnFingerDown += HandleFingerDown;
            LeanTouch.OnFingerUp += HandleFingerUp;
        }


        private void OnDisable()
        {
            LeanTouch.OnFingerDown -= HandleFingerDown;
            LeanTouch.OnFingerUp -= HandleFingerUp;
        }
        private void HandleFingerUp(LeanFinger obj)
        {
            if (!_isCursorActive) return;
            if (_texture == null)
            {
                Debug.LogWarning("Please specify the texture to be used when finger is lifted. Bypassing for now.");
                return;
            }
            Cursor.SetCursor(_texture, _offset, CursorMode.ForceSoftware);
        }

        private void HandleFingerDown(LeanFinger obj)
        {
            if (!_isCursorActive) return;
            if (_texture == null)
            {
                Debug.LogWarning("Please specify the texture to be used when finger is down. Bypassing for now.");
                return;
            }
            Cursor.SetCursor(_downTexture, _offset, CursorMode.ForceSoftware);
        }
    }
}

#endif