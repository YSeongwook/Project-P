using EnumTypes;
using EventLibrary;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChapterColliection : MonoBehaviour
{
    private List<Chapter> chapters = new List<Chapter>();

    private void Awake()
    {
        chapters.Clear();

        foreach (Transform child in transform)
        {
            var childChapter =  child.GetComponent<Chapter>();
            if(childChapter == null) continue;

            chapters.Add(childChapter);
        }

        EventManager<UIEvents>.StartListening<string>(UIEvents.OnEnableChapterMoveButton, SetChapterButtonActive);
    }

    private void OnDestroy()
    {
        EventManager<UIEvents>.StopListening<string>(UIEvents.OnEnableChapterMoveButton, SetChapterButtonActive);
    }

    private void SetChapterButtonActive(string openChapter)
    {
        int _openChapter = int.Parse(openChapter);
        foreach (Chapter chapter in chapters)
        {
            chapter.DeactivateButton(_openChapter);
        }
    }
}
