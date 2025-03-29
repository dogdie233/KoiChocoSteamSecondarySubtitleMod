# 恋选双语字幕插件

基于 [BepInEx](https://github.com/BepInEx/BepInEx) 制作的插件

## 安装教程

### 懒人版

[Bilibili视频教程](https://www.bilibili.com/video/BV1wPZeYHEyb/?t=111)

### 并非懒人版

 1. 去 [BepInEx的Release页](https://github.com/BepInEx/BepInEx/releases) 下载**BepInEx5**（不能是6），注意下载 `x86` 版本
 2. 把压缩包里面的所有内容解压到游戏目录（和.exe在同一个目录）
 3. 下载一个 [UnityHub](https://unity.com/download)，再在里面下载 `Unity2021.3.45f1`
 4. 使用刚刚下载的Unity新建一个空的工程(模板选默认的Universal2D就可以)
 5. 打开工程之后去 File > Build Settings > Player Settings > Other Settings ，把 `Scripting Backend` 改成 `Mono`，把 `Api Compatibility Level` 改成 `.NET Framework`
 6. 在 Build Settings 里把 `Architecture` 改成 `Intel 32-bit`
 7. 打包到随便一个文件夹
 8. 打开刚刚打包的目录，进入XXX_Data文件夹，进入Managed文件夹，复制里面所有的dll
 9. 进入如下路径 `<游戏根目录>/BepInEx` 新建一个文件夹叫 `unstripped`
 10. 把刚刚复制的dll全部丢进去
 11. 返回游戏根目录，编辑文件 `doorstop_config.ini`， 把 `dll_search_path_override = ` 改成 `dll_search_path_override = BepInEx\unstripped;KoiChoco_Data\Managed`
 12. 启动游戏，等到游戏弹出窗口的时候就可以关掉了，再打开BepInEx文件夹，如果出现了新的`plugins`文件夹则说明BepInEx加载正常，前面的努力没有白费
 13. 打开本仓库的 [Release](https://github.com/dogdie233/KoiChocoSteamSecondarySubtitleMod/releases/)，下载最新的版本的 `KoiChocoSteamSecondarySubtitle.dll`
 14. 把下载的dll丢进 `<游戏根目录>/BepInEx/plugins`文件夹
 15. 再次启动游戏（必须从steam启动），进入设置，如果看到多了几个按钮则说明ok了

## 构建教程

 1. Clone本仓库
 2. 在仓库根目录(.sln所处的文件夹)新建一个文件夹，名为`lib`
 3. 把 `<游戏根目录>/KoiChoco_Data/Managed`里的所有dll复制到`lib`文件夹
 4. 请确保你安装插件成功，打开`<游戏根目录>/BepInEx/unstripped`文件夹，把里面的所有dll复制到`lib`文件夹内，覆盖同名
 5. 再启动你的Visual Studio或者是Rider就可以了

## 许可证
本 MOD 源码采用 **Apache 2.0 许可证**（参见 [LICENSE](https://github.com/dogdie233/KoiChocoSteamSecondarySubtitleMod/blob/master/LICENSE)）。  
本项目依赖于 **BepInEx**（LGPL v2.1 许可证），请访问 [BepInEx 官方仓库](https://github.com/BepInEx/BepInEx) 获取更多信息。  
