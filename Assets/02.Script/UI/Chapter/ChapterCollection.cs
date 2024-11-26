using System.Collections.Generic;
using EnumTypes;
using EventLibrary;
using UnityEngine;

public class ChapterCollection : MonoBehaviour
{
    private List<Chapter> _chapters = new List<Chapter>();

    private void Awake()
    {
        _chapters.Clear();

        foreach (Transform child in transform)
        {
            var childChapter =  child.GetComponent<Chapter>();
            if(childChapter == null) continue;

            _chapters.Add(childChapter);
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
        foreach (Chapter chapter in _chapters)
        {
            chapter.DeactivateButton(_openChapter);
        }
    }
}