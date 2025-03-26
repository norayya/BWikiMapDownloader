# BWikiMapDownloader

## 运行环境 (命令行)

dotnet 9 runtime

dotnet-script

OpenCvSharp runtime(DLL)

可用物理内存(RAM) >= 5GB

可用存储空间(Storage) >= 1GB

## 获取地图边界

dotnet script boundary.csx

## 下载地图贴片

dotnet script download.csx

## 拼接完整地图

dotnet script tiling.csx

## 帮助链接

> [安装dotnet 9 runtime(运行时)](https://dotnet.microsoft.com/en-us/download/dotnet/9.0)

> [安装dotnet-script](https://github.com/dotnet-script/dotnet-script)

Release包附带OpenCV运行库, 如果需要更换请按照包内示例引入文件.

如Bwiki地图版本更新, 可以修改`public.csx`文件内`MAP_VERSION`变量来控制下载版本.

执行脚本可能需要从nuget加载包, 请确认网络通畅.
