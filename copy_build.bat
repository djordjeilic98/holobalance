del /S /Q C:\Work\Holobalance-App-Android\src\main\assets\*
xcopy C:\Work\Holobalance-App\Build\unityLibrary\src\main\assets C:\Work\Holobalance-App-Android\src\main\assets /i /c /k /e /r /y

del /S /Q C:\Work\Holobalance-App-Android\src\main\jniLibs\armeabi-v7a\*
xcopy C:\Work\Holobalance-App\Build\unityLibrary\src\main\jniLibs\armeabi-v7a C:\Work\Holobalance-App-Android\src\main\jniLibs\armeabi-v7a /i /c /k /e /r /y

REM C:\Work\Holobalance-App\Build\unityLibrary\src\main\assets
REM C:\Work\Holobalance-App-Android\src\main\assets
REM C:\Work\Holobalance-App\Build\unityLibrary\src\main\jniLibs\armeabi-v7a
REM C:\Work\Holobalance-App-Android\src\main\jniLibs\armeabi-v7a

pause