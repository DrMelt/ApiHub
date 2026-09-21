# APIHub

LLM 提供商与模型目录相关的 .NET 包。

## 包

- [`APIHub`](https://github.com/DrMelt/APIHub/tree/main/APIHub) 提供商与模型目录的数据模型与校验。
- [`APIHub.ChatClient`](https://github.com/DrMelt/APIHub/tree/main/APIHub.ChatClient) 由提供商定义建立 OpenAI 兼容客户端。
- [`APIHub.Json`](https://github.com/DrMelt/APIHub/tree/main/APIHub.Json) 目录内容与 JSON 文本的互转。

## 开发

```shell
dotnet build APIHub.slnx
dotnet test APIHub.slnx
dotnet pack APIHub.slnx -c Release -o artifacts
```

## 版本与发布

版本由 MinVer 从 git tag 推导，tag 形如 `v0.2.0`。

- tag 提交产出与 tag 相同的版本。
- 正式 tag 之后的提交产出下一个补丁版本的预发布版本，高度是相对该 tag 的提交数，如 `v0.2.0` 之后三个提交产出 `0.2.1-alpha.0.3`。
- 预发布 tag 之后的提交保留 tag 版本，只追加高度，如 `v0.2.0-rc.1` 之后三个提交产出 `0.2.0-rc.1.3`。
- 没有 tag 时产出 `0.0.0-alpha.0.<高度>`，高度相对首个提交计数。

推导需要完整 git 历史与 tag，找不到 tag 时退化为默认版本且不报错，浅克隆可能触发该退化。

推送 `v*` tag 会触发 CI，打包产物作为附件创建同名 GitHub Release。

## 许可

Apache-2.0
