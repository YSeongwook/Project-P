using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Sprite[] lobbySprites; // 각 챕터에 맞는 스프라이트 배열
    [SerializeField] private Image lobbyBackground; // 배경 이미지

    private void Awake()
    {
        // PlayerViewModel의 현재 챕터 정보를 확인하고 로비 배경을 업데이트
        UpdateLobbyBackground();
        
        // PlayerViewModel이 챕터가 변경될 때마다 배경을 업데이트
        PlayerInformation.Instance.PlayerViewModel.PropertyChanged += OnPlayerPropertyChanged;
    }

    private void OnDestroy()
    {
        // 이벤트 핸들러 해제
        PlayerInformation.Instance.PlayerViewModel.PropertyChanged -= OnPlayerPropertyChanged;
    }

    // PlayerViewModel의 속성이 변경될 때 호출되는 메서드
    private void OnPlayerPropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(PlayerViewModel.CurrentChapter))
        {
            UpdateLobbyBackground();
        }
    }

    // 현재 챕터에 따라 로비 배경 이미지를 변경하는 메서드
    private void UpdateLobbyBackground()
    {
        int currentChapter = PlayerInformation.Instance.PlayerViewModel.CurrentChapter;

        // 현재 챕터에 맞는 스프라이트가 있는지 확인
        if (currentChapter >= 0 && currentChapter < lobbySprites.Length)
        {
            lobbyBackground.sprite = lobbySprites[currentChapter];
        }
        else
        {
            Debug.LogError("로비 배경 스프라이트가 설정되지 않았습니다. 현재 챕터: " + currentChapter);
        }
    }
}