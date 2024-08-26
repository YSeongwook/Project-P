package com.unity3d.player

import android.util.Log

import com.kakao.sdk.auth.AuthApiClient
import com.kakao.sdk.auth.model.OAuthToken
import com.kakao.sdk.common.model.ClientError
import com.kakao.sdk.common.model.ClientErrorCause
import com.kakao.sdk.common.model.KakaoSdkError
import com.kakao.sdk.user.UserApiClient
import com.kakao.sdk.talk.TalkApiClient
import com.unity3d.player.UnityPlayer

class KakaoLogin {
    val context = UnityPlayer.currentActivity
    
    fun checkKakaoLoginStatus() {
        if (AuthApiClient.instance.hasToken())
        {
            UnityPlayer.UnitySendMessage("AlertMsg", "OnAlertMsg", "로그인 완료")

            // 로그인 데이터 중 테이블명/회원번호만 넘겨서 데이터 저장하기.
            UserApiClient.instance.me { user, error ->
                if (error != null) {
                    Log.e("UnityLog", "사용자 정보 요청 실패", error)
                }
                else if (user != null) {
                    UnityPlayer.UnitySendMessage("MySqlManager", "ReadData",
                        "MemberInfo||${user.id}"
                    )
                }
            }
            
            UserApiClient.instance.accessTokenInfo { _, error ->
                if (error != null) {
                    if (error is KakaoSdkError && error.isInvalidTokenError() == true) {
                        //로그인 필요
                        
                    }
                    else {
                        //기타 에러
                    }
                }
                else {
                    //토큰 유효성 체크 성공(필요 시 토큰 갱신됨)
                }
            }
        }
        else 
        {
            //로그인 필요
            UnityPlayer.UnitySendMessage("KakaoLoginManager", "OnLoginBtn", "")
        }
    }
    
    fun KakaoLogin() {
        // 로그인 공통 callback 구성
        val callback: (OAuthToken?, Throwable?) -> Unit = { token, error ->
            if (error != null) {
                //Log.e(TAG, "로그인 실패", error)
                //println("로그인 실패 : $error")
                Log.e("UnityLog", "로그인 실패 ${error}", error)
            }
            else if (token != null) {
                //Log.i(TAG, "로그인 성공 ${token.accessToken}")
                //println("로그인 성공 ${token.accessToken}")
                Log.e("UnityLog", "로그인 성공 ${token.accessToken}", error)
            }
        }
        
        // 카카오톡이 설치되어 있으면 카카오톡으로 로그인, 아니면 카카오계정으로 로그인
        if (UserApiClient.instance.isKakaoTalkLoginAvailable(context)) {
            UserApiClient.instance.loginWithKakaoTalk(context) { token, error ->
                if(error != null){
                    Log.e("UnityLog", "카카오톡으로 로그인 실패", error)

                    // 사용자가 카카오톡 설치 후 디바이스 권한 요청 화면에서 로그인을 취소한 경우,
                    // 의도적인 로그인 취소로 보고 카카오계정으로 로그인 시도 없이 로그인 취소로 처리 (예 : 뒤로가기)
                    if(error is ClientError && error.reason == ClientErrorCause.Cancelled){
                        return@loginWithKakaoTalk
                    }

                    // 카카오톡에 연결된 카카오계정이 없는 경우, 카카오계정으로 로그인 시도
                    UserApiClient.instance.loginWithKakaoAccount(context, callback = callback)
                }else if (token != null){
                    Log.i("UnityLog", "카카오톡 로그인 성공 ${token.accessToken}")
                    UnityPlayer.UnitySendMessage("KakaoLoginManager", "OffLoginBtn", "")
                    UnityPlayer.UnitySendMessage("AlertMsg", "OnAlertMsg", "로그인 완료")
                    //UnityPlayer.UnitySendMessage("MySqlManager", "FirstLoginSuccess", "")
                    GetUserData()
                }
            }
        } else {
            UserApiClient.instance.loginWithKakaoAccount(context, callback = callback)
        }
    }

    fun GetUserData() {
        // 사용자 정보 요청 (기본)
        UserApiClient.instance.me { user, error ->
            if (error != null) {
                Log.e("UnityLog", "사용자 정보 요청 실패", error)
            }
            else if (user != null) {
                Log.i("UnityLog", "사용자 정보 요청 성공" +
                        "\n회원번호: ${user.id}" +
                        "\n이메일: ${user.kakaoAccount?.email}" +
                        "\n닉네임: ${user.kakaoAccount?.profile?.nickname}" +
                        "\n프로필사진: ${user.kakaoAccount?.profile?.thumbnailImageUrl}")
                
                UnityPlayer.UnitySendMessage("MySqlManager", "GetUserData",
                    "${user.id}" + "||" +
                    "${user.kakaoAccount?.email}" + "||" +
                    "${user.kakaoAccount?.profile?.nickname}" + "||" +
                    "${user.kakaoAccount?.profile?.thumbnailImageUrl}"
                )
            }
        }
    }

    fun GetFriendsList() {
        // 카카오톡 친구 목록 가져오기 (기본)
        TalkApiClient.instance.friends { friends, error ->
            if (error != null) {
                Log.e("UnityLog", "카카오톡 친구 목록 가져오기 실패", error)
            }
            else if (friends != null) {
                Log.i("UnityLog", "Total Count: ${friends.totalCount}, Favorite Count: ${friends.favoriteCount}")

                if (friends?.elements.isNullOrEmpty()) {
                    Log.i("UnityLog", "카카오톡 친구 목록이 비어 있습니다.")
                    // 빈 목록에 대한 처리
                    UnityPlayer.UnitySendMessage("KakaoSystem", "OnGetFriendsListResult", "[]")
                }else{
                    //Log.i("UnityLog", "카카오톡 친구 목록 가져오기 성공 \n${friends.elements?.joinToString("\n")}")

                    val friendsListString = friends.elements?.joinToString("\n") ?: "[]"
                    Log.i("UnityLog", "카카오톡 친구 목록 가져오기 성공 \n$friendsListString")
                    UnityPlayer.UnitySendMessage("KakaoSystem", "OnGetFriendsListResult", friendsListString)
                }
            }
        }
    }
}