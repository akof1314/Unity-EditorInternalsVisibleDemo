# Unity Editor Internals Visible Demo
## 原因

在使用 Unity Editor 制作编辑器工具的时候，经常无法对 Editor 内的 internal 类和方法进行访问，需要使用反射方式。反射方式写起来比较麻烦，也不便于调试和更新，Unity 升级了版本，内部接口变化，导致反射失效。

## 解决

通过查看`UnityEditor.dll`文件，可以发现其声明了一些友元程序集，比如`[assembly: InternalsVisibleTo("UnityEditor.Facebook.Extensions")]`，那么我们就找一个不会用上的程序集，来访问内部方法。
![](https://img-blog.csdnimg.cn/20201227112648817.png)

## 步骤

1. Demo 基于 Unity 2018，但每个版本都可以实现
2. 创建自定义包文件夹，如：EditorInternalsVisibleDemo\PackagesCustom\com.wuhuan.internalsvisibledemo
3. 在包里创建`package.json`文件
4. 在包里创建`UnityEditor.Facebook.Extensions.asmdef`文件

`package.json`文件
```javascript
{
  "name": "com.wuhuan.internalsvisibledemo",
  "displayName": "InternalsVisibleDemo",
  "version": "1.0.0",
  "unity": "2018.4",
  "description": "internalsvisibledemo.",
  "keywords": [
    "internalsvisibledemo"
  ],
  "category": "Editor",
  "dependencies": {}
}
```

`UnityEditor.Facebook.Extensions.asmdef`文件

```java
{
    "name": "UnityEditor.Facebook.Extensions",
    "references": [],
    "optionalUnityReferences": [],
    "includePlatforms": [
        "Editor"
    ],
    "excludePlatforms": []
}
```

## 源码

Github 地址：[https://github.com/akof1314/Unity-EditorInternalsVisibleDemo](https://github.com/akof1314/Unity-EditorInternalsVisibleDemo)
