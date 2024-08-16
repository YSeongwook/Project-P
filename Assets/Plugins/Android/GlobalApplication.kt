package com.DefaultCompany.KakaoTest

import android.app.Application
import com.kakao.sdk.common.KakaoSdk
import com.kakao.sdk.common.util.Utility

class GlobalApplication : Application() {
    override fun onCreate() {
        super.onCreate()
        // 다른 초기화 코드들

        // Kakao SDK 초기화
        KakaoSdk.init(this, "7978192c1c943eebb4eca36b35800e29")
        var keyHash = Utility.getKeyHash(this)
        println("UnityLog :  $keyHash")
    }
}