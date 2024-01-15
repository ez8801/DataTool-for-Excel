using System.Text.RegularExpressions;

namespace EZ.DataTool.Util
{
	public interface IKoreanTextDetector
	{
		public bool HasKoreanText(string text);
	}

	public class KoreanTextDetector : IKoreanTextDetector
	{
		public bool HasKoreanText(string text)
		{
			return Regex.IsMatch(text, @"[ㄱ-ㅎ가-힣]");
		}
	}
}