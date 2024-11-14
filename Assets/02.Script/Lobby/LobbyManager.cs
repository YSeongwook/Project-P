using System;
using EnumTypes;
using EventLibrary;
using UnityEngine;
using UnityEngine.UI;

public class LobbyManager : MonoBehaviour
{
    [SerializeField] private Sprite[] lobbySprites; // 각 단계별 스프라이트 배열
    [SerializeField] private Image backgroundImage; // 배경 이미지

    private Canvas _canvas;

    private void Awake()
    {
        _canvas = backgroundImage.GetComponentInParent<Canvas>();
    }

    private void OnEnable()
    {
        EventManager<DataEvents>.StartListening(DataEvents.UpdateLobby, UpdateLobbyBackground);
        EventManager<StageEvent>.StartListening<bool>(StageEvent.SetMiniGame, SetLobbyUI);
    }

    private void Start()
    {
        // 게임 시작 시 로비 배경 업데이트
        UpdateLobbyBackground();
    }

    private void OnDisable()
    {
        // 이벤트 핸들러 해제
        EventManager<DataEvents>.StopListening(DataEvents.UpdateLobby, UpdateLobbyBackground);
        EventManager<StageEvent>.StopListening<bool>(StageEvent.SetMiniGame, SetLobbyUI);
    }

    // 현재 챕터와 스테이지에 따라 로비 배경 이미지를 변경하는 메서드
    private void UpdateLobbyBackground()
    {
        int currentChapter = PlayerInformation.Instance.PlayerViewModel.CurrentChapter;
        int currentStage = PlayerInformation.Instance.PlayerViewModel.CurrentStage;

        // 챕터와 스테이지 값이 유효한 경우에만 배경을 업데이트
        if (currentChapter <= 0 || currentStage <= 0)
        {
            return;
        }

        // 현재 스테이지가 주요 스테이지(10, 20, 30)인지 확인
        int latestClearedStage = Mathf.FloorToInt(currentStage / 10f) * 10;
        if (latestClearedStage > 30) latestClearedStage = 30;

        // 주요 스테이지가 아닐 경우 로비 이미지 업데이트 중단
        if (!ShouldChangeBackground(currentChapter, latestClearedStage))
        {
            DebugLogger.Log("주요 스테이지가 아니므로 로비 이미지 변경을 건너뜁니다.");
            return;
        }

        int spriteIndex = GetSpriteIndex(currentChapter, latestClearedStage);
        if (spriteIndex >= 0 && spriteIndex < lobbySprites.Length)
        {
            backgroundImage.sprite = lobbySprites[spriteIndex];
        }
        else
        {
            DebugLogger.LogError("스프라이트 인덱스 범위를 벗어났습니다.");
        }
    }

    // 특정 챕터와 스테이지일 때 배경 변경 여부를 결정하는 메서드
    private bool ShouldChangeBackground(int chapter, int stage)
    {
        return stage % 10 == 0 && stage <= 30;
    }

    // 챕터와 스테이지에 맞는 스프라이트 인덱스를 반환하는 메서드
    private int GetSpriteIndex(int chapter, int stage)
    {
        // 클리어된 주요 스테이지(10, 20, 30)의 누적 개수 계산
        int clearedStages = 0;

        // 1챕터의 주요 스테이지는 1개(10)만 있음
        if (chapter > 1) clearedStages += 1;

        // 2챕터부터는 각 챕터마다 주요 스테이지가 3개씩 있음
        for (int ch = 2; ch < chapter; ch++)
        {
            clearedStages += 3;
        }

        // 현재 챕터의 주요 스테이지 클리어 개수 추가
        if (stage >= 10) clearedStages += 1;
        if (stage >= 20) clearedStages += 1;
        if (stage >= 30) clearedStages += 1;

        // 첫 번째 주요 스테이지 클리어 시 1번 인덱스를 할당
        int spriteIndex = clearedStages;
        return (spriteIndex >= 1 && spriteIndex < lobbySprites.Length) ? spriteIndex : 0;
    }

    // 로비 UI의 활성화 여부 설정 메서드
    private void SetLobbyUI(bool SetEnable)
    {
        _canvas.enabled = SetEnable;
    }
}
