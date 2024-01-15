using UnityEngine;
using UnityEngine.UIElements;

namespace EZ.DataTool.View
{
    public class PopupView : VisualElement
    {
        private VisualElement _rootVisualElement;

        public PopupView(VisualElement rootVisualElement)
        {
            style.width = 574;
            style.height = 204;

            if (ColorUtility.TryParseHtmlString("3C3C3C", out var color))
            {
                style.backgroundColor = new StyleColor(color);
            }

            var titleBar = new VisualElement();
            titleBar.style.flexDirection = FlexDirection.Row;
            titleBar.Add(new Label() { name = "Title" });

            var spacer = new VisualElement();
            spacer.style.flexGrow = 1;
            titleBar.Add(spacer);

            var closeButton = new Button(OnClose) { text = "X" };
            ColorUtility.TryParseHtmlString("E04343", out var closeButtonColor);
            closeButton.style.color = closeButtonColor;
            titleBar.Add(closeButton);

            Add(titleBar);

            var messageLabel = new Label() { name = "Message" };
            Add(messageLabel);

            _rootVisualElement = rootVisualElement;
        }

        public void OnClose()
        {
            _rootVisualElement.Remove(this);
        }

        public void Show(string title, string message)
        {
            this.Q<Label>("Title").text = title;
            this.Q<Label>("Message").text = message;

            _rootVisualElement.Add(this);
        }
    }
}