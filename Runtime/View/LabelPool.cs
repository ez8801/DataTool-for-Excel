using UnityEngine.UIElements;

namespace EZ.DataTool
{
    public class LabelPool : System.IDisposable
    {
        private UnityEngine.Pool.ObjectPool<Label> _pool;

        public LabelPool()
        {
            _pool = new UnityEngine.Pool.ObjectPool<Label>(CreatePooledItem, OnTakeFromPool, OnReturnedToPool, maxSize: 300);
        }

        void OnTakeFromPool(Label label)
        {
            label.style.display = DisplayStyle.Flex;
        }

        Label CreatePooledItem()
        {
            return new Label();
        }

        void OnReturnedToPool(Label label)
        {
            label.style.display = DisplayStyle.None;
            label.text = string.Empty;
        }

        public Label Get()
        {
            return _pool.Get();
        }

        public void Release(Label element)
        {
            _pool.Release(element);
        }

        void System.IDisposable.Dispose()
        {
            _pool.Dispose();
        }
    }
}
