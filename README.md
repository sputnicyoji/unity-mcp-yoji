# Yoji Unity MCP

个人使用的 Unity Editor MCP bridge。

目标很窄：

- 通过 Git URL 直接安装到 Unity 工程。
- 使用仓库内置 Python server，不依赖上游 PyPI 包。
- 移除 telemetry / analytics / external reporting。
- 不保留上游网站、CI、发布、赞助、MCPB 打包材料。

## 安装

Unity Package Manager -> Add package from git URL：

```text
https://github.com/sputnicyoji/unity-mcp-yoji.git
```

如果安装指定分支：

```text
https://github.com/sputnicyoji/unity-mcp-yoji.git#beta
```

本仓库根目录就是 UPM package。
不需要 `?path=/MCPForUnity`。

## 使用

1. 打开 Unity 工程。
2. 安装本 Git package。
3. 打开菜单：`Window > Yoji Unity MCP`。
4. 使用窗口里的 client 配置或 HTTP server 启动按钮。

第一次启动需要本机有 `uv` / `uvx`。
服务器从包内 `Server~` 目录启动。

## 隐私

已移除上报链路。

- 没有 external reporting sender。
- 没有 analytics endpoint。
- 没有外部统计请求。
- Unity 侧不会生成或保存 customer UUID。
- Python server 不会向外发送使用记录。

仍然会有本地网络通信：

```text
Unity Editor <-> local Python server
127.0.0.1 only by default
```

## 保留内容

保留运行必需内容：

```text
Editor/
Runtime/
Server~/
package.json
LICENSE
```

`Server~` 是 UPM 隐藏目录。
Unity 不编译其中的 Python 文件。

## 许可证

本 fork 仍基于 MIT 代码。
原始版权声明保留在 `LICENSE`。
这是 MIT 条款要求。
