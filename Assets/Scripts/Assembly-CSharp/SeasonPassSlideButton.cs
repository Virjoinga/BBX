using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SeasonPassSlideButton : MonoBehaviour
{
	[SerializeField]
	private ScrollRect scrollRect;

	[SerializeField]
	private TextMeshProUGUI pageCountText;

	private int currentPageCount = 1;

	private float stepCount;

	private float pageCount;

	private Vector2 currentHorizontalPos = Vector2.zero;

	private void Start()
	{
		stepCount = 0.08695652f;
	}

	public void OnPreviousButton()
	{
		currentPageCount--;
		if (currentPageCount < 1)
		{
			currentPageCount = 1;
			return;
		}
		pageCount -= stepCount;
		pageCountText.text = $"PAGE {currentPageCount}/<color=#808080ff>13</color>";
		if (currentPageCount == 1)
		{
			scrollRect.DOHorizontalNormalizedPos(0f, 0.3f);
		}
		else
		{
			scrollRect.DOHorizontalNormalizedPos(pageCount, 0.3f);
		}
	}

	public void OnNextButton()
	{
		currentPageCount++;
		if (currentPageCount > 13)
		{
			currentPageCount = 13;
			return;
		}
		pageCount += stepCount;
		pageCountText.text = $"PAGE {currentPageCount}/<color=#808080ff>13</color>";
		if (currentPageCount == 13)
		{
			scrollRect.DOHorizontalNormalizedPos(1f, 0.3f);
		}
		else
		{
			scrollRect.DOHorizontalNormalizedPos(pageCount, 0.3f);
		}
	}
}
