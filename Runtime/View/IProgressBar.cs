
namespace EZ.DataTool.View
{
    public interface IProgressBar
    {
        void DisplayProgressBar();
        void DisplayProgressBar(float progress);
        void ClearProgressBar();
    }
}