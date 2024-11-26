package com.DefaultCompany.KakaoTest

import android.app.Application
import com.kakao.sdk.common.KakaoSdk
import com.kakao.sdk.common.util.Utility

class GlobalApplication : Application() {
    override fun onCreate() {
        super.onCreate()
        // 다른 초기화 코드들

        // Kakao SDK 초기화
        KakaoSdk.init(this, "d1a626dc8072fc0b0212ad14488d5e77")
        var keyHash = Utility.getKeyHash(this)
        println("UnityLog :  $keyHash")
    }
}