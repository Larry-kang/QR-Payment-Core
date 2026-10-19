# 主被掃支付交易核心（QR-Payment-Core）

以通用 QR 支付為題的 C#／.NET 早期 POC，目前聚焦交易記錄 API 與資料約束；主掃、被掃付款流程及支付服務模擬器列為後續規劃。

- **主掃**：使用者掃描商家 QR Code（merchant-presented）。
- **被掃**：商家掃描使用者付款 QR Code（consumer-presented）。

上述兩種流程是探索目標，目前均未實作。本專案不處理真實金流，不代表任何支付機構產品或官方實作。

## 實際完成範圍

| 狀態 | 範圍 |
|---|---|
| 已完成並驗證 | .NET 10 分層骨架、SQLite migration 建立資料表、`POST /api/Payment` 建立 `Created` 交易與保存金額；依序重送相同 key 回傳 409、缺少 key 回傳 400，見本輪驗證紀錄 |
| 部分完成 | 冪等性只有 key 查詢與唯一索引；不比對 payload、不重播原回應，併發衝突尚無完整處理。Transaction 有狀態列舉，但無受控狀態機。MerchantId、OrderId 雖由 DTO 接收，目前未驗證或保存 |
| 尚未實作／規劃 | 主掃／被掃 QR 產生、解析與 token、支付服務模擬器、付款執行、Unknown 恢復、退款、Wallet／Ledger、Outbox／Inbox、重試、對帳、結算、前端、Redis／訊息佇列與正式部署 |

既有 xUnit 測試僅一個空白測試，不能證明業務正確性。完整結果見 [去識別與驗證紀錄](docs/neutralization-review.md)。[規格索引](docs/specs/README.md) 中較廣的平台能力皆為未來草案，不屬本輪交付。

## 建置、測試、執行

需要 .NET 10 SDK；不需真實支付帳號或公司資源。由 repository 根目錄執行：

```sh
dotnet restore QR-Payment-Core.sln
dotnet build QR-Payment-Core.sln --no-restore
dotnet test QR-Payment-Core.sln --no-build
```

初次執行需建立本機 SQLite schema；程式不會自動 migration。EF 工具版本與目前套件對齊：

```sh
QR_PAYMENT_DB="$(pwd)/qr-payment.db"
dotnet tool install dotnet-ef --version 10.0.1 --tool-path ./.tools
./.tools/dotnet-ef database update --project src/QrPayment.Infrastructure --startup-project src/QrPayment.WebAPI --connection "Data Source=$QR_PAYMENT_DB"
dotnet run --project src/QrPayment.WebAPI --no-launch-profile -- --urls http://localhost:5090 --ConnectionStrings:DefaultConnection="Data Source=$QR_PAYMENT_DB"
```

以上為 POSIX shell 指令，先以 repository 根目錄建立資料庫的絕對路徑，讓 EF 工具與 API 使用同一檔案，避免工具工作目錄差異。PowerShell 請以 `$QR_PAYMENT_DB = Join-Path (Get-Location) "qr-payment.db"` 設定同名變數後執行其餘指令。資料庫在根目錄產生並已被忽略。正式或既有資料庫請自行指定正確位置，勿將新檔名誤當資料遷移。

可用下列合成請求，或 [HTTP 範例](src/QrPayment.WebAPI/QrPayment.WebAPI.http)：

```sh
curl -i http://localhost:5090/api/Payment \
  -H 'Content-Type: application/json' \
  -H 'Idempotency-Key: synthetic-demo-001' \
  -d '{"merchantId":"fictional-merchant-001","amount":100,"orderId":"synthetic-order-001"}'
```

首次成功回傳 200 與 `transactionId`、數字狀態 `0`（Created）；同一 key 依序重送回傳 409。這只建立資料記錄，沒有扣款、支付授權或 QR 交易。Development 模式可於 `/swagger` 檢視 API。

## 模擬與資料邊界

範例的商家、訂單、冪等 key 與金額均為明確虛構的合成資料。未使用真實支付 QR 格式、品牌圖示、內部 API、商家資料、帳號或憑證。未來模擬服務僅用於控制成功、拒絕、逾時等測試情境，目前尚無可呼叫的支付服務模擬器，也未實作任何品牌協定。

本機 SQLite 不納入版本控制；歷史提交中的舊名稱與舊資料庫不會因本輪改名消失。本輪不重寫 Git 歷史。

## 本人與 AI Agent 的貢獻

定義交易記錄規格與驗收情境，以 Agent 協作建立並驗證早期 POC。完成範圍與限制列於上方表格及驗證紀錄。
